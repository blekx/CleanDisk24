using CleanDisk24.Database;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace CleanDisk24.DataWorker
{
    public static partial class DataWorkerAgent
    {
        //private DB DB;
        //public MainWindow mw;
        /*
        public DataWorkerAgent(DB dB, MainWindow mw)
        {
            this.DB = dB;// ?? throw new ArgumentNullException(nameof(dB));
            this.mw = mw;// ?? throw new ArgumentNullException(nameof(mw));
        }
        */

        /// <summary>Displays all roots from DB.</summary>
        public static ObservableCollection<MyRootPlace> GetAllRoots(DB DB)
        {
            ObservableCollection<MyRootPlace> oc = new ObservableCollection<MyRootPlace>();
            foreach (MyRootPlace root in DB.AllRoots)
                if (Directory.Exists(root.WholePath))
                    oc.Add(root);
            return oc;
        }
        /// <summary>Displays all Hard drives from DB.</summary>
        internal static ObservableCollection<MyRootDrive> GetAllDiscs(DB DB) => DB.AllRootDrives;

        private static ObservableCollection<DriveInfo> FindAllUnits() => new ObservableCollection<DriveInfo>(DriveInfo.GetDrives());

        ///<summary>Gets Drives and adds an example Directory. Into: DB.AllRoots </summary>
        public static void ResetAllRoots(DB DB)
        {
            ObservableCollection<DriveInfo> allNew = FindAllUnits();
            ObservableCollection<MyRootPlace> result = new ObservableCollection<MyRootPlace>();
            foreach (DriveInfo di in allNew)
            {
                result.Add(new MyRootDrive(di.Name, di.DriveType, di.IsReady, di.RootDirectory, di));
            }
            //AddExampleDirectoryTo_DB_AllRoots(result);
            AddExampleDirectoryTo(result, DB);

            DB.AllRoots = result;
        }

        //private void AddExampleDirectoryTo_DB_AllRoots()
        private static void AddExampleDirectoryTo(ObservableCollection<MyRootPlace> destination, DB DB)
        {
            List<MyRootDirectory> list = new List<MyRootDirectory>() { DB.prodigyNTB, DB.prodigyPC };
            foreach (MyRootDirectory rootDirectory in list)
                if (Directory.Exists(rootDirectory.WholePath))
                {
                    //DB.AllRoots.Add(rootDirectory);
                    destination.Add(rootDirectory);
                    //System.Windows.MessageBox.Show("Added a root directory (" + rootDirectory.WholePath + ")");
                    DB.Log("Added a root directory (" + rootDirectory.WholePath + ")");
                }
        }

        public static void AddDirectoryToDBList(MyDirectory md, DB DB) => DB.AllDirectories.Add(md);

        ///<summary>Finds parent in DB (or creates). Returns null, if given directory is top level.</summary>
        private static MyPlace FindOrCreateParentDirectoryOf(DirectoryInfo directory, DB DB)
        {
            DirectoryInfo parent = directory.Parent;
            if (parent == null) return null;
            DirectoryInfo root = directory.Root;
            MyRootDrive rootImage = ThisRootDrive(root, DB);
            if (parent.FullName == root.FullName) return rootImage;
            return FindOrCreateThisParentMiddirectoryOf(rootImage, parent);
        }

        private static MyDirectory FindOrCreateThisParentMiddirectoryOf(MyRootDrive root, DirectoryInfo directory)
        {
            string partPath = StringSplitter.Split_GetRemainingPart(directory.FullName);
            MyDirectory result = FindDirectory(root, partPath)
                 ?? CreateMyDirectoryImage(root, directory);
            return result;
        }
        private static MyDirectory CreateMyDirectoryImage(MyRootDrive root, DirectoryInfo directory)
        {
            DirectoryInfo parent = directory.Parent;
            if (parent.FullName == root.WholePath) return new MyDirectory(root, directory);
            else return FindOrCreateThisParentMiddirectoryOf(root, parent);
        }
        ///<summary>Continues searching in the given Part of path-string</summary>
        ///<param name="particularPath">Must not start or end with backslash</param>
        private static MyDirectory FindDirectory(MyPlace searchedDirectory, string particularPath)
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


        private static MyRootDrive FindOrCreateRootDrive(DirectoryInfo directory, DB DB)
            //=> DB.AllRoots.FirstOrDefault(mp => mp.Name == directory.Name && mp.GetType() == typeof(MyRootDrive)) as MyRootDrive
            => DB.AllRootDrives.FirstOrDefault(root => root.Name == directory.Root.Name)
            ?? CreateMyRootDriveImage(directory);

        private static MyRootDrive CreateMyRootDriveImage(DirectoryInfo canBeSubDirectory)
        {
            DriveInfo di = new DriveInfo(canBeSubDirectory.Root.Name);
            return new MyRootDrive(di.Name, di.DriveType, di.IsReady, canBeSubDirectory.Root, di);
        }

        ///<summary>Returns the root from DB, which represents this DirectoryInfo. Or creates a new RootDrive.</summary>
        private static MyRootDrive ThisRootDrive(DirectoryInfo directory, DB DB)
            => FindOrCreateRootDrive(directory, DB);// as MyRootDrive;

        ///<summary>Can be root</summary>
        public static MyPlace CreateMyDirectoryImage(DirectoryInfo directory, DB DB)
        {
            MyPlace parent = FindOrCreateParentDirectoryOf(directory, DB);
            if (parent == null)
            {
                return FindOrCreateRootDrive(directory, DB);
            }
            return new MyDirectory(parent, directory);
        }
        public static MyDirectory CreateMyDirectoryImage(MyPlace parent, DirectoryInfo directory)
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
