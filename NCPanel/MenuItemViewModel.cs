﻿using NCPExtension;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
            {
                var parts = cmd?.Split(' ');
                if (parts is null or { Length: 0 })
                {
                    Run = null;
                    return;
                }
                var executable = parts[0];
                var arguments = string.Join(" ", parts.Skip(1));
                Run = ReactiveCommand.Create(() =>
                {
                    try
                    {
                        new Process
                        {
                            StartInfo = new ProcessStartInfo(executable)
                            {
                                UseShellExecute = true,
                                Arguments = arguments,
                            }
                        }.Start();
                    }
                    catch (Exception)
                    {
                    }
                });
            });
        }

        [Reactive]
        public string? CommandLine { get; set; }

        public string? IconFile { get; set; }

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
                    title = Title,
                    index = Index,
                    icon = IconFile
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
                Index = save.Value.index;
                IconFile = save.Value.icon;
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
            public string? icon;
            public byte[]? image;
            public int index;
            public string? title;
        }
    }
}