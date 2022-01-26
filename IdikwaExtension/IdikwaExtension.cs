using NCPExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IdikwaExtension
{
    public class BasicMenuItem : INCPMenuItem
    {
        public byte[]? Image { get; set; }

        public int Index { get; set; }

        public ICommand? Run { get; set; }

        public string? Title { get; set; }

        public object? Visual => null;
    }

    public class Command : ICommand, INotifyPropertyChanged
    {
        private Action action;
        private bool canExecute;

        public Command(Action a)
        {
            action = a;
            CanExec = false;
        }

        public event EventHandler? CanExecuteChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool CanExec
        {
            get => canExecute;
            set
            {
                if (canExecute != value)
                {
                    canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanExec)));
                }
            }
        }

        public bool CanExecute(object? parameter)
        {
            return CanExec;
        }

        public void Execute(object? parameter)
        {
            action();
        }
    }

    public class IdikwaExtension : INCPCommand
    {
        private bool recording;

        public IdikwaExtension()
        {
            CancellationTokenSource = new CancellationTokenSource();
            Run = Command = new Command(() =>
            {
                if (Recording)
                {
                    Dispatcher?.Invoke(() =>
                    {
                        var instance = Exe;
                        instance.StartInfo.ArgumentList.Add("--capture-record");
                        instance.Start();
                    });
                }
                else
                {
                    Dispatcher?.Invoke(() =>
                    {
                        var instance = Exe;
                        instance.StartInfo.ArgumentList.Add("--start-record");
                        instance.Start();
                    });
                }
            });
            CancelRecord = new Command(() =>
            {
                Dispatcher?.Invoke(() =>
                {
                    var instance = Exe;
                    instance.StartInfo.ArgumentList.Add("--stop-record");
                    instance.Start();
                });
            });
            QueueCommand = new Command(() =>
            {
                Dispatcher?.Invoke(() =>
                {
                    var instance = Exe;
                    instance.StartInfo.ArgumentList.Add("--queue-record");
                    instance.Start();
                });
            });
        }

        public Command CancelRecord { get; }

        public Command Command { get; }

        public IEnumerable<INCPMenuItem>? ContextMenu { get; set; }

        public string? Description => "Audio recording software";

        public string? ExeLocation { get; private set; }

        public byte[]? Image => null;

        public string? Name => "Idikwa";

        public Command QueueCommand { get; }

        public ImageSource? RecordOff { get; private set; }

        public ImageSource? RecordOn { get; private set; }

        public ICommand Run { get; }

        public object? Visual { get; private set; }

        private CancellationToken CancellationToken => CancellationTokenSource.Token;

        private CancellationTokenSource CancellationTokenSource { get; }

        private IDispatcher? Dispatcher { get; set; }

        private Process Exe => new Process
        {
            StartInfo = new ProcessStartInfo(ExeLocation ?? "")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        private bool Recording
        {
            get => recording;
            set => recording = CancelRecord.CanExec = QueueCommand.CanExec = value;
        }

        public static ImageSource ImageFromBytes(byte[] imgSource)
        {
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imgSource))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel();
        }

        public async void Init(InitializationInfo initializationInfo)
        {
            Dispatcher = initializationInfo.uiDispatcher;
            RecordOn = ImageFromBytes(File.ReadAllBytes(Path.Combine(initializationInfo.pluginLocation.FullName, "recordOn.png")));
            RecordOff = ImageFromBytes(File.ReadAllBytes(Path.Combine(initializationInfo.pluginLocation.FullName, "recordOff.png")));
            Visual = new IdikwaVisual()
            {
                DataContext = this
            };
            ExeLocation = Path.Combine(initializationInfo.pluginLocation.FullName, "idikwa-api.exe");
            CancelRecord.CanExec = false;
            ContextMenu = new[]
            {
                new BasicMenuItem
                {
                    Title = "Put in waiting queue",
                    Index = 0,
                    Run = QueueCommand,
                    Image = File.ReadAllBytes(Path.Combine(initializationInfo.pluginLocation.FullName, "queue.png"))
                },
                new BasicMenuItem
                {
                    Title = "Stop recording",
                    Index = 1,
                    Run = CancelRecord,
                    Image = File.ReadAllBytes(Path.Combine(initializationInfo.pluginLocation.FullName, "stop.png"))
                },
            };
            await Connect();
        }

        private async Task CaptureRecordChanges()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (CancellationToken.IsCancellationRequested)
                        break;
                    var instance = Exe;
                    instance.StartInfo.ArgumentList.Add("--wait-recording");
                    instance.Start();
                    var response = instance.StandardOutput.ReadToEnd();
                    instance.WaitForExit();
                    if (!response.StartsWith("error:"))
                    {
                        Dispatcher?.Invoke(() =>
                        {
                            Recording = response.StartsWith("true");
                        });
                    }
                    else
                    {
                        Dispatcher?.Invoke(() =>
                        {
                            Recording = false;
                            Command.CanExec = false;
                        });
                        await Connect();
                        break;
                    }
                }
            });
        }

        private async Task Connect()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (CancellationToken.IsCancellationRequested)
                        break;
                    var instance = Exe;
                    instance.StartInfo.ArgumentList.Add("--recording");
                    instance.Start();
                    var response = instance.StandardOutput.ReadToEnd();
                    instance.WaitForExit();
                    if (!response.StartsWith("error:"))
                    {
                        Dispatcher?.Invoke(() =>
                        {
                            Recording = response.StartsWith("true");
                            Command.CanExec = true;
                        });
                        break;
                    }
                }
                await CaptureRecordChanges();
            });
        }
    }
}