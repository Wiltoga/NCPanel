using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace NCPanel
{
    public class MainWindowViewModel : ReactiveObject, IDisposable
    {
        private ReadOnlyObservableCollection<CommandWrapperViewModel> commands;

        public MainWindowViewModel(Data data)
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
            Layout = data.Layout;
            foreach (var command in data.Commands)
            {
                var loadedCommand = new CommandViewModel
                {
                    CommandLine = command.CommandLine,
                    Description = command.Description,
                    IconFile = command.IconName,
                    Name = command.Name,
                };
                if (loadedCommand.IconFile is not null)
                    loadedCommand.Image = DataStorage.LoadImage(loadedCommand.IconFile);
                foreach (var menuitem in command.ContextMenu)
                {
                    var loadedMenuItem = new MenuItemViewModel
                    {
                        CommandLine = menuitem.CommandLine,
                        IconFile = menuitem.IconName,
                        Index = menuitem.Index,
                        Title = menuitem.Title,
                    };
                    if (loadedMenuItem.IconFile is not null)
                        loadedMenuItem.Image = DataStorage.LoadImage(loadedMenuItem.IconFile);
                    loadedCommand.ContextMenu.Add(loadedMenuItem);
                }
                CommandsSource.Add(new CommandWrapperViewModel(loadedCommand, this));
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

        public void Dispose()
        {
            foreach (var command in Commands)
            {
                command.Dispose();
            }
        }

        public Data Export()
        {
            return new Data(Layout, CommandsSource.Items
                .Select(command =>
                {
                    var viewModelCommand = (CommandViewModel)command.Source;
                    if (viewModelCommand.IconFile is null && viewModelCommand.Image is not null)
                    {
                        viewModelCommand.IconFile = Guid.NewGuid().ToString();
                        DataStorage.SaveImage(viewModelCommand.Image, viewModelCommand.IconFile);
                    }
                    return new CommandData(
                        command.Source.Name,
                        command.Source.Description,
                        viewModelCommand.CommandLine,
                        viewModelCommand.IconFile,
                        command.Source.ContextMenu?
                            .Select(menuitem =>
                            {
                                var viewModelMenuItem = (MenuItemViewModel)menuitem;
                                if (viewModelMenuItem.IconFile is null && viewModelMenuItem.Image is not null)
                                {
                                    viewModelMenuItem.IconFile = Guid.NewGuid().ToString();
                                    DataStorage.SaveImage(viewModelMenuItem.Image, viewModelMenuItem.IconFile);
                                }
                                return new MenuItemData(
                                    menuitem.Title,
                                    viewModelMenuItem.CommandLine,
                                    viewModelMenuItem.IconFile,
                                    viewModelMenuItem.Index);
                            }).ToArray()
                            ?? Array.Empty<MenuItemData>());
                }).ToArray());
        }
    }
}