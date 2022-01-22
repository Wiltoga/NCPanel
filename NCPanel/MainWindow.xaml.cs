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
            lockOpenedSizes = false;
            OpenedWith = Width;
            OpenedHeight = Height;
            MinHeight = 250;
            MinWidth = 250;
            Handle = new WindowInteropHelper(this).Handle;

            ViewModel.WhenAnyValue(vm => vm.Open).Subscribe(opened =>
            {
                lockOpenedSizes = true;
                var middleOfWindow = new Point(Left + Width / 2, Top + Height / 2);
                if (opened)
                {
                    MinHeight = 250;
                    MinWidth = 250;
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
                if (
                    middleOfWindow.X <= screen.WorkingArea.Left + screen.WorkingArea.Width / 2
                    && middleOfWindow.Y <= screen.WorkingArea.Top + screen.WorkingArea.Height / 2)
                {
                    Left = screen.WorkingArea.Left;
                    Top = screen.WorkingArea.Top;
                }
                else if (
                    middleOfWindow.X > screen.WorkingArea.Left + screen.WorkingArea.Width / 2
                    && middleOfWindow.Y <= screen.WorkingArea.Top + screen.WorkingArea.Height / 2)
                {
                    Left = screen.WorkingArea.Left + screen.WorkingArea.Width - Width;
                    Top = screen.WorkingArea.Top;
                }
                else if (
                    middleOfWindow.X <= screen.WorkingArea.Left + screen.WorkingArea.Width / 2
                    && middleOfWindow.Y > screen.WorkingArea.Top + screen.WorkingArea.Height / 2)
                {
                    Left = screen.WorkingArea.Left;
                    Top = screen.WorkingArea.Top + screen.WorkingArea.Height - Height;
                }
                else if (
                    middleOfWindow.X > screen.WorkingArea.Left + screen.WorkingArea.Width / 2
                    && middleOfWindow.Y > screen.WorkingArea.Top + screen.WorkingArea.Height / 2)
                {
                    Left = screen.WorkingArea.Left + screen.WorkingArea.Width - Width;
                    Top = screen.WorkingArea.Top + screen.WorkingArea.Height - Height;
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
                    var middleOfWindow = new Point(Left + Width / 2, Top + Height / 2);
                    var screen = Screen.FromPoint(new System.Drawing.Point((int)middleOfWindow.X, (int)middleOfWindow.Y));
                    var safeArea = new Rect(
                        screen.WorkingArea.Left + screen.WorkingArea.Width / 3,
                        screen.WorkingArea.Top + screen.WorkingArea.Height / 3,
                        screen.WorkingArea.Width / 3,
                        screen.WorkingArea.Height / 3);
                    if (safeArea.Contains(middleOfWindow))
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
                    if (mousePos.X < Left && mousePos.Y < Top)
                    {
                        var distX = mousePos.X - Left;
                        var distY = mousePos.Y - Top;
                        if (MaxCursorDistanceToLeave * MaxCursorDistanceToLeave < distX * distX + distY * distY)
                            ViewModel.Open = false;
                    }
                    else if (mousePos.X > Left + Width && mousePos.Y < Top)
                    {
                        var distX = Left + Width - mousePos.X;
                        var distY = mousePos.Y - Top;
                        if (MaxCursorDistanceToLeave * MaxCursorDistanceToLeave < distX * distX + distY * distY)
                            ViewModel.Open = false;
                    }
                    else if (mousePos.X > Left + Width && mousePos.Y > Top + Height)
                    {
                        var distX = Left + Width - mousePos.X;
                        var distY = Top + Height - mousePos.Y;
                        if (MaxCursorDistanceToLeave * MaxCursorDistanceToLeave < distX * distX + distY * distY)
                            ViewModel.Open = false;
                    }
                    else if (mousePos.X < Left && mousePos.Y > Top + Height)
                    {
                        var distX = mousePos.X - Left;
                        var distY = Top + Height - mousePos.Y;
                        if (MaxCursorDistanceToLeave * MaxCursorDistanceToLeave < distX * distX + distY * distY)
                            ViewModel.Open = false;
                    }
                    else if (mousePos.X < Left)
                    {
                        var distX = mousePos.X - Left;
                        if (MaxCursorDistanceToLeave * MaxCursorDistanceToLeave < distX * distX)
                            ViewModel.Open = false;
                    }
                    else if (mousePos.X > Left + Width)
                    {
                        var distX = Left + Width - mousePos.X;
                        if (MaxCursorDistanceToLeave * MaxCursorDistanceToLeave < distX * distX)
                            ViewModel.Open = false;
                    }
                    else if (mousePos.Y < Top)
                    {
                        var distY = mousePos.Y - Top;
                        if (MaxCursorDistanceToLeave * MaxCursorDistanceToLeave < distY * distY)
                            ViewModel.Open = false;
                    }
                    else if (mousePos.Y > Top + Height)
                    {
                        var distY = Top + Height - mousePos.Y;
                        if (MaxCursorDistanceToLeave * MaxCursorDistanceToLeave < distY * distY)
                            ViewModel.Open = false;
                    }
                }
            };
            timer.Start();
            Closed += (sender, e) => timer.Stop();
        }

        private IntPtr Handle { get; }

        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

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

        private void ParsePosition()
        {
            if (WindowState == WindowState.Maximized)
            {
                ViewModel.ExtensionMode = ExtensionMode.Maximized;
                return;
            }
            var middleOfWindow = new Point(Left + Width / 2, Top + Height / 2);
            var screen = Screen.FromPoint(new System.Drawing.Point((int)middleOfWindow.X, (int)middleOfWindow.Y));
            var safeArea = new Rect(
                screen.WorkingArea.Left + screen.WorkingArea.Width / 3,
                screen.WorkingArea.Top + screen.WorkingArea.Height / 3,
                screen.WorkingArea.Width / 3,
                screen.WorkingArea.Height / 3);
            if (safeArea.Contains(middleOfWindow))
                ViewModel.ExtensionMode = ExtensionMode.None;
            else if (
                middleOfWindow.X < screen.WorkingArea.Left + screen.WorkingArea.Width / 2
                && middleOfWindow.Y < screen.WorkingArea.Top + screen.WorkingArea.Height / 2)
                ViewModel.ExtensionMode = ExtensionMode.TopLeft;
            else if (
                middleOfWindow.X > screen.WorkingArea.Left + screen.WorkingArea.Width / 2
                && middleOfWindow.Y < screen.WorkingArea.Top + screen.WorkingArea.Height / 2)
                ViewModel.ExtensionMode = ExtensionMode.TopRight;
            else if (
                middleOfWindow.X < screen.WorkingArea.Left + screen.WorkingArea.Width / 2
                && middleOfWindow.Y > screen.WorkingArea.Top + screen.WorkingArea.Height / 2)
                ViewModel.ExtensionMode = ExtensionMode.BottomLeft;
            else if (
                middleOfWindow.X > screen.WorkingArea.Left + screen.WorkingArea.Width / 2
                && middleOfWindow.Y > screen.WorkingArea.Top + screen.WorkingArea.Height / 2)
                ViewModel.ExtensionMode = ExtensionMode.BottomRight;
        }
    }
}