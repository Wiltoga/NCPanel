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
                    Visual = new Image
                    {
                        Source = Utils.ImageFromBytes(imgSource)
                    };
                }
            }
        }

        public CommandWrapperViewModel Parent { get; }
        public INCPMenuItem Source { get; }
        public object? Visual { get; }
    }
}