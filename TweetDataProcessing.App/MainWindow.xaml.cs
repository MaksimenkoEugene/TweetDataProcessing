using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Windows;

using TweetDataProcessing.Common.Autofac;
using TweetDataProcessing.Common.Common;
using TweetDataProcessing.Contracts.Repositories;
using TweetDataProcessing.Repositories.Contracts;

namespace TweetDataProcessing.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// TODO: need to implement pattern MVVM for best practice
        /// </summary>
        public MainWindow()
        {
            ContainerConfig.Configure();
            InitializeComponent();
        }

        private const string emptyDateFrom = "Please, choose date from";
        private const string emptyDateTo = "Please, choose date to";

        public delegate void UpdateUIResult(string text);
        ThreadStart threadStart;
        Thread updateThread;

        protected IDependencyResolver DependencyResolver => AutofacService.Instance.Resolve<IDependencyResolver>();

        private ILogger Logger => this.DependencyResolver.Resolve<ILogger>();

        private IGetTweetData GetTweetData => this.DependencyResolver.Resolve<IGetTweetData>();

        private IGetTimePerformance GetTimePerformance => this.DependencyResolver.Resolve<IGetTimePerformance>();

        private void btnGetTweets_Click(object sender, RoutedEventArgs e)
        {
            var dateFrom = this.dpDateFrom.SelectedDate;
            var dateTo = this.dpDateTo.SelectedDate;

            threadStart = new ThreadStart(() => GetThreadStarted(dateFrom, dateTo));
            updateThread = new Thread(threadStart);
            updateThread.Start();
        }

        private void GetThreadStarted(DateTime? dateFrom, DateTime? dateTo)
        {
            UpdateUIResult updateUIResult = new UpdateUIResult(UpdateUITextOutputInfo);

            using (var logger = this.Logger)
            {
                if (!dateFrom.HasValue)
                {
                    MessageBox.Show(emptyDateFrom, "Notification");
                    logger.WriteLog(emptyDateFrom);
                    return;
                }
                else if (!dateTo.HasValue)
                {
                    MessageBox.Show(emptyDateTo, "Notification");
                    logger.WriteLog(emptyDateTo);
                    return;
                }

                using (var client = new HttpClient())
                using (var tweetData = this.GetTweetData)
                using (var timePerformance = this.GetTimePerformance)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(updateUIResult, "Please waiting...");
                        Dispatcher.BeginInvoke(updateUIResult, Environment.NewLine);
                        Stopwatch stopWatch = new Stopwatch();
                        logger.WriteLog("Process was started");
                        timePerformance.SetStartTime();

                        string baseApiAddress = ConfigurationManager.AppSettings["baseApiAddress"];
                        client.BaseAddress = new Uri(baseApiAddress);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var tweets = tweetData.GetTweets(client, dateFrom.Value, dateTo.Value).Result;

                        var results = tweets
                            .Distinct()
                            .OrderBy(x => x.Stamp)
                            .ToList();

                        timePerformance.SetEndTime();

                        Dispatcher.BeginInvoke(updateUIResult, String.Format("Count: {0}", results.Count));
                        Dispatcher.BeginInvoke(updateUIResult, Environment.NewLine);
                        Dispatcher.BeginInvoke(updateUIResult, String.Format("RunTime: {0}", timePerformance.GetElapsedTime()));
                        Dispatcher.BeginInvoke(updateUIResult, Environment.NewLine);
                        Dispatcher.BeginInvoke(updateUIResult, "Tweets: ");
                        Dispatcher.BeginInvoke(updateUIResult, Environment.NewLine);

                        foreach (var result in results)
                        {
                            Dispatcher.BeginInvoke(updateUIResult, Environment.NewLine);
                            Dispatcher.BeginInvoke(updateUIResult, String.Format(@"{0}-{1}-{2}", result.Id, result.Stamp, result.Text.Replace("\n", "\\n").Replace("\r", "\\r")));
                        }

                        logger.WriteLog(String.Format("Count: {0}", results.Count));
                        logger.WriteLog(String.Format("RunTime: {0}", timePerformance.GetElapsedTime()));
                        logger.WriteLog("Process was finished");
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLog(ex.Message);
                    }
                }
            }
        }

        private void UpdateUITextOutputInfo(string text)
        {
            this.txtOutputInfo.AppendText(text);
        }
    }
}
