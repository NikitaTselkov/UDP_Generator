using System.Net;
using System.Net.Sockets;
using UDP.Core;
using UDP.Reciver;

Console.WriteLine("UDP Resiver");

var recever = new ReceverModel();

recever.ReceveTrafficLoopAsinc();


Console.ReadLine();
