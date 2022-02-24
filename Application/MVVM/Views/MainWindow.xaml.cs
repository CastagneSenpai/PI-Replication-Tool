using System;
using System.Windows;
using System.Windows.Input;
using Core;
using NLog;

namespace Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private NlogMemoryTarget _Target;
        public MainWindow()
        {
            InitializeComponent();

            //this.Loaded += (s, e) => {
            //    _Target = new NlogMemoryTarget("text box output", LogLevel.Trace);
            //    _Target.Log += log => LogText(log);
            //};
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
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

        //private void LogText(string message)
        //{
        //    this.Dispatcher.Invoke((Action)delegate () {
        //        this.MessageView.AppendText(message + "\n");
        //        this.MessageView.ScrollToEnd();
        //    });
        //}
    }
}
