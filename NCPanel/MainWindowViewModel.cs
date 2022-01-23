using DynamicData;
using NCPExtension;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCPanel
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ReadOnlyObservableCollection<CommandWrapperViewModel> commands;

        public MainWindowViewModel()
        {
            Open = true;
            Layout = Layout.Grid;
            ExtensionMode = ExtensionMode.None;
            CommandsSource = new SourceList<CommandWrapperViewModel>();
            CommandsSource.Connect()
                .Sort(Comparer<CommandWrapperViewModel>.Create((left, right) =>
                left.Source.Name == right.Source.Name
                    ? 0
                    : (left.Source.Name?.CompareTo(right.Source.Name) ?? -1)))
                .Bind(out commands)
                .Subscribe();
        }

        public IEnumerable<CommandWrapperViewModel> Commands => commands;

        [Reactive]
        public ExtensionMode ExtensionMode { get; set; }

        [Reactive]
        public Layout Layout { get; set; }

        [Reactive]
        public bool Open { get; set; }

        private SourceList<CommandWrapperViewModel> CommandsSource { get; }
    }
}