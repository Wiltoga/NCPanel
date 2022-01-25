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
    public class CommandViewModel : ReactiveObject, INCPCommand, IEditableObject, IDisposable
    {
        private Saved? save;
        private IDisposable subscriber;

        public CommandViewModel()
        {
            ContextMenu = new ObservableCollection<INCPMenuItem>();
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

        IEnumerable<INCPMenuItem>? INCPCommand.ContextMenu => ContextMenu;

        public ObservableCollection<INCPMenuItem> ContextMenu { get; }

        [Reactive]
        public string? Description { get; set; }

        [Reactive]
        public byte[]? Image { get; set; }

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public ICommand? Run { get; set; }

        public object? Visual { get; }

        public void BeginEdit()
        {
            if (save is null)
            {
                save = new Saved
                {
                    commandLine = CommandLine,
                    description = Description,
                    image = Image,
                    name = Name,
                    contextMenu = ContextMenu.ToArray()
                };
                foreach (var menuitem in ContextMenu)
                {
                    ((MenuItemViewModel)menuitem).BeginEdit();
                }
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
                    ((MenuItemViewModel)item).CancelEdit();
                }
            }
        }

        public void Dispose()
        {
            subscriber.Dispose();
        }

        public void EndEdit()
        {
            save = null;
            foreach (var menuitem in ContextMenu)
            {
                ((MenuItemViewModel)menuitem).EndEdit();
            }
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