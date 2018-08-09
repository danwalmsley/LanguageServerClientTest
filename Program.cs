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
using System.IO;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol;

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
                Arguments = "-compile-commands-dir=c:\\dev\\repos\\ILMD"
            };


            var factory = new LoggerFactory();

            await Task.Factory.StartNew(async () =>
            {
                using (var client = new LanguageClient(factory, serverStartInfo))
                {
                    var doc = client.ClientCapabilities.TextDocument = new TextDocumentClientCapabilities
                    {
                        CodeAction = new CodeActionCapability { DynamicRegistration = true },
                        CodeLens = new CodeLensCapability { DynamicRegistration = true },
                        ColorProvider = new ColorProviderCapability { DynamicRegistration = true },
                        Completion = new CompletionCapability
                        {
                            CompletionItem = new CompletionItemCapability
                            {
                                CommitCharactersSupport = true,
                                DocumentationFormat = new Container<MarkupKind>(MarkupKind.Markdown, MarkupKind.Plaintext),
                                SnippetSupport = true
                            },
                            CompletionItemKind = new CompletionItemKindCapability { ValueSet = new Container<CompletionItemKind>((CompletionItemKind[])Enum.GetValues(typeof(CompletionItemKind))) },
                            ContextSupport = true,
                            DynamicRegistration = true
                        },
                        Definition = new DefinitionCapability
                        {
                            DynamicRegistration = true,
                        },
                        DocumentHighlight = new DocumentHighlightCapability
                        {
                            DynamicRegistration = true,
                        },
                        DocumentLink = new DocumentLinkCapability
                        {
                            DynamicRegistration = true,
                        },
                        DocumentSymbol = new DocumentSymbolCapability
                        {
                            DynamicRegistration = true,
                            SymbolKind = new SymbolKindCapability
                            {
                                ValueSet = new Container<SymbolKind>((SymbolKind[])Enum.GetValues(typeof(SymbolKind)))
                            }
                        },
                        Formatting = new DocumentFormattingCapability
                        {
                            DynamicRegistration = true
                        },
                        Hover = new HoverCapability
                        {
                            DynamicRegistration = true,
                            ContentFormat = new Container<MarkupKind>((MarkupKind[])Enum.GetValues(typeof(MarkupKind)))
                        },
                        Implementation = new ImplementationCapability { DynamicRegistration = true },
                        OnTypeFormatting = new DocumentOnTypeFormattingCapability { DynamicRegistration = true },
                        PublishDiagnostics = new PublishDiagnosticsCapability { RelatedInformation = true },
                        RangeFormatting = new DocumentRangeFormattingCapability { DynamicRegistration = true },
                        References = new ReferencesCapability { DynamicRegistration = true },
                        Rename = new RenameCapability { DynamicRegistration = true },
                        SignatureHelp = new SignatureHelpCapability
                        {
                            DynamicRegistration = true,
                            SignatureInformation = new SignatureInformationCapability
                            {
                                ContentFormat = new Container<MarkupKind>((MarkupKind[])Enum.GetValues(typeof(MarkupKind)))
                            }
                        },
                        Synchronization = new SynchronizationCapability { DidSave = true, DynamicRegistration = true, WillSave = true, WillSaveWaitUntil = true },
                        TypeDefinition = new TypeDefinitionCapability { DynamicRegistration = true },
                    };

                    var workspace = client.ClientCapabilities.Workspace = new WorkspaceClientCapabilities
                    {
                        ApplyEdit = true,
                        Configuration = true,
                        DidChangeConfiguration = new DidChangeConfigurationCapability { DynamicRegistration = true },
                        DidChangeWatchedFiles = new DidChangeWatchedFilesCapability { DynamicRegistration = true },
                        ExecuteCommand = new ExecuteCommandCapability { DynamicRegistration = true },
                        Symbol = new WorkspaceSymbolCapability
                        {
                            DynamicRegistration = true,
                            SymbolKind = new SymbolKindCapability
                            {
                                ValueSet = new Container<SymbolKind>((SymbolKind[])Enum.GetValues(typeof(SymbolKind)))
                            }
                        },
                        WorkspaceEdit = new WorkspaceEditCapability { DocumentChanges = true },
                        WorkspaceFolders = true
                    };



                    client.ClientCapabilities.TextDocument.PublishDiagnostics = new PublishDiagnosticsCapability
                    {
                        RelatedInformation = true
                    };


                    client.HandleNotification("dummy/notify", () =>
                    {
                        Log.Information("Received dummy notification from language server.");
                    });



                    await client.Initialize("c:\\dev\\repos\\ILMD", new InitializedParams
                    {
                    });




                    var caps = client.ServerCapabilities;
                    var ccaps = client.ClientCapabilities;


                    client.TextDocument.OnPublishDiagnostics((uri, diags) =>
                    {

                    });



                    string filePath = "c:\\dev\\repos\\ILMD\\PeripheralBoard\\PeripheralBoard.DiscoveryBoard\\main.cpp";

                    client.TextDocument.DidOpen(filePath, "cpp", File.ReadAllText(filePath));
                }
            });

            while(true)
            {
                Thread.Sleep(10);
            }
        }
    }
}
