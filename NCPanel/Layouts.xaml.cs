using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NCPanel
{
    public partial class Layouts : ResourceDictionary
    {
        private void DotedCardButton_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element is null)
                return;
            var menu = (element)?.TryFindResource("menu") as ContextMenu;
            if (menu is not null)
            {
                menu.PlacementTarget = element;
                menu.IsOpen = true;
            }
        }
    }
}