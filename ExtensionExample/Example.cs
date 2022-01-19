using NCPExtension;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;

namespace ExtensionExample
{
    public class Example : INCPExtension
    {
        public static void Main()
        {
        }

        public void Init()
        {
            MessageBox.Show("init");
        }

        public void Run()
        {
            var dialog = new Window();
            dialog.Content = new Label
            {
                Content = JsonConvert.SerializeObject(new
                {
                    value1 = "test1",
                    value2 = "test2",
                }, Formatting.Indented)
            };
            dialog.ShowDialog();
        }
    }
}