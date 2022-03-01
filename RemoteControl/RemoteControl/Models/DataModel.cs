using System.ComponentModel;
using System.Linq;

namespace RemoteControl.Models
{
    public class DataModel : INotifyPropertyChanged
    {
        public const uint Error = 0xffffffff;

        uint snum = Error;
        public uint SNum
        {
            get => snum;
            set
            {
                snum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SNum)));
            }
        }

        uint current = Error;
        public uint Current { get => current; set => current = value; }

        uint maxi = Error;
        public uint Maxi { get => maxi; set => maxi = value; }
        
        uint remaining = Error;
        public uint Remaining
        {
            get => remaining;
            set
            {
                remaining = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Remaining)));
            }
        }

        uint[] aptid = new uint[3] { Error, Error, Error };

        public uint[] aptId
        {
            get => aptid;
            set
            {
                aptid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(aptId)));
            }
        }

        public string AptId
        {
            get => aptid.Aggregate("", (r, m) => r += m.ToString("X") + "   ");
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptId)));
            }
        }

        public IUsbDevice UsbDevice;

        public event PropertyChangedEventHandler PropertyChanged;

        public DataModel(IUsbDevice usbDevice)
        {
            UsbDevice = usbDevice;
        }

    }
}
