using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace CleanDisk24
{
    public class Database
    {
        private MainWindow mw;
        public Database(MainWindow mw) { this.mw = mw; }

        ///<summary>Resets the   AllRootDrives on "set{}"</summary>
        private ObservableCollection<MyRootPlace> _allRoots;         //ok
        public ObservableCollection<MyRootPlace> AllRoots            //ok
        {
            get
            {
                if (_allRoots == null)
                {
                    _allRoots = new ObservableCollection<MyRootPlace>();
                    _allRoots.Add(defaultRootDrive);

                    string message = "There was no Root directory at all.";
                    //System.Windows.MessageBox.Show(message);
                    mw.Log(message);
                }
                return _allRoots;
            }
            set
            {
                _allRoots = value;
                ObservableCollection<MyRootDrive> onlyDrives = new ObservableCollection<MyRootDrive>();
                //foreach (MyRootDrive drive in value) AllRootDrives.Add(drive);
                foreach (MyRootPlace drive in value) if (drive is MyRootDrive) AllRootDrives.Add(drive as MyRootDrive);
                AllRootDrives = onlyDrives;
            }
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

        private MyRootDrive defaultRootDrive = MyItemDirectoryOrFile.Def();
            //new MyRootDrive("fake_Name", DriveType.Unknown, true, null, null);// { Name = "C:" };  // ok
        public static MyRootDirectory prodigyNTB = new MyRootDirectory("Prodigy", @"C:\Music\Prodigy");
        public static MyRootDirectory prodigyPC = new MyRootDirectory("Prodigy", @"D:\Music\Music\Prodigy");
        public static MyRootDirectory emptyFakeRoot = new MyRootDirectory("", "") { Items = new List<MyItemDirectoryOrFile>() };
    }
}
