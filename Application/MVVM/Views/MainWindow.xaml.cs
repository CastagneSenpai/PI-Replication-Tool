using OSIsoft.AF.PI;
using PI_Replication_Tool.MVVM.Models;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();

            // Chargement de la liste des serveurs sources

            // Chargement de la liste des serveurs cibles

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
    
    }
}
