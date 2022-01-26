using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCPExtension
{
    /**
     * Initialization informations given to custom commands
     */

    public readonly struct InitializationInfo
    {
        /**
         * Location of the current plugin on the file system. Use this if you need external resources.
         */
        public readonly DirectoryInfo pluginLocation;
        /**
         * True if the app is currently in portable mode.
         */
        public readonly bool portable;
        /**
         * Dispatcher of the main UI thread
         */
        public readonly IDispatcher uiDispatcher;

        public InitializationInfo(DirectoryInfo pluginLocation, bool portable, IDispatcher uiDispatcher)
        {
            this.pluginLocation = pluginLocation;
            this.portable = portable;
            this.uiDispatcher = uiDispatcher;
        }
    }
}