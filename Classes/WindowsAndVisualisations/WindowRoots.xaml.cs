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
using System.Windows.Shapes;
using CleanDisk24.DataWorker;

namespace CleanDisk24
{
    /// <summary>
    /// Interaction logic for WindowRoots.xaml
    /// </summary>
    public partial class WindowRoots : Window
    {
        public MainWindow mw;
        private Database Database { get; }
        //public Database DB;
        System.Diagnostics.Stopwatch stopwatch;

        public WindowRoots(MainWindow mainWindow)
        {
            this.mw = mainWindow;
            //this.DB = mainWindow.dataWorker.;
            Database = ((App)Application.Current).Database; 
            DataWorkerAgent.ResetAllRoots(Database);  // !!
            DataContext = new FoldersDataViewModel(Database);
            InitializeComponent();
            stopwatch = new System.Diagnostics.Stopwatch(); //for logging
            stopwatch.Start();
        }

        public void Log(string text)
        {
            #region reg: string time = ((int)(stopwatch.Elapsed.TotalMil
            string time = ((int)(stopwatch.Elapsed.TotalMilliseconds * 10)).ToString();
            if (time.Length > 7) time = time.Insert(time.Length - 7, " ");
            time = time.Insert(time.Length - 4, ".");
            #endregion
            text = $"({time}s) " + text;
            //SPLog.Children.Add(new TextBlock() { Text = text });
            TextBoxLog.Text += Environment.NewLine + text;
            //TextBoxLog.Text += Environment.NewLine + t.ToString() + Environment.NewLine + t2.ToString();
            SVLog.ScrollToEnd();
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

        /// <summary>
        /// Click on one of the ROOTS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LbChooseDirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyPlace selectedDirectory = (sender as ListBox).SelectedItem as MyPlace;
            //await Task.Run(() =>
            //{
            string message =
            await
                DataWorkerAgent.SetBrowser1_async(selectedDirectory, Database);
            //mw.DataWorker.SetBrowsedDirectory(selectedDirectory, mw.DataWorker.Browser1);
            //});
            DataContext = new FoldersDataViewModel(Database);
            Log(message);
        }
    }
}
