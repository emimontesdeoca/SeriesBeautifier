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

            /// HELLO
            log.Text += Log.getLog("Welcome to SeriesBeautifier (づ￣ ³￣)づ");
            log.Text += Log.getLog("Everything is loaded");
            log.ScrollToHome();

            /// Style
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

                    /// Get file count for progress bar
                    int filecounter = Directory.GetFiles(item).Count();

                    /// L O G S
                    log.Text += Log.getLog("Found " + filecounter + " files inside");

                    /// Progressbar
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

                /// Style stuff
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
            ///Log 
            log.Text += await Log.getLogAsync("Starting process for folders");

            /// Dictionary for before and after
            Dictionary<string, string> changes = new Dictionary<string, string>();

            foreach (var item in folders)
            {
                /// Get new folder name
                string resFolder = await Beautifier.beautifyString(item);

                /// Log
                log.Text += await Log.getLogAsync("New folder name: " + resFolder);

                /// Progress bar to make it async
                progressbar.Value++;

                /// Add to dictionary
                changes.Add(item, resFolder);
            }

            /// Some ints for S T A T I S T I C S
            int totalitemschanged = 0;
            int totalexceptions = 0;

            /// Foreach in dictionary
            foreach (var item in changes)
            {
                /// Get before and after, as they are necessary parameters for directory move method
                string before = folderpath + "\\" + item.Key;
                string name = item.Value;

                /// If capitalize
                if (capitalize.IsChecked == true)
                {
                    /// L O G
                    log.Text += await Log.getLogAsync("CapItAliZinG ¯\\(°_o)/¯");

                    /// Capitalize
                    name = name.First().ToString().ToUpper() + name.Substring(1);
                }

                /// Get whatever style 
                string after = folderpath + "\\" + name;

                /// If it does exist
                if (!Directory.Exists(after))
                {
                    try
                    {
                        /// Rename
                        Directory.Move(before, after);

                        /// Log
                        log.Text += await Log.getLogAsync("Starting process for files inside this folder");

                        /// STATISTICS
                        totalitemschanged++;

                        /// Beautify inside elements
                        foreach (var file in Directory.GetFiles(folderpath + "\\" + item.Value).ToList())
                        {
                            try
                            {
                                /// Before and after, in order to use move method
                                string fileold = file.Split('\\').Last();
                                string filenew = await Beautifier.beautifyString(fileold);

                                /// Get parent folder
                                string parentfolder = Directory.GetParent(file).FullName;

                                /// L O G S
                                log.Text += await Log.getLogAsync("New file name: " + filenew);

                                /// Rename
                                File.Move(parentfolder + "\\" + fileold, parentfolder + "\\" + filenew);

                                /// STATISTICS
                                totalitemschanged++;
                            }
                            catch (Exception)
                            {
                                /// Log
                                log.Text += await Log.getLogAsync("There was an exception 乁( ◔ ౪◔)「      ┑(￣Д ￣)┍");

                                /// STATISTICS
                                totalexceptions++;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        /// Log
                        log.Text += await Log.getLogAsync("There was an exception 乁( ◔ ౪◔)「      ┑(￣Д ￣)┍");

                        /// STATISTICS
                        totalexceptions++;
                    }
                }
                else
                {
                    /// Log
                    log.Text += await Log.getLogAsync("The folder already exist!");

                    /// STATISTICS
                    totalexceptions++;
                }
            }

            /// Logs
            log.Text += await Log.getLogAsync("Process finished");
            log.Text += await Log.getLogAsync("Renamed " + totalitemschanged + " items of " + totalfound + " total with " + totalexceptions + " exceptions");

            /// Sorting
            if (sort.IsChecked == true)
            {
                ///  L O G
                log.Text += await Log.getLogAsync("Starting folder sort");

                /// Get folders again
                string[] folders = Directory.GetDirectories(foldertxt.Text);

                /// root path of everything
                string defaultpath = foldertxt.Text + "\\";

                foreach (var item in folders)
                {
                    /// Get folder name
                    string folder = item.Replace(foldertxt.Text, "").Replace("\\", "");

                    /// Get folder uppercase letter
                    string letter = folder[0].ToString().ToUpper();

                    /// Fixes for folders
                    if (letter == "." || letter == "?" || letter == "." || letter == ">" || letter == "<" || letter == "*" || letter == "\"" || letter == "/" || letter == ":" || letter == "|")
                    {
                        letter = "$%@";
                    }

                    /// Name
                    string directoryname = folder.Split('\\').Last();

                    /// Final location
                    string finalplace = defaultpath + letter + '\\' + directoryname;

                    /// Directory does exist
                    if (Directory.Exists(defaultpath + letter))
                    {
                        /// M O V E
                        Directory.Move(defaultpath + folder, finalplace);
                    }
                    else
                    {
                        /// Create directory first
                        Directory.CreateDirectory(defaultpath + letter);

                        // M O V E
                        Directory.Move(defaultpath + folder, finalplace);
                    }
                }
                /// L O G S B A B Y
                log.Text += await Log.getLogAsync("Finished sort");
            }

            /// B Y E
            log.Text += await Log.getLogAsync("Thanks for using me, have a good day ༼ つ ◕_◕ ༽つ");

            /// Styling
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
