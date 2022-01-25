using DynamicData;
using DynamicData.Binding;
using NCPExtension;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NCPanel
{
    public class CommandWrapperViewModel : ReactiveObject, IDisposable
    {
        private ReadOnlyObservableCollection<MenuItemWrapperViewModel> contextMenu;
        private IDisposable subscriber;

        public CommandWrapperViewModel(INCPCommand command, MainWindowViewModel parent)
        {
            Parent = parent;
            Source = command;
            Name = command.Name;
            if (command.ContextMenu is INotifyCollectionChanged)
            {
                var toObservableMethod = typeof(ObservableCollectionEx).GetMethods().Where(m =>
                    m.Name == nameof(ObservableCollectionEx.ToObservableChangeSet)
                    && m.GetParameters().Length == 1
                    && m.GetParameters().First().ParameterType.Name == "TCollection").First();
                var generic = toObservableMethod.MakeGenericMethod(command.ContextMenu.GetType(), typeof(INCPMenuItem));
                var sourceSet = generic.Invoke(null, new[] { command.ContextMenu }) as IObservable<IChangeSet<INCPMenuItem>>;
                if (sourceSet is not null)
                    ContextMenuSource = new SourceList<MenuItemWrapperViewModel>(sourceSet.Transform(menuitem => new MenuItemWrapperViewModel(menuitem, this)).DisposeMany());
            }
            if (ContextMenuSource is null)
            {
                ContextMenuSource = new SourceList<MenuItemWrapperViewModel>();
                if (command.ContextMenu is not null)
                    ContextMenuSource.Edit(updater => updater.AddRange(command.ContextMenu.Select(menuitem => new MenuItemWrapperViewModel(menuitem, this))));
            }
            subscriber = ContextMenuSource.Connect()
                .AutoRefresh(o => o.Index)
                .Sort(Utils.MenuItemWrapperComparer)
                .Bind(out contextMenu)
                .Subscribe();
            if (Source is CommandViewModel genericCommand)
            {
                ContextMenuSource.Add(new GeneratedMenuItemViewModel(INCPMenuItem.Separator(0), this));
                ContextMenuSource.Add(new GeneratedMenuItemViewModel(new MenuItemViewModel
                {
                    Title = "Edit",
                    Index = 1,
                    Image = Properties.Resources.edit,
                    Run = ReactiveCommand.Create(() =>
                    {
                        if (parent.ExtensionMode != ExtensionMode.None &&
                            parent.ExtensionMode != ExtensionMode.Maximized)
                            parent.Open = false;
                        genericCommand.BeginEdit();
                        var dialog = new CommandEdition(genericCommand)
                        {
                            Owner = App.Current.MainWindow
                        };
                        if (dialog.ShowDialog() is true)
                        {
                            genericCommand.EndEdit();
                            DataStorage.Save(Parent.Export());
                        }
                        else
                            genericCommand.CancelEdit();
                    })
                }, this));
                ContextMenuSource.Add(new GeneratedMenuItemViewModel(new MenuItemViewModel
                {
                    Title = "Delete",
                    Image = Properties.Resources.delete,
                    Index = 2,
                    Run = ReactiveCommand.Create(() =>
                    {
                        Parent.CommandsSource.Remove(this);
                        DataStorage.Save(Parent.Export());
                    })
                }, this));
            }
            Visual = command.Visual;
            if (Visual is null)
            {
                var imgSource = command.Image;
                if (imgSource is not null)
                {
                    Visual = new Image
                    {
                        Source = Utils.ImageFromBytes(imgSource)
                    };
                }
            }
            if (command is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(INCPCommand.Image))
                    {
                        var imgSource = command.Image;
                        if (imgSource is not null)
                        {
                            Visual = new Image
                            {
                                Source = Utils.ImageFromBytes(imgSource)
                            };
                        }
                    }
                    else if (e.PropertyName == nameof(INCPCommand.Visual))
                    {
                        Visual = command.Visual;
                    }
                    else if (e.PropertyName == nameof(INCPCommand.Name))
                    {
                        Name = command.Name;
                    }
                };
            }
        }

        public IEnumerable<MenuItemWrapperViewModel> ContextMenu => contextMenu;

        [Reactive]
        public string? Name { get; private set; }

        public MainWindowViewModel Parent { get; }
        public INCPCommand Source { get; }

        [Reactive]
        public object? Visual { get; private set; }

        private SourceList<MenuItemWrapperViewModel> ContextMenuSource { get; }

        public void Dispose()
        {
            subscriber.Dispose();
        }
    }
}