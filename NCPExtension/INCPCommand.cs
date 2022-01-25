using System.Windows.Input;

namespace NCPExtension
{
    /**
     * Interface to create a custom command. Implements this to be displayed in NCPanel
     */

    public interface INCPCommand
    {
        /**
         * The list of menuitems contained in the context menu (right click) of the command
         */
        IEnumerable<INCPMenuItem>? ContextMenu { get; }
        /**
         * Short description of the command
         */
        string? Description { get; }
        /**
         * Image data (PNG encoded preferred) of the icon of the command. Use either this or Visual.
         */
        byte[]? Image { get; }
        /**
         * The displayed name of the command
         */
        string? Name { get; }
        /**
         * The action performed when left clicking on the command
         */
        ICommand? Run { get; }
        /**
         * Custom visual of the command. Can be anything put into the visual tree.
         * Should be used for dynamic icons, by assigning a WPF control to Visual.
         * Use either this or Image.
         */
        object? Visual { get; }
        /**
         * Initial call to the command when loaded into the app.
         */

        void Init(InitializationInfo initializationInfo);
    }
}