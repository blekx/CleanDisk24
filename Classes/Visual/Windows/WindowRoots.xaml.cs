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
        //private int ClickedOnNone
        /// <summary> In case some item was removed from the list, there is a way to give it back. </summary>
        private enum RemovingState
        {
            ///<summary> WGB </summary>
            WaitingToConfirmRemovalOrGiveBackItemToList,
            ///<summary> Not Waiting for any confirmation to remove item from list. </summary>
            NotWGB,
            ///<summary> Ctrl pressed + Waiting to confirm removal Or to give item back to list </summary>
            C_WGB,
            ///<summary> Ctrl pressed + Not Waiting for any confirmation to remove item from list. </summary>
            C_NWGB,
        }
        private RemovingState state = RemovingState.NotWGB;
        private RemovingState StateWrong
        {
            get => state;
            set
            {
                switch (state)
                {
                    case RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList:
                        switch (value)
                        {
                            case RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList:
                                // Perform action for staying in the same state
                                break;
                            case RemovingState.NotWGB:
                                // Perform action for transitioning to NotWGB state
                                break;
                            case RemovingState.C_WGB:
                                // Perform action for transitioning to C_WGB state
                                break;
                            case RemovingState.C_NWGB:
                                // Perform action for transitioning to C_NWGB state
                                break;
                        }
                        break;
                    case RemovingState.NotWGB:
                        switch (value)
                        {
                            case RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList:
                                // Perform action for transitioning to WaitingToConfirm state
                                break;
                            case RemovingState.NotWGB:
                                // Perform action for staying in the same state
                                break;
                            case RemovingState.C_WGB:
                                // Perform action for transitioning to C_WGB state
                                break;
                            case RemovingState.C_NWGB:
                                // Perform action for transitioning to C_NWGB state
                                break;
                        }
                        break;
                    case RemovingState.C_WGB:
                        switch (value)
                        {
                            case RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList:
                                // Perform action for transitioning to WaitingToConfirm state
                                break;
                            case RemovingState.NotWGB:
                                // Perform action for transitioning to NotWGB state
                                break;
                            case RemovingState.C_WGB:
                                // Perform action for staying in the same state
                                break;
                            case RemovingState.C_NWGB:
                                // Perform action for transitioning to C_NWGB state
                                break;
                        }
                        break;
                    case RemovingState.C_NWGB:
                        switch (value)
                        {
                            case RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList:
                                // Perform action for transitioning to WaitingToConfirm state
                                break;
                            case RemovingState.NotWGB:
                                // Perform action for transitioning to NotWGB state
                                break;
                            case RemovingState.C_WGB:
                                // Perform action for transitioning to C_WGB state
                                break;
                            case RemovingState.C_NWGB:
                                // Perform action for staying in the same state
                                break;
                        }
                        break;
                }
            }

        }

        private RemovingState State
        {
            get => state;
            set
            {
                switch (state) // <---From the current state...
                {
                    case RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList:
                        switch (value) // ...into a new state:
                        {
                            case RemovingState.NotWGB:
                                // No more waiting
                                break;
                            case RemovingState.C_WGB:
                                // Ctrl pressed
                                break;
                            case RemovingState.C_NWGB:
                                // Ctrl pressed
                                // No more waiting
                                throw new Exception("Too many state changes at once.");
                                break;
                        }
                        break;
                    case RemovingState.NotWGB:
                        switch (value)
                        {
                            case RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList:
                                // Waiting to give back item
                                break;
                            case RemovingState.C_WGB:
                                // Ctrl pressed
                                // Waiting to give back item
                                throw new Exception("Too many state changes at once.");
                                break;
                            case RemovingState.C_NWGB:
                                // Ctrl pressed
                                break;
                        }
                        break;
                    case RemovingState.C_WGB:
                        switch (value)
                        {
                            case RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList:
                                // Ctrl released
                                break;
                            case RemovingState.NotWGB:
                                // Ctrl released
                                // No more waiting
                                throw new Exception("Too many state changes at once.");
                                break;
                            case RemovingState.C_NWGB:
                                // No more waiting
                                break;
                        }
                        break;
                    case RemovingState.C_NWGB:
                        switch (value)
                        {
                            case RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList:
                                // Ctrl released
                                // Waiting to give back item
                                throw new Exception("Too many state changes at once.");
                                break;
                            case RemovingState.NotWGB:
                                // Ctrl released
                                break;
                            case RemovingState.C_WGB:
                                // Waiting to give back item
                                break;
                        }
                        break;
                }
                state = value; // Update the state after processing
            }
        }

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
            RemovingState state = RemovingState.NotWGB;
        }

        #region Window Start and End
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
            // if (e.OriginalSource == WindowBackground) //other way is on the panels: preview=>e.Handled=true;
            this.Close();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Prevent the window from closing immediately
            e.Cancel = true;

            // Start a timer to delay the actual closing
            TimerCloser = new Timer(3000); // 3000 milliseconds = 3 seconds
            TimerCloser.Elapsed += TimerCloser_Elapsed;
            TimerCloser.AutoReset = false; // Only fire the event once
            TimerCloser.Start();
            RestorePreviousWindowCommunicatingFromDB();
            //System.Threading.Thread.Sleep(3000);
        }
        private void TimerCloser_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Close the window after the delay
            Dispatcher.Invoke(() =>
            {
                Closing -= Window_Closing;
                Close();
            });
        }
        #endregion

        #region Panels events
        /// <summary> Panel 1, Click on one of the ROOTS </summary>
        private async void LbChooseDirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // from panel 1: into panel 2 scan the chosen directory
            // opposite to doubleclick, which adds the item into the list (into panel 3)

            string brokenSays = "nothing";
            //
            brokenSays = await Broken_LbChooseDirectory_SelectionChanged(sender, e);
            Log("broken methood compleeted and: " + brokenSays);

            BrowseDirectoryPanel2((sender as ListBox).SelectedItem as MyPlace);
        }

        /// <summary> Panel 1, Click on one of the ROOTS </summary>
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

        /// <summary> Panel 1 </summary>        
        private void lbChooseDirectory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // add item to list, if not already there
            AddRootDirectory((sender as ListBox).SelectedItem as MyRootPlace);
        }

        /// <summary> Panel 2-Head, Browse Up </summary>
        private void lbChooseDirectory_Sub_Head_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary> Panel 2-Head, Ctrl+Click => Add </summary>
        private void lbChooseDirectory_Sub_Head_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == RemovingState.C_NWGB)
            {
                AddRootDirectory((sender as ListBox).SelectedItem as MyRootPlace);
            }
            //or stop waiting for possible recovery of the removed item:
            if (State == RemovingState.C_WGB)
            {
                State = RemovingState.C_NWGB;
            }
        }

        /// <summary> Panel 2, DBClick_BrowseSub </summary>
        private void lbChooseDirectory_Sub_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BrowseDirectoryPanel2((sender as ListBox).SelectedItem as MyDirectory);
        }

        /// <summary> Panel 2, DBClick_BrowseSub Ctrl+Click => Add</summary>
        private void lbChooseDirectory_Sub_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == RemovingState.C_NWGB)
            {
                AddRootDirectory((sender as ListBox).SelectedItem as MyRootPlace);
            }
            //or stop waiting for possible recovery of the removed item:
            if (State == RemovingState.C_WGB)
            {
                State = RemovingState.C_NWGB;
            }
        }

        /// <summary> Panel 3, Click on one of the ROOTS </summary>
        private void lbChoosenDirectories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //from panel 3: into panel 4 scan the chosen directory
            // check: ((( opposite to doubleclick, which removes the item )))
            BrowseDirectoryPanel4((sender as ListBox).SelectedItem as MyDirectory);
        }

        /// <summary> Panel 3, DB Click on one of the ROOTS </summary>
        private void lbChoosenDirectories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //DBClick_Remove + Target Panel4_Head to create GiveBack item
            if (State == RemovingState.NotWGB)
            {
                State = RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList;
                var removed = (sender as ListBox).SelectedItem as MyDirectory;
                BrowseDirectoryPanel4(removed); // ?duplication from single click?
                RemoveChosenDirectory(removed);
            }
            if (State == RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList)
            {
                BrowseDirectoryPanel4((sender as ListBox).SelectedItem as MyDirectory); // ?duplication from single click?

            }
            if (State == RemovingState.C_NWGB)
            {
                State = RemovingState.C_WGB;
                var removed = (sender as ListBox).SelectedItem as MyDirectory;
                BrowseDirectoryPanel4(removed); // ?duplication from single click?
                RemoveChosenDirectory(removed);
            }
            if (State == RemovingState.C_WGB)
            {
                //State = RemovingState.C_WGB;
                var removed = (sender as ListBox).SelectedItem as MyDirectory;
                BrowseDirectoryPanel4(removed); // ?duplication from single click?
                RemoveChosenDirectory(removed);
            }
        }

        /// <summary> Panel 4_Head, DBClick_(!GiveBackWaiting)_ParentDirectoryInBrowser + (GBW)_GiveBack,  </summary>
        private void lbChoosenDirectory_Sub_Head_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (State == RemovingState.NotWGB || State == RemovingState.C_NWGB)
            {
                BrowseDirectoryPanel4(DataWorkerAgent.GetBrowser2Directory(Database).Parent as MyDirectory);
            }
            else
            {
                AddRootDirectory(DataWorkerAgent.GetCurrentlyRemovedChoosen_MyRootPlace(Database));
            }

        }

        /// <summary> Panel 4_Head, (GBW)CtrlClick_GiveBack + (!GBW)_GBW=>true) </summary>
        private void lbChoosenDirectory_Sub_Head_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == RemovingState.C_WGB)
            {
                AddRootDirectory(DataWorkerAgent.GetCurrentlyRemovedChoosen_MyRootPlace(Database));
                State = RemovingState.C_NWGB;
            }
            if (State == RemovingState.NotWGB) // "false undo" hack
            {
                var dir = DataWorkerAgent.GetBrowser2Directory(Database);
                DataWorkerAgent.SetupCurrentlyRemovedTo(Database, dir as MyRootPlace);
                BrowseDirectoryPanel4(dir as MyDirectory);
                State = RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList;
            }

        }

        /// <summary> Panel 4, (Browser): (!GBW)DBClick_BrowseSub+(GBW)_GBW=>false,   </summary>
        private void lbChoosenDirectories_Sub_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (State == RemovingState.NotWGB)
            {
                BrowseDirectoryPanel4((sender as ListBox).SelectedItem as MyDirectory);
            }
            if (State == RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList)
            {
                State = RemovingState.NotWGB;
            }
        }

        /// <summary> Panel 4, (Browser): (!GBW)CtrlClick_Add, (GiveBackWaiting)=> all red,  </summary>
        private void lbChoosenDirectories_Sub_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == RemovingState.C_NWGB)
            {
                AddRootDirectory((sender as ListBox).SelectedItem as MyRootPlace);
            }
            if (State == RemovingState.C_WGB)
            {
                // all red
            }
        }
        #endregion

        private void AddRootDirectory(MyRootPlace myRootPlace)
        {
            if (DataWorkerAgent.IsInChosenRoots(myRootPlace, Database))
            {
                //highlight in panel 4
                throw new NotImplementedException();
            }
            else
            {
                DataWorkerAgent.AddToChosenRoots(myRootPlace, Database);
            }
        }

        /// <summary>Scan 1 (given) directory and then project it into panel 2 and also to its header.</summary>
        private async void BrowseDirectoryPanel2(MyPlace dir) //MyDirectory myDirectory)
        {
            //  var dir = DataWorkerAgent.FindDirectory(Database, dir); //bad way? 05-15
            //ModelRootsHead=
            string tryingToSetPanel2 = await DataWorkerAgent.SetBrowser1_async(dir as MyPlace, Database);
            Log(tryingToSetPanel2);
        }

        /// <summary>Scan 1 (given) directory and then project it into panel 4 and also to its header.</summary>
        private void BrowseDirectoryPanel4(MyDirectory myDirectory)
        {
            throw new NotImplementedException();
        }

        private void RemoveChosenDirectory(MyDirectory removed)
        {
            throw new NotImplementedException();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                if (State == RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList) State = RemovingState.C_WGB;
                else if (State == RemovingState.NotWGB) State = RemovingState.C_NWGB;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                if (State == RemovingState.C_WGB) State = RemovingState.WaitingToConfirmRemovalOrGiveBackItemToList;
                else if (State == RemovingState.C_NWGB) State = RemovingState.NotWGB;
            }
        }

        private void lbChooseDirectory_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
