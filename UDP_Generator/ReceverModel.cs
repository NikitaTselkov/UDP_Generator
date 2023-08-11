using System.Net.Sockets;
using System.Net;
using UDP.Core;

namespace UDP.Reciver
{
    public class ReceverModel
    {
        private readonly Logger _logger;
        private bool _isStarted;

        public ReceverModel(string loggerTitle = "")
        {
            _logger = string.IsNullOrEmpty(loggerTitle) ? new Logger("Recever") : new Logger(loggerTitle);
        }

        public void StopReceveTrafficLoop()
        {
            _isStarted = false;
        }

        public async void ReceveTrafficLoopAsinc()
        {
            _isStarted = true;

            await Task.Run(() =>
            {
                UdpClient clientSender = null, clientRecever = null;

                try
                {                 
                    IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Parse(Config.SenderIp), Config.SenderPort);
                    IPEndPoint receveEndPoint = new IPEndPoint(IPAddress.Parse(Config.ReceveIp), Config.RecevePort);
                    var recevedPackets = 0;

                    _logger.PushMessage("Начато прослушивание");
               
                    clientRecever = new UdpClient(receveEndPoint);
                    clientSender = new UdpClient(Config.SenderPort);

                    while (_isStarted)
                    {
                        clientRecever.Client.ReceiveTimeout = Config.ReceveTimeout;
                        
                        try 
                        {
                            IPEndPoint RemoteEndPoint = null;
                            var result = clientRecever.Receive(ref RemoteEndPoint);
                            recevedPackets++;

                            _logger.PushMessage($"Получено {recevedPackets} пакетов");

                            if (result?.Length > 0)
                            {
                                _logger.PushMessage($"Получено {result.Length} байт от {senderEndPoint}");

                                byte[] packetSend = BitConverter.GetBytes(result.Length);

                                clientSender.Send(packetSend, senderEndPoint);
                                _logger.PushMessage($"Отправлено: {packetSend.Length} байт", LoggerTypes.Info);
                            }

                        }
                        catch (SocketException e)
                        {
                            _logger.PushMessage(e.Message);
                        }
                    }
                }
                catch (SocketException e)
                { 
                    _logger.PushMessage(e.Message, LoggerTypes.Error);
                }
                finally
                {
                    clientRecever?.Close();
                    _logger.PushMessage("Соединение закрыто");
                }
            });
        }
    }
}
