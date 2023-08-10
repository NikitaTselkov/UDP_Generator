using UDP.Sender;

Console.WriteLine("UDP Sender");

var sender = new SenderModel();

sender.GenerateRandomUdpTrafficAsinc();

Console.ReadLine();

