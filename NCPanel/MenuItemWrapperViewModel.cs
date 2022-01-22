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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NCPanel
{
    public class MenuItemWrapperViewModel
    {
        private ReadOnlyObservableCollection<MenuItemWrapperViewModel> subMenuItems;

        public MenuItemWrapperViewModel(INCPMenuItem menuitem, CommandWrapperViewModel parent)
        {
            Parent = parent;
            Source = menuitem;
            if (menuitem.SubMenuItems is INotifyCollectionChanged)
            {
                var toObservableMethod = typeof(ObservableCollectionEx).GetMethods().Where(m =>
                    m.Name == nameof(ObservableCollectionEx.ToObservableChangeSet)
                    && m.GetParameters().Length == 1
                    && m.GetParameters().First().ParameterType.Name == "TCollection").First();
                var generic = toObservableMethod.MakeGenericMethod(menuitem.SubMenuItems.GetType(), typeof(INCPMenuItem));
                var sourceSet = generic.Invoke(null, new[] { menuitem.SubMenuItems }) as IObservable<IChangeSet<INCPMenuItem>>;
                if (sourceSet is not null)
                    SubMenuItemsSource = new SourceList<MenuItemWrapperViewModel>(sourceSet.Transform(menuitem => new MenuItemWrapperViewModel(menuitem, Parent)));
            }
            if (SubMenuItemsSource is null)
            {
                SubMenuItemsSource = new SourceList<MenuItemWrapperViewModel>();
                if (menuitem.SubMenuItems is not null)
                    SubMenuItemsSource.Edit(updater => updater.AddRange(menuitem.SubMenuItems.Select(menuitem => new MenuItemWrapperViewModel(menuitem, Parent))));
            }
            SubMenuItemsSource.Connect()
                .Bind(out subMenuItems)
                .Subscribe();
            Visual = menuitem.GetVisual();
            if (Visual is null)
            {
                var imgSource = menuitem.GetImage();
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
                    Visual = image;
                }
            }
        }

        public CommandWrapperViewModel Parent { get; }
        public INCPMenuItem Source { get; }
        public IEnumerable<MenuItemWrapperViewModel> SubMenuItems => subMenuItems;
        public object? Visual { get; }
        private SourceList<MenuItemWrapperViewModel> SubMenuItemsSource { get; }
    }
}