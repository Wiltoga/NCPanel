using NCPExtension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExtensionExample
{
    public class Example : INCPCommand
    {
        public IEnumerable<INCPMenuItem>? ContextMenu => new[]
        {
            new ExampleMenuItem()
        };

        public string? Description => "description";

        public byte[]? Image => null;

        public string? Name => "Example";

        public ICommand? Run => new Command();

        public object? Visual => null;

        public void Init(InitializationInfo initializationInfo)
        {
            Console.WriteLine($"plugin location : {initializationInfo.pluginLocation}");
        }

        private class Command : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                Console.WriteLine("run");
                try
                {
                    var dialog = new Window();
                    dialog.Content = new TextBox
                    {
                        IsReadOnly = true,
                        Text = JsonConvert.SerializeObject(new
                        {
                            value1 = "test1",
                            value2 = "test2",
                        }, Formatting.Indented)
                    };
                    dialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private class ExampleMenuItem : INCPMenuItem
        {
            public byte[]? Image => null;

            public int Index => 0;

            public ICommand? Run => new Command();

            public string? Title => "some menu item";

            public object? Visual => null;

            private class Command : ICommand
            {
                public event EventHandler? CanExecuteChanged;

                public bool CanExecute(object? parameter)
                {
                    return true;
                }

                public void Execute(object? parameter)
                {
                    Console.WriteLine("menu item run");
                    try
                    {
                        var dialog = new Window();
                        dialog.Content = new TextBox
                        {
                            IsReadOnly = true,
                            Text = JsonConvert.SerializeObject(new
                            {
                                value1 = "hello from",
                                value2 = "the menu item !",
                            }, Formatting.Indented)
                        };
                        dialog.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }
    }
}