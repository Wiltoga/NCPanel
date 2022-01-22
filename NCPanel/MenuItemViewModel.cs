using NCPExtension;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NCPanel
{
    public class MenuItemViewModel : ReactiveObject, INCPMenuItem
    {
        public MenuItemViewModel(string? title, ICommand? run, IEnumerable<INCPMenuItem>? subMenuItems, byte[]? image)
        {
            Title = title;
            Run = run;
            SubMenuItems = subMenuItems;
            Image = image;
        }

        public ICommand? Run { get; }
        public IEnumerable<INCPMenuItem>? SubMenuItems { get; }
        public string? Title { get; }
        private byte[]? Image { get; }

        public byte[]? GetImage()
        {
            return Image;
        }

        public object? GetVisual()
        {
            return null;
        }
    }
}