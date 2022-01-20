using NCPExtension;
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

        public MainWindow()
        {
            InitializeComponent();
            var timer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher)
            { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += (sender, e) =>
            {
                if (!ViewModel.Open)
                {
                    if (IsMouseOver)
                        ViewModel.Open = true;
                }
                else if (WindowState != WindowState.Maximized)
                {
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
        }

        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;
    }
}