using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace CleanDisk24
{
    public interface ILoggable
    {
        void Log(string message);
    }

    public class Database
    {
        private ILoggable WindowForCommunication { get; set; }
        private MainWindow mw;
        public Database(MainWindow mw) { this.mw = mw; }
        public Database() { }

        ///<summary>Resets the   AllRootDrives on "set{}"</summary>
        private ObservableCollection<MyRootPlace> _allRoots;         //ok
        public ObservableCollection<MyRootPlace> AllRoots            //ok
        {
            get
            {
                if (_allRoots == null)
                {
                    string message = "There was no Root directory at all.";
                    //System.Windows.MessageBox.Show(message);
                    mw.Log(message);

                    _allRoots = new ObservableCollection<MyRootPlace>();
                    _allRoots.Add(defaultRootDrive);
                }
                return _allRoots;
            }
            set
            {
                _allRoots = value;
                ObservableCollection<MyRootDrive> onlyDrives = new ObservableCollection<MyRootDrive>();
                //foreach (MyRootDrive drive in value) AllRootDrives.Add(drive);
                foreach (MyRootPlace drive in value) if (drive is MyRootDrive) onlyDrives.Add(drive as MyRootDrive);
                AllRootDrives = onlyDrives;
            }
        }

        /// <summary>
        /// Tries to start a communication channel from database
        /// </summary>
        /// <param name="window"></param>
        /// <returns>a sentence what happened</returns>
        internal string SetWindowForCommunication(ILoggable window)
        {
            return SetWindowForCommunication(window, false);
        }
        /// <summary>
        /// Tries to start a communication channel from database, OR Overwrites to new.
        /// </summary>
        /// <param name="window"></param>
        /// <returns>a sentence what happened</returns>
        internal string SetWindowForCommunication(ILoggable window, bool forceOverwrite)
        {
            if (WindowForCommunication == null)
            {
                WindowForCommunication = window;
                return $"Communication starts through: {(window as Window).Name}";
            }
            else
            {
                if (forceOverwrite)
                {
                    string previous = (WindowForCommunication as Window).Name;
                    WindowForCommunication = window;
                    return $"Communication window changed from {previous} to: {(window as Window).Name}";
                }
                return $"Communicating already through: {(WindowForCommunication as Window).Name}";
            }
        }

        /// <summary>
        /// Tries to send message to the window which asked to be the communicator
        /// </summary>
        /// <param name="v"></param>
        internal void Log(string message)//, ILoggable window)
        {
            /*
            window = (ILoggable)WindowForCommunication;
            window.Log(message);*/
            WindowForCommunication.Log(message);
        }

        public ObservableCollection<MyRootPlace> SetOfChosenRoots;

        ///<summary>Gets set by AllRoots change</summary>
        public ObservableCollection<MyRootDrive> AllRootDrives { get; private set; } = new ObservableCollection<MyRootDrive>();
        public List<MyDirectory> AllDirectories { get; private set; } = new List<MyDirectory>();
        public List<MyFile> AllFiles { get; private set; } = new List<MyFile>();

        public void ReloadAllRoots(ObservableCollection<MyRootPlace> newData)
        {
            _allRoots = newData;
        }

        private static MyRootDrive defaultRootDrive = MyItemDirectoryOrFile.Def();
        //new MyRootDrive("fake_Name", DriveType.Unknown, true, null, null);// { Name = "C:" };  // ok
        public static MyRootDirectory prodigyNTB = new MyRootDirectory("Prodigy", @"C:\Music\Prodigy");
        public static MyRootDirectory prodigyPC = new MyRootDirectory("Prodigy", @"D:\Music\Music\Prodigy");
        public static MyRootDirectory emptyFakeRoot = new MyRootDirectory("", "") { Items = new List<MyItemDirectoryOrFile>() };
    }
}
