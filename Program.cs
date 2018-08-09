using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client;
using System;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class LoggerFactory : ILoggerFactory
    {
        private readonly SerilogLoggerProvider _loggerProvider;

        public LoggerFactory()
        {
            _loggerProvider = new SerilogLoggerProvider(new Serilog.LoggerConfiguration().CreateLogger());
        }

        public void AddProvider(ILoggerProvider provider)
        {
            
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggerProvider.CreateLogger(categoryName);
        }

        public void Dispose()
        {
            
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            ProcessStartInfo serverStartInfo = new ProcessStartInfo("c:\\Program Files\\LLVM\\bin\\clangd.exe")
            {
            };


            var factory = new LoggerFactory();

            using (var client = new LanguageClient(factory, serverStartInfo))
            {
                client.HandleNotification("dummy/notify", () =>
                {
                    Log.Information("Received dummy notification from language server.");
                });

                await client.Initialize("c:\\dev\\repos\\ILMD");

                var caps = client.ServerCapabilities;
                var ccaps = client.ClientCapabilities;

                

            }
        }
    }
}
