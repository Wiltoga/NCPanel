using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCPanel
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            Open = true;
            Layout = Layout.Grid;
            ExtensionMode = ExtensionMode.None;
        }

        [Reactive]
        public ExtensionMode ExtensionMode { get; set; }

        [Reactive]
        public Layout Layout { get; set; }

        [Reactive]
        public bool Open { get; set; }
    }
}