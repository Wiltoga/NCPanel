using NCPExtension;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NCPanel
{
    public class CommandViewModel : ReactiveObject, INCPCommand, IEditableObject
    {
        private Saved? save;

        public CommandViewModel()
        {
            Visual = new Image
            {
                Source = null
            };
            ContextMenu = new ObservableCollection<INCPMenuItem>();
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

        IEnumerable<INCPMenuItem>? INCPCommand.ContextMenu => ContextMenu;

        public ObservableCollection<INCPMenuItem> ContextMenu { get; }

        [Reactive]
        public string? Description { get; set; }

        [Reactive]
        public byte[]? Image { get; set; }

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public ICommand? Run { get; private set; }

        private Image Visual { get; }

        public void BeginEdit()
        {
            if (save is not null)
            {
                save = new Saved
                {
                    commandLine = CommandLine,
                    description = Description,
                    image = Image,
                    name = Name,
                    contextMenu = ContextMenu.ToArray()
                };
            }
        }

        public void CancelEdit()
        {
            if (save is not null)
            {
                CommandLine = save.Value.commandLine;
                Description = save.Value.description;
                Image = save.Value.image;
                Name = save.Value.name;
                ContextMenu.Clear();
                foreach (var item in save.Value.contextMenu)
                {
                    ContextMenu.Add(item);
                }
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

        public void Init()
        {
        }

        private struct Saved
        {
            public string? commandLine;
            public INCPMenuItem[] contextMenu;
            public string? description;
            public byte[]? image;
            public string? name;
        }
    }
}