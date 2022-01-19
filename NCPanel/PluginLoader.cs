using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NCPanel
{
    public class PluginLoader
    {
        private const string PluginFolderName = "plugins";
        private ReadOnlyObservableCollection<AssemblyName> availablePlugins;
        private SourceCache<AssemblyName, string> availablePluginsSource;
        private FileSystemWatcher watcher;

        static PluginLoader()
        {
            PluginsPath = Path.Combine(Environment.CurrentDirectory, PluginFolderName);
        }

        public PluginLoader()
        {
            availablePluginsSource = new SourceCache<AssemblyName, string>(assembly => assembly.FullName);
            availablePluginsSource.Connect()
                .Bind(out availablePlugins)
                .Subscribe();
            Directory.CreateDirectory(PluginsPath);
            watcher = new FileSystemWatcher(PluginsPath)
            {
                IncludeSubdirectories = true
            };
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += Watcher_Renamed;
            watcher.EnableRaisingEvents = true;
            ForceRefresh();
        }

        public ReadOnlyObservableCollection<AssemblyName> AvailablePlugins => availablePlugins;

        private static string PluginsPath { get; }

        public void ForceRefresh()
        {
            foreach (var pluginDirInfo in new DirectoryInfo(PluginsPath).GetDirectories())
            {
                if (ParseFolder(pluginDirInfo) is AssemblyName name)
                    availablePluginsSource.Edit(updater => updater.AddOrUpdate(name));
            }
        }

        private static DirectoryInfo GetPluginFolder(string path)
        {
            var relativePath = Path.GetRelativePath(PluginsPath, path);
            return new DirectoryInfo(Path.Combine(PluginsPath, relativePath.Split(Path.DirectorySeparatorChar)[0]));
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

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (ParseFolder(GetPluginFolder(e.FullPath)) is AssemblyName name)
                availablePluginsSource.Edit(updater => updater.AddOrUpdate(name));
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (AvailablePlugins.FirstOrDefault(name => name.FullName == GetPluginFolder(e.FullPath).Name) is AssemblyName name)
                availablePluginsSource.Edit(updater => updater.Remove(name));
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (AvailablePlugins.FirstOrDefault(name => name.FullName == GetPluginFolder(e.OldFullPath).Name) is AssemblyName name)
                availablePluginsSource.Remove(name);
            else if (ParseFolder(GetPluginFolder(e.FullPath)) is AssemblyName addedName)
                availablePluginsSource.Edit(updater => updater.AddOrUpdate(addedName));
        }
    }
}