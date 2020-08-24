using Fizzler.Systems.HtmlAgilityPack;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        public static string ErrorNothingToDo = "There is no new word to get pronounciation ♥";
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
        public string Link { get; set; }

        public string Pronunciation { get; set; }
        public string HanViet { get; set; }
        public string Comment { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private HtmlAgilityPack.HtmlDocument agilityHtmlDoc = new HtmlAgilityPack.HtmlDocument();

        private string csv_source_file_name;

        private Queue<string> links;
        private string currentLink;

        private IList<Word> dictionary;
        private IList<Word> previousPronounciations = new List<Word>();
        private IList<Word> neverGetPronounciations;

        private int _total = 0;

        

        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();

            web.LoadCompleted += _web_Navigated;
            status.Text = Status.DoingNothing;
            LOG("Welcome♪");
        }

        private string getContent(string html, string className)
        {
            var startFindIndex = html.IndexOf(className);
            if (startFindIndex == -1)
            {
                return string.Empty;
            }

            var scanRange = html.Substring(startFindIndex);
            var closeTagIndex = scanRange.IndexOf(">");
            var openTagIndex = scanRange.IndexOf("</div>");

            return scanRange.Substring(closeTagIndex + 1, openTagIndex - closeTagIndex - 1);
        }

        private IList<string> getContents(string html, string tag, string className)
        {
            var pattern = $@"<\s*{tag}[^>]*class=""{className}[^>]*>(.*?)<\s*/\s*{tag}>";
            var matches = Regex.Matches(html, pattern);

            var patternGetContent = @"(?<=>)(.*?)(?=<)";
            var contents = new List<string>();
            foreach (Match match in matches)
            {
                var content = Regex.Match(match.Value, patternGetContent)?.Value;
                if (!string.IsNullOrEmpty(content))
                {
                    contents.Add(content);
                }
            }

            return contents;
        }

        private IList<string> getContents_HtmlAgilityPack(HtmlAgilityPack.HtmlDocument agilityHtmlDoc, string cssSelector, bool isCommentContent = false)
        {
            var results = agilityHtmlDoc.DocumentNode.QuerySelectorAll(cssSelector);

            var contents = new List<string>();
            foreach (var node in results)
            {
                var content = node.InnerText;
                if (!string.IsNullOrEmpty(content))
                {
                    if (isCommentContent)
                    {
                        contents.Add(GetComments(node));
                    }
                    else
                    {
                        contents.Add(content);
                    }
                }
            }

            return contents;
        }

        private string GetComments(HtmlAgilityPack.HtmlNode htmlNode)
        {
            var commentResult = new StringBuilder();

            var commentNode = htmlNode.QuerySelector("div.user-mean");

            // user comment
            var commentValue = commentNode.QuerySelector("div.value-mean-and-delete");
            var commentParagraphs = commentValue.QuerySelectorAll("p.mean.cl-content").ToList();

            if (commentParagraphs.Count == 0)
            {
                return string.Empty;
            }

            var mergeCmtParagraph = new StringBuilder();
            for (var i = 0; i < commentParagraphs.Count; i++)
            {
                var cmtParagraph = commentParagraphs[i];
                mergeCmtParagraph.Append(cmtParagraph.InnerHtml.Replace("\t",string.Empty)
                                                               .Replace("\r", string.Empty)
                                                               .Replace("\n", string.Empty)
                                                               ).Append("<br>");
            }
            commentResult.Append(mergeCmtParagraph.ToString());

            // comment vote
            var commentVote = commentNode.QuerySelector("div.user-infor-comment.cl-content");
            var likeNumber = commentNode.QuerySelector("div.user-like div.inline:last-child")?.InnerHtml;
            var dislikeNumber = commentNode.QuerySelector("div.user-dislike div.inline:last-child")?.InnerHtml;
            commentResult.Insert(0, $"▲{likeNumber} ▼{dislikeNumber}<br>");

            return commentResult.ToString();
        }

        private void _web_Navigated(object sender, NavigationEventArgs e)
        {
            var pronounciationText = string.Empty;
            var hanvietText = string.Empty;
            var textFeedback = string.Empty;

            var retry = 3;
            do
            {
                dynamic doc = web.Document;
                var html = doc.documentElement.InnerHtml as string;

                // 1st way string Index
                //pronounciationText = getContent(html, "phonetic-word");
                //hanvietText = getContent(html, "han-viet-word cl-content");
                //textFeedback = getContent(html, "txt-number-feedback1");

                // 2nd way regex
                //var pronounciationTexts = getContents(html, "div", "phonetic-word");
                //var hanvietTexts = getContents(html, "div", "han-viet-word cl-content");
                //var textFeedbacks = getContents(html, "span", "txt-number-feedback1");
                //var comments = getContents(html, "p", "mean cl-content");

                // 3rd way HtmlAgility
                agilityHtmlDoc.LoadHtml(html);
                var pronounciationTexts = getContents_HtmlAgilityPack(agilityHtmlDoc, "div.phonetic-word");
                var hanvietTexts = getContents_HtmlAgilityPack(agilityHtmlDoc, "div.han-viet-word.cl-content");
                //var textFeedbacks = getContents_HtmlAgilityPack(agilityHtmlDoc, "span.txt-number-feedback1");
                var comments = getContents_HtmlAgilityPack(agilityHtmlDoc, "div.wrapper div div.item-mean", isCommentContent: true);

                pronounciationText = pronounciationTexts.FirstOrDefault();
                hanvietText = hanvietTexts.FirstOrDefault();
                textFeedback = comments.Count > 0 ? comments.Aggregate((running, next) => running + next) : string.Empty;

                if (!string.IsNullOrEmpty(pronounciationText) && !string.IsNullOrEmpty(hanvietText) && !string.IsNullOrEmpty(textFeedback))
                {
                    retry = 0;
                }
                else
                {
                    var frame = new DispatcherFrame();
                    new Thread(() =>
                    {
                        // asynchronously wait for the event/timeout
                        Thread.Sleep(3000);
                        // signal the secondary dispatcher to stop
                        frame.Continue = false;
                    }).Start();
                    Dispatcher.PushFrame(frame);
                }

                retry--;
            }
            while (retry > 0);

            var word = neverGetPronounciations.FirstOrDefault(x => x.Link == currentLink);
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

                word.HanViet = hanvietText;
                word.Comment = textFeedback;
            }

            if (links.Count == 0)
            {
                dynamic activeX = web.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, web, new object[] { });

                activeX.Silent = true;

                status.Text = Status.CompleteGetPronounciation;
                LOG("----- Finish ------");

                return;
            }

            currentLink = links.Dequeue();
            web.Navigate(currentLink);

            status.Text = Status.Processing + " " + Math.Round(100*((double)(_total - links.Count)/_total), 0) + "%";
        }

        private void Get_Pronounciation_Button_Click(object sender, RoutedEventArgs e)
        {
            links?.Clear();

            neverGetPronounciations = (from d in dictionary
                                       join p in previousPronounciations
                                       on d.Value equals p.Value into dp
                                       from j in dp.DefaultIfEmpty()
                                       where j == null
                                       select d).ToList();

            links = new Queue<string>
                (
                    neverGetPronounciations.Select(word => word.Link).ToList()
                );

            _total = links.Count;

            if (links.Count == 0)
            {
                LOG(Status.ErrorNothingToDo);
                return;
            }

            currentLink = links.Dequeue();
            web.Navigate(currentLink);

            status.Text = Status.Processing;
            LOG(status.Text);
        }

        private void Get_FileCSV_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialization.
                OpenFileDialog browseDialog = new OpenFileDialog();
                browseDialog.Filter = "CSV file (*.csv)|*.csv";

                // Verification
                if (browseDialog.ShowDialog() == true)
                {
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
                    LOG("Loaded file: " + System.IO.Path.GetFileName(browseDialog.FileName));
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
                var newLine = $"{item.Value}\t「{item.Pronunciation}」{item.HanViet}<br>{item.Meaning}<br><br>{item.Comment}<br>{link}";
                csv.AppendLine(newLine);
            }

            File.WriteAllText(newFileCSVPath, csv.ToString());

            status.Text = Status.CompleteExportCSV;
            LOG(status.Text);
        }

        private void LOG(string content)
        {
            logBox.Text += (content + "\n");
            log.Info(content);
        }

        private void Input_Previous_Pronounciation_File(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialization.
                OpenFileDialog browseDialog = new OpenFileDialog();
                browseDialog.Filter = "CSV file (*.csv)|*.csv";

                // Verification
                if (browseDialog.ShowDialog() == true)
                {
                    previousPronounciations = File.ReadAllLines(browseDialog.FileName)
                        .Select(x => new Word
                        {
                            Value = x.Split('\t')[0],
                        }).ToList();

                    status.Text = Status.LoadCVSDone;
                    LOG("Loaded Previous Pronounciation file: " + System.IO.Path.GetFileName(browseDialog.FileName));
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
    }
}
