using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public static string folderpath = "";
        public static int totalfound = 0;

        int MaximumProgressBar = 100;
        public MainWindow()
        {
            InitializeComponent();
            log.Text += Log.getLog("Welcome to SeriesBeautifier (づ￣ ³￣)づ");
            log.Text += Log.getLog("Everything is loaded");
            log.ScrollToHome();
            beautifybtn.IsEnabled = false;
            progressbar.Value = 0;
            progressbar.Maximum = MaximumProgressBar;


        }



        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {

            log.Text = "";

            /// Set maximum progress bar to 0
            MaximumProgressBar = 0;

            /// Log
            log.Text += Log.getLog("Looking in " + foldertxt.Text);
            folderpath = foldertxt.Text;



            try
            {
                ///  Get folders in path
                string[] fileArray = Directory.GetDirectories(foldertxt.Text);

                /// Logs
                log.Text += Log.getLog("Found " + fileArray.Count() + " items");
                log.Text += Log.getLog("Listing all folders");

                // Reset folder
                folders.Clear();
                totalfound = 0;
                foreach (var item in fileArray)
                {
                    /// String for log
                    string i = item.Replace(foldertxt.Text, "").Replace("\\", "");
                    log.Text += Log.getLog(i);
                    totalfound++;

                    int filecounter = Directory.GetFiles(item).Count();
                    log.Text += Log.getLog("Found " + filecounter + " files inside");
                    MaximumProgressBar += filecounter;
                    totalfound += filecounter;

                    /// Add to list
                    folders.Add(i);

                    /// Set maximun progress bar
                    MaximumProgressBar++;
                }

                /// Log
                log.Text += Log.getLog("Total found " + totalfound + " items");
                log.Text += Log.getLog("Press Beautify to start");

                progressbar.Maximum = MaximumProgressBar;
                log.ScrollToEnd();
                beautifybtn.IsEnabled = true;
            }
            catch (Exception)
            {
                /// Log
                log.Text += Log.getLog("There is problem with the folder path, check it is correct, then search again");
            }
        }

        private async void beautifybtn_Click(object sender, RoutedEventArgs e)
        {
            log.Text += await Log.getLogAsync("Starting process for folders");

            Dictionary<string, string> changes = new Dictionary<string, string>();

            foreach (var item in folders)
            {
                string resFolder = await Beautifier.beautifyString(item);
                log.Text += await Log.getLogAsync("New folder name: " + resFolder);
                progressbar.Value++;

                changes.Add(item, resFolder);
            }

            int totalitemschanged = 0;
            int totalexceptions = 0;

            foreach (var item in changes)
            {
                string before = folderpath + "\\" + item.Key;
                string after = folderpath + "\\" + item.Value;

                if (!Directory.Exists(after))
                {
                    try
                    {
                        Directory.Move(before, after);

                        log.Text += await Log.getLogAsync("Starting process for files inside this folder");
                        totalitemschanged++;
                        /// beautfy inside elements
                        foreach (var file in Directory.GetFiles(folderpath + "\\" + item.Value).ToList())
                        {

                            try
                            {
                                string fileold = file.Split('\\').Last();
                                string filenew = await Beautifier.beautifyString(fileold);

                                string parentfolder = Directory.GetParent(file).FullName;
                                log.Text += await Log.getLogAsync("New file name: " + filenew);

                                File.Move(parentfolder + "\\" + fileold, parentfolder + "\\" + filenew);
                                totalitemschanged++;
                            }
                            catch (Exception)
                            {
                                log.Text += await Log.getLogAsync("There was an exception 乁( ◔ ౪◔)「      ┑(￣Д ￣)┍");
                                totalexceptions++;

                            }
                        }
                    }
                    catch (Exception)
                    {
                        log.Text += await Log.getLogAsync("There was an exception 乁( ◔ ౪◔)「      ┑(￣Д ￣)┍");
                        totalexceptions++;

                    }
                }
            }

            log.Text += await Log.getLogAsync("Process finished");
            log.Text += await Log.getLogAsync("Renamed " + totalitemschanged + " items of " + totalfound + " total with " + totalexceptions + " exceptions");
            log.Text += await Log.getLogAsync("Thanks for using me, have a good day ༼ つ ◕_◕ ༽つ");

            progressbar.Value = MaximumProgressBar;
            log.ScrollToEnd();
            beautifybtn.IsEnabled = false;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
