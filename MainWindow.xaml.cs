using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Queue<string> links;
        private string currentWord;
        private string pronounciationText;

        public MainWindow()
        {
            InitializeComponent();
            web.LoadCompleted += _web_Navigated;
        }

        private void _web_Navigated(object sender, NavigationEventArgs e)
        {
            var retry = 10;
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

            pronounciation.Text += (pronounciationText + "\r\n");

            if (links.Count == 0)
            {
                dynamic activeX = web.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, web, new object[] { });

                activeX.Silent = true;

                return;
            }

            currentWord = links.Dequeue();
            web.Navigate(currentWord);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            links?.Clear();
            pronounciation.Text = string.Empty;

            links = new Queue<string>
                (
                    words.Text.Replace("\r\n", "\n")
                        .Split(new char[] { '\n' })
                        .Select(x => @"https://mazii.net/search?dict=javi&type=w&query=" + x + "&hl=vi-VN")
                        .ToList()
                );
            currentWord = links.Dequeue();
            web.Navigate(currentWord);
        }
    }
}
