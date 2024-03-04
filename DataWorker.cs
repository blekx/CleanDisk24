using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanDisk24
{
    public partial class DataWorker
    {
        private Database DB;
        public MainWindow mw;

        public DataWorker(Database dB, MainWindow mw)
        {
            this.DB = dB;// ?? throw new ArgumentNullException(nameof(dB));
            this.mw = mw;// ?? throw new ArgumentNullException(nameof(mw));
        }


        /// <summary>Displays all roots from DB.</summary>
        public ObservableCollection<MyRootPlace> GetAllRoots()
        {
            ObservableCollection<MyRootPlace> oc = new ObservableCollection<MyRootPlace>();
            foreach (MyRootPlace root in DB.AllRoots)
                if (Directory.Exists(root.WholePath))
                    oc.Add(root);
            return oc;
        }
        /// <summary>Displays all Hard drives from DB.</summary>
        internal ObservableCollection<MyRootDrive> GetAllDiscs() => DB.AllRootDrives;

        private ObservableCollection<DriveInfo> FindAllUnits() => new ObservableCollection<DriveInfo>(DriveInfo.GetDrives());

        ///<summary>Gets Drives and adds an example Directory. Into: DB.AllRoots </summary>
        public void ResetAllRoots()
        {
            ObservableCollection<DriveInfo> allNew = FindAllUnits();
            ObservableCollection<MyRootPlace> result = new ObservableCollection<MyRootPlace>();
            foreach (DriveInfo di in allNew)
            {
                result.Add(new MyRootDrive(di.Name, di.DriveType, di.IsReady, di.RootDirectory, di));
            }
            //AddExampleDirectoryTo_DB_AllRoots(result);
            AddExampleDirectoryTo(result);

            DB.AllRoots = result;
        }

        //private void AddExampleDirectoryTo_DB_AllRoots()
        private void AddExampleDirectoryTo(ObservableCollection<MyRootPlace> destination)
        {
            List<MyRootDirectory> list = new List<MyRootDirectory>() { Database.prodigyNTB, Database.prodigyPC };
            foreach (MyRootDirectory rootDirectory in list)
                if (Directory.Exists(rootDirectory.WholePath))
                {
                    //DB.AllRoots.Add(rootDirectory);
                    destination.Add(rootDirectory);
                    //System.Windows.MessageBox.Show("Added a root directory (" + rootDirectory.WholePath + ")");
                    mw.Log("Added a root directory (" + rootDirectory.WholePath + ")");
                }
        }

        public void AddDirectoryToDBList(MyDirectory md) => DB.AllDirectories.Add(md);

        ///<summary>Finds parent in DB (or creates). Returns null, if given directory is top level.</summary>
        private MyPlace FindOrCreateParentDirectoryOf(DirectoryInfo directory)
        {
            DirectoryInfo parent = directory.Parent;
            if (parent == null) return null;
            DirectoryInfo root = directory.Root;
            MyRootDrive rootImage = ThisRootDrive(root);
            if (parent.FullName == root.FullName) return rootImage;
            return FindOrCreateThisParentMiddirectoryOf(rootImage, parent);
        }

        private MyDirectory FindOrCreateThisParentMiddirectoryOf(MyRootDrive root, DirectoryInfo directory)
        {
            string partPath = StringSplitter.Split_GetRemainingPart(directory.FullName);
            MyDirectory result = FindDirectory(root, partPath)
                 ?? CreateMyDirectoryImage(root, directory);
            return result;
        }
        private MyDirectory CreateMyDirectoryImage(MyRootDrive root, DirectoryInfo directory)
        {
            DirectoryInfo parent = directory.Parent;
            if (parent.FullName == root.WholePath) return new MyDirectory(root, directory);
            else return FindOrCreateThisParentMiddirectoryOf(root, parent);
        }
        ///<summary>Continues searching in the given Part of path-string</summary>
        ///<param name="particularPath">Must not start or end with backslash</param>
        private MyDirectory FindDirectory(MyPlace searchedDirectory, string particularPath)
        {
            if (searchedDirectory == null) return null;
            string firstDir = "";
            string remainingPath = "";
            bool CanSplitPath = StringSplitter.Split(ref firstDir, ref remainingPath, particularPath);
            if (CanSplitPath) return FindDirectory(searchedDirectory.GetSubDir(firstDir), remainingPath);
            //else
            foreach (MyDirectory md in searchedDirectory.Items)
                if (md.Name == remainingPath) return md;
            //else
            return null;
        }


        private MyRootDrive FindOrCreateRootDrive(DirectoryInfo directory)
            //=> DB.AllRoots.FirstOrDefault(mp => mp.Name == directory.Name && mp.GetType() == typeof(MyRootDrive)) as MyRootDrive
            => DB.AllRootDrives.FirstOrDefault(root => root.Name == directory.Root.Name)
            ?? CreateMyRootDriveImage(directory);

        private MyRootDrive CreateMyRootDriveImage(DirectoryInfo canBeSubDirectory)
        {
            DriveInfo di = new DriveInfo(canBeSubDirectory.Root.Name);
            return new MyRootDrive(di.Name, di.DriveType, di.IsReady, canBeSubDirectory.Root, di);
        }

        ///<summary>Returns the root from DB, which represents this DirectoryInfo. Or creates a new RootDrive.</summary>
        private MyRootDrive ThisRootDrive(DirectoryInfo directory)
            => FindOrCreateRootDrive(directory);// as MyRootDrive;

        ///<summary>Can be root</summary>
        public MyPlace CreateMyDirectoryImage(DirectoryInfo directory)
        {
            MyPlace parent = FindOrCreateParentDirectoryOf(directory);
            if (parent == null)
            {
                return FindOrCreateRootDrive(directory);
            }
            return new MyDirectory(parent, directory);
        }
        public MyDirectory CreateMyDirectoryImage(MyPlace parent, DirectoryInfo directory)
        {
            return new MyDirectory(parent, directory); ;
        }
    }
    //public static class MyDirectoryEstensions
    //{
    //    public static void SetParent(this MyDirectory child, MyDirectory parent)
    //    {
    //        child.Parent = parent;
    //        parent.Items.Add(child);
    //    }
    //
    //}
}
