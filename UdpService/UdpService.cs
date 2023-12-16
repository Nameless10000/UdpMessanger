using Logger;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

namespace UdpServiceLib
{
    public class UdpService
    {
        private MessangerLogger _logger;
        private readonly int _localPort;
        private readonly int _remotePort;
        private readonly IPAddress _localAddress = IPAddress.Parse("127.0.0.1");

        public UdpService(int localPort, int remotePort, string logPath = null)
        {
            InitLogger(logPath);
            _localPort = localPort;
            _remotePort = remotePort;
        }

        public async Task<MessageDto?> ReceiveMessageAsync()
        {
            try
            {
                using var receiver = new UdpClient(_localPort);

                var data = await receiver.ReceiveAsync();
                var messageJsonStr = data.Buffer.GetString();
                return JsonConvert.DeserializeObject<MessageDto>(messageJsonStr);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message);
                return null;
            }
        }

        public async Task<MessageDto?> SendMessageAsync(string userName, string messageText)
        {
            try
            {
                var sender = new UdpClient();
                var endPoint = new IPEndPoint(_localAddress, _remotePort);

                var messageDto = new MessageDto
                {
                    Text = messageText,
                    UserName = userName,
                    SentAt = DateTime.Now
                };

                var messageJsonStr = JsonConvert.SerializeObject(messageDto);
                var messageBytes = messageJsonStr.GetBytes();
                await sender.SendAsync(messageBytes, messageBytes.Length, endPoint);
                return messageDto;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message);
                return null;
            }
        }

        private string GetLogFileNum(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            return fileName.Replace("log_", "").Replace(".txt", "");
        }

        private void InitLogger(string logPath)
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataFullPath = Path.GetFullPath(appDataPath);

            var logFolderPath = Path.Combine(appDataFullPath, "Logs");
            if (!Directory.Exists(logFolderPath))
                Directory.CreateDirectory(logFolderPath);


            if (string.IsNullOrWhiteSpace(logPath))
            {
                var logFilesNums = Directory.GetFiles(logFolderPath)
                .Where(fileName => Path.GetFileName(fileName).StartsWith("log_"))
                .Select(filePath =>
                    int.TryParse(GetLogFileNum(filePath), out var fileNumber)
                    ? fileNumber
                    : 0);

                var logFileNum = logFilesNums.Any() ? logFilesNums.Max() + 1 : 1;

                logPath = Path.Combine(logFolderPath, $"log_{logFileNum}.txt");
            }

            File.Create(logPath).Dispose();

            _logger = new MessangerLogger(logPath);
        }
    }
}