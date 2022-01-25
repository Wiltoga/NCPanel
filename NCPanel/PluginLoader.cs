using DynamicData;
using NCPExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NCPanel
{
    public class PluginLoader
    {
        public IObservable<IChangeSet<Plugin>> AvailablePluginsConnect;
        private const string PluginFolderName = "plugins";
        private SourceList<Plugin> availablePluginsSource;

        static PluginLoader()
        {
            PluginsPath = Path.Combine(Environment.CurrentDirectory, PluginFolderName);
        }

        public PluginLoader()
        {
            availablePluginsSource = new SourceList<Plugin>();
            AvailablePluginsConnect = availablePluginsSource.Connect();
            Directory.CreateDirectory(PluginsPath);
            Refresh();
        }

        private static string PluginsPath { get; }

        public void Refresh()
        {
            foreach (var pluginDirInfo in new DirectoryInfo(PluginsPath).GetDirectories())
            {
                if (ParseFolder(pluginDirInfo) is AssemblyName name)
                {
                    var path = Path.Combine(pluginDirInfo.FullName, $"{name.FullName}.dll");
                    var loader = new PluginLoadContext(path);
                    var assembly = loader.LoadFromAssemblyName(name);
                    var commands = new List<INCPCommand>();
                    foreach (var exportedType in assembly.ExportedTypes)
                    {
                        if (exportedType.GetInterfaces().Contains(typeof(INCPCommand)))
                        {
                            var constructor = exportedType.GetConstructor(Type.EmptyTypes);
                            if (constructor is not null)
                            {
                                var command = (INCPCommand)constructor.Invoke(null);
                                commands.Add(command);
                            }
                        }
                    }
                    if (!availablePluginsSource.Items.Any(plugin => plugin.Name == name.FullName))
                        availablePluginsSource.Add(new Plugin(name.FullName, commands));
                }
            }
        }

        private AssemblyName? ParseFolder(DirectoryInfo folder)
        {
            var dllInfo = new FileInfo(Path.Combine(folder.FullName, $"{folder.Name}.dll"));
            if (dllInfo.Exists)
            {
                return new AssemblyName(Path.GetFileNameWithoutExtension(dllInfo.Name));
            }
            else
                return null;
        }

        private class PluginLoadContext : AssemblyLoadContext
        {
            private AssemblyDependencyResolver _resolver;

            public PluginLoadContext(string pluginPath)
            {
                _resolver = new AssemblyDependencyResolver(pluginPath);
            }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath is not null)
                {
                    return LoadFromAssemblyPath(assemblyPath);
                }

                return null;
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                if (libraryPath is not null)
                {
                    return LoadUnmanagedDllFromPath(libraryPath);
                }

                return IntPtr.Zero;
            }
        }
    }
}