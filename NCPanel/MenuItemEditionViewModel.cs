using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NCPanel
{
    public class MenuItemEditionViewModel : ReactiveObject, IDisposable
    {
        private IDisposable subscriber;

        public MenuItemEditionViewModel(MenuItemViewModel source)
        {
            Source = source;
            if (source.Image is not null)
                Icon = Utils.ImageFromBytes(source.Image);
            subscriber = Source.WhenAnyValue(o => o.Image)
                .Subscribe(img =>
                {
                    if (img is not null)
                        Icon = Utils.ImageFromBytes(img);
                    else
                        Icon = null;
                });
        }

        [Reactive]
        public ImageSource? Icon { get; private set; }

        [Reactive]
        public bool InsertBot { get; set; }

        [Reactive]
        public bool InsertTop { get; set; }

        public MenuItemViewModel Source { get; }

        public void Dispose()
        {
            subscriber.Dispose();
        }
    }
}