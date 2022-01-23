using System.Windows.Input;

namespace NCPExtension
{
    public interface INCPCommand
    {
        IEnumerable<INCPMenuItem>? ContextMenu { get; }
        string? Description { get; }
        byte[]? Image { get; }
        string? Name { get; }
        ICommand? Run { get; }
        object? Visual { get; }

        void Init();
    }
}