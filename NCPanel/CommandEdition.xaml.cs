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

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}