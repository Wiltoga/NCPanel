using DynamicData;
using NCPExtension;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace NCPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MaxCursorDistanceToLeave = 200;
        private bool lockOpenedSizes;
        private double OpenedHeight;
        private double OpenedWith;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(DataStorage.Load());
            lockOpenedSizes = false;
            OpenedWith = Width;
            OpenedHeight = Height;
            MinHeight = 250;
            MinWidth = 350;
            Handle = new WindowInteropHelper(this).Handle;

            ViewModel.WhenAnyValue(vm => vm.Open).Subscribe(opened =>
            {
                lockOpenedSizes = true;
                var mode = GetExtensionMode();
                var middleOfWindow = new Point(Left + Width / 2, Top + Height / 2);
                if (opened)
                {
                    MinHeight = 250;
                    MinWidth = 350;
                    Width = OpenedWith;
                    Height = OpenedHeight;
                }
                else
                {
                    MinHeight = 250;
                    MinWidth = 48;
                    Width = 48;
                    Height = 250;
                }
                var screen = Screen.FromPoint(new System.Drawing.Point((int)middleOfWindow.X, (int)middleOfWindow.Y));
                switch (mode)
                {
                    case ExtensionMode.TopLeft:
                        Left = screen.WorkingArea.Left;
                        Top = screen.WorkingArea.Top;
                        break;

                    case ExtensionMode.TopRight:
                        Left = screen.WorkingArea.Left + screen.WorkingArea.Width - Width;
                        Top = screen.WorkingArea.Top;
                        break;

                    case ExtensionMode.BottomLeft:
                        Left = screen.WorkingArea.Left;
                        Top = screen.WorkingArea.Top + screen.WorkingArea.Height - Height;
                        break;

                    case ExtensionMode.BottomRight:
                        Left = screen.WorkingArea.Left + screen.WorkingArea.Width - Width;
                        Top = screen.WorkingArea.Top + screen.WorkingArea.Height - Height;
                        break;
                }
                lockOpenedSizes = false;
            });

            var timer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += (sender, e) =>
            {
                if (ViewModel.Open && WindowState != WindowState.Maximized)
                {
                    if (GetExtensionMode() == ExtensionMode.None)
                    {
                        if (Topmost)
                            Topmost = false;
                        return;
                    }
                    else
                    {
                        if (!Topmost)
                            Topmost = true;
                    }
                    var mousePos = Control.MousePosition;
                    double distX = 0;
                    double distY = 0;
                    if (mousePos.X < Left && mousePos.Y < Top)
                    {
                        distX = mousePos.X - Left;
                        distY = mousePos.Y - Top;
                    }
                    else if (mousePos.X > Left + Width && mousePos.Y < Top)
                    {
                        distX = Left + Width - mousePos.X;
                        distY = mousePos.Y - Top;
                    }
                    else if (mousePos.X > Left + Width && mousePos.Y > Top + Height)
                    {
                        distX = Left + Width - mousePos.X;
                        distY = Top + Height - mousePos.Y;
                    }
                    else if (mousePos.X < Left && mousePos.Y > Top + Height)
                    {
                        distX = mousePos.X - Left;
                        distY = Top + Height - mousePos.Y;
                    }
                    else if (mousePos.X < Left)
                    {
                        distX = mousePos.X - Left;
                    }
                    else if (mousePos.X > Left + Width)
                    {
                        distX = Left + Width - mousePos.X;
                    }
                    else if (mousePos.Y < Top)
                    {
                        distY = mousePos.Y - Top;
                    }
                    else if (mousePos.Y > Top + Height)
                    {
                        distY = Top + Height - mousePos.Y;
                    }
                    if (MaxCursorDistanceToLeave * MaxCursorDistanceToLeave < distX * distX + distY * distY)
                        ViewModel.Open = false;
                }
            };
            timer.Start();
            Closed += (sender, e) => timer.Stop();
        }

        private IntPtr Handle { get; }

        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DataStorage.Save(ViewModel.Export());
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            ParsePosition();
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ViewModel.Open = true;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (!lockOpenedSizes && ViewModel.Open)
            {
                OpenedWith = Width;
                OpenedHeight = Height;
            }
            ParsePosition();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            ParsePosition();
        }

        private ExtensionMode GetExtensionMode()
        {
            var middleOfWindow = new Point(Left + Width / 2, Top + Height / 2);
            var screen = Screen.FromPoint(new System.Drawing.Point((int)middleOfWindow.X, (int)middleOfWindow.Y));
            var verticalPercentOffset = Top / (screen.WorkingArea.Height - Height);
            var horizontalPercentOffset = Left / (screen.WorkingArea.Width - Width);
            if (verticalPercentOffset > .4f && verticalPercentOffset < .6f
                && horizontalPercentOffset > .4f && horizontalPercentOffset < .6f)
                return ExtensionMode.None;
            else if (verticalPercentOffset <= .5f && horizontalPercentOffset <= .5f)
                return ExtensionMode.TopLeft;
            else if (verticalPercentOffset > .5f && horizontalPercentOffset <= .5f)
                return ExtensionMode.BottomLeft;
            else if (verticalPercentOffset > .5f && horizontalPercentOffset > .5f)
                return ExtensionMode.BottomRight;
            else
                return ExtensionMode.TopRight;
        }

        private void NewCommandButton_Click(object sender, RoutedEventArgs e)
        {
            var command = new CommandViewModel
            {
                Name = "New command"
            };
            var dialog = new CommandEdition(command)
            {
                Owner = this
            };
            if (dialog.ShowDialog() is true)
            {
                ViewModel.CommandsSource.Add(new CommandWrapperViewModel(command, ViewModel));
                DataStorage.Save(ViewModel.Export());
            }
        }

        private void ParsePosition()
        {
            ViewModel.ExtensionMode = GetExtensionMode();
            if (WindowState == WindowState.Maximized)
            {
                ViewModel.ExtensionMode = ExtensionMode.Maximized;
                return;
            }
        }
    }
}