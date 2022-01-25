using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCPExtension
{
    public readonly struct InitializationInfo
    {
        public readonly DirectoryInfo pluginLocation;
        public readonly bool portable;

        public InitializationInfo(DirectoryInfo pluginLocation, bool portable)
        {
            this.pluginLocation = pluginLocation;
            this.portable = portable;
        }
    }
}