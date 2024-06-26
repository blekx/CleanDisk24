﻿using CleanDisk24.Classes.Visual.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace CleanDisk24.Database
{
    public class DB
    {
        private ILoggable WindowForCommunication { get; set; }
        private MainWindow mw;
        public DB(MainWindow mw) { this.mw = mw; }
        public DB() { }

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
                    Log(message);

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

        #region Communication, LOGging
        /// <summary>
        /// Tries to start a communication channel from database
        /// </summary>
        /// <param name="window"></param>
        /// <returns>a sentence what happened</returns>
        internal string SetWindowForCommunication(ILoggable window)
        {
            ILoggable uselessReference_PreviousWindow;
            return SetWindowForCommunication(window, false, out uselessReference_PreviousWindow, null);
        }
        /// <summary>
        /// Tries to start a communication channel from database, OR Overwrites to new.
        /// </summary>
        /// <param name="newWindow"></param>
        /// <returns>a sentence what happened</returns>
        internal string SetWindowForCommunication(ILoggable newWindow, bool forceOverwrite, out ILoggable previousWindow, Action restorePreviousWindow)
        {
            if (WindowForCommunication == null)
            {
                WindowForCommunication = newWindow;
                previousWindow = null;
                return $"Communication starts through: {(newWindow as Window).Name}";
            }
            else
            {
                previousWindow = WindowForCommunication;
                if (forceOverwrite)
                {
                    string previous = (WindowForCommunication as Window).Name;
                    WindowForCommunication = newWindow;
                    return $"Communication window changed from {previous} to: {(newWindow as Window).Name}";
                }
                else
                {
                    return $"Communicating already through: {(WindowForCommunication as Window).Name}";
                }
            }
        }

        /// <summary>
        /// Tries to send message to the window which asked to be the communicator.
        /// </summary>
        /// <param name="v"></param>
        internal void Log(string message)//, ILoggable window)
        {
            /*
            window = (ILoggable)WindowForCommunication;
            window.Log(message);*/
            WindowForCommunication.Log(message);
        }
        #endregion

        public ObservableCollection<MyRootPlace> SetOfChosenRoots { get; set; } = new ObservableCollection<MyRootPlace>();

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

        public MyRootPlace CurrentlyRemoved { get; set; }
        public MyDirectory Browser1 { get; set; }
        public MyDirectory Browser2 { get; set; }
    }
}
