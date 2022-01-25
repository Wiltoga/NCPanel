using NCPExtension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NCPanel
{
    public static class Utils
    {
        static Utils()
        {
            MenuItemComparer = Comparer<INCPMenuItem>.Create((left, right) => left.Index.CompareTo(right.Index));
            MenuItemWrapperComparer = Comparer<MenuItemWrapperViewModel>.Create((left, right) =>
            {
                if (left is GeneratedMenuItemViewModel)
                {
                    if (right is GeneratedMenuItemViewModel)
                        return MenuItemComparer.Compare(left.Source, right.Source);
                    else
                        return 1;
                }
                else if (right is GeneratedMenuItemViewModel)
                    return -1;
                else
                    return MenuItemComparer.Compare(left.Source, right.Source);
            });
        }

        public static IComparer<INCPMenuItem> MenuItemComparer { get; }
        public static IComparer<MenuItemWrapperViewModel> MenuItemWrapperComparer { get; }

        public static DirectoryInfo Combine(this DirectoryInfo source, params string[] paths)
        {
            return new DirectoryInfo(Path.Combine(new[] { source.FullName }.Concat(paths).ToArray()));
        }

        public static FileInfo CombineFile(this DirectoryInfo source, params string[] paths)
        {
            return new FileInfo(Path.Combine(new[] { source.FullName }.Concat(paths).ToArray()));
        }

        public static DirectoryInfo Create(this DirectoryInfo source, params string[] paths)
        {
            return Directory.CreateDirectory(Path.Combine(new[] { source.FullName }.Concat(paths).ToArray()));
        }

        public static ImageSource ImageFromBytes(byte[] imgSource)
        {
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imgSource))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}