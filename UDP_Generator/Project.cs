using UDP.Reciver;

Console.WriteLine("UDP Resiver");

var recever = new ReceverModel(loggerTitle: "Recever CMD");

recever.ReceveTrafficLoopAsinc();


Console.ReadLine();

recever.StopReceveTrafficLoop();
