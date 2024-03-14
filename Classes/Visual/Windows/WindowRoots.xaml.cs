using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CleanDisk24.Database;
using CleanDisk24.DataWorker;

namespace CleanDisk24.Classes.Visual.Windows
{
    /// <summary>
    /// Interaction logic for WindowRoots.xaml
    /// </summary>
    public partial class WindowRoots : Window, ILoggable
    {
        public MainWindow mw;
        private DB Database { get; }
        //public DB DB;
        System.Diagnostics.Stopwatch stopwatch_ForLog;
        private enum LoggingWay { thisWindow, Database, };
        private LoggingWay loggingWay = LoggingWay.thisWindow;
        private ILoggable previousWindowForCommunicationFromDatabase;
        private System.Timers.Timer TimerCloser { get; set; }

        public WindowRoots(MainWindow mainWindow)
        {
            this.mw = mainWindow;
            //this.DB = mainWindow.dataWorker.;
            Database = ((App)Application.Current).Database;
            DataWorkerAgent.ResetAllRoots(Database);  // !!
            DataContext = new FoldersDataViewModel(Database);
            InitializeComponent();
            stopwatch_ForLog = new System.Diagnostics.Stopwatch();
            stopwatch_ForLog.Start();
            Log(Database.SetWindowForCommunication(this, true, out previousWindowForCommunicationFromDatabase, RestorePreviousWindowCommunicatingFromDB));

        }

        private void RestorePreviousWindowCommunicatingFromDB()
        {
            Log($"Will back again communicate by: {(previousWindowForCommunicationFromDatabase as Window).Name}");
            loggingWay = LoggingWay.Database;
            ILoggable checking;
            Log(Database.SetWindowForCommunication(previousWindowForCommunicationFromDatabase, true, out checking, null));
            Log((checking as Window).Name);
        }

        public void Log(string text)
        {
            #region reg: string time = ((int)(stopwatch.Elapsed.TotalMil
            //string time = ((int)(stopwatch_ForLog.Elapsed.TotalMilliseconds * 10)).ToString();
            //if (time.Length > 7) time = time.Insert(time.Length - 7, " ");
            //if (time.Length > 4) time = time.Insert(time.Length - 4, ".");
            string time = $"{stopwatch_ForLog.Elapsed.TotalSeconds:0 000.0000}";
            #endregion
            text = $"({time}s) " + text;
            //SPLog.Children.Add(new TextBlock() { Text = text });
            if (loggingWay == LoggingWay.thisWindow)
            {
                TextBoxLog.Text += Environment.NewLine + text;
                //TextBoxLog.Text += Environment.NewLine + t.ToString() + Environment.NewLine + t2.ToString();
                SVLog.ScrollToEnd();
            }
            else if (loggingWay == LoggingWay.Database)
            {
                Database.Log(text);
            }
            //else
            //{
            //    TextBoxLog.Text += Environment.NewLine + "Loggind Error, while trying to log this:";
            //    TextBoxLog.Text += Environment.NewLine + text;
            //}
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private async void LbChooseDirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string brokenSays = "nothing";
            //
            brokenSays = await Broken_LbChooseDirectory_SelectionChanged(sender, e);
            Log("broken methood compleeted and: " + brokenSays);
        }


        /// <summary>
        /// Click on one of the ROOTS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task<string> Broken_LbChooseDirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string result = "Bad";
            MyPlace selectedDirectory = (sender as ListBox).SelectedItem as MyPlace;
            //await Task.Run(() =>
            //{
            try
            {
                string message = await DataWorkerAgent.SetBrowser1_async(selectedDirectory, Database);
                //mw.DataWorker.SetBrowsedDirectory(selectedDirectory, mw.DataWorker.Browser1);
                //});
                DataContext = new FoldersDataViewModel(Database);
                Log(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                result = "Good";
            }
            return result;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Prevent the window from closing immediately
            e.Cancel = true;

            // Start a timer to delay the actual closing
            TimerCloser = new Timer(3000); // 3000 milliseconds = 3 seconds
            TimerCloser.Elapsed += Timer_Elapsed;
            TimerCloser.AutoReset = false; // Only fire the event once
            TimerCloser.Start();
            RestorePreviousWindowCommunicatingFromDB();
            //System.Threading.Thread.Sleep(3000);
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Close the window after the delay
            Dispatcher.Invoke(() =>
            {
                Closing -= Window_Closing;
                Close();
            });
        }
    }
}
