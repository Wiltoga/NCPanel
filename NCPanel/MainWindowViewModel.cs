using DynamicData;
using DynamicData.Binding;
using NCPExtension;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCPanel
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ReadOnlyObservableCollection<CommandWrapperViewModel> commands;

        public MainWindowViewModel()
        {
            PluginLoader = new PluginLoader();
            Open = true;
            Layout = Layout.Grid;
            ExtensionMode = ExtensionMode.None;
            CommandsSource = new SourceList<CommandWrapperViewModel>();
            var pluginSelector = PluginLoader.AvailablePluginsConnect.TransformMany(plugin => plugin.Commands.Select(command => new CommandWrapperViewModel(command, this)));
            pluginSelector.Subscribe();
            CommandsSource.Connect()
                .Or(pluginSelector)
                .AutoRefresh(o => o.Name)
                .Sort(Comparer<CommandWrapperViewModel>.Create((left, right) =>
                left.Name == right.Name
                    ? 0
                    : (left.Name?.CompareTo(right.Name) ?? -1)))
                .Bind(out commands)
                .Subscribe();
            for (int i = 0; i < 4; ++i)
            {
                CommandViewModel source;
                CommandsSource.Add(new CommandWrapperViewModel(source = new CommandViewModel
                {
                    Name = "test" + i,
                    Run = ReactiveCommand.Create(() => Console.WriteLine("test")),
                    Description = "some description",
                    //Image = File.ReadAllBytes("icon.png")
                }, this));
                source.ContextMenu.Add(new MenuItemViewModel
                {
                    Title = "test",
                    //Image = File.ReadAllBytes("icon.png"),
                    CommandLine = "icon.png",
                    Index = 0
                });
                source.ContextMenu.Add(new MenuItemViewModel
                {
                    Title = "test2",
                    CommandLine = "icon.png",
                    Index = 1
                });
                source.ContextMenu.Add(new MenuItemViewModel
                {
                    Title = "test3",
                    CommandLine = "icon.png",
                    Index = 3
                });
                source.ContextMenu.Add(new MenuItemViewModel
                {
                    Title = "wololo",
                    CommandLine = "icon.png",
                    Index = 2
                });
            }
        }

        public IEnumerable<CommandWrapperViewModel> Commands => commands;
        public SourceList<CommandWrapperViewModel> CommandsSource { get; }

        [Reactive]
        public ExtensionMode ExtensionMode { get; set; }

        [Reactive]
        public Layout Layout { get; set; }

        [Reactive]
        public bool Open { get; set; }

        private PluginLoader PluginLoader { get; }
    }
}