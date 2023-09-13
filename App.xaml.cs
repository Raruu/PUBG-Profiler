using System.Diagnostics;
using System.Windows;
using System.Security.Principal;

namespace PUBG_Profiler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)); 
        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 1) Process.Start("explorer.exe", "steam://rungameid/578080");
            else
            {
                MainWindow mainWindow = new MainWindow();
                if (IsAdministrator())
                {
                    mainWindow.ShowDialog();
                }
                else
                {
                    MessageBoxResult MsBox = MessageBox.Show("This Application needs run as Administrator mode!\nDo you want still to open this app!?", "Warning. . ."
                        , MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);
                    if (MsBox == MessageBoxResult.Yes)
                        mainWindow.ShowDialog();
                }
            }
            App.Current.Shutdown();
        }
    }
}
