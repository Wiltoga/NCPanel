using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NCPanel
{
    public class DataStorage
    {
        private const string ImagesFolderName = "icons";
        private const string PortableFolderName = "Data";
        private const string SettingsFileName = "settings.json";
        private static readonly string DataFolder;

        static DataStorage()
        {
#if DEBUG
            DataFolder = PortableFolderName;
#else
            if (Directory.Exists(PortableFolderName))
                DataFolder = PortableFolderName;
            else
                DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCPanel");
#endif
        }

        public static bool IsPortable { get; }

        public static Data Load()
        {
            var dataDir = Directory.CreateDirectory(DataFolder);
            var settingsFile = dataDir.CombineFile(SettingsFileName);
            var defaultData = new Data(Layout.Grid, Array.Empty<CommandData>());
            if (!settingsFile.Exists)
            {
                var json = JsonSerializer.Serialize(defaultData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                using (var writer = new StreamWriter(new FileStream(settingsFile.FullName, FileMode.Create, FileAccess.Write)))
                {
                    writer.Write(json);
                    writer.Flush();
                }
            }
            {
                using (var reader = new StreamReader(settingsFile.OpenRead()))
                    return JsonSerializer.Deserialize<Data>(reader.ReadToEnd()) ?? defaultData;
            }
        }

        public static byte[]? LoadImage(string name)
        {
            var dataDir = Directory.CreateDirectory(DataFolder);
            var imagesDir = dataDir.Create(ImagesFolderName);
            var imageFile = imagesDir.CombineFile(name);
            if (imageFile.Exists)
                return File.ReadAllBytes(imageFile.FullName);
            else
                return null;
        }

        public static void Save(Data data)
        {
            var dataDir = Directory.CreateDirectory(DataFolder);
            var settingsFile = dataDir.CombineFile(SettingsFileName);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            using (var writer = new StreamWriter(new FileStream(settingsFile.FullName, FileMode.Create, FileAccess.Write)))
            {
                writer.Write(json);
                writer.Flush();
            }
        }

        public static void SaveImage(byte[] image, string name)
        {
            var dataDir = Directory.CreateDirectory(DataFolder);
            var imagesDir = dataDir.Create(ImagesFolderName);
            var imageFile = imagesDir.CombineFile(name);
            using (var memory = new MemoryStream(image))
            {
                using (var output = new FileStream(imageFile.FullName, FileMode.Create, FileAccess.Write))
                {
                    memory.CopyTo(output);
                    output.Flush();
                }
            }
        }
    }

    public record MenuItemData(string? Title, string? CommandLine, string? IconName, int Index);
    public record CommandData(string? Name, string? Description, string? CommandLine, string? IconName, MenuItemData[] ContextMenu);
    public record Data(Layout Layout, CommandData[] Commands);
}