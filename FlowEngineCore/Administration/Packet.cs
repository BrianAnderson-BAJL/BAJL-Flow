using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using static FlowEngineCore.Administration.Messages.BaseResponse;
using FlowEngineCore.Administration.Messages;
using System.Net.Security;

namespace FlowEngineCore.Administration
{
  public class Packet
  {
    /// <summary>
    /// Response messages are allways one bigger in the enum, they MUST be 1 (one) bigger, the MessageProcessor will add 1 to the packet type to send an access denied response
    /// </summary>
    public enum PACKET_TYPE
    {
      _Unknown = 0,
      CloseConnection,
      Trace,
      About,
      AboutResponse,
      UserLogin,
      UserLoginResponse,
      UserLogout,
      UserLogoutResponse,
      UserAdd,
      UserAddResponse,
      UserEdit,
      UserEditResponse,
      UserDelete,
      UserDeleteResponse,
      UserLoginIdCheck,
      UserLoginIdCheckResponse,
      UsersGet,
      UsersGetResponse,
      UserChangePassword,
      UserChangePasswordResponse,
      SecurityProfilesGet,
      SecurityProfilesGetResponse,
      SecurityProfileAdd,
      SecurityProfileAddResponse,
      SecurityProfileEdit,
      SecurityProfileEditResponse,
      SecurityProfileDelete,
      SecurityProfileDeleteResponse,
      FlowSave,
      FlowSaveResponse,
      FlowOpen,
      FlowOpenResponse,
      FlowsGet,
      FlowsGetResponse,
      FlowDebug,         // debug a specific flow and register for events
      FlowDebugResponse,
      FlowDebugAlways,   // register for all debug events even if not debugging a specific flow
      FlowDebugAlwaysResponse,
      TemplatesGet,
      TemplatesGetResponse,
      ServerSettingsGet,
      ServerSettingsGetResponse,
      ServerSettingsEdit,
      ServerSettingsEditResponse,
      StatisticsRegister,
      StatisticsRegisterResponse,
      StatisticsData,
      StatisticsDeregister,
      StatisticsDeregisterResponse,
      ServerPluginsGet,
      ServerPluginsGetResponse,
      zMaxValue,
    }
    private static int NextPacketId = 0;
    public PACKET_TYPE PacketType = PACKET_TYPE._Unknown;
    private int mReadPosition;
    private byte[] ReceiveData = { };
    private List<byte> SendData = new(1024);
    public int PacketId { get; private set; } = 0;
    public static bool ReversePacketId = false;

    private int GetNextPacketId()
    {
      if (ReversePacketId == true)
      {
        if (NextPacketId <= int.MinValue)
          NextPacketId = -1;
        else
          NextPacketId--;
      }
      else
      {
        if (NextPacketId >= int.MaxValue)
          NextPacketId = 1;
        else
          NextPacketId++;
      }
      return NextPacketId;
    }

    public int SendLength
    { 
      get { return SendData.Count; } 
    }

    public byte[] DataToSend
    {
      get {return SendData.ToArray(); }
    }

    public Packet()
    {
      mReadPosition = 0;
    }

    public Packet(PACKET_TYPE packetType, int packetId = 0)
    {
      //Add buffer space for the length
      SendData.Add(0);
      SendData.Add(0);
      SendData.Add(0);
      SendData.Add(0);
      PacketType = packetType;
      if (packetId == 0)
        PacketId = GetNextPacketId();
      else
        PacketId = packetId;
      AddData(packetType);
      AddData(PacketId);
     
    }

    public BaseResponse.RESPONSE_CODE PeekResponseCode()
    {
      PeekData(out int Val);
      return (BaseResponse.RESPONSE_CODE)Val;
    }

    public void ReadAllData(BinaryReader br)
    {
      byte[] temp = br.ReadBytes(sizeof(int));
      if (temp is not null && temp.Length == 4)
      {
        int length = BinaryPrimitives.ReadInt32BigEndian(temp);

        ReceiveData = br.ReadBytes(length);
        GetData(out PacketType);
        GetData(out int val); //Can't put 'PacketId' property here, gives an error about using properties in out or ref parameters
        PacketId = val;
      }
    }

    public void ReadAllTlsData(SslStream stream, byte[] temp)
    {
      if (temp.Length == 4)
      {
        int length = BinaryPrimitives.ReadInt32BigEndian(temp);
        ReceiveData = new byte[length];
        stream.ReadTimeout = 5000; //If we don't receive the rest of the packet within 5 seconds the connection will be killed
        int totalReceiveAmount = 0;
        do
        {
          int rcvAmount = stream.Read(ReceiveData, totalReceiveAmount, length);
          length -= rcvAmount;
          totalReceiveAmount += rcvAmount;
        } while (length > 0);
        GetData(out PacketType);
        GetData(out int val); //Can't put 'PacketId' property here, gives an error about using properties in out or ref parameters
        PacketId = val;
      }
    }

    public void ResetReadPosition()
    {
      mReadPosition = sizeof(PACKET_TYPE) + sizeof(int); //Default position is just after the packet type and the packet id which is a short, two bytes and an int, 4 bytes
    }

    public void AddData(Enum Val)
    {
      AddData(Convert.ToInt32(Val));
    }

    public void AddData(bool Val)
    {
      byte temp = 0;
      if (Val == true)
        temp = 1;
      AddData(temp);
    }

    public void AddData(byte Val)
    {
      SendData.Add(Val);
    }

    public void AddData(byte[] Val)
    {
      SendData.AddRange(Val);
    }

    public void AddData(short Val)
    {
      int size = sizeof(short);
      byte[] Temp = new byte[size];
      BinaryPrimitives.WriteInt16BigEndian(Temp, Val);
      SendData.AddRange(Temp);
    }

    public void AddData(int Val)
    {
      int size = sizeof(int);
      byte[] Temp = new byte[size];
      BinaryPrimitives.WriteInt32BigEndian(Temp, Val);
      SendData.AddRange(Temp);
    }

    public void AddData(long Val)
    {
      int size = sizeof(long);
      byte[] Temp = new byte[size];
      BinaryPrimitives.WriteInt64BigEndian(Temp, Val);
      SendData.AddRange(Temp);
    }

    public void AddData(DateTime Val)
    {
      AddData(Val.Ticks);
    }

    public void AddData(TimeSpan Val)
    {
      AddData(Val.Ticks);
    }
    /// <summary>
    /// decimals are stored in 4 32 bit integers internally, so lets treat them as 4 ints
    /// </summary>
    /// <param name="Val"></param>
    public void AddData(decimal Val)
    {
      int size = sizeof(int);
      int[] temp = new int[size];
      decimal.GetBits(Val, temp);
      for (int x = 0; x < temp.Length; x++)
      {
        byte[] tempBytes = new byte[size];
        BinaryPrimitives.WriteInt32BigEndian(tempBytes, temp[x]);
        SendData.AddRange(tempBytes);
      }
    }

    public void AddData(string Val)
    {
      byte[] Temp = Encoding.UTF8.GetBytes(Val);
      AddData(Temp.Length); //Add the length of the string to the packet
      SendData.AddRange(Temp);
    }

    public void GetData(out FlowEngineCore.Administration.Messages.BaseResponse.RESPONSE_CODE Val)
    {
      GetData(out int temp);
      Val = (RESPONSE_CODE)temp;
    }

    public void PeekData(out byte[] Val, int length)
    {
      Val = new byte[length];
      Array.Copy(ReceiveData, mReadPosition, Val, 0, length);
    }

    
    /// <summary>
    /// This function will only work if the enum is using the default int (4 bytes) size of an enum
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="val"></param>
    public void GetData<T>(out T val) where T : struct, System.Enum, IConvertible
    {
      GetData(out int temp);
      val = (T)(object)temp; //That is rather ugly, convert to object, then to T (enum)
    }

    public void GetData(out byte[] Val, int length)
    {
      Val = new byte[length];
      Array.Copy(ReceiveData, mReadPosition, Val, 0, length);
      mReadPosition += length;
    }

    public void GetData(out bool Val)
    {
      Val = false;
      byte temp = ReceiveData[mReadPosition];
      if (temp != 0)
        Val = true;
      mReadPosition += 1;
    }

    public void GetData(out byte Val)
    {
      Val = ReceiveData[mReadPosition];
      mReadPosition += 1;
    }

    public void GetData(out short Val)
    {
      GetData(out byte[] temp, sizeof(short));
      Val = BinaryPrimitives.ReadInt16BigEndian(temp);
      //mPosition was added to already in the GetData above.
    }

    public void PeekData(out int Val)
    {
      PeekData(out byte[] temp, sizeof(int));
      Val = BinaryPrimitives.ReadInt32BigEndian(temp);
      //mPosition was added to already in the GetData above.
    }

    public void GetData(out int Val)
    {
      GetData(out byte[] temp, sizeof(int));
      Val = BinaryPrimitives.ReadInt32BigEndian(temp);
      //mPosition was added to already in the GetData above.
    }

    public void GetData(out long Val)
    {
      GetData(out byte[] temp, sizeof(long));
      Val = BinaryPrimitives.ReadInt64BigEndian(temp);
      //mPosition was added to already in the GetData above.
    }

    public void GetData(out DateTime Val)
    {
      GetData(out long ticks);
      Val = new DateTime(ticks);
    }
    public void GetData(out TimeSpan Val)
    {
      GetData(out long ticks);
      Val = TimeSpan.FromTicks(ticks);
    }

    public void GetData(out decimal Val)
    {
      int size = sizeof(int);
      int[] temp = new int[size];

      Val = 0;
      for (int x = 0; x < temp.Length; x++)
      {
        GetData(out byte[] tempData, size);
        temp[x] = BinaryPrimitives.ReadInt32BigEndian(tempData);
        //mPosition was added to already in the GetData above.
      }
      Val = new decimal(temp);
    }

    
    public void GetData(out string Val)
    {
      GetData(out int Len);
      Val = Encoding.UTF8.GetString(ReceiveData, mReadPosition, Len);
      mReadPosition += Len;
    }

    /// <summary>
    /// Will write the length of the packet to the beginning of the data array
    /// </summary>
    public void FinalizePacketBeforeSending()
    {
      byte[] Temp = new byte[sizeof(int)];
      BinaryPrimitives.WriteInt32BigEndian(Temp, SendData.Count - 4);
      for (int x = 0; x < Temp.Length; x++)
      {
        SendData[x] = Temp[x];
      }
    }



  }
}