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

        public InitializationInfo(DirectoryInfo pluginLocation)
        {
            this.pluginLocation = pluginLocation;
        }
    }
}