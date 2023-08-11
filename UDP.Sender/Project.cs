using UDP.Sender;

Console.WriteLine("UDP Sender");

var sender = new SenderModel(loggerTitle: "Sendeer CMD");

sender.GenerateRandomUdpTrafficAsinc();

Console.ReadLine();

sender.StopGenerateRandomUdpTraffic();
