using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UDP.Core;


var random = new Random();
var lostPackets = 0;
var sendPackets = 0;

Console.WriteLine("UDP Sender");


GenerateRandomUdpTrafficAsinc();


Console.ReadLine();

async void GenerateRandomUdpTrafficAsinc()
{
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
            string remoteMAC = GetRemoteMAC(remotePoint);

            // Проверить MAC    
            if (remoteMAC != Config.ReceiveMac.Replace("-", ""))
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
                clientRecever.Client.ReceiveTimeout = Config.ReceiveTimeout;
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