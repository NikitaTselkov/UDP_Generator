using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using UDP.Core;

namespace UDP.Sender
{
    public class SenderModel
    {
        private readonly Logger _logger;
        private bool _isStarted;

        public SenderModel(string loggerTitle = "")
        {
            _logger = string.IsNullOrEmpty(loggerTitle) ? new Logger("Sender") : new Logger(loggerTitle);
        }

        public void StopGenerateRandomUdpTraffic()
        { 
            _isStarted = false;
        }

        public async void GenerateRandomUdpTrafficAsinc()
        {
            var random = new Random();
            _isStarted = true;

            await Task.Run(() =>
            {
                while (_isStarted)
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

                _logger.PushMessage("Начата отправка");

                foreach (var packet in message.Packets)
                {
                    if (_isStarted == false)
                        break;

                    int bytes = clientSender.Send(packet.ToArray(), remotePoint);

                    lostPackets += bytes;
                    sendPackets++;

                    _logger.PushMessage($"Отправлено: {bytes} байт на {remotePoint}");
                    _logger.PushMessage($"Отправлено: {sendPackets} пакетов");

                    try
                    {
                        clientRecever.Client.ReceiveTimeout = Config.ReceveTimeout;
                        var packetReceve = clientRecever.Receive(ref receveEndPoint);
                        lostPackets -= BitConverter.ToInt32(packetReceve);
                        _logger.PushMessage($"Получено: {packetReceve.Length} байт", LoggerTypes.Info);
                        _logger.PushMessage($"Удаленный адрес: {receveEndPoint}", LoggerTypes.Info);
                    }
                    catch (SocketException e) { _logger.PushMessage(e.Message, LoggerTypes.Error); }

                    _logger.PushMessage($"Потеряно: {lostPackets} байт");
                }
            }
            catch (SocketException e)
            {
                _logger.PushMessage(e.Message, LoggerTypes.Error);
            }
            finally
            {
                clientSender?.Close();
                clientRecever?.Close();
                _logger.PushMessage("Соединение закрыто");
            }
        }
    }
}
