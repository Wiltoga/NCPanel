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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NCPanel
{
    public class MenuItemWrapperViewModel : ReactiveObject
    {
        public MenuItemWrapperViewModel(INCPMenuItem menuitem, CommandWrapperViewModel parent)
        {
            Parent = parent;
            Source = menuitem;
            Visual = menuitem.Visual;
            Index = menuitem.Index;
            if (Visual is null)
            {
                var imgSource = menuitem.Image;
                if (imgSource is not null)
                {
                    Visual = new Image
                    {
                        Source = Utils.ImageFromBytes(imgSource)
                    };
                }
            }
            if (menuitem is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(INCPMenuItem.Image))
                    {
                        var imgSource = menuitem.Image;
                        if (imgSource is not null)
                        {
                            Visual = new Image
                            {
                                Source = Utils.ImageFromBytes(imgSource)
                            };
                        }
                    }
                    else if (e.PropertyName == nameof(INCPMenuItem.Visual))
                    {
                        Visual = menuitem.Visual;
                    }
                    else if (e.PropertyName == nameof(INCPMenuItem.Index))
                    {
                        Index = menuitem.Index;
                    }
                };
            }
        }

        [Reactive]
        public int Index { get; private set; }

        public CommandWrapperViewModel Parent { get; }
        public INCPMenuItem Source { get; }

        [Reactive]
        public object? Visual { get; private set; }
    }
}