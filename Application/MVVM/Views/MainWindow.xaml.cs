using OSIsoft.AF.PI;
using PI_Replication_Tool.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PI_Replication_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public ObservableCollection 

        public MainWindow()
        {
            InitializeComponent();

            //List<string> list = new List<string>();
            //list.Add("ezkdos"); list.Add("561616"); list.Add("cksdc64");

            //list<string> piservernames = new list<string>();
            //foreach (var server in piservers.getpiservers())
            //{
            //    piservernames.add(server.name);
            //}

            var PILocalServers = PIServers.GetPIServers();

            // Chargement de la liste des serveurs sources
            listSourceServer.ItemsSource = PILocalServers;

            // Chargement de la liste des serveurs cibles
            listTargetServer.ItemsSource = PILocalServers;
        }

        private void Button_Continue_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Hello World");
            PIConnectionManager pIConnection = new PIConnectionManager();
            //var s = "";
            //foreach (var server in PIServers.GetPIServers())
            //{
            //    s += " " + server; 
            //}
            //MessageBox.Show($"un test: {s}");
            var c = pIConnection.ConnectToServer("PI-CENTER-HQ");
            var v = pIConnection.GetPIPointValue(c, "sinusoid");
            MessageBox.Show(v.ToString());

            //if (c)
            //{
            //    MessageBox.Show("C'est good");
            //}
            //else {
            //    MessageBox.Show("Ca pues");
            //}
            
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed) 
            {
                DragMove();
            }
        }

        private void Button_MinimizeClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WindowsStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else 
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
    }
}
