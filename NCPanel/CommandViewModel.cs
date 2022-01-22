using NCPExtension;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NCPanel
{
    public class CommandViewModel : ReactiveObject, INCPCommand
    {
        public CommandViewModel(string name, string description, ICommand? run, IEnumerable<INCPMenuItem>? contextMenu, byte[]? image)
        {
            Name = name;
            Description = description;
            Run = run;
            ContextMenu = contextMenu;
            Image = image;
        }

        public IEnumerable<INCPMenuItem>? ContextMenu { get; }
        public string Description { get; }
        public string Name { get; }

        public ICommand? Run { get; }
        private byte[]? Image { get; }

        public byte[]? GetImage()
        {
            return Image;
        }

        public object? GetVisual()
        {
            return null;
        }

        public void Init()
        {
        }
    }
}