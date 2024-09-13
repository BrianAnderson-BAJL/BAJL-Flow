using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowEngineCore;
using FlowEngineCore.Administration;
using FlowEngineCore.Administration.Messages;
using FlowEngineCore.Statistics;


namespace FlowEngineDesigner
{
  public partial class frmStatistics : Form
  {
    private class ComboFrequency
    {
      public FlowEngineCore.Statistics.STATISTICS_UPDATE_FREQUENCY Frequency;

      public ComboFrequency(FlowEngineCore.Statistics.STATISTICS_UPDATE_FREQUENCY frequency)
      {
        Frequency = frequency;
      }

      public override string ToString()
      {
        return Frequency.ToString() + " - " + ((int)Frequency) + " second(s)";
      }
    }
    private readonly double[] xSeconds = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 };
    const string STATS_ACTIVE = "Statistics Active";
    const string STATS_INACTIVE = "Statistics Inactive";

    private List<Statistic> OldStatistics = new List<Statistic>();
    private int mCurrentSecond = 0;

    private Statistic? CurrentStats;
    //private Statistic? SelectedStats = null;
    private List<int> flowsStartedPerSecond = new List<int>(60);
    private List<ScottPlot.Color> defaultColors = new List<ScottPlot.Color>(50);
    public frmStatistics()
    {
      InitializeComponent();
    }

    private void frmStatistics_Load(object sender, EventArgs e)
    {
      cEventManager.StatisticsHandler += cEventManager_StatisticsHandler;

      for (int x = 0; x < 60; x++)
        flowsStartedPerSecond.Add(0);  //Setup the flows per second, initialize it with 60 seconds of data

      cmbFrequency.Items.Add(new ComboFrequency(STATISTICS_UPDATE_FREQUENCY.SuperFast));
      cmbFrequency.Items.Add(new ComboFrequency(STATISTICS_UPDATE_FREQUENCY.Fast));
      cmbFrequency.Items.Add(new ComboFrequency(STATISTICS_UPDATE_FREQUENCY.Medium));
      cmbFrequency.Items.Add(new ComboFrequency(STATISTICS_UPDATE_FREQUENCY.Slow));
      cmbFrequency.SelectedIndex = 0; //SuperFast
      tslStatus.Text = STATS_INACTIVE;
      tslStatus.ForeColor = System.Drawing.Color.Red;

      for (int x = 0; x < 50; x++)
      {
        defaultColors.Add(ScottPlot.Color.RandomHue());
      }
      //StatisticsRegister reg = new(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, FlowEngineCore.Statistics.STATISTICS_UPDATE_FREQUENCY.Medium);
      //cServer.SendAndResponse(reg.GetPacket(), Callback_StatisticsRegister);
    }

    private void Callback_StatisticsRegister(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() == FlowEngineCore.Administration.Messages.BaseResponse.RESPONSE_CODE.Success)
      {
        tslStatus.Text = STATS_ACTIVE;
        tslStatus.ForeColor = Color.Green;
      }
    }
    private void Callback_StatisticsDeregister(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() == FlowEngineCore.Administration.Messages.BaseResponse.RESPONSE_CODE.Success)
      {
        tslStatus.Text = STATS_INACTIVE;
        tslStatus.ForeColor = Color.Red;
      }
    }

    private void cEventManager_StatisticsHandler(object? sender, FlowEngineCore.Administration.Packet packet)
    {
      packet.GetData(out mCurrentSecond);
      packet.GetData(out int secondsOfData);
      int startIndex = mCurrentSecond - secondsOfData;
      int firstBatchStartIndex = -1;
      if (startIndex < 0)
      {
        firstBatchStartIndex = 60 + startIndex;
        startIndex = 0;
      }
      if (firstBatchStartIndex >= 0)
      {
        for (int x = firstBatchStartIndex; x < 60; x++)
        {
          packet.GetData(out int flowCount);
          flowsStartedPerSecond[x] = flowCount;
        }
      }
      for (int x = startIndex; x < mCurrentSecond; x++)
      {
        packet.GetData(out int flowCount);
        flowsStartedPerSecond[x] = flowCount;
      }
      Statistic statistic = new Statistic(packet);

      //packet.GetData(out bool containsOldData);
      List<Statistic> tempStatistics = new List<Statistic>();
      //if (containsOldData == true)
      //{
        packet.GetData(out int oldDataCount);
        for (int x = 0; x < oldDataCount; x++)
        {
          Statistic oldData = new Statistic(packet);
          tempStatistics.Add(oldData);
        }
      //}

      OldStatistics.Clear();
      OldStatistics.Add(statistic);
      OldStatistics.AddRange(tempStatistics);

      OldStatistics.Sort((s1,s2) => s2.StartTime.CompareTo(s1.StartTime));

      this.Invoke(new Action(() =>
      {
        int selectedIndex = cmbTimePeriod.SelectedIndex;
        cmbTimePeriod.Items.Clear();
        cmbTimePeriod.Items.AddRange(OldStatistics.ToArray());
        if (selectedIndex == -1 && cmbTimePeriod.Items.Count > 0)
          cmbTimePeriod.SelectedIndex = 0;
        else
          cmbTimePeriod.SelectedIndex = selectedIndex;

      }));
    }



    private void UpdateStatisticsOnForm()
    {

      if (CurrentStats is null)
        return;

      int maxY = flowsStartedPerSecond.Max();
      if (maxY < 10)
        maxY = 10;
      else
        maxY += 2; //Still want a roof buffer
      plotFlowsPerSecond.Plot.Clear();
      plotFlowsPerSecond.Plot.Axes.SetLimitsX(0, 59);
      plotFlowsPerSecond.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic();
      plotFlowsPerSecond.Plot.Axes.SetLimitsY(0, maxY);
      plotFlowsPerSecond.Plot.XLabel("Seconds");
      plotFlowsPerSecond.Plot.YLabel("Flows");

      plotFlowsPerSecond.Plot.Add.Scatter(xSeconds, flowsStartedPerSecond.ToArray());

      var verticalLine = plotFlowsPerSecond.Plot.Add.VerticalLine(mCurrentSecond - 1);
      verticalLine.Color = ScottPlot.Color.FromColor(Color.Red);
      plotFlowsPerSecond.Plot.Title("Flows started in last minute");
      plotFlowsPerSecond.Refresh();

      //Show pie chart of the ratio of flows executed in this 15 minute period.
      List<ScottPlot.PieSlice> pieSlices = new(16);
      int colorIndex = 0;
      foreach (var val in CurrentStats.mFlowsRequestsStarted)
      {
        ScottPlot.PieSlice slice = new ScottPlot.PieSlice(val.Value, defaultColors[colorIndex++], val.Key + " (" + val.Value + ")");
        pieSlices.Add(slice);
      }
      plotFlowsPie.Plot.Clear();
      ScottPlot.Plottables.Pie pie = plotFlowsPie.Plot.Add.Pie(pieSlices);

      plotFlowsPie.Plot.Axes.SetLimitsX(-1.5, 1.5);
      plotFlowsPie.Plot.Axes.SetLimitsY(-1.5, 1.5);
      plotFlowsPie.Refresh();

      //Populate the flows execution times into the list view
      int selectedIndex = -1;
      if (lvFlows.SelectedIndices.Count > 0)
        selectedIndex = lvFlows.SelectedIndices[0];
      lvFlows.Items.Clear();
      foreach (var val in CurrentStats.mFlowTime)
      {
        Statistic.StatisticsTime st = val.Value;
        ListViewItem lvi = lvFlows.Items.Add(val.Key);
        lvi.SubItems.Add(st.CountOfTimes.ToString());
        lvi.SubItems.Add(FlowEngineCore.Global.ConvertToString(st.MinTime, false));
        lvi.SubItems.Add(FlowEngineCore.Global.ConvertToString(st.AvgTime, false));
        lvi.SubItems.Add(FlowEngineCore.Global.ConvertToString(st.MaxTime, false));
      }
      //Reselect the selected item, if there was one.
      if (selectedIndex > -1 && lvFlows.Items.Count > selectedIndex)
        lvFlows.SelectedIndices.Add(selectedIndex);
    }


    private void tslStatus_Click(object sender, EventArgs e)
    {
      if (tslStatus.Text == STATS_ACTIVE)
      {
        StatisticsDeregister reg = new(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey);
        cServer.SendAndResponse(reg.GetPacket(), Callback_StatisticsDeregister);
      }
      else
      {
        StatisticsRegister reg = new(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey, FlowEngineCore.Statistics.STATISTICS_UPDATE_FREQUENCY.Slow);
        cServer.SendAndResponse(reg.GetPacket(), Callback_StatisticsRegister);
      }
      tslStatus.ForeColor = Color.Blue;
      tslStatus.Text = "Request in progress";
    }

    private void frmStatistics_FormClosing(object sender, FormClosingEventArgs e)
    {
      cEventManager.StatisticsHandler -= cEventManager_StatisticsHandler;
      StatisticsDeregister reg = new(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey);
      cServer.SendAndResponse(reg.GetPacket(), Callback_StatisticsDeregister);

    }

    private void cmbFrequency_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cmbFrequency.SelectedIndex == -1)
        return;

      ComboFrequency? freq = cmbFrequency.SelectedItem as ComboFrequency;

      StatisticsRegister reg = new(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey, freq!.Frequency);
      cServer.SendAndResponse(reg.GetPacket(), Callback_StatisticsRegister);

    }

    private void cmbTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cmbTimePeriod.SelectedIndex == -1)
        return;

      CurrentStats = cmbTimePeriod.SelectedItem as Statistic;
      UpdateStatisticsOnForm();
    }
  }
}
