using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Start(object sender, StartupEventArgs e)
        {
            if (!properties.Settings.Default.initialized)
            {
                StartupUri = new Uri(@"/views/SketchWindow.xaml", UriKind.Relative);
                return;
            }
            StartupUri = new Uri(@"/views/initialization/InitializationMainWindow.xaml", UriKind.Relative);
        }
    }
}
