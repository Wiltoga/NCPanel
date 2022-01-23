using DynamicData;
using DynamicData.Binding;
using NCPExtension;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public class CommandWrapperViewModel
    {
        private ReadOnlyObservableCollection<MenuItemWrapperViewModel> contextMenu;

        public CommandWrapperViewModel(INCPCommand command, MainWindowViewModel parent)
        {
            Parent = parent;
            Source = command;
            command.Init();
            if (command.ContextMenu is INotifyCollectionChanged)
            {
                var toObservableMethod = typeof(ObservableCollectionEx).GetMethods().Where(m =>
                    m.Name == nameof(ObservableCollectionEx.ToObservableChangeSet)
                    && m.GetParameters().Length == 1
                    && m.GetParameters().First().ParameterType.Name == "TCollection").First();
                var generic = toObservableMethod.MakeGenericMethod(command.ContextMenu.GetType(), typeof(INCPMenuItem));
                var sourceSet = generic.Invoke(null, new[] { command.ContextMenu }) as IObservable<IChangeSet<INCPMenuItem>>;
                if (sourceSet is not null)
                    ContextMenuSource = new SourceList<MenuItemWrapperViewModel>(sourceSet.Transform(menuitem => new MenuItemWrapperViewModel(menuitem, this)));
            }
            if (ContextMenuSource is null)
            {
                ContextMenuSource = new SourceList<MenuItemWrapperViewModel>();
                if (command.ContextMenu is not null)
                    ContextMenuSource.Edit(updater => updater.AddRange(command.ContextMenu.Select(menuitem => new MenuItemWrapperViewModel(menuitem, this))));
            }
            ContextMenuSource.Connect()
                .Sort(Comparer<MenuItemWrapperViewModel>.Create((left, right) =>
                {
                    if (left is GeneratedMenuItemViewModel genLeft)
                    {
                        if (right is GeneratedMenuItemViewModel genRight)
                            return genLeft.Index.CompareTo(genRight.Index);
                        else
                            return 1;
                    }
                    else if (right is GeneratedMenuItemViewModel genRight)
                        return -1;
                    else
                        return 0;
                }))
                .Bind(out contextMenu)
                .Subscribe();
            if (Source is CommandViewModel genericCommand)
            {
                ContextMenuSource.Add(new GeneratedMenuItemViewModel(INCPMenuItem.Separator, this, 0));
                ContextMenuSource.Add(new GeneratedMenuItemViewModel(new MenuItemViewModel
                {
                    Title = "Edit",
                    Image = Properties.Resources.edit
                }, this, 1));
                ContextMenuSource.Add(new GeneratedMenuItemViewModel(new MenuItemViewModel
                {
                    Title = "Delete",
                    Image = Properties.Resources.delete
                }, this, 2));
            }
            Visual = command.GetVisual();
            if (Visual is null)
            {
                var imgSource = command.GetImage();
                if (imgSource is not null)
                {
                    Visual = new Image
                    {
                        Source = Utils.ImageFromBytes(imgSource)
                    };
                }
            }
        }

        public IEnumerable<MenuItemWrapperViewModel> ContextMenu => contextMenu;
        public MainWindowViewModel Parent { get; }
        public INCPCommand Source { get; }
        public object? Visual { get; }
        private SourceList<MenuItemWrapperViewModel> ContextMenuSource { get; }
    }
}