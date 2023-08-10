using Prism.Commands;
using System;
using System.ComponentModel.DataAnnotations;
using UDP.Reciver;
using UDP.Sender;
using UDP.Core;
using System.Collections.ObjectModel;

namespace UDP.View.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<string> Macs { get; set; }

        private string _title = "UDP Generator";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        [Required]
        [RegularExpression("^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")]
        public string SenderIp
        {
            get { return Config.SenderIp; }
            set 
            {
                if (IsValid(nameof(SenderIp)))
                    Config.SenderIp = value; 
            }
        }

        [Required]
        [RegularExpression("^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")]
        public string ReceveIp
        {
            get { return Config.ReceveIp; }
            set
            {
                if (IsValid(nameof(ReceveIp)))
                    Config.ReceveIp = value;
            }
        }

        [Required]
        [Range(0, ushort.MaxValue)]
        public int SenderPort
        {
            get { return Config.SenderPort; }
            set
            {
                if (IsValid(nameof(SenderPort)))
                    Config.SenderPort = value;
            }
        }

        [Required]
        [Range(0, ushort.MaxValue)]
        public int RecevePort
        {
            get { return Config.RecevePort; }
            set
            {
                if (IsValid(nameof(RecevePort)))
                    Config.RecevePort = value; 
            }
        }

        [Required]
        [Range(0, int.MaxValue)]
        public int ReceveTimeout
        {
            get { return Config.ReceveTimeout; }
            set
            {
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
                if (IsValid(nameof(Mac)) && !string.IsNullOrEmpty(value))
                    SetProperty(ref _mac, value.ToUpper());
            }
        }

        [Required]
        [Range(0, int.MaxValue)]
        public int MinUdpPacketSize
        {
            get { return Config.MinUdpPacketSize; }
            set 
            {
                if (IsValid(nameof(MinUdpPacketSize)))
                    Config.MinUdpPacketSize = value;
            }
        }

        [Required]
        [Range(0, int.MaxValue)]
        public int MaxUdpPacketSize
        {
            get { return Config.MaxUdpPacketSize; }
            set
            {
                if (IsValid(nameof(MaxUdpPacketSize)))
                    Config.MaxUdpPacketSize = value; 
            }
        }


        private DelegateCommand _generateRandomUdpTraffic;
        public DelegateCommand GenerateRandomUdpTraffic =>
            _generateRandomUdpTraffic ?? (_generateRandomUdpTraffic = new DelegateCommand(ExecuteGenerateRandomUdpTraffic));

        private DelegateCommand _startListenUdpTraffic;
        public DelegateCommand StartListenUdpTraffic =>
            _startListenUdpTraffic ?? (_startListenUdpTraffic = new DelegateCommand(ExecuteStartListenUdpTraffic));

        private DelegateCommand _addMac;
        public DelegateCommand AddMac =>
            _addMac ?? (_addMac = new DelegateCommand(ExecuteAddMac));

        private DelegateCommand<string> _deleteMac;
        public DelegateCommand<string> DeleteMac =>
            _deleteMac ?? (_deleteMac = new DelegateCommand<string>(ExecuteDeleteMac));


        public MainWindowViewModel()
        {
            Macs = new ObservableCollection<string>(Config.Macs);
        }


        void ExecuteGenerateRandomUdpTraffic()
        {
            var sender = new SenderModel();
            sender.GenerateRandomUdpTrafficAsinc();
        }
        
        void ExecuteStartListenUdpTraffic()
        {
            var recever = new ReceverModel();
            recever.ReceveTrafficLoopAsinc();
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
                Config.Macs.Remove(Mac);
            }
        }
    }
}
