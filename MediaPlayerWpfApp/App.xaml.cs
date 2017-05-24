using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MediaPlayerWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled error has just occurred. Contact the application developer or System Administrator.\n"
                + e.Exception.Message, "Multiple Videos Player", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
