using NCPExtension;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NCPanel
{
    public class MenuItemViewModel : ReactiveObject, INCPMenuItem
    {
        public MenuItemViewModel()
        {
            Visual = new Image
            {
                Source = null
            };
        }

        [Reactive]
        public ICommand? Run { get; set; }

        [Reactive]
        public string? Title { get; set; }

        public Image Visual { get; }

        public byte[]? GetImage()
        {
            return null;
        }

        public object? GetVisual()
        {
            return null;
        }
    }
}