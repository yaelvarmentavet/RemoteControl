using System.Linq;

namespace RemoteControl.Models
{
    //public enum EError
    //{
    //    ERROR = -1,
    //    OK = 0,
    //}

    //public enum ECount
    //{
    //    //public static readonly byte COUNTDOWN = 0x01;
    //    //public static readonly byte COUNTUP = 0x02;
    //    DOWN = 1,
    //    UP = 2
    //}

    //public enum EPulses
    //{
    //    //public static readonly UInt16 PULSES100 = 100;
    //    //public static readonly UInt16 PULSES400 = 400;
    //    PULSES100 = 100,
    //    PULSES400 = 400
    //}

    //public enum EProcess
    //{
    //    //public static readonly byte STATUS = 0x00;
    //    //public static readonly byte START = 0x01;
    //    //public static readonly byte STOP = 0x02;
    //    STATUS = 0,
    //    START = 1,
    //    STOP = 2
    //}

    public class Packet
    {
        public byte Sop;
        public byte Eop;
        public delegate ushort DCheck(byte[] buffer);
        public delegate bool DAssign(byte[] buffer);
        public DCheck Dcheck;
        public DAssign Dassign;

        public unsafe uint ArrayToUshort(byte* buffer)
        {
            return (uint)(*buffer << 8) + (uint)(*(buffer + 1));
        }

        public byte[] UshortToArray(ushort buffer)
        {
            return new byte[] { (byte)((buffer & 0xFF00) >> 8), (byte)(buffer & 0x00FF) };
        }

        public unsafe uint ArrayToUint(byte* buffer)
        {
            return (uint)(*(buffer) << 24) +
                (uint)(*(buffer + 1) << 16) +
                (uint)(*(buffer + 2) << 8) +
                (uint)(*(buffer + 3));
        }

        //public bool PacketGet(byte[] buffer, ref byte[] res, int size)
        public bool PacketGet(byte[] buffer, ref byte[] res, ref bool sopeop)
        {
            //res = buffer.SkipWhile(b => b != Sop).ToArray();
            //while (res.Length >= size)
            //{
            //    //buffer = buffer.SkipWhile(b => b != Sop).ToArray();
            //    res = res.SkipWhile(b => b != Sop).ToArray();
            //    if (res.Length >= size)
            //    {
            //        if (res[size - 1 - 2] == Eop)
            //            return true;
            //        res = res.Skip(1).ToArray();
            //    }
            //}
            //return false;

            //res = buffer.SkipWhile(b => b != Sop).TakeWhile(b => b != Eop).Take(3).ToArray();
            int idx = -1;
            int count = 0;
            bool found = false;
            ushort check = 0;
            res = buffer.SkipWhile(b => b != Sop).Where((b, i) =>
            {
                if (b == Eop)
                {
                    idx = i;
                    found = true;
                }
                if (idx > 0)
                    count++;
                if (count == 2)
                    check = (ushort)(b << 8);
                if (count == 3)
                    check += b;
                if (count > 3)
                    return false;
                return true;
            }).ToArray();
            sopeop = found;
            if (sopeop)
            {
                if (check == Dcheck(res))
                    return true;
            }
            return false;
        }

        public uint PacketParse(ref byte[] buffer, ref bool found)
        {
            byte[] res = new byte[0];
            byte[] buf = buffer;
            uint count = 0;
            bool sopeop = true;

            found = false;

            while (sopeop)
            {
                if (PacketGet(buf, ref res, ref sopeop))
                {
                    if (Dassign(res))
                        found = true;
                    count++;
                }
                buf = buf.Skip(res.Length).ToArray();
            }

            if (count > 0)
                buffer = buf;
            
            return count;
        }
    }
}
