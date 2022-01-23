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
        public byte[]? Image => null;
        public ICommand? Run => null;

        public string? Title => null;
        public object? Visual => null;
    }
}