using NCPExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCPanel
{
    public class GeneratedMenuItemViewModel : MenuItemWrapperViewModel
    {
        public GeneratedMenuItemViewModel(INCPMenuItem menuitem, CommandWrapperViewModel parent, int index) : base(menuitem, parent)
        {
            Index = index;
        }

        public int Index { get; }
    }
}