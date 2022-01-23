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
        public static readonly INCPMenuItem Separator = new MenuItemSeparator();
        byte[]? Image { get; }
        ICommand? Run { get; }
        string? Title { get; }
        object? Visual { get; }
    }
}