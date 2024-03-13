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

namespace CleanDisk24.Classes.Visual.Windows
{
    /// <summary>
    /// Interaction logic for WindowDebug.xaml
    /// </summary>
    public partial class WindowDebug : Window, ILoggable
    {
        public WindowDebug()
        {
            InitializeComponent();
            EdgeMover.CreateEdgesForWindow(this, mainGrid, this.DragThisWindowAndLogAlsoWhichElementCaused);
                //.DragMove);
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
            
        public void DragThisWindowAndLogAlsoWhichElementCaused(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
            Log((sender as Rectangle).Name.ToString() + " moving " + this.ToString());
        }

        public void Log(string message)
        {
            TB_Log.Text += Environment.NewLine + message;
            ScrollV_TB_Log.ScrollToEnd();
        }
    }
}
