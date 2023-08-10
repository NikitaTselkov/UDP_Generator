using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using UDP.Core;

namespace UDP.Sender
{
    public class SenderModel
    {
        public async void GenerateRandomUdpTrafficAsinc()
        {
            var random = new Random();

            await Task.Run(() =>
            {
                while (true)
                {
                    var data = new byte[random.Next(Config.MinUdpPacketSize, Config.MaxUdpPacketSize)];
                    Array.Fill(data, (byte)random.Next(0, byte.MaxValue));

                    SendTrafficLoop(data);
                }
            });
        }

        void SendTrafficLoop(byte[] data)
        {
            UdpClient clientSender = null, clientRecever = null;

            int lostPackets = 0;
            int sendPackets = 0;

            try
            {
                var message = new UdpMessage(data);
                var remotePoint = new IPEndPoint(IPAddress.Parse(Config.ReceveIp), Config.RecevePort);
                var senderPoint = new IPEndPoint(IPAddress.Parse(Config.SenderIp), Config.SenderPort);
                IPEndPoint receveEndPoint = null;

                clientSender = new UdpClient(Config.RecevePort);
                clientRecever = new UdpClient(senderPoint);

                foreach (var packet in message.Packets)
                {
                    string stringMAC = GetRemoteMAC(remotePoint);
                    string remoteMAC = string.Concat(stringMAC.ToUpper().Select((c, i) => c + (i % 2 != 0 ? "-" : ""))).TrimEnd('-');

                    // Проверить MAC
                    if (Config.Macs.Contains(remoteMAC))
                    {
                        Console.WriteLine("MAC адрес не совпадает");
                        continue;
                    }

                    int bytes = clientSender.Send(packet.ToArray(), remotePoint);

                    lostPackets += bytes;
                    sendPackets++;

                    Console.WriteLine($"Отправлено: {bytes} байт");
                    Console.WriteLine($"Отправлено: {sendPackets} пакетов");

                    try
                    {
                        clientRecever.Client.ReceiveTimeout = Config.ReceveTimeout;
                        var packetReceve = clientRecever.Receive(ref receveEndPoint);
                        lostPackets -= BitConverter.ToInt32(packetReceve);
                        Console.WriteLine($"получено: {packetReceve.Length} байт");
                        Console.WriteLine($"Удаленный адрес: {receveEndPoint}");
                    }
                    catch (SocketException e) { }

                    Console.WriteLine($"потеряно: {lostPackets} байт");
                }

                Console.WriteLine($"Отправлен файл размером: {message.Size} байт");
            }
            finally
            {
                clientSender?.Close();
                clientRecever?.Close();
                Console.WriteLine("Соединение закрыто");
            }
        }

        string GetRemoteMAC(IPEndPoint remoteEndpoint)
        {
            string remoteMac = string.Empty;

            try
            {
                IPAddress remoteIpAddress = remoteEndpoint.Address;

                var macs = from nic in NetworkInterface.GetAllNetworkInterfaces()
                           where nic.NetworkInterfaceType != NetworkInterfaceType.Loopback
                               && nic.OperationalStatus == OperationalStatus.Up
                           select nic.GetPhysicalAddress();

                remoteMac = macs.FirstOrDefault()?.ToString() ?? string.Empty;
            }
            catch { }

            return remoteMac;
        }
    }
}
