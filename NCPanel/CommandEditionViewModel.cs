using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace NCPanel
{
    public class CommandEditionViewModel : ReactiveObject
    {
        public CommandEditionViewModel(CommandViewModel source)
        {
            Source = source;
            if (source.Image is not null)
                Icon = Utils.ImageFromBytes(source.Image);
            Source.WhenAnyValue(o => o.Image)
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

        public CommandViewModel Source { get; }
    }
}