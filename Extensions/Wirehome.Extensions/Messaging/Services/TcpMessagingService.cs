﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using Wirehome.Contracts.Logging;
using Wirehome.Extensions.Contracts;
using Wirehome.Extensions.Messaging.Core;

namespace Wirehome.Extensions.Messaging.Services
{
    public class TcpMessagingService : ITcpMessagingService
    {
        private readonly ILogger _logService;
        private readonly IEventAggregator _eventAggregator;

        public TcpMessagingService(ILogService logService, IEventAggregator eventAggregator)
        {
            _logService = logService.CreatePublisher(nameof(TcpMessagingService));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        }

        public Task Initialize()
        {
            _eventAggregator.SubscribeForAsyncResult<ITcpMessage>(MessageHandler);
            return Task.CompletedTask;
        }
        
        private Task<object> MessageHandler(IMessageEnvelope<ITcpMessage> message)
        {
            return SendMessage(message);
        }

        private async Task<object> SendMessage(IMessageEnvelope<ITcpMessage> message)
        {
            try
            {
                using (var socket = new TcpClient())
                {
                    var uri = new Uri($"tcp://{message.Message.MessageAddress()}");
                    await socket.ConnectAsync(uri.Host, uri.Port).ConfigureAwait(false);
                    using (var stream = socket.GetStream())
                    {
                        var messageBytes = message.Message.Serialize();
                        await stream.WriteAsync(messageBytes, 0, messageBytes.Length, message.CancellationToken).ConfigureAwait(false);
                        return await ReadString(stream).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Error(ex, $"Message {message.GetType().Name} failed during send TCP message");
            }
            return null;
        }

        private static async Task<string> ReadString(NetworkStream stream)
        {
            var bytesRead = 0;
            var buffer = new byte[256];
            var result = new List<byte>();

            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                result.AddRange(buffer.Take(bytesRead));
            }
            while (bytesRead == buffer.Length);

            return System.Text.Encoding.UTF8.GetString(result.ToArray());
        }
    }

   
}