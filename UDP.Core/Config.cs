using System.Net;

namespace UDP.Core
{
    public static class Config
    {
        public static string SenderIp => "192.168.0.100";
        public static string ReceveIp => "127.0.0.1";
        public static int SenderPort => 5556;
        public static int RecevePort => 5555;
        public static int ReceiveTimeout => 2000;
        public static string SenderMac => "70-85-C2-D6-EC-13";
        public static string ReceiveMac => "70-85-C2-D6-EC-13";
        public static int MinUdpPacketSize => 125000; // 1 Мбит
        public static int MaxUdpPacketSize => 125000000; // 1 ГБит
    }
}
