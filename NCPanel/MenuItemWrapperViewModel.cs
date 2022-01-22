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
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NCPanel
{
    public class MenuItemWrapperViewModel
    {
        public MenuItemWrapperViewModel(INCPMenuItem menuitem, CommandWrapperViewModel parent)
        {
            Parent = parent;
            Source = menuitem;
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
                    Visual = new Image
                    {
                        Source = image
                    };
                }
            }
        }

        public CommandWrapperViewModel Parent { get; }
        public INCPMenuItem Source { get; }
        public object? Visual { get; }
    }
}