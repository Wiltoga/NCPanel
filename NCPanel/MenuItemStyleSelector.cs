using NCPExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NCPanel
{
    public class MenuItemStyleSelector : StyleSelector
    {
        public override Style? SelectStyle(object item, DependencyObject container)
        {
            if (item is MenuItemWrapperViewModel itemWrapper && container is FrameworkElement fe)
            {
                if (itemWrapper.Source.GetType().CustomAttributes.Any(ca => ca.AttributeType == typeof(SeparatorAttribute)))
                    return fe.FindResource("separatorMenuItem") as Style;
                else
                    return fe.FindResource("basicMenuItem") as Style;
            }
            else
                return null;
        }
    }
}