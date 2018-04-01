using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimeBeautify
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> folders = new List<string>();
        List<string> files = new List<string>();
        int MaximumProgressBar = 100;
        public MainWindow()
        {
            InitializeComponent();
            log.Text += Log.getLog("Welcome to AnimeBeautifier, everything is loaded");
            log.ScrollToHome();
            beautifybtn.IsEnabled = false;
            progressbar.Value = 0;
            progressbar.Maximum = MaximumProgressBar;
        }



        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {
            /// Set maximum progress bar to 0
            MaximumProgressBar = 0;

            /// Log
            log.Text += Log.getLog("Looking in " + foldertxt.Text);

            ///  Get folders in path
            string[] fileArray = Directory.GetDirectories(foldertxt.Text);

            /// Logs
            log.Text += Log.getLog("Found " + fileArray.Count() + " items");
            log.Text += Log.getLog("Listing all items");


            foreach (var item in fileArray)
            {
                /// String for log
                string i = item.Replace(foldertxt.Text, "").Replace("\\", "");
                log.Text += Log.getLog(i);

                /// Add to list
                folders.Add(i);

                /// Set maximun progress bar
                MaximumProgressBar++;
            }

            /// Log
            log.Text += Log.getLog("Press Beautify to start");

            progressbar.Maximum = MaximumProgressBar;
            log.ScrollToEnd();
            beautifybtn.IsEnabled = true;
        }

        private async void beautifybtn_Click(object sender, RoutedEventArgs e)
        {
            log.Text += await Log.getLogAsync("Starting process for folders");
            foreach (var item in folders)
            {
                string resFolder = await Beautifier.beautifyString(item);
                log.Text += await Log.getLogAsync("New name: " + resFolder);
                progressbar.Value++;
            }

            //log.Text += Log.getLog("Starting process for files");
            //foreach (var item in files)
            //{
            //    string resFiles = await Beautifier.beautifyString(item);
            //    log.Text += Log.getLog("New name: " + resFiles);
            //    progressbar.Value++;
            //}

            log.ScrollToEnd();

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
