using System.Windows.Input;

namespace NCPExtension
{
    public interface INCPCommand
    {
        IEnumerable<INCPMenuItem>? ContextMenu { get; }
        string? Description { get; }
        string? Name { get; }

        ICommand? Run { get; }

        byte[]? GetImage();

        object? GetVisual();

        void Init();
    }
}