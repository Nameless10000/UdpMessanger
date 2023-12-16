using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using Sockets;
using Newtonsoft.Json;
using UdpServiceLib;

IPAddress localAddress = IPAddress.Parse("127.0.0.1");
Console.Write("Введите свое имя: ");
string? username = Console.ReadLine();
Console.Write("Введите порт для приема сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var localPort)) return;
Console.Write("Введите порт для отправки сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var remotePort)) return;
Console.WriteLine();

var udpService = new UdpService(localPort, remotePort);

// запускаем получение сообщений
Task.Run(ReceiveMessageAsync);
// запускаем ввод и отправку сообщений
await SendMessageAsync();

// отправка сообщений в группу
async Task SendMessageAsync()
{
    while (true)
    {
        var message = Console.ReadLine();
        await udpService.SendMessageAsync(username, message);
        Console.WriteLine(message);
    }
}
// отправка сообщений
async Task ReceiveMessageAsync()
{
    while (true)
    {
        var message = await udpService.ReceiveMessageAsync();

        Console.WriteLine(message);
    }
}

public static class Exts
{
    public static byte[] GetBytes(this string data)
    {
        return Encoding.UTF8.GetBytes(data);
    }
}