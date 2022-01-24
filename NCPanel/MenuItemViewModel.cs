using NCPExtension;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NCPanel
{
    public class MenuItemViewModel : ReactiveObject, INCPMenuItem, IEditableObject, IDisposable
    {
        private Saved? save;
        private IDisposable subscriber;

        public MenuItemViewModel()
        {
            subscriber = this.WhenAnyValue(o => o.CommandLine).Subscribe(cmd =>
            Run = cmd is not null
                ? ReactiveCommand.Create(() =>
                {
                    try
                    {
                        new Process
                        {
                            StartInfo = new ProcessStartInfo(cmd)
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                    }
                    catch (Exception)
                    {
                    }
                })
                : null);
        }

        [Reactive]
        public string? CommandLine { get; set; }

        [Reactive]
        public byte[]? Image { get; set; }

        [Reactive]
        public int Index { get; set; }

        [Reactive]
        public ICommand? Run { get; set; }

        [Reactive]
        public string? Title { get; set; }

        public object? Visual { get; }

        public void BeginEdit()
        {
            if (save is null)
            {
                save = new Saved
                {
                    commandLine = CommandLine,
                    image = Image,
                    title = Title
                };
            }
        }

        public void CancelEdit()
        {
            if (save is not null)
            {
                CommandLine = save.Value.commandLine;
                Image = save.Value.image;
                Title = save.Value.title;
            }
        }

        public void Dispose()
        {
            subscriber.Dispose();
        }

        public void EndEdit()
        {
            save = null;
        }

        private struct Saved
        {
            public string? commandLine;
            public byte[]? image;
            public string? title;
        }
    }
}