using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RemoteControl.Models
{
    public class DataModel : INotifyPropertyChanged
    {
        //public class Reply
        //{
        //    public bool Found = false;
        //    public byte[] RxBuffer;
        //}

        private string cmtFL;
        public string CmtFL
        {
            get => cmtFL;
            set
            {
                //if (value != null)
                //{
                cmtFL = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CmtFL)));
                //}
            }
        }

        private string cmtRL;
        public string CmtRL
        {
            get => cmtRL;
            set
            {
                //if (value != null)
                //{
                cmtRL = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CmtRL)));
                //}
            }
        }

        private string cmtFR;
        public string CmtFR
        {
            get => cmtFR;
            set
            {
                //if (value != null)
                //{
                cmtFR = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CmtFR)));
                //}
            }
        }

        private string cmtRR;
        public string CmtRR
        {
            get => cmtRR;
            set
            {
                //if (value != null)
                //{
                cmtRR = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CmtRR)));
                //}
            }
        }

        public Color CmtFLColor
        {
            get => cmtFL == "N" || cmtFL == "T" ? Color.Green : Color.Red;
        }

        public Color CmtRLColor
        {
            get => cmtRL == "N" || cmtRL == "T" ? Color.Green : Color.Red;
        }

        public Color CmtFRColor
        {
            get => cmtFR == "N" || cmtFR == "T" ? Color.Green : Color.Red;
        }

        public Color CmtRRColor
        {
            get => cmtRR == "N" || cmtRR == "T" ? Color.Green : Color.Red;
        }

        private uint cowid = UERROR;
        public uint CowId
        {
            get => cowid;
            set
            {
                cowid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CowId)));
            }
        }

        private bool cowIdOk = false;
        public bool CowIdOk
        {
            get => cowIdOk;
            set
            {
                cowIdOk = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CowIdOk)));
            }
        }

        private string tagid = string.Empty;
        public string TagId
        {
            get => tagid;
            set
            {
                tagid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagId)));
            }
        }

        //public float Progress0
        //{
        //    get => (Aptxs[0].ProcessPulses - Aptxs[0].PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress0)));
        //    }
        //}

        //public float Progress1
        //{
        //    get => (Aptxs[1].ProcessPulses - Aptxs[1].PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress1)));
        //    }
        //}

        //public float Progress2
        //{
        //    get => (Aptxs[2].ProcessPulses - Aptxs[2].PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress2)));
        //    }
        //}

        //public float Progress3
        //{
        //    get => (Aptxs[3].ProcessPulses - Aptxs[3].PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress3)));
        //    }
        //}

        private bool fl = false;
        public Color FL
        {
            get => fl ? Color.Cyan : Color.AliceBlue;
            set
            {
                fl = value == Color.Cyan ? true : false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FL)));
            }
        }

        private bool rl = false;
        public Color RL
        {
            get => rl ? Color.Cyan : Color.AliceBlue;
            set
            {
                rl = value == Color.Cyan ? true : false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RL)));
            }
        }

        private bool fr = false;
        public Color FR
        {
            get => fr ? Color.Cyan : Color.AliceBlue;
            set
            {
                fr = value == Color.Cyan ? true : false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FR)));
            }
        }

        private bool rr = false;
        public Color RR
        {
            get => rr ? Color.Cyan : Color.AliceBlue;
            set
            {
                rr = value == Color.Cyan ? true : false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RR)));
            }
        }

        private bool autoTransition = false;
        public bool AutoTransition
        {
            get => autoTransition;
            set
            {
                autoTransition = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoTransition)));
                if (value == true)
                {
                    if (CmtFRColor == Color.Red)
                    {
                        fr = true;
                        FR = FR;
                    }
                    if (CmtRRColor == Color.Red)
                    {
                        rr = true;
                        RR = RR;
                    }
                    if (CmtRLColor == Color.Red)
                    {
                        rl = true;
                        RL = RL;
                    }
                    if (CmtFLColor == Color.Red)
                    {
                        fl = true;
                        FL = FL;
                    }
                }
            }
        }

        private string[] usbPorts = new string[0];
        //public string UsbPorts
        //{
        //    get => usbPorts.Aggregate("", (r, v) => r += v + " ");
        //    set
        //    {
        //        //usbPorts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UsbPorts)));
        //    }
        //}

        //private uint packetCounter = 0;
        //public uint PacketCounter
        //{
        //    get => packetCounter;
        //    set
        //    {
        //        packetCounter = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PacketCounter)));
        //    }
        //}

        //private string devices = string.Empty;
        private Dictionary<string, uint> packetCounters = new Dictionary<string, uint>();
        public string PacketCounters
        {
            get
            {
                //List<TxPacket> txPackets = new List<TxPacket>();
                //if (TxQue.Any())
                //    txPackets = TxQue.Where(t => t.packetType != PacketType.EMPTY).OrderBy(t => t.packetType).ToList();
                //return txPackets.Aggregate("", (r, v) => r += v.packetType + " ") + "\n" +
                //return (TxQue.Where(t => t.packetType != PacketType.EMPTY) != null ? 
                //    TxQue.Where(t => t.packetType != PacketType.EMPTY).OrderBy(t => t.packetType).ToList() :
                //    TxQue).
                //if (PortsDebug)
                //{
                return TxQue.Where(t => t?.packetType != PacketType.EMPTY).
                    OrderBy(t => t?.packetType).
                    Aggregate("", (r, t) => r += t?.packetType + " ") + "\n" +
                    usbPorts.Aggregate("", (r, u) => r += u + " ") + "\n" +
                    packetCounters.Aggregate("", (r, p) => r += p.Key + DELIMITER + p.Value + "\n") +
                    //Aptxs.Aggregate("", (r, a) => r += a.Id + " Pressure " + a.PressureOK + "\n");
                    Aptxs.Aggregate("", (r, a) => r += "APTX" + a.Id + DELIMITER + "PRESSURE" + DELIMITER + a.Pressure + "\n");
                //}
                //else
                //{
                //    return string.Empty;
                //}
            }
            //get => packetCounters.Aggregate("", (r, v) => r += v + DELIMITER);
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PacketCounters)));
            }
        }

        private bool portsDebug = true;
        public bool PortsDebug
        {
            get => portsDebug;
            set
            {
                portsDebug = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PortsDebug)));
            }
        }

        public Color PortsDebugColor
        {
            get => portsDebug ? Color.Green : Color.Default;
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PortsDebugColor)));
            }
        }

        private readonly string version = VERSION + " " + DateTime.Now;
        public string Version
        {
            get => version;
        }

        //private uint processPulses0 = UERROR;
        //public uint ProcessPulses0
        //{
        //    get => processPulses0;
        //    set
        //    {
        //        processPulses0 = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessPulses0)));
        //    }
        //}

        //private uint processPulses1 = UERROR;
        //public uint ProcessPulses1
        //{
        //    get => processPulses1;
        //    set
        //    {
        //        processPulses1 = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessPulses1)));
        //    }
        //}

        //private uint processPulses2 = UERROR;
        //public uint ProcessPulses2
        //{
        //    get => processPulses2;
        //    set
        //    {
        //        processPulses2 = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessPulses2)));
        //    }
        //}

        //private uint processPulses3 = UERROR;
        //public uint ProcessPulses3
        //{
        //    get => processPulses3;
        //    set
        //    {
        //        processPulses3 = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessPulses3)));
        //    }
        //}

        //public float Progress0
        //{
        //    //get => (ProcessPulses - PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    get => (float)ProcessPulses0 / (float)Aptx.ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress0)));
        //    }
        //}

        //public float Progress1
        //{
        //    //get => (ProcessPulses - PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    get => (float)ProcessPulses1 / (float)Aptx.ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress1)));
        //    }
        //}

        //public float Progress2
        //{
        //    //get => (ProcessPulses - PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    get => (float)ProcessPulses2 / (float)Aptx.ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress2)));
        //    }
        //}

        //public float Progress3
        //{
        //    //get => (ProcessPulses - PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    get => (float)ProcessPulses3 / (float)Aptx.ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress3)));
        //    }
        //}

        private enum PacketType
        {
            EMPTY,
            REMOTE_STATUS_0,
            REMOTE_STATUS_1,
            REMOTE_STATUS_2,
            REMOTE_STATUS_3,
            REMOTE_START,
            REMOTE_STOP,
            RFID_TAG,
            APTX1_ID,
            APTX1_SNUM,
            APTX1_CURRENT,
            APTX1_APTXID,
            ECOMILK_ID,
            ECOMILK_RCW_START,
            ECOMILK_RCW_STOP,
            ECOMILK_RCCW_START,
            ECOMILK_RCCW_STOP,
            ECOMILK_AF_START,
            ECOMILK_AB_START,
            ECOMILK_TCW_START,
            ECOMILK_TCW_STOP,
            ECOMILK_TCCW_START,
            ECOMILK_TCCW_STOP,
            ECOMILK_XF_START,
            ECOMILK_XF_STOP,
            ECOMILK_MZD_STOP,
            ECOMILK_MZD_START,
            ECOMILK_MZU_STOP,
            ECOMILK_MZU_START,
            ECOMILK_AB_STOP,
            ECOMILK_AF_STOP,
        }

        private enum ProcedureType
        {
            APTX2,
            ECOMILK,
        }

        private class TxPacket
        {
            public static TxPacket Empty = new TxPacket() { device = string.Empty, packetType = PacketType.EMPTY };
            public string device;
            public PacketType packetType;
            public byte[] packet;
        }
        
        public const string VERSION = "Armenta - Remote Control Application V1.4";
        public const uint UERROR = 0xFFFFFFFF;
        private const int OK = 0;
        private const int ERROR = -1;
        private enum ProjectType
        {
            APTX2,
            ECOMILK,
        }
        private const ProjectType projectType = ProjectType.APTX2;

        private const string APTX1 = "APTX1";
        private const string REMOTE = "REMOTE";
        private const string ECOMILK = "ECOMILK";
        private const string RFID = "RFID";

        //private const uint APTX_COUNT = 4;

        //private readonly uint[] STATEs; // = new uint[APTX_COUNT];
        //private const int QUARTERS_NUMBER = 4;
        //private const int CONNECT_TIMEOUT = 3000;
        //private const int REQUEST_TIMEOUT = 1000; // in msec
        //private const int REQUEST_RETRIES = 3;
        //private const int REPLY_TIMEOUT = 1000; // in msec
        //private const int REPLY_RETRIES = 3;
        //private const int TXQUE_TIMEOUT = 1;
        private const int TXDEQUE_TIMEOUT = 100;
        //private const int TXRX_RETRIES = 1;
        //private const int RXBUFFER_SIZE = 1024;

        private const string LOGFILE_COWS = "LOGFILE_COWS";
        private const int MAX_RXBUFFER_LENGTH = 1024;
        private const string DELIMITER = "/";

        //public Command AddCow { get; }
        public Command TappedFL { get; }
        public Command TappedRL { get; }
        public Command TappedFR { get; }
        public Command TappedRR { get; }

        public Command NextPageCMT { get; }
        public Command NextPageTreatment { get; }

        public Command PortsDebugSwitch { get; }

        //public string[] Devices;

        //public Aptx[] Aptxs = new Aptx[Aptx.APTXIDs.Length].Select((a, i) => { a = new Aptx(); a.Id = Aptx.APTXIDs[i]; return a; }).ToArray();
        public Aptx[] Aptxs = new Aptx[Aptx.APTXIDs.Length].Select((a, i) => { a = new Aptx(); a.Id = (ushort)i; return a; }).ToArray();
        //private Semaphore SemaphoreAptxs = new Semaphore(1, 1);
        public Aptx Aptx = new Aptx();

        //public Aptx Aptx0 = new Aptx();
        //public Aptx Aptx1 = new Aptx();
        //public Aptx Aptx2 = new Aptx();
        //public Aptx Aptx3 = new Aptx();

        //private Dictionary<uint, string> Cows = new Dictionary<uint, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IUsbSerial UsbSerial;

        //private ConcurrentDictionary<string, string> Ports = new ConcurrentDictionary<string, string>();
        private Dictionary<string, string> Ports = new Dictionary<string, string>();
        //private Semaphore SemaphorePorts = new Semaphore(1, 1);
        private object PortsLock = new object();

        //ManualResetEvent WaitHandleEcomilk = new ManualResetEvent(false);
        //ManualResetEvent WaitHandleRemote = new ManualResetEvent(false);
        //ManualResetEvent WaitHandleRfid = new ManualResetEvent(false);
        //ManualResetEvent WaitHandleAptx1 = new ManualResetEvent(false);

        //System.Collections.Concurrent.ConcurrentDictionary<> conc;
        //private bool Connected = false;
        private byte PauseResume = Aptx.STOP;

        //private byte[] RxBufferRfid = new byte[0];// RXBUFFER_SIZE];
        //private byte[] RxBufferAptx1 = new byte[0];// RXBUFFER_SIZE];
        //private byte[] RxBufferEcomilk = new byte[0];// RXBUFFER_SIZE];
        //private byte[] RxBufferRemote = new byte[0];// RXBUFFER_SIZE];
        private Dictionary<string, byte[]> RxBuffers = new Dictionary<string, byte[]>();
        //private Semaphore SemaphoreRxBuffers = new Semaphore(1, 1);
        private object RxBuffersLock = new object();
        private List<string> RxDump = new List<string>();

        private List<TxPacket> TxQue;
        //private Semaphore SemaphoreTxQue = new Semaphore(1, 1);
        private object TxQueLock = new object();
        //ManualResetEvent WaitHandleTxQue = new ManualResetEvent(false);

        private Timer TxDequeTimer;

        private int Count = 0;
        private uint PulsesPrev = 0;

        private ProcedureType Procedure = ProcedureType.APTX2;

        //private Semaphore SemaphorePacketCounters = new Semaphore(1, 1);
        private object PacketCountersLock = new object();

        public DataModel(IUsbSerial usbSerial)
        {
            //Aptxs = new Aptx[Aptx.APTXIDs.Length].Select((a, i) => { a = new Aptx(); a.Id = Aptx.APTXIDs[i]; return a; }).ToArray();
            //if (Device.RuntimePlatform == Device.Android)
            if (projectType == ProjectType.APTX2)
            {
                //Devices = new string[] { REMOTE };
                //Devices = new string[] { REMOTE, APTX1 };
                TxQue = new List<TxPacket>() {
                    //new TxPacket() { device = ECOMILK, packetType = PacketType.ECOMILK_ID, packet = Encoding.UTF8.GetBytes("ecomilkid\r")},
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_3, packet = Aptxs[3].PacketBuild() },
                    //new TxPacket() { device = APTX1, packetType = PacketType.APTX1_ID, packet = Encoding.UTF8.GetBytes("getid,3#")},
                    //new TxPacket() { device = RFID, packetType = PacketType.RFID_TAG, packet = new RfId().PacketBuild()},
                };
            }
            //else if (Device.RuntimePlatform == Device.UWP)
            else if (projectType == ProjectType.ECOMILK)
            {
                //Devices = new string[] { ECOMILK, REMOTE, RFID, APTX1 };
                //Devices = new string[] { APTX1 };
                //Devices = new string[] { REMOTE };
                TxQue = new List<TxPacket>() {
                    new TxPacket() { device = ECOMILK, packetType = PacketType.ECOMILK_ID, packet = Encoding.UTF8.GetBytes("\recomilkid\r")},
                    
                    new TxPacket() { device = RFID, packetType = PacketType.RFID_TAG, packet = new RfId().PacketBuild()},
                    
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_0, packet = Aptxs[0].PacketBuild() },
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_1, packet = Aptxs[1].PacketBuild() },
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_2, packet = Aptxs[2].PacketBuild() },
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_3, packet = Aptxs[3].PacketBuild() },
                    
                    new TxPacket() { device = APTX1, packetType = PacketType.APTX1_ID, packet = Encoding.UTF8.GetBytes("getid,3#\r")},
                };
                Procedure = ProcedureType.ECOMILK;
            }
            //List<string> l = new List<string>();
            //devices = TxQue.Aggregate("", (r, v) =>
            //        {
            //            if (!l.Contains(v.device))
            //            {
            //                r += v.device + " ";
            //                l.Add(v.device);
            //            }
            //            return r;
            //        });

            this.UsbSerial = usbSerial;
            //this.UsbSerial.Event((sender, args) =>
            //{
            //    SemaphorePorts.WaitOne();
            //    if (args is PortEventArgs)
            //    {
            //        if (Ports.ContainsValue((args as PortEventArgs).Port))
            //        {
            //            //string key = Ports.Select((kv) => kv.Value == (args as PortEventArgs).Port ? kv : new KeyValuePair<string, string>()).First().Key;
            //            string key = Ports.First((kv) => kv.Value == (args as PortEventArgs).Port).Key;
            //            Ports.Remove(key);
            //            CowId = UERROR;
            //            Aptx.SNum = UERROR;
            //            Aptx.CurrentPulses = UERROR;
            //            Aptx.Remaining = UERROR;
            //            Aptx.aptxId[0] = UERROR;
            //            Aptx.aptxId[1] = UERROR;
            //            Aptx.aptxId[2] = UERROR;
            //            packetCounters = new Dictionary<string, uint>();
            //        }
            //    }
            //    SemaphorePorts.Release();
            //}, null);

            PortsDebugSwitch = new Command(() =>
            {
                PortsDebug = PortsDebug ? PortsDebug = false : PortsDebug = true;
            });

            NextPageCMT = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CMTPage());
            });
            NextPageTreatment = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage());
            });
            //AddCow = new Command(() =>
            //{
            //    if (!Cows.ContainsKey(CowId))
            //        Cows.Add(CowId, TagId);
            //});

            //TappedFL = new Command<Label>((lbl) =>
            TappedFL = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                fl = fl ? false : true;
                FL = FL;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundCyan");
            });

            TappedRL = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                rl = rl ? false : true;
                RL = RL;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundCyan");
            });

            TappedFR = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                fr = fr ? false : true;
                FR = FR;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundCyan");
            });

            TappedRR = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                rr = rr ? false : true;
                RR = RR;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundCyan");
            });

            //Task.Run(async () => { while (true) await TxDeque(); });

            TxDequeTimer = new Timer(async (a) => { await TxDeque(); }, null, 0, TXDEQUE_TIMEOUT);
        }

        private async Task TxRx(TxPacket txPacket, string device)
        {
            try
            {
                int ret = -1;
                //for (int i = 0; i < TXRX_RETRIES; i++)
                //{
                if (Ports.TryGetValue(device, out string port))
                {
                    ret = await PortRequest(txPacket, port);
                    await PortReply(device, port);
                }
                else
                {
                    string[] ports = UsbSerial.GetPorts().ToArray();
                    usbPorts = ports;
                    //UsbPorts = UsbPorts;
                    PacketCounters = PacketCounters;
                    foreach (string prt in ports)
                    {
                        if (!Ports.Values.Contains(prt))
                        {
                            ret = await PortRequest(txPacket, prt);
                            //    }
                            //}
                            //foreach (string prt in ports)
                            //{
                            //    if (!Ports.Values.Contains(prt))
                            //    {
                            if (await PortReply(device, prt))
                            {
                                //SemaphorePorts.WaitOne();
                                lock (PortsLock)
                                {
                                    if (!Ports.ContainsKey(device))
                                    {
                                        Ports.Add(device, prt);
                                    }
                                }
                                //SemaphorePorts.Release();
                            }
                        }
                    }
                }
                //}
            }
            catch
            {
                //SemaphorePorts.WaitOne();
                lock (PortsLock)
                {
                    if (Ports.ContainsKey(device))
                        Ports.Remove(device);
                }
                //SemaphorePorts.Release();
                switch (device)
                {
                    case RFID:
                        CowId = UERROR;
                        break;
                    case REMOTE:
                        Aptx.SNum = UERROR;
                        //Aptx.CurrentPulses = UERROR; 
                        Aptx.Pulse = UERROR;
                        Aptx.Remaining = UERROR;
                        //Aptx.aptxId[0] = UERROR;
                        //Aptx.aptxId[1] = UERROR;
                        //Aptx.aptxId[2] = UERROR;
                        Aptx.AptxId = UERROR;
                        //Aptx.AptxId = UERROR;
                        //Aptx.AptxId = UERROR;
                        //Aptx.AptPulsesCurrent = UERROR; 
                        Aptx.AptPulse = UERROR;
                        Aptx.AptRemaining = UERROR;
                        break;
                }
                //SemaphorePacketCounters.WaitOne();
                ////packetCounters = new Dictionary<string, uint>();
                lock (PacketCountersLock)
                {
                    IEnumerable<KeyValuePair<string, uint>> kvs = packetCounters.Where(p => p.Key.Contains(device));
                    foreach (KeyValuePair<string, uint> kv in kvs)
                        packetCounters.Remove(kv.Key);
                }
                //SemaphorePacketCounters.Release();
            }
        }

        private async Task TxDeque()
        {
            //while (true)
            //{
            try
            {
                if (TxQue.Any())
                {
                    TxPacket txPacket = TxQue.First();
                    string device = txPacket.device;

                    //bool res = false;
                    //while (true)
                    //{
                    //    res = await Task.Run(async () => { return await TxRx(txPacket, device); });
                    //    if (res)
                    //        break;
                    //}
                    //TxRx(txPacket, device).Wait(1000);
                    Task.Run(async () => { await TxRx(txPacket, device); });
                    //Task.Run(async () => { TxRx(txPacket, device).Wait(100); });

                    //SemaphoreTxQue.WaitOne();
                    lock (TxQueLock)
                    {
                        TxQue.Remove(txPacket);
                        if (txPacket.packetType != PacketType.EMPTY)
                            TxQue.Add(txPacket);
                    }
                    //SemaphoreTxQue.Release();

                }
                //else
                //    Thread.Sleep(TXRX_TIMEOUT);
            }
            catch
            {
                //Thread.Sleep(TXRX_TIMEOUT);
            }
            //}
        }

        //private async Task<bool> PortReply(string device, string port, Dictionary<string, byte[]> RxBuffers)
        private async Task<bool> PortReply(string device, string port)
        {
            //SemaphoreRxBuffers.WaitOne();
            //if (!RxBuffers.TryGetValue(port, out byte[] rxBuffer))
            //{
            //    //rxBuffers.Add(port, (rxBuffer = new byte[0]));
            //    rxBuffer = new byte[0];
            //    RxBuffers.Add(port, rxBuffer);
            //}
            //if (rxBuffer.Length > MAX_RXBUFFER_LENGTH)
            //    rxBuffer = new byte[0];
            //SemaphoreRxBuffers.Release();

            byte[] buffer = new byte[1024];
            //data += await UsbSerial.Read(port, buffer);
            int length = 0;
            bool found = false;
            string data;
            if ((length = await UsbSerial.Read(port, buffer)) > 0)
            {
                //SemaphoreRxBuffers.WaitOne();
                lock (RxBuffersLock)
                {
                    if (!RxBuffers.TryGetValue(port, out byte[] rxBuffer))
                    {
                        //rxBuffers.Add(port, (rxBuffer = new byte[0]));
                        rxBuffer = new byte[0];
                        RxBuffers.Add(port, rxBuffer);
                    }
                    if (rxBuffer.Length > MAX_RXBUFFER_LENGTH)
                        rxBuffer = new byte[0];
                    //SemaphoreRxBuffers.Release();
                    rxBuffer = rxBuffer.Concat(buffer.Take(length)).ToArray();
                    //}

                    //if (rxBuffer.Length > 0)
                    //if (length > 0)
                    //{
                    switch (device)
                    {
                        case ECOMILK:
                            data = Encoding.UTF8.GetString(rxBuffer);
                            found = data.Contains(device);
                            if (found)
                            {
                                rxBuffer = new byte[0];
                                //PacketCounter++;
                                //SemaphorePacketCounters.WaitOne();
                                lock (PacketCountersLock)
                                {
                                    if (!packetCounters.ContainsKey(port + DELIMITER + device))
                                        packetCounters.Add(port + DELIMITER + device, 0);
                                    packetCounters[port + DELIMITER + device]++;
                                    PacketCounters = PacketCounters;
                                }
                                //SemaphorePacketCounters.Release();
                            }
                            break;
                        case RFID:
                            //byte[] RxBuffer = new byte[20];
                            //RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
                            //RxBuffer[10] = RfId.PREEMBLE;
                            //RxBuffer[10 + 19] = RfId.END_MARK;
                            RfId rfId = new RfId();
                            //RxBuffer[10 + 20] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[0];
                            //RxBuffer[10 + 21] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[1];
                            uint pcktCnt = rfId.PacketParse(ref rxBuffer, ref found);
                            if (pcktCnt > 0)
                            {
                                //SemaphorePacketCounters.WaitOne();
                                lock (PacketCountersLock)
                                {
                                    if (!packetCounters.ContainsKey(port + DELIMITER + device))
                                        packetCounters.Add(port + DELIMITER + device, 0);
                                    packetCounters[port + DELIMITER + device] += pcktCnt;
                                    PacketCounters = PacketCounters;
                                    //PacketCounters.Contains(port + DELIMITER + device);
                                }
                                //SemaphorePacketCounters.Release();
                            }
                            if (found)
                            {
                                unsafe
                                {
                                    fixed (byte* pepc = rfId.EPC)
                                    {
                                        CowId = rfId.ArrayToUint(pepc);
                                    }
                                }
                            }
                            break;
                        case REMOTE:
                            //byte[] RxBuffer = new byte[30];
                            //RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
                            //RxBuffer[10] = Aptx.STX;
                            //RxBuffer[11] = Aptx.APTXIDs[0];
                            //RxBuffer[10 + 32] = Aptx.ETX;
                            //Aptx aptx = new Aptx();
                            //RxBuffer[10 + 33] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[0];
                            //RxBuffer[10 + 34] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[1];
                            RxDump.Add(string.Format("port {0} rxBuffer.Length {1} length {2}", port, rxBuffer.Length, length));
                            //Aptx aptx = new Aptx();
                            pcktCnt = Aptx.PacketParse(ref rxBuffer, ref found);
                            if ((pcktCnt > 0) && (Aptxs.Any(a => a.Id == Aptx.Id)))
                            {
                                //SemaphorePacketCounters.WaitOne();
                                lock (PacketCountersLock)
                                {
                                    if (!packetCounters.ContainsKey(port + DELIMITER + device + DELIMITER + Aptx.Id))
                                        packetCounters.Add(port + DELIMITER + device + DELIMITER + Aptx.Id, 0);
                                    packetCounters[port + DELIMITER + device + DELIMITER + Aptx.Id] += pcktCnt;
                                    PacketCounters = PacketCounters;
                                }
                                //SemaphorePacketCounters.Release();
                                if (found)
                                {
                                    //if (Aptx.APTXIDs.Contains((byte)Aptx.Id))
                                    //SemaphoreAptxs.WaitOne();
                                    //if (Aptxs.Any(a => a.Id == Aptx.Id))
                                    //{
                                    //Aptxs[Aptx.Id] = Aptx;
                                    //Aptxs[aptx.Id] = aptx;
                                    //Aptx = aptx;
                                    AptxUpdate();
                                    //}
                                    //SemaphoreAptxs.Release();
                                }
                            }
                            break;
                        case APTX1:
                            {
                                //data = Encoding.UTF8.GetString(rxBuffer.Where(b => b != 0x00).ToArray());
                                data = Encoding.UTF8.GetString(rxBuffer);
                                if (!Ports.ContainsKey(device))
                                {
                                    found = data.Contains("1F-85-01");
                                    if (!found)
                                        found = data.Contains("0x1f-0x85-0x01");
                                    if (found)
                                    {
                                        rxBuffer = new byte[0];
                                        //PacketCounter++;
                                        //SemaphorePacketCounters.WaitOne();
                                        lock (PacketCountersLock)
                                        {
                                            if (!packetCounters.ContainsKey(port + DELIMITER + device))
                                                packetCounters.Add(port + DELIMITER + device, 0);
                                            packetCounters[port + DELIMITER + device]++;
                                            PacketCounters = PacketCounters;
                                        }
                                        //SemaphorePacketCounters.Release();
                                    }
                                }
                                else
                                {
                                    if (data.Contains("SNUM "))
                                    {
                                        uint[] snum = DataParse(data, "SNUM ", NumberStyles.Number);
                                        if (snum[0] == 123)
                                        {
                                            Aptx.SNum++;
                                            rxBuffer = new byte[0];
                                            //PacketCounter++;
                                            //SemaphorePacketCounters.WaitOne();
                                            lock (PacketCountersLock)
                                            {
                                                if (!packetCounters.ContainsKey(port + DELIMITER + device))
                                                    packetCounters.Add(port + DELIMITER + device, 0);
                                                packetCounters[port + DELIMITER + device]++;
                                                PacketCounters = PacketCounters;
                                            }
                                            //SemaphorePacketCounters.Release();
                                        }
                                    }
                                }
                            }
                            break;
                    }
                    //}

                    //SemaphoreRxBuffers.WaitOne();
                    if (RxBuffers.ContainsKey(port))
                    {
                        RxBuffers.Remove(port);
                    }
                    RxBuffers.Add(port, rxBuffer);
                    //SemaphoreRxBuffers.Release();
                }
            }
            return found;
        }

        //private async Task<int> PortRequest(string device, string port, uint state)
        private async Task<int> PortRequest(TxPacket txPacket, string port)
        {
            int ret = ERROR;
            if (txPacket.packetType != PacketType.EMPTY)
            {
                switch (txPacket.device)
                {
                    case ECOMILK:
                        if (txPacket.packetType != PacketType.ECOMILK_ID)
                            txPacket.packetType = PacketType.EMPTY;
                        break;
                    case REMOTE:
                        if (txPacket.packetType != PacketType.REMOTE_STATUS_0 &&
                            txPacket.packetType != PacketType.REMOTE_STATUS_1 &&
                            txPacket.packetType != PacketType.REMOTE_STATUS_2 &&
                            txPacket.packetType != PacketType.REMOTE_STATUS_3)
                            txPacket.packetType = PacketType.EMPTY;
                        break;
                    case APTX1:
                        if (!Ports.ContainsKey(txPacket.device))
                        {
                            txPacket.packetType = PacketType.APTX1_ID;
                            txPacket.packet = Encoding.UTF8.GetBytes("#getid,3#");
                        }
                        else
                        {
                            txPacket.packetType = PacketType.APTX1_SNUM;
                            txPacket.packet = Encoding.UTF8.GetBytes("#testread,3#");
                        }
                        break;
                }

                //if((!CowIdOk) || (port == "COM14"))
                ret = await UsbSerial.Write(port, txPacket.packet);
            }
            return ret;
        }

        private void AptxUpdate()
        {
            //Aptx.AptxId = Aptx.AptxId;
            //Aptx.Remaining = Aptx.Maxi - Aptx.CurrentPulses;
            //Aptx.PressureOK = Aptx.PressureOK;
            //Aptx.PressureLow = Aptx.PressureLow;
            //Aptx.BatteryOK = Aptx.BatteryOK;
            //Aptx.BatteryLow = Aptx.BatteryLow;
            //Aptx.RemainingOK = Aptx.RemainingOK;
            //Aptx.RemainingLow = Aptx.RemainingLow;
            //Aptx.AptPulsesOK = Aptx.AptPulsesOK;
            //Aptx.AptPulsesLow = Aptx.AptPulsesLow;

            //Aptxs[Aptx.Id].ProcessPulses = Aptxs[Aptx.Id].ProcessPulses;
            //Aptxs[Aptx.Id].Progress = Aptxs[Aptx.Id].Progress;
            //Aptxs[Aptx.Id].StatusMessage = Aptxs[Aptx.Id].StatusMessage;
            //Aptxs[Aptx.Id].StatusColor = Aptxs[Aptx.Id].StatusColor;

            if (Aptxs.Any(a => a.Id == Aptx.Id))
            {
                Aptxs[Aptx.Id].SNum = Aptx.SNum;
                Aptxs[Aptx.Id].Maxi = Aptx.Maxi;
                //Aptxs[Aptx.Id].ProcessPulses = Aptx.ProcessPulses;
                Aptxs[Aptx.Id].OperationPulse = Aptx.OperationPulse;
                //Aptxs[Aptx.Id].aptxId[0] = Aptx.aptxId[0];
                //Aptxs[Aptx.Id].AptxId = Aptx.AptxId;
                Aptxs[Aptx.Id].Pressure = Aptx.Pressure;
                Aptxs[Aptx.Id].Battery = Aptx.Battery;
                //Aptxs[Aptx.Id].MotorIsRunning = Aptx.MotorIsRunning;
                //Aptxs[Aptx.Id].AptPulses = Aptx.AptPulses;
                //Aptxs[Aptx.Id].MotorTemperature = Aptx.MotorTemperature;
                //Aptxs[Aptx.Id].MotorVoltage = Aptx.MotorVoltage;
                //Aptxs[Aptx.Id].SpeedOfBullet = Aptx.SpeedOfBullet;
                //Aptxs[Aptx.Id].Motor = Aptx.Motor;
                Aptxs[Aptx.Id].Operation = Aptx.Operation;
                Aptxs[Aptx.Id].Temperature = Aptx.Temperature;
                Aptxs[Aptx.Id].Voltage = Aptx.Voltage;
                Aptxs[Aptx.Id].Speed = Aptx.Speed;
                Aptxs[Aptx.Id].CowId = Aptx.CowId;
                //Aptxs[Aptx.Id].CurrentPulses = Aptx.CurrentPulses;
                Aptxs[Aptx.Id].Pulse = Aptx.Pulse;

                Aptxs[Aptx.Id].AptxId = Aptx.AptxId;
                //Aptxs[Aptx.Id].AptPulsesCurrent = Aptx.AptPulsesCurrent;
                Aptxs[Aptx.Id].AptPulse = Aptx.AptPulse;
                Aptxs[Aptx.Id].AptRemaining = Aptx.AptRemaining;
                //Aptxs[Aptx.Id].Remaining = Aptx.Maxi - Aptx.CurrentPulses;
                Aptxs[Aptx.Id].Remaining = Aptx.Remaining;
                Aptxs[Aptx.Id].PressureOK = Aptx.PressureOK;
                Aptxs[Aptx.Id].PressureLow = Aptx.PressureLow;
                //Aptxs[Aptx.Id].BatteryOK = Aptx.BatteryOK;
                //Aptxs[Aptx.Id].BatteryLow = Aptx.BatteryLow;
                Aptxs[Aptx.Id].Battery100Per = Aptx.Battery100Per;
                Aptxs[Aptx.Id].Battery75Per = Aptx.Battery75Per;
                Aptxs[Aptx.Id].Battery50Per = Aptx.Battery50Per;
                Aptxs[Aptx.Id].Battery25Per = Aptx.Battery25Per;
                Aptxs[Aptx.Id].Battery15Per = Aptx.Battery15Per;
                Aptxs[Aptx.Id].RemainingOK = Aptx.RemainingOK;
                Aptxs[Aptx.Id].RemainingLow = Aptx.RemainingLow;
                //Aptxs[Aptx.Id].AptPulsesOK = Aptx.AptPulsesOK;
                //Aptxs[Aptx.Id].AptPulsesLow = Aptx.AptPulsesLow;
                Aptxs[Aptx.Id].OperationRun = Aptx.OperationRun;
                Aptxs[Aptx.Id].OperationStop = Aptx.OperationStop;

                //Aptxs[Aptx.Id].ProcessPulses = Aptx.ProcessPulses;
                Aptxs[Aptx.Id].OperationPulse = Aptx.OperationPulse;
                Aptxs[Aptx.Id].Progress = Aptx.Progress;
                Aptxs[Aptx.Id].StatusMessage = Aptx.StatusMessage;
                Aptxs[Aptx.Id].StatusColor = Aptx.StatusColor;
            }
            //Aptxs[0].ProcessPulses = Aptxs[0].ProcessPulses;
            //Aptxs[0].Progress = Aptxs[0].Progress;
            //Aptxs[0].StatusMessage = Aptxs[0].StatusMessage;
            //Aptxs[0].StatusColor = Aptxs[0].StatusColor;
            //Aptxs[1].ProcessPulses = Aptxs[1].ProcessPulses;
            //Aptxs[1].Progress = Aptxs[1].Progress;
            //Aptxs[1].StatusMessage = Aptxs[1].StatusMessage;
            //Aptxs[1].StatusColor = Aptxs[1].StatusColor;
            //Aptxs[2].ProcessPulses = Aptxs[2].ProcessPulses;
            //Aptxs[2].Progress = Aptxs[2].Progress;
            //Aptxs[2].StatusMessage = Aptxs[2].StatusMessage;
            //Aptxs[2].StatusColor = Aptxs[2].StatusColor;
            //Aptxs[3].ProcessPulses = Aptxs[3].ProcessPulses;
            //Aptxs[3].Progress = Aptxs[3].Progress;
            //Aptxs[3].StatusMessage = Aptxs[3].StatusMessage;
            //Aptxs[3].StatusColor = Aptxs[3].StatusColor;
            //if (Aptx.Id == 3)
            //{
            //    Aptxs[3].ProcessPulses = Aptx.ProcessPulses;
            //    Aptxs[3].Progress = Aptx.Progress;
            //    Aptxs[3].StatusMessage = Aptx.StatusMessage;
            //    Aptxs[3].StatusColor = Aptx.StatusColor;
            //}

            switch (Procedure)
            {
                case ProcedureType.APTX2:
                    //if ((Aptx.ProcessPulses == 200) && (PulsesPrev > 190) && (PulsesPrev < 200))
                    if ((Aptx.OperationPulse == 200) && (PulsesPrev > 190) && (PulsesPrev < 200))
                    {
                        Count++;
                        if (Count == 2)
                        {
                            if (AutoTransition)
                            {
                                if (fr)
                                {
                                    fr = false;
                                    FR = FR;
                                    Count = 0;
                                    //CmtSave(string.Format("CmtFR: {0}", CmtFR));
                                }
                                else
                                {
                                    if (rr)
                                    {
                                        rr = false;
                                        RR = RR;
                                        Count = 0;
                                        //CmtSave(string.Format("CmtRR: {0}", CmtRR));
                                    }
                                    else
                                    {
                                        if (rl)
                                        {
                                            rl = false;
                                            RL = RL;
                                            Count = 0;
                                            //CmtSave(string.Format("CmtRL: {0}", CmtRL));
                                        }
                                        else
                                        {
                                            if (fl)
                                            {
                                                fl = false;
                                                FL = FL;
                                                Count = 0;
                                                //CmtSave(string.Format("CmtFL: {0}", CmtFL));
                                            }
                                        }
                                    }
                                }
                            }
                            if (!fl && !rl && !fr && !rr)
                                CmtSave(string.Format("CmtFL: {0} CmtRL: {1} CmtFR: {2} CmtRR: {3}", CmtFL, CmtRL, CmtFR, CmtRR));
                        }
                    }
                    //if ((Aptx.ProcessPulses > 10) && (PulsesPrev < 10) && (Count >= 2))
                    //    Count = 0;
                    //PulsesPrev = Aptx.ProcessPulses;
                    PulsesPrev = Aptx.OperationPulse;
                    break;
                case ProcedureType.ECOMILK:
                    CmtSave(string.Format("CmtFL: {0} CmtRL: {1} CmtFR: {2} CmtRR: {3}", CmtFL, CmtRL, CmtFR, CmtRR));
                    break;
            }
        }

        private uint[] DataParse(string data, string pattern, NumberStyles numberStyles)
        {
            uint[] num = new uint[6];
            string snum = data.Contains(pattern) ?
                new string(data.Substring(data.IndexOf(pattern) + pattern.Length)?.TakeWhile(c => ((c != '\r') && (c != 'p') && (c != ' ')))?.ToArray())
                : string.Empty;
            if (((numberStyles == NumberStyles.HexNumber) && (snum.Length <= 8)) ||
                ((numberStyles == NumberStyles.Number) && (snum.Length <= 10)))
            {
                uint.TryParse(snum,
                             NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | numberStyles,
                             CultureInfo.InvariantCulture,
                             out num[0]);
            }
            else
            {
                for (int i = 0; i < snum.Length / 8; i++)
                {
                    uint.TryParse(new string(snum.Skip(i * 8).Take(8).ToArray()),
                                 NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | numberStyles,
                                 CultureInfo.InvariantCulture,
                                 out num[i]);
                }
            }
            return num;
        }

        public void CmtSave(string cmtData)
        {
            //if (CowId != UERROR)
            //{
            string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
            //File.AppendAllText(LOGFILE_COWS, string.Format("CowId: {0} CmtFL: {1} CmtRL: {2} CmtFR: {3} CmtRR: {4} Date: {5}\n",
            //    CowId, CmtFL, CmtRL, CmtFR, CmtRR, DateTime.Now));
            File.AppendAllText(LOGFILE_COWS, string.Format("CowId: {0} {1} Date: {2}\n",
                CowId, cmtData, DateTime.Now));
            //}
        }

        public void CmtRead()
        {
            string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
            if (File.Exists(LOGFILE_COWS))
            {
                string data = File.ReadAllText(LOGFILE_COWS);
                if (data.Contains("CowId"))
                {
                    //string line = data.Split(new char[] { '\n' }).Where(l => l.Contains("CowId")).Last();
                    //uint[] cowId = DataParse(line, "CowId: ", NumberStyles.Number);
                    //if (cowId[0] == CowId)
                    IEnumerable<string> lines = data.Split(new char[] { '\n' }).Where(l =>
                    {
                        uint[] cowId = DataParse(l, "CowId: ", NumberStyles.Number);
                        if (cowId[0] == CowId)
                            return true;
                        else return false;
                    });
                    if (lines.Any())
                    {
                        string line = lines.Last();
                        CmtFL = CmtParse(line, "CmtFL: ");
                        CmtRL = CmtParse(line, "CmtRL: ");
                        CmtFR = CmtParse(line, "CmtFR: ");
                        CmtRR = CmtParse(line, "CmtRR: ");
                    }
                }
            }
        }

        private string CmtParse(string line, string pattern)
        {
            return new string(line.Substring(line.IndexOf(pattern) + pattern.Length).Take(1).ToArray());
        }

        public int ProcessStart()
        {
            Aptx[] aptxs = Aptxs;
            PauseResume = Aptx.START;
            Task.Run(() =>
            {
                foreach (Aptx aptx in aptxs)
                {
                    //aptx.PulsesPrev = aptx.ProcessPulses;
                    Process(aptx, Aptx.START);
                    //Thread.Sleep(22000);
                }

                //foreach (Aptx aptx in aptxs)
                //{
                //    //aptx.PulsesPrev = aptx.ProcessPulses;
                //    if (aptx.Id == 1 || aptx.Id == 2)
                //        Process(aptx, Aptx.START);
                //    //aptx.ProcessPulses = 0;
                //}

                ////while (Aptxs[1].ProcessPulses < 96 || Aptxs[1].ProcessPulses >= 100)
                //while (aptxs.Where(a => a.Id == 1 || a.Id == 2).Where(a => a.ProcessPulses < 96 || a.ProcessPulses == 100).Any())
                //    Thread.Sleep(1000);

                //Thread.Sleep(22000);

                //foreach (Aptx aptx in aptxs)
                //{
                //    //aptx.PulsesPrev = aptx.ProcessPulses;
                //    if (aptx.Id == 0 || aptx.Id == 3)
                //        Process(aptx, Aptx.START);
                //}

                //for (int i = 0; i < 30; i++)
                //{
                //    Thread.Sleep(1000);
                //    foreach (Aptx aptx in aptxs)
                //    {
                //        if (aptx.Id == 0 || aptx.Id == 3)
                //            Process(aptx, Aptx.RESUME, (ushort)Aptx.PULSES100);
                //    }
                //}
            });
            return OK;
        }

        public int ProcessStop()
        {
            Aptx[] aptxs = Aptxs;
            PauseResume = Aptx.STOP;
            foreach (Aptx aptx in aptxs)
            {
                //if (await Process(aptx, Aptx.STOP) == ERROR)
                //return ERROR;
                Process(aptx, Aptx.STOP);
            }
            return OK;
        }

        public int ProcessPauseResume()
        {
            Aptx[] aptxs = Aptxs;
            //PauseResume = PauseResume == Aptx.STOP ? PauseResume = Aptx.START : PauseResume = Aptx.STOP;
            PauseResume = PauseResume == Aptx.STOP ? Aptx.RESUME : Aptx.STOP;
            foreach (Aptx aptx in aptxs)
            {
                //Process(aptx, PauseResume, (ushort)(Aptx.PULSES100 - (aptx.ProcessPulses - aptx.PulsesPrev)));
                Process(aptx, PauseResume, (ushort)Aptx.PULSES100);
            }
            return OK;
        }

        //public async Task<int> Process(Aptx aptx, byte process, ushort pulses = Aptx.PULSES100)
        public void Process(Aptx aptx, byte process, ushort pulses = Aptx.PULSES100)
        {
            //int response = await UsbSerial.Write(Ports.TryGetValue(REMOTE, out string val) ? val : string.Empty,
            //    aptx.PacketBuild(process, pulses));

            TxQueEnque(REMOTE, process == Aptx.START ? PacketType.REMOTE_START : PacketType.REMOTE_STOP, aptx.PacketBuild(process, pulses));

            //string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
            //File.AppendAllText(LOGFILE_COWS, string.Format("Date: {0} Process: {1} Pulses: {2} Cow Id: {3} Current Pulses: {4}\n",
            //    //DateTime.Now, response == OK ? "OK" : "FAULT", process == Aptx.START ? "START" : "STOP",
            //    DateTime.Now, process == Aptx.START ? "START" : "STOP",
            //    pulses, CowId, aptx.ProcessPulses));
            //return response;
        }

        private void TxQueEnque(string device, PacketType packetType, byte[] packet)
        {
            if (Ports.ContainsKey(device))
            {
                //SemaphoreTxQue.WaitOne();
                lock (TxQueLock)
                {
                    TxQue.Insert(0, new TxPacket()
                    {
                        device = device,
                        packetType = packetType,
                        packet = packet,
                    });
                }
                //SemaphoreTxQue.Release();
            }
        }

        public void RCWStart()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_RCW_START, Encoding.UTF8.GetBytes("rcw 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rcw 1\r"));
        }

        public void RCWStop()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_RCW_STOP, Encoding.UTF8.GetBytes("rcw 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rcw 0\r"));
        }

        public void RCCWStart()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_RCCW_START, Encoding.UTF8.GetBytes("rccw 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rccw 1\r"));
        }

        public void RCCWStop()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_RCCW_STOP, Encoding.UTF8.GetBytes("rccw 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rccw 0\r"));
        }




        public void AFStart()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_AF_START, Encoding.UTF8.GetBytes("ayf 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("af 1\r"));
        }

        public void AFStop()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_AF_STOP, Encoding.UTF8.GetBytes("ayf 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("af 0\r"));
        }

        public void ABStart()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_AB_START, Encoding.UTF8.GetBytes("ayb 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("ab 1\r"));
        }

        public void ABStop()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_AB_STOP, Encoding.UTF8.GetBytes("ayb 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("ab 0\r"));
        }




        public void MZUStart()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_MZU_START, Encoding.UTF8.GetBytes("mzu 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzu 1\r"));
        }

        public void MZUStop()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_MZU_STOP, Encoding.UTF8.GetBytes("mzu 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzu 0\r"));
        }

        public void MZDStart()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_MZD_START, Encoding.UTF8.GetBytes("mzd 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzd 1\r"));
        }

        public void MZDStop()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_MZD_STOP, Encoding.UTF8.GetBytes("mzd 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzd 0\r"));
        }

        public void TCWStart()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_TCW_START, Encoding.UTF8.GetBytes("tcw 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("tcw 1\r"));
        }

        public void TCWStop()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_TCW_STOP, Encoding.UTF8.GetBytes("tcw 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("tcw 1\r"));
        }

        public void TCCWStart()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_TCCW_START, Encoding.UTF8.GetBytes("tccw 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("tcw 1\r"));
        }

        public void TCCWStop()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_TCCW_STOP, Encoding.UTF8.GetBytes("tccw 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("tcw 1\r"));
        }

        public void XFStart()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_XF_START, Encoding.UTF8.GetBytes("xf 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("xf 1\r"));
        }

        public void XFStop()
        {
            TxQueEnque(ECOMILK, PacketType.ECOMILK_XF_STOP, Encoding.UTF8.GetBytes("xf 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("xf 1\r"));
        }

        //private async Task Rx(object odevice)
        //{
        //    Dictionary<string, byte[]> rxBuffers = new Dictionary<string, byte[]>();
        //    if (odevice is string)
        //    {
        //        string device = odevice as string;
        //        string data = string.Empty;
        //        while (true)
        //        {
        //            try
        //            {
        //                switch (device)
        //                {
        //                    case ECOMILK:
        //                        WaitHandleEcomilk.WaitOne();
        //                        break;
        //                    case REMOTE:
        //                        WaitHandleRemote.WaitOne();
        //                        break;
        //                    case RFID:
        //                        WaitHandleRfid.WaitOne();
        //                        break;
        //                    case APTX1:
        //                        WaitHandleAptx1.WaitOne();
        //                        break;
        //                }
        //                if (Ports.TryGetValue(device, out string port))
        //                {
        //                    await PortReply(device, port, rxBuffers);
        //                }
        //                else
        //                {
        //                    string[] ports = UsbSerial.GetPorts().ToArray();
        //                    usbPorts = ports;
        //                    UsbPorts = UsbPorts;
        //                    foreach (string prt in ports)
        //                    {
        //                        if (!Ports.Values.Contains(prt))
        //                        {
        //                            if (await PortReply(device, prt, rxBuffers))
        //                            {
        //                                SemaphorePorts.WaitOne();
        //                                if (!Ports.ContainsKey(device))
        //                                    Ports.Add(device, prt);
        //                                SemaphorePorts.Release();
        //                            }
        //                        }
        //                    }
        //                }
        //                if (usbPorts.Length == 0)
        //                    Thread.Sleep(CONNECT_TIMEOUT);
        //            }
        //            catch
        //            {
        //            }
        //        }
        //    }
        //}

        //private async Task Tx()
        //{
        //    //string data = string.Empty;
        //    //string device = string.Empty;
        //    //Stopwatch stopWatch = new Stopwatch();
        //    //stopWatch.Start();
        //    //uint state = STATEs.First();
        //    while (true)
        //    {
        //        try
        //        {
        //            //foreach (string dev in Devices)
        //            //{
        //            //    device = dev;

        //            if (TxQue.Any())
        //            {
        //                TxPacket txPacket = TxQue.First();
        //                string device = txPacket.device;
        //                int ret = -1;
        //                switch (device)
        //                {
        //                    case ECOMILK:
        //                        WaitHandleAptx1.Reset();
        //                        WaitHandleRfid.Reset();
        //                        WaitHandleRemote.Reset();
        //                        WaitHandleEcomilk.Set();
        //                        break;
        //                    case REMOTE:
        //                        WaitHandleEcomilk.Reset();
        //                        WaitHandleAptx1.Reset();
        //                        WaitHandleRfid.Reset();
        //                        WaitHandleRemote.Set();
        //                        break;
        //                    case RFID:
        //                        WaitHandleEcomilk.Reset();
        //                        WaitHandleRemote.Reset();
        //                        WaitHandleAptx1.Reset();
        //                        WaitHandleRfid.Set();
        //                        break;
        //                    case APTX1:
        //                        WaitHandleEcomilk.Reset();
        //                        WaitHandleRemote.Reset();
        //                        WaitHandleRfid.Reset();
        //                        WaitHandleAptx1.Set();
        //                        break;
        //                }
        //                //ThreadPool.QueueUserWorkItem(async (o) => { await Rx(device); });
        //                for (int i = 0; i < REQUEST_RETRIES; i++)
        //                {
        //                    if (Ports.TryGetValue(device, out string port))
        //                    {
        //                        //if (await PortRequest(device, port, state) < 0)
        //                        ret = await PortRequest(txPacket, port);
        //                    }
        //                    else
        //                    {
        //                        string[] ports = UsbSerial.GetPorts().ToArray();
        //                        usbPorts = ports;
        //                        UsbPorts = UsbPorts;
        //                        foreach (string prt in ports)
        //                        {
        //                            if (!Ports.Values.Contains(prt))
        //                                //await PortRequest(device, prt, 0);
        //                                ret = await PortRequest(txPacket, prt);
        //                        }
        //                    }
        //                    Thread.Sleep(REQUEST_TIMEOUT);
        //                }
        //                //if (ret > 0)
        //                //{
        //                SemaphoreTxQue.WaitOne();
        //                TxQue.Remove(txPacket);
        //                if (txPacket.packetType != PacketType.EMPTY)
        //                    TxQue.Add(txPacket);
        //                SemaphoreTxQue.Release();
        //                //}
        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        //TxType packetType = txPacket.packet;
        //switch (device)
        //{
        //case ECOMILK:
        //    switch (packetType)
        //    {
        //        case TxType.ECOMILK_ID:
        //            //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("ecomilkid\r"));
        //            //TxQUpdate(txPacket);
        //            packet = Encoding.UTF8.GetBytes("ecomilkid\r");
        //            break;
        //    }
        //    break;
        //case RFID:
        //    switch (packetType)
        //    {
        //        case TxType.RFID_TAG:
        //            //ret = await UsbSerial.Write(port, new RfId().PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = new RfId().PacketBuild();
        //            break;
        //    }
        //    break;
        //case REMOTE:
        //    switch (packetType)
        //    {
        //        case TxType.REMOTE_START:
        //            //ret = await UsbSerial.Write(port, Aptxs[0].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[0].PacketBuild();
        //            break;
        //        case TxType.REMOTE_STATUS_0:
        //            //ret = await UsbSerial.Write(port, Aptxs[0].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[0].PacketBuild();
        //            break;
        //        case TxType.REMOTE_STATUS_1:
        //            //ret = await UsbSerial.Write(port, Aptxs[1].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[1].PacketBuild();
        //            break;
        //        case TxType.REMOTE_STATUS_2:
        //            //ret = await UsbSerial.Write(port, Aptxs[2].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[2].PacketBuild();
        //            break;
        //        case TxType.REMOTE_STATUS_3:
        //            //ret = await UsbSerial.Write(port, Aptxs[3].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[3].PacketBuild();
        //            break;
        //    }
        //    break;
        //if (device == APTX1)
        //{
        //    //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("getid,3#"));
        //    if (Ports.ContainsKey(device))
        //    {
        //        if (Aptx.SNum == UERROR)
        //            //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("testread,3#"));
        //            packet = Encoding.UTF8.GetBytes("testread,3#");
        //        else if (Aptx.CurrentPulses == UERROR)
        //            //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("find,3#"));
        //            packet = Encoding.UTF8.GetBytes("find,3#");
        //        else if ((Aptx.aptxId[0] == UERROR) ||
        //                  (Aptx.aptxId[1] == UERROR) ||
        //                  (Aptx.aptxId[2] == UERROR))
        //        {
        //            //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("readid#"));
        //            packet = Encoding.UTF8.GetBytes("readid#");
        //            txPacket = TxPacket.Empty;
        //        }
        //    }
        //}
        //if(packet.Length > 0)
        //    ret = await UsbSerial.Write(port, packet);
        //if (txPacket != TxPacket.Empty)
        //{
        //    //TxQueUpdate(txPacket);
        //    SemaphoreTxQ.WaitOne();
        //    TxQue.Remove(txPacket);
        //    TxQue.Add(txPacket);
        //    SemaphoreTxQ.Release();
        //}
        //    return ret;
        //}

        //private void TxQueUpdate(TxPacket txPacket)
        //{
        //    SemaphoreTxQ.WaitOne();
        //    TxQue.Remove(txPacket);
        //    TxQue.Add(txPacket);
        //    SemaphoreTxQ.Release();
        //}

        //if (Aptx.SNum == UERROR)
        //{
        //    if (data.Contains("SNUM "))
        //    {
        //        uint[] snum = DataParse(data, "SNUM ", NumberStyles.Number);
        //        Aptx.SNum = snum[0];
        //        rxBuffer = new byte[0];
        //    }
        //}
        //if ((Aptx.aptxId[0] == UERROR) ||
        //    (Aptx.aptxId[1] == UERROR) ||
        //    (Aptx.aptxId[2] == UERROR))
        //{
        //    if (data.Contains("readid Device_id"))
        //    {
        //        uint[] aptid = DataParse(data, "readid Device_id", NumberStyles.HexNumber);
        //        Aptx.aptxId[0] = aptid[0];
        //        Aptx.aptxId[1] = aptid[1];
        //        Aptx.aptxId[2] = aptid[2];
        //        Aptx.AptxId = Aptx.AptxId;
        //        rxBuffer = new byte[0];
        //    }
        //}
        //if ((Aptx.CurrentPulses == UERROR) || (Aptx.Maxi == UERROR))
        //{
        //    if (data.Contains("MAXI "))
        //    {
        //        uint[] maxi = DataParse(data, "MAXI ", NumberStyles.Number);
        //        Aptx.Maxi = maxi[0];
        //        rxBuffer = new byte[0];
        //    }
        //    if (data.Contains("Found: ") || data.Contains("pulses written "))
        //    {
        //        uint[] current = DataParse(data, "Found: ", NumberStyles.Number);
        //        if (current[0] == 0)
        //            current = DataParse(data, "pulses written ", NumberStyles.Number);
        //        Aptx.CurrentPulses = current[0];
        //        rxBuffer = new byte[0];
        //    }
        //    if ((Aptx.Maxi != UERROR) && (Aptx.CurrentPulses != UERROR))
        //        Aptx.Remaining = Aptx.Maxi - Aptx.CurrentPulses;
        //}

        //        data = Encoding.UTF8.GetString(rxBuffer);
        //        if (!(found = data.Contains(ECOMILK)))
        //        {
        //            RfId rfId = new RfId();
        //            if (found = rfId.PacketParse(ref rxBuffer))
        //            {
        //                unsafe
        //                {
        //                    fixed (byte* pepc = rfId.EPC)
        //                    {
        //                        CowId = rfId.ArrayToUint(pepc);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (found = Aptx.PacketParse(ref rxBuffer))
        //                {
        //                    if (Aptx.APTXIDs.Contains((byte)Aptx.Id))
        //                    {
        //                        AptxUpdate();
        //                        Aptxs[Aptx.Id - 1] = Aptx;
        //                    }
        //                }
        //                else
        //                {
        //                    data = Encoding.UTF8.GetString(rxBuffer.Where(b => b != 0x00).ToArray());
        //                    if (!Ports.ContainsKey(device))
        //                    {
        //                        found = data.Contains("1F-85-01");
        //                        if (!found)
        //                            found = data.Contains("0x1f-0x85-0x01");
        //                        if (found)
        //                            rxBuffer = new byte[0];
        //                    }
        //                    else
        //                    {
        //                        if (data.Contains("SNUM "))
        //                        {
        //                            uint[] snum = DataParse(data, "SNUM ", NumberStyles.Number);
        //                            if (snum[0] == 123)
        //                            {
        //                                Aptx.SNum++;
        //                                rxBuffer = new byte[0];
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    SemaphoreRxBuffers.WaitOne();
        //    if (RxBuffers.ContainsKey(port))
        //    {
        //        RxBuffers.Remove(port);
        //        RxBuffers.Add(port, rxBuffer);
        //    }
        //    SemaphoreRxBuffers.Release();
        //    return found;
        //}

        //switch (txPacket.packetType)
        //{
        //    case PacketType.APTX1_ID:
        //        //if (Ports.ContainsKey(txPacket.device))
        //        //{
        //        txPacket.packetType = PacketType.APTX1_SNUM;
        //        txPacket.packet = Encoding.UTF8.GetBytes("testread,3#");
        //        //}
        //        break;
        //    case PacketType.APTX1_SNUM:
        //        //if (Aptx.SNum != UERROR)
        //        //{
        //        txPacket.packetType = PacketType.APTX1_CURRENT;
        //        txPacket.packet = Encoding.UTF8.GetBytes("find,3#");
        //        //}
        //        break;
        //    case PacketType.APTX1_CURRENT:
        //        //if (Aptx.CurrentPulses != UERROR)
        //        //{
        //        //txPacket.packetType = PacketType.APTX1_APTXID;
        //        //txPacket.packet = Encoding.UTF8.GetBytes("readid#");
        //        txPacket.packetType = PacketType.APTX1_APTXID;
        //        txPacket.packet = Encoding.UTF8.GetBytes("readid#");
        //        //}
        //        break;
        //    case PacketType.APTX1_APTXID:
        //        //if ((Aptx.aptxId[0] != UERROR) ||
        //        //    (Aptx.aptxId[1] != UERROR) ||
        //        //    (Aptx.aptxId[2] != UERROR))
        //        //{
        //        //    txPacket.packetType = PacketType.EMPTY;
        //        //}
        //        txPacket.packetType = PacketType.APTX1_ID;
        //        txPacket.packet = Encoding.UTF8.GetBytes("getid,3#");
        //        break;
        //}

        //if (Aptx.SNum == UERROR)
        //{
        //    txPacket.packetType = PacketType.APTX1_SNUM;
        //    txPacket.packet = Encoding.UTF8.GetBytes("testread,3#");
        //}
        //else if (Aptx.CurrentPulses == UERROR)
        //{
        //    txPacket.packetType = PacketType.APTX1_CURRENT;
        //    txPacket.packet = Encoding.UTF8.GetBytes("find,3#");
        //}
        //else if ((Aptx.aptxId[0] == UERROR) ||
        //    (Aptx.aptxId[1] == UERROR) ||
        //    (Aptx.aptxId[2] == UERROR))
        //{
        //    txPacket.packetType = PacketType.APTX1_ID;
        //    txPacket.packet = Encoding.UTF8.GetBytes("getid,3#");
        //}
    }
}