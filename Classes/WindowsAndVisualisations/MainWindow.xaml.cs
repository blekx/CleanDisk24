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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CleanDisk24.DataWorker;

namespace CleanDisk24
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ILoggable
    {
        //public DataWorkerAgent Agent;
        private Database Database { get; }
        private EdgeMover edgeMover;
        public MainWindow()
        {
            InitializeComponent();
            //Agent = new DataWorkerAgent(new Database(this), this);
            //DataWorker.StartupLoadData();
            Database = ((App)Application.Current).Database;
            edgeMover = new EdgeMover();
            EdgeMover.CreateEdgesForWindow(this, mainGrid, this.DragMove);
            //edgeMover = new EdgeMover(this, mainGrid, this.DragMove);
            Log(Database.SetWindowForCommunication(this));
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btScanner_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btSetRoots_Click(object sender, RoutedEventArgs e)
        {
            WindowRoots windowRoots = new WindowRoots(this);
            windowRoots.Show();
        }


        private void btTest_Click(object sender, RoutedEventArgs e)
        {
            System.IO.DirectoryInfo di;
            if (System.IO.Directory.Exists(@"D:\Music\Music\Prodigy"))
            {
                di = new System.IO.DirectoryInfo(@"D:\Music\Music\Prodigy");
            }
            else if (System.IO.Directory.Exists(@"C:\Music\Prodigy"))
            {
                di = new System.IO.DirectoryInfo(@"C:\Music\Prodigy");
            }
            else di = null;
            try
            {
                DataWorkerAgent.ScanAndAddDirectory_Initialize(di, Database);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void Log(string message)
        {
            TB_Log.Text += Environment.NewLine + message;
            ScrollV_TB_Log.ScrollToEnd();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {

        }
    }
}
