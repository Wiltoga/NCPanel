using NCPExtension;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NCPanel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IDispatcher UIDispatcher;

        static App()
        {
            UIDispatcher = new MainDispatcher();
        }

        private class MainDispatcher : IDispatcher
        {
            public void Invoke(Action callback)
            {
                Current.Dispatcher.Invoke(callback);
            }

            public async Task InvokeAsync(Action callback)
            {
                await Current.Dispatcher.InvokeAsync(callback);
            }
        }
    }
}