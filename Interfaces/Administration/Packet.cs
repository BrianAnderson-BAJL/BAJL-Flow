using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using static Core.Administration.Messages.BaseResponse;
using Core.Administration.Messages;

namespace Core.Administration
{
  public class Packet
  {
    /// <summary>
    /// Response messages are allways one bigger in the enum, they MUST be 1 (one) bigger, the MessageProcessor will add 1 to the packet type to send an access denied response
    /// </summary>
    public enum PACKET_TYPE : short
    {
      _Unknown = 0,
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
    }

    public PACKET_TYPE PacketType = PACKET_TYPE._Unknown;
    private int mLength;
    private int mPosition;
    //private Span<byte> Data = new Span<byte>();
    public byte[] Data = { };

    public int Length
    { 
      get { return mLength + sizeof(int); } 
    }

    public Packet()
    {
      mPosition = 0;
      mLength = 0; //Length does not include the size of the length itself
    }

    public Packet(PACKET_TYPE packetType, int maxSize = 1024)
    {
      PacketType = packetType;
      Data = new byte[maxSize];
      mPosition = 4; // Need to leave room for the length that is inserted in FinalizePacketBeforeSending()
      AddData((short)packetType);
    }

    public BaseResponse.RESPONSE_CODE PeekResponseCode()
    {
      int Val;
      PeekData(out Val);
      return (BaseResponse.RESPONSE_CODE)Val;
    }

    public void ReadAllData(BinaryReader br)
    {
      byte[] temp = br.ReadBytes(sizeof(int));

      mLength = BinaryPrimitives.ReadInt32BigEndian(temp);

      Data = br.ReadBytes(mLength);
      short packetType;
      GetData(out packetType);
      PacketType = (PACKET_TYPE)packetType;
    }

    public void ResetReadPosition()
    {
      mPosition = 2; //Default position is just after the packet type which is a short, two bytes
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
      Data[mPosition] = temp;
      mLength += 1;
      mPosition += 1;
    }

    public void AddData(byte Val)
    {
      Data[mPosition] = Val;
      mLength += 1;
      mPosition += 1;
    }

    public void AddData(byte[] Val)
    {
      Array.Copy(Val, Data, Val.Length);
      mLength += Val.Length;
      mPosition += Val.Length;
    }

    public void AddData(short Val)
    {
      int size = sizeof(short);
      byte[] Temp = new byte[size];
      BinaryPrimitives.WriteInt16BigEndian(Temp, Val);
      Temp.CopyTo(Data, mPosition);
      mLength += size;
      mPosition += size;
    }

    public void AddData(int Val)
    {
      int size = sizeof(int);
      byte[] Temp = new byte[size];
      BinaryPrimitives.WriteInt32BigEndian(Temp, Val);
      Temp.CopyTo(Data, mPosition);
      mLength += size;
      mPosition += size;
    }

    public void AddData(long Val)
    {
      int size = sizeof(long);
      byte[] Temp = new byte[size];
      BinaryPrimitives.WriteInt64BigEndian(Temp, Val);
      Temp.CopyTo(Data, mPosition);
      mLength += size;
      mPosition += size;
    }

    public void AddData(DateTime Val)
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
        tempBytes.CopyTo(Data, mPosition);
        mLength += size;
        mPosition += size;
      }
    }

    public void AddData(string Val)
    {
      byte[] Temp = Encoding.UTF8.GetBytes(Val);
      AddData(Temp.Length); //Add the length of the string to the packet
      Temp.CopyTo(Data, mPosition);

      mPosition += Temp.Length;
      mLength += Temp.Length;
    }

    public void GetData(out Core.Administration.Messages.BaseResponse.RESPONSE_CODE Val)
    {
      int temp;
      GetData(out temp);
      Val = (RESPONSE_CODE)temp;
    }

    //public void GetData(out byte[] Val)
    //{
    //  Val = new byte[mLength - (mPosition - 4)];
    //  Array.Copy(Data, mPosition, Val, 0, mLength - (mPosition - 4));
    //  mPosition += Val.Length;
    //}
    public void PeekData(out byte[] Val, int length)
    {
      Val = new byte[length];
      Array.Copy(Data, mPosition, Val, 0, length);
      //mPosition += length;
    }


    public void GetData(out byte[] Val, int length)
    {
      Val = new byte[length];
      Array.Copy(Data, mPosition, Val, 0, length);
      mPosition += length;
    }

    public void GetData(out bool Val)
    {
      Val = false;
      byte temp = Data[mPosition];
      if (temp != 0)
        Val = true;
      mPosition += 1;
    }

    public void GetData(out byte Val)
    {
      Val = 0;
      Val = Data[mPosition];
      mPosition += 1;
    }

    public void GetData(out short Val)
    {
      Val = 0;
      byte[] temp;
      GetData(out temp, sizeof(short));
      Val = BinaryPrimitives.ReadInt16BigEndian(temp);
      //mPosition was added to already in the GetData above.
    }

    public void PeekData(out int Val)
    {
      Val = 0;
      byte[] temp;
      PeekData(out temp, sizeof(int));
      Val = BinaryPrimitives.ReadInt32BigEndian(temp);
      //mPosition was added to already in the GetData above.
    }

    public void GetData(out int Val)
    {
      Val = 0;
      byte[] temp;
      GetData(out temp, sizeof(int));
      Val = BinaryPrimitives.ReadInt32BigEndian(temp);
      //mPosition was added to already in the GetData above.
    }

    public void GetData(out long Val)
    {
      Val = 0;
      byte[] temp;
      GetData(out temp, sizeof(long));
      Val = BinaryPrimitives.ReadInt64BigEndian(temp);
      //mPosition was added to already in the GetData above.
    }

    public void GetData(out DateTime Val)
    {
      long ticks;
      GetData(out ticks);
      Val = new DateTime(ticks);
    }

    public void GetData(out decimal Val)
    {
      int size = sizeof(int);
      int[] temp = new int[size];

      Val = 0;
      for (int x = 0; x < temp.Length; x++)
      {
        byte[] tempData;
        GetData(out tempData, size);
        temp[x] = BinaryPrimitives.ReadInt32BigEndian(tempData);
        //mPosition was added to already in the GetData above.
      }
      Val = new decimal(temp);
    }

    
    public void GetData(out string Val)
    {
      Val = "";
      int Len;
      GetData(out Len);
      Val = Encoding.UTF8.GetString(Data, mPosition, Len);
      mPosition += Len;
    }

    /// <summary>
    /// Will write the length of the packet to the beginning of the data array
    /// </summary>
    public void FinalizePacketBeforeSending()
    {
      byte[] Temp = new byte[sizeof(int)];
      BinaryPrimitives.WriteInt32BigEndian(Temp, mLength);
      Temp.CopyTo(Data, 0);
    }



  }
}