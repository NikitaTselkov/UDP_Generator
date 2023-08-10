using System.Net;
using System.Net.Sockets;
using UDP.Core;


Console.WriteLine("UDP Resiver");

ReceiveTrafficLoopAsinc();


Console.ReadLine();

async void ReceiveTrafficLoopAsinc()
{
    await Task.Run(() =>
    {
        UdpClient clientSender = null, clientRecever = null;
        IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Parse(Config.SenderIp), Config.SenderPort);
        IPEndPoint receveEndPoint = new IPEndPoint(IPAddress.Parse(Config.ReceveIp), Config.RecevePort);
        var recevedPackets = 0;

        try
        {
            clientRecever = new UdpClient(receveEndPoint);
            clientSender = new UdpClient(Config.SenderPort);

            while (true)
            {
                IPEndPoint RemoteEndPoint = null;
                var result = clientRecever.Receive(ref RemoteEndPoint);
                recevedPackets++;

                Console.WriteLine($"Получено {recevedPackets} пакетов");

                if (result.Length > 0)
                {
                    Console.WriteLine($"Получено {result.Length} байт");
                    Console.WriteLine($"Удаленный адрес: {RemoteEndPoint}");

                    byte[] packetSend = BitConverter.GetBytes(result.Length);

                    clientSender.Send(packetSend, senderEndPoint);
                    Console.WriteLine($"Отправлено: {packetSend.Length} байт");

                    clientRecever.Client.ReceiveTimeout = Config.ReceiveTimeout;
                }
            }
        }
        catch (SocketException e) { }
        finally
        {
            clientRecever?.Close();
            Console.WriteLine("Соединение закрыто");
        }
    });
}