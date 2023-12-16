using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using UdpServiceLib;

namespace Messanger.ViewModels
{
    public class MainWinViewModel : ReactiveObject
    {
        private UdpService _udpService;
        public MainWinViewModel()
        {
            ConnectionCommand = ReactiveCommand.Create(Connect,
                this.WhenAnyValue(vm => vm.IsConnected)
                    .Select(isConnected => !isConnected));

            SendCommand = ReactiveCommand.Create(SendMessage, this.WhenAnyValue(vm => vm.IsConnected, vm => vm.NewMessage).Select(data =>
            {
                return data.Item1 && !string.IsNullOrWhiteSpace(data.Item2);
            }));
        }

        private string _userName;

        public string UserName
        {
            get => _userName;
            set => this.RaiseAndSetIfChanged(ref _userName, value, nameof(UserName));
        }

        private string _newMessage;

        public string NewMessage
        {
            get => _newMessage;
            set => this.RaiseAndSetIfChanged(ref _newMessage, value, nameof(NewMessage));
        }

        private int _localPort = 22;

        public int LocalPort
        {
            get => _localPort;
            set => this.RaiseAndSetIfChanged(ref _localPort, value, nameof(LocalPort));
        }

        private int _remotePort = 1022;

        public int RemotePort
        {
            get => _remotePort;
            set => this.RaiseAndSetIfChanged(ref _remotePort, value, nameof(RemotePort));
        }

        private string _logPath;

        public string LogPath
        {
            get => _logPath;
            set => this.RaiseAndSetIfChanged(ref _logPath, value, nameof(LogPath));
        }

        private ConcurrentBag<MessageDto> _messages = new();

        public ConcurrentBag<MessageDto> Messages
        {
            get => _messages;
            set => this.RaiseAndSetIfChanged(ref _messages, value, nameof(Messages));
        }

        private MessageDto _receivedMessage;

        public MessageDto ReceivedMessage
        {
            get => _receivedMessage;
            set => this.RaiseAndSetIfChanged(ref _receivedMessage, value, nameof(ReceivedMessage));
        }

        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set => this.RaiseAndSetIfChanged(ref _isConnected, value, nameof(IsConnected));
        }


        private async Task Connect()
        {
            _udpService = new UdpService(LocalPort, RemotePort);
            IsConnected = true;

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var message = await _udpService.ReceiveMessageAsync();

                    if (message == null)
                        return;

                    Messages.Add(message);
                    Messages = new(_messages.OrderByDescending(m => m.SentAt));
                }
            });

            await _udpService.SendMessageAsync(UserName, $"{UserName} connected");
        }

        private ReactiveCommand<Unit, Task> _connectionCommand;

        public ReactiveCommand<Unit, Task> ConnectionCommand
        {
            get => _connectionCommand;
            set => this.RaiseAndSetIfChanged(ref _connectionCommand, value);
        }

        private async Task SendMessage()
        {
            var createdMessage = await _udpService.SendMessageAsync(UserName, NewMessage);
            NewMessage = "";
            Messages.Add(createdMessage);

            Messages = new(_messages.OrderByDescending(m => m.SentAt));
        }

        private ReactiveCommand<Unit, Task> _sendCommand;

        public ReactiveCommand<Unit, Task> SendCommand
        {
            get => _sendCommand;
            set => this.RaiseAndSetIfChanged(ref _sendCommand, value);
        }
    }
}
