using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NCPanel
{
    /// <summary>
    /// Logique d'interaction pour WindowStyle.xaml
    /// </summary>
    public partial class CustomWindowStyle : ResourceDictionary
    {
        public static readonly DependencyProperty ButtonCornerRadiusProperty = DependencyProperty.RegisterAttached("ButtonCornerRadius", typeof(CornerRadius), typeof(CustomWindowStyle), new FrameworkPropertyMetadata(new CornerRadius(5), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty HideTitleBarProperty = DependencyProperty.RegisterAttached("HideTitleBar", typeof(bool), typeof(CustomWindowStyle), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static CornerRadius GetButtonCornerRadius(DependencyObject element)
        {
            return (CornerRadius)element.GetValue(ButtonCornerRadiusProperty);
        }

        public static bool GetHideTitleBar(DependencyObject element)
        {
            return (bool)element.GetValue(HideTitleBarProperty);
        }

        public static void SetButtonCornerRadius(DependencyObject element, CornerRadius value)
        {
            element.SetValue(ButtonCornerRadiusProperty, value);
        }

        public static void SetHideTitleBar(DependencyObject element, bool value)
        {
            element.SetValue(HideTitleBarProperty, value);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject obj)
                Window.GetWindow(obj).Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject obj)
            {
                var window = Window.GetWindow(obj);
                window.WindowState = window.WindowState switch
                {
                    WindowState.Normal => WindowState.Maximized,
                    _ => WindowState.Normal,
                };
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject obj)
                Window.GetWindow(obj).WindowState = WindowState.Minimized;
        }
    }
}