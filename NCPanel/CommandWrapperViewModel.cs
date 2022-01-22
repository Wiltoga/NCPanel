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
                .Bind(out contextMenu)
                .Subscribe();
            Visual = command.GetVisual();
            if (Visual is null)
            {
                var imgSource = command.GetImage();
                if (imgSource is not null)
                {
                    var image = new BitmapImage();
                    using (var mem = new MemoryStream(imgSource))
                    {
                        mem.Position = 0;
                        image.BeginInit();
                        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = null;
                        image.StreamSource = mem;
                        image.EndInit();
                    }
                    image.Freeze();
                    Visual = new Image
                    {
                        Source = image
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