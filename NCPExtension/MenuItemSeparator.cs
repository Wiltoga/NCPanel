using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NCPExtension
{
    internal sealed class MenuItemSeparator : INCPMenuItem
    {
        public ICommand? Run => null;

        public IEnumerable<INCPMenuItem>? SubMenuItems => null;

        public string? Title => null;

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