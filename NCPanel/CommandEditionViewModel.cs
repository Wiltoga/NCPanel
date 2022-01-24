using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace NCPanel
{
    public class CommandEditionViewModel : ReactiveObject, IDisposable
    {
        private ReadOnlyObservableCollection<MenuItemEditionViewModel> contextMenu;
        private ICollection<IDisposable> subDisposables;

        public CommandEditionViewModel(CommandViewModel source)
        {
            subDisposables = new List<IDisposable>();
            Source = source;
            if (source.Image is not null)
                Icon = Utils.ImageFromBytes(source.Image);
            subDisposables.Add(Source.WhenAnyValue(o => o.Image)
                .Subscribe(img =>
                {
                    if (img is not null)
                        Icon = Utils.ImageFromBytes(img);
                    else
                        Icon = null;
                }));
            subDisposables.Add(source.ContextMenu.ToObservableChangeSet()
                .Transform(menuitem => new MenuItemEditionViewModel((MenuItemViewModel)menuitem))
                .AutoRefresh(o => o.Source.Index)
                .Sort(Comparer<MenuItemEditionViewModel>.Create((left, right) => Utils.MenuItemComparer.Compare(left.Source, right.Source)))
                .Bind(out contextMenu)
                .DisposeMany()
                .Subscribe());
        }

        public ReadOnlyObservableCollection<MenuItemEditionViewModel> ContextMenu => contextMenu;

        [Reactive]
        public ImageSource? Icon { get; private set; }

        public CommandViewModel Source { get; }

        public void Dispose()
        {
            foreach (var disposable in subDisposables)
            {
                disposable.Dispose();
            }
        }
    }
}