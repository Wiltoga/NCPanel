using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NCPanel
{
    /// <summary>
    /// Interaction logic for CommandEdition.xaml
    /// </summary>
    public partial class CommandEdition : Window
    {
        public CommandEdition(CommandViewModel command)
        {
            InitializeComponent();
            DataContext = new CommandEditionViewModel(command);
        }

        private CommandEditionViewModel ViewModel => (CommandEditionViewModel)DataContext;

        private void DraggyRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var menuItem = (MenuItemEditionViewModel)((FrameworkElement)sender).DataContext;
            var source = (DependencyObject)sender;

            DragDrop.DoDragDrop(source, menuItem, DragDropEffects.Move);
        }

        private void EditCommandCommandLineButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = false,
                Title = "Open file",
                Filter = "All files|*.*"
            };
            if (dialog.ShowDialog() is true)
            {
                ViewModel.Source.CommandLine = dialog.FileName;
            }
        }

        private async void EditCommandIconButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = false,
                Title = "Change icon",
                Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|All files|*.*"
            };
            if (dialog.ShowDialog() is true)
            {
                ViewModel.Source.Image = await File.ReadAllBytesAsync(dialog.FileName);
            }
        }

        private void EditMenuItemCommandLineButton_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItemEditionViewModel)((FrameworkElement)sender).DataContext;
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = false,
                Title = "Open file",
                Filter = "All files|*.*"
            };
            if (dialog.ShowDialog() is true)
            {
                menuItem.Source.CommandLine = dialog.FileName;
            }
        }

        private async void EditMenuItemIconButton_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItemEditionViewModel)((FrameworkElement)sender).DataContext;
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = false,
                Title = "Change icon",
                Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|All files|*.*"
            };
            if (dialog.ShowDialog() is true)
            {
                menuItem.Source.Image = await File.ReadAllBytesAsync(dialog.FileName);
            }
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            var menuItem = (MenuItemEditionViewModel)((FrameworkElement)sender).DataContext;
            menuItem.InsertTop = menuItem.InsertBot = false;
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            var border = (Grid)sender;
            var menuItem = (MenuItemEditionViewModel)border.DataContext;
            if (e.Data.GetDataPresent(typeof(MenuItemEditionViewModel)))
            {
                var point = e.GetPosition(border);
                if (point.Y < border.ActualHeight / 2)
                {
                    menuItem.InsertTop = true;
                    menuItem.InsertBot = false;
                }
                else
                {
                    menuItem.InsertTop = false;
                    menuItem.InsertBot = true;
                }
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            var border = (Grid)sender;
            var menuItem = (MenuItemEditionViewModel)border.DataContext;
            menuItem.InsertBot = menuItem.InsertTop = false;
            if (e.Data.GetDataPresent(typeof(MenuItemEditionViewModel)))
            {
                var point = e.GetPosition(border);
                var dragged = (MenuItemEditionViewModel)e.Data.GetData(typeof(MenuItemEditionViewModel));
                var offset = point.Y < border.ActualHeight / 2 ? 0 : 1;
                var targetIndex = menuItem.Source.Index + offset;
                if (menuItem.Source.Index + offset == dragged.Source.Index
                    || menuItem.Source.Index + offset == dragged.Source.Index + 1)
                    return;
                if (menuItem.Source.Index + offset < dragged.Source.Index)
                {
                    foreach (var item in ViewModel.ContextMenu.Where(menu => menu.Source.Index >= menuItem.Source.Index + offset && menu.Source.Index < dragged.Source.Index).ToArray())
                    {
                        ++item.Source.Index;
                    }
                }
                else if (menuItem.Source.Index + offset > dragged.Source.Index)
                {
                    foreach (var item in ViewModel.ContextMenu.Where(menu => menu.Source.Index < menuItem.Source.Index + offset && menu.Source.Index > dragged.Source.Index).ToArray())
                    {
                        --item.Source.Index;
                    }
                }
                dragged.Source.Index = targetIndex - 1;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}