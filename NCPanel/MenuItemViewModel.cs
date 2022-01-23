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
    public class MenuItemViewModel : ReactiveObject, INCPMenuItem, IEditableObject
    {
        private Saved? save;

        public MenuItemViewModel()
        {
            Visual = new Image
            {
                Source = null
            };
            this.WhenAnyValue(o => o.CommandLine).Subscribe(cmd =>
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
            this.WhenAnyValue(o => o.Image).Subscribe(img =>
            Visual.Source = img is not null
                ? Utils.ImageFromBytes(img)
                : null);
        }

        [Reactive]
        public string? CommandLine { get; set; }

        [Reactive]
        public byte[]? Image { get; set; }

        [Reactive]
        public ICommand? Run { get; private set; }

        [Reactive]
        public string? Title { get; set; }

        private Image Visual { get; }

        public void BeginEdit()
        {
            if (save is not null)
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

        public void EndEdit()
        {
            save = null;
        }

        public byte[]? GetImage()
        {
            return null;
        }

        public object? GetVisual()
        {
            return Visual;
        }

        private struct Saved
        {
            public string? commandLine;
            public byte[]? image;
            public string? title;
        }
    }
}