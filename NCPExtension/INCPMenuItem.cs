using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NCPExtension
{
    public interface INCPMenuItem
    {
        byte[]? Image { get; }

        int Index { get; }

        ICommand? Run { get; }

        string? Title { get; }

        object? Visual { get; }

        public static INCPMenuItem Separator(int index) => new MenuItemSeparator(index);
    }
}