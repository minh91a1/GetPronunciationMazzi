using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace GetPronunciationMazzi
{
    public static class Status
    {
        public static string Error = "Error";
        public static string ErrorLoadCSVFirst = "Please Load CSV File First";

        public static string DoingNothing = "Doing Nothig ~";
        public static string Processing = "Processing...";
        public static string LoadCVSDone = "Load CSV Successful";

        public static string CompleteGetPronounciation = "Get Pronuonciation Completed!";
        public static string CompleteExportCSV = "Export CSV Completed!";
    }

    public class Word
    {
        public string Value { get; set; }
        public string Meaning { get; set; }
        public string Pronunciation { get; set; }
        public string Link { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string csv_source_file_name;

        private Queue<string> links;
        private string currentLink;
        private string pronounciationText;

        private IList<Word> dictionary;

        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();

            web.LoadCompleted += _web_Navigated;
            status.Text = Status.DoingNothing;
        }

        private void _web_Navigated(object sender, NavigationEventArgs e)
        {
            pronounciationText = string.Empty;
            var retry = 5;
            do
            {
                dynamic doc = web.Document;
                var html = doc.documentElement.InnerHtml as string;
                var startFindIndex = html.IndexOf("phonetic-word");
                if (startFindIndex != -1)
                {
                    var scanRange = html.Substring(startFindIndex, 200);
                    var closeTagIndex = scanRange.IndexOf(">");
                    var openTagIndex = scanRange.IndexOf("</div>");
                    pronounciationText = scanRange.Substring(closeTagIndex + 1, openTagIndex - closeTagIndex - 1);

                    retry = 0;
                }
                else
                {
                    var frame = new DispatcherFrame();
                    new Thread(() =>
                    {
                        // asynchronously wait for the event/timeout
                        Thread.Sleep(1000);
                        // signal the secondary dispatcher to stop
                        frame.Continue = false;
                    }).Start();
                    Dispatcher.PushFrame(frame);
                }

                retry--;
            }
            while (retry > 0);

            var word = dictionary.FirstOrDefault(x => x.Link == currentLink);
            if (word != null)
            {
                if (string.IsNullOrEmpty(pronounciationText))
                {
                    word.Pronunciation = word.Value;
                }
                else
                {
                    word.Pronunciation = pronounciationText.Trim();
                }
            }

            if (links.Count == 0)
            {
                dynamic activeX = web.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, web, new object[] { });

                activeX.Silent = true;

                status.Text = Status.CompleteGetPronounciation;

                return;
            }

            currentLink = links.Dequeue();
            web.Navigate(currentLink);
        }

        private void Get_Pronounciation_Button_Click(object sender, RoutedEventArgs e)
        {
            links?.Clear();

            links = new Queue<string>
                (
                    dictionary.Select(word => word.Link).ToList()
                );
            currentLink = links.Dequeue();
            web.Navigate(currentLink);

            status.Text = Status.Processing;
        }

        private void Get_FileCSV_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                log.Info("Init Open file dialog");
                // Initialization.
                OpenFileDialog browseDialog = new OpenFileDialog();
                browseDialog.Filter = "CSV file (*.csv)|*.csv";

                log.Info("Show Open file dialog");
                // Verification
                if (browseDialog.ShowDialog() == true)
                {
                    log.Info("Finish Open file dialog");

                    dictionary = File.ReadAllLines(browseDialog.FileName)
                        .Skip(1)
                        .Select(x => new Word
                        {
                            Value = x.Split(',')[1].Replace("\"", ""),
                            Meaning = x.Split(',')[2].Replace("\"", ""),
                            Link = @"https://mazii.net/search?dict=javi&type=w&query=" + x.Split(',')[1].Replace("\"", "") + "&hl=vi-VN",
                        }).ToList();

                    csv_source_file_name = browseDialog.FileName;
                    status.Text = Status.LoadCVSDone;
                }
            }
            catch (Exception ex)
            {
                // Info.  
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);

                status.Text = Status.Error + ":" + ex.Message;
            }
        }

        private void Export_To_CSV_Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(csv_source_file_name))
            {
                status.Text = Status.ErrorLoadCSVFirst;
                return;
            }

            var newFileCSVPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(csv_source_file_name),System.IO.Path.GetFileNameWithoutExtension(csv_source_file_name) + "_pronounciation.csv");

            var csv = new StringBuilder();
            foreach (var item in dictionary)
            {
                var link = $"<a href=\"{item.Link}\">mazzi.net</a>";
                var newLine = $"{item.Value}\t「{item.Pronunciation}」<br>{item.Meaning}<br><br>{link}";
                csv.AppendLine(newLine);
            }

            File.WriteAllText(newFileCSVPath, csv.ToString());

            status.Text = Status.CompleteExportCSV;
        }
    }
}
