using System.Windows;
using ViewModels;
using Views;

namespace PI_Replication_Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel()
            };
            //MainWindowViewModel context = new MainWindowViewModel();
            //app.DataContext = context;
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
