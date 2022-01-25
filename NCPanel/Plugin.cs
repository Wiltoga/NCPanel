using DynamicData;
using NCPExtension;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NCPanel
{
    public class Plugin
    {
        public Plugin(string name, IEnumerable<INCPCommand> commands)
        {
            Name = name;
            Commands = commands;
        }

        public IEnumerable<INCPCommand> Commands { get; }
        public string Name { get; }
    }
}