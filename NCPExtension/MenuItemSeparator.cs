using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NCPExtension
{
    [Separator]
    internal sealed class MenuItemSeparator : INCPMenuItem
    {
        public MenuItemSeparator(int index) => Index = index;

        public byte[]? Image => null;
        public int Index { get; }
        public ICommand? Run => null;

        public string? Title => null;
        public object? Visual => null;
    }
}