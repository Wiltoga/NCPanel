using NCPExtension;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NCPanel
{
    public class CommandViewModel : ReactiveObject, INCPCommand
    {
        public CommandViewModel()
        {
            Visual = new Image
            {
                Source = null
            };
            ContextMenu = new ObservableCollection<INCPMenuItem>();
        }

        IEnumerable<INCPMenuItem>? INCPCommand.ContextMenu => ContextMenu;
        public ObservableCollection<INCPMenuItem> ContextMenu { get; }

        [Reactive]
        public string? Description { get; set; }

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public ICommand? Run { get; set; }

        public Image Visual { get; }

        public byte[]? GetImage()
        {
            return null;
        }

        public object? GetVisual()
        {
            return Visual;
        }

        public void Init()
        {
        }
    }
}