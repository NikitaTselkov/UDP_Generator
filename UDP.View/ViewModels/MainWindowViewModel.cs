using Prism.Commands;
using System;
using System.ComponentModel.DataAnnotations;
using UDP.Reciver;
using UDP.Sender;
using UDP.Core;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Threading;

namespace UDP.View.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<string> Macs { get; set; }
        public ObservableCollection<string> Log { get; set; }

        private Dispatcher _dispatcher;

        private string _title = "UDP Generator";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _isStartGenerateTrafficButtonPressed;
        public bool IsStartGenerateTrafficButtonPressed
        {
            get { return _isStartGenerateTrafficButtonPressed; }
            set { SetProperty(ref _isStartGenerateTrafficButtonPressed, value); }
        }

        private bool _isStartReceveButtonPressed;
        public bool IsStartReceveButtonPressed
        {
            get { return _isStartReceveButtonPressed; }
            set { SetProperty(ref _isStartReceveButtonPressed, value); }
        }


        private string _senderIp = Config.SenderIp;
        [Required]
        [RegularExpression("^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")]
        public string SenderIp
        {
            get { return _senderIp; }
            set 
            {
                SetProperty(ref _senderIp, value);
                if (IsValid(nameof(SenderIp)))
                    Config.SenderIp = value; 
            }
        }

        private string _receveIp = Config.ReceveIp;
        [Required]
        [RegularExpression("^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")]
        public string ReceveIp
        {
            get { return _receveIp; }
            set
            {
                SetProperty(ref _receveIp, value);
                if (IsValid(nameof(ReceveIp)))
                    Config.ReceveIp = value;
            }
        }

        private int _senderPort = Config.SenderPort;
        [Required]
        [Range(0, ushort.MaxValue)]
        public int SenderPort
        {
            get { return _senderPort; }
            set
            {
                SetProperty(ref _senderPort, value);
                if (IsValid(nameof(SenderPort)))
                    Config.SenderPort = value;
            }
        }

        private int _recevePort = Config.RecevePort;
        [Required]
        [Range(0, ushort.MaxValue)]
        public int RecevePort
        {
            get { return _recevePort; }
            set
            {
                SetProperty(ref _recevePort, value);
                if (IsValid(nameof(RecevePort)))
                    Config.RecevePort = value; 
            }
        }

        private int _receveTimeout = Config.ReceveTimeout;
        [Required]
        [Range(0, int.MaxValue)]
        public int ReceveTimeout
        {
            get { return _receveTimeout; }
            set
            {
                SetProperty(ref _receveTimeout, value);
                if (IsValid(nameof(ReceveTimeout)))
                    Config.ReceveTimeout = value;
            }
        }

        private string _mac;
        [RegularExpression("^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$")]
        public string Mac
        {
            get { return _mac; }
            set
            {
                SetProperty(ref _mac, value.ToUpper());
            }
        }

        private int _minUdpPacketSize = Config.MinUdpPacketSize;
        [Required]
        [Range(0, int.MaxValue)]
        public int MinUdpPacketSize
        {
            get { return _minUdpPacketSize; }
            set 
            {
                SetProperty(ref _minUdpPacketSize, value);
                if (IsValid(nameof(MinUdpPacketSize)))
                    Config.MinUdpPacketSize = value;
            }
        }

        private int _maxUdpPacketSize = Config.MaxUdpPacketSize;
        [Required]
        [Range(0, int.MaxValue)]
        public int MaxUdpPacketSize
        {
            get { return _maxUdpPacketSize; }
            set
            {
                SetProperty(ref _maxUdpPacketSize, value);
                if (IsValid(nameof(MaxUdpPacketSize)))
                    Config.MaxUdpPacketSize = value; 
            }
        }


        private DelegateCommand _generateRandomUdpTraffic;
        public DelegateCommand GenerateRandomUdpTraffic =>
            _generateRandomUdpTraffic ?? (_generateRandomUdpTraffic = new DelegateCommand(ExecuteGenerateRandomUdpTraffic, () => IsAllValid()));

        private DelegateCommand _startListenUdpTraffic;
        public DelegateCommand StartListenUdpTraffic =>
            _startListenUdpTraffic ?? (_startListenUdpTraffic = new DelegateCommand(ExecuteStartListenUdpTraffic, () => IsAllValid()));

        private DelegateCommand _addMac;
        public DelegateCommand AddMac =>
            _addMac ?? (_addMac = new DelegateCommand(ExecuteAddMac));

        private DelegateCommand<string> _deleteMac;
        public DelegateCommand<string> DeleteMac =>
            _deleteMac ?? (_deleteMac = new DelegateCommand<string>(ExecuteDeleteMac));


        public MainWindowViewModel()
        {
            Macs = new ObservableCollection<string>(Config.Macs);
            Log = new ObservableCollection<string>();
            _dispatcher = Dispatcher.CurrentDispatcher;

            Logger.OnLogChanged += UpdateLogChanged;
        }

        ~MainWindowViewModel()
        {
            Logger.OnLogChanged -= UpdateLogChanged;
        }

        private SenderModel _sender = new SenderModel();
        void ExecuteGenerateRandomUdpTraffic()
        {
            IsStartGenerateTrafficButtonPressed = !IsStartGenerateTrafficButtonPressed;

            if (IsStartGenerateTrafficButtonPressed)
                _sender.GenerateRandomUdpTrafficAsinc();
            else
                _sender.StopGenerateRandomUdpTraffic();
        }


        private ReceverModel _recever = new ReceverModel();
        void ExecuteStartListenUdpTraffic()
        {
            IsStartReceveButtonPressed = !IsStartReceveButtonPressed;

            if (IsStartReceveButtonPressed)
                _recever.ReceveTrafficLoopAsinc();
            else
                _recever.StopReceveTrafficLoop();
        }

        void ExecuteAddMac()
        {
            if (IsValid(nameof(Mac))
                && !string.IsNullOrEmpty(Mac)
                && !Macs.Contains(Mac))
            {
                Macs.Add(Mac);
                Config.Macs.Add(Mac);
            }
        }
        
        void ExecuteDeleteMac(string param)
        {
            if (Macs.Contains(param))
            {
                Macs.Remove(param);
                Config.Macs.Remove(param);
            }
        }

        private void UpdateLogChanged(LogEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Message))
            {
                _dispatcher.BeginInvoke(() =>
                {
                    Log.Add(e.Message);
                });
            }
        }
    }
}
