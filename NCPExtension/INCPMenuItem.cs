using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NCPExtension
{
    /**
     * Interface to implement to use in a command as part of the context menu
     */

    public interface INCPMenuItem
    {
        /**
         * Image data (PNG encoded preferred) of the icon of the menu item. Use either this or Visual.
         */
        byte[]? Image { get; }
        /**
         * Order of the menu item in the list.
         */

        int Index { get; }
        /**
         * Command performed when clicking on the menu item
         */

        ICommand? Run { get; }
        /**
         * Text displayed on the menu item
         */

        string? Title { get; }
        /**
         * Custom visual of the menu item. Can be anything put into the visual tree.
         * Should be used for dynamic icons, by assigning a WPF control to Visual.
         * Use either this or Image.
         */

        object? Visual { get; }
        /**
         * Create a separator menu item
         */

        public static INCPMenuItem Separator(int index) => new MenuItemSeparator(index);
    }
}