namespace UDP.Core
{
    public static class Config
    {
        public static string SenderIp { get; set; } = "192.168.0.100";
        public static string ReceveIp { get; set; } = "127.0.0.1";
        public static int SenderPort { get; set; } = 5556;
        public static int RecevePort { get; set; } = 5555;
        public static int ReceveTimeout { get; set; } = 2000;
        public static HashSet<string> Macs { get; set; } = new HashSet<string>() { "05-75-C3-A2-13-23" };
        public static int MinUdpPacketSize { get; set; } = 125000; // 1 Мбит
        public static int MaxUdpPacketSize { get; set; } = 125000000; // 1 ГБит
    }
}
