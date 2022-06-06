using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace RemoteControl.Models
{
    public class RfId : Packet, INotifyPropertyChanged
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe struct ReadTypeCUIIResponse
        {
            [MarshalAs(UnmanagedType.U2)]
            public byte Preamble;
            public byte MsgType;
            public byte Code;
            public byte PLMSB;
            public byte PLLSB;
            public byte PCMSB;
            public byte PCLSB;
            public byte EPCMSB;
            public byte EPC1;
            public byte EPC2;
            public byte EPC3;
            public byte EPC4;
            public byte EPC5;
            public byte EPC6;
            public byte EPC7;
            public byte EPC8;
            public byte EPC9;
            public byte EPC10;
            public byte EPCLSB;
            public byte End_Mark;
            public byte CRC_16MSB;
            public byte CRC_16LSB;
        }

        private byte[] epc = new byte[12].Select(e => e = BERROR).ToArray();
        public byte[] EPC
        {
            get => epc;
            set
            {
                epc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EPC)));
            }
        }

        private const uint UERROR = 0xFFFFFFFF;
        private const byte BERROR = 0xFF;

        public const byte PREEMBLE = 0xBB;
        public const byte END_MARK = 0x7E;
        private const byte MSG_TYPE_READ_TYPE_C_UII = 0x00;
        private const byte MSG_TYPE_RESPONSE = 0x01;
        private const byte CODE_READ_TYPE_C_UII = 0x22;
        private const byte CODE_ERROR = 0xFF;

        public event PropertyChangedEventHandler PropertyChanged;

        public RfId()
        {
            Sop = PREEMBLE;
            Eop = END_MARK;

            //epc = new byte[12].Select(e => e = BERROR).ToArray();

            Dcheck = CrcCalc;
            Dassign = PacketAssign;
        }

        public unsafe bool PacketAssign(byte[] buffer)
        {
            //unsafe //we'll now pin unmanaged struct over managed byte array
            //{
            if (buffer.Length == sizeof(ReadTypeCUIIResponse))
            {
                //if (PacketGet(buffer, ref res, sizeof(ReadTypeCUIIResponse)))
                fixed (byte* pbuffer = buffer)
                {
                    ReadTypeCUIIResponse* readTypeCUIIResponse = (ReadTypeCUIIResponse*)pbuffer;
                    if (readTypeCUIIResponse->MsgType == MSG_TYPE_RESPONSE)
                    {
                        if (readTypeCUIIResponse->Code == CODE_READ_TYPE_C_UII)
                        {
                            //if (ArrayToUshort((byte*)&readTypeCUIIResponse->CRC_16MSB) == CrcCalc(res.Skip(1).Take(sizeof(ReadTypeCUIIResponse) - 3).ToArray()))
                            //{
                            EPC[0] = readTypeCUIIResponse->EPCMSB;
                            EPC[1] = readTypeCUIIResponse->EPC1;
                            EPC[2] = readTypeCUIIResponse->EPC2;
                            EPC[3] = readTypeCUIIResponse->EPC3;
                            EPC[4] = readTypeCUIIResponse->EPC4;
                            EPC[5] = readTypeCUIIResponse->EPC5;
                            EPC[6] = readTypeCUIIResponse->EPC6;
                            EPC[7] = readTypeCUIIResponse->EPC7;
                            EPC[8] = readTypeCUIIResponse->EPC8;
                            EPC[9] = readTypeCUIIResponse->EPC9;
                            EPC[10] = readTypeCUIIResponse->EPC10;
                            EPC[11] = readTypeCUIIResponse->EPCLSB;
                            //buffer = res.Skip(sizeof(ReadTypeCUIIResponse)).ToArray();
                            return true;
                            //}
                        }
                    }
                }
            }
            //buffer = res.Skip(sizeof(ReadTypeCUIIResponse)).ToArray();
            //}
            return false;
        }

        public byte[] PacketBuild()
        {
            return new byte[] { PREEMBLE }.Concat(
                CrcAppend(new byte[] { MSG_TYPE_READ_TYPE_C_UII, CODE_READ_TYPE_C_UII, 0, 0, END_MARK })).ToArray();
        }

        public ushort CrcCalc(byte[] buffer)
        {
            ushort crc = 0xFFFF;
            buffer = buffer.Skip(1).Take(buffer.Length - 3).ToArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                crc ^= (ushort)(buffer[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            return crc;
        }

        private byte[] CrcAppend(byte[] buffer)
        {
            //ushort crc = 0;
            //for (int i = 0; i < buffer.Length; i++)
            //{
            //    crc ^= (ushort)(buffer[i] << 8);
            //    for (int j = 0; j < 8; j++)
            //    {
            //        if ((crc & 0x8000) != 0)
            //            crc = (ushort)((crc << 1) ^ 0x1021);
            //        else
            //            crc <<= 1;
            //    }
            //}
            return buffer.Concat(UshortToArray(CrcCalc(buffer))).ToArray();
        }
    }
}
