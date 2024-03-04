using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CleanDisk24.DataWorker; 

namespace CleanDisk24
{
    public class FoldersDataViewModel : INotifyPropertyChanged
    {
        private Database DB { get; set; }
        //private DataWorkerAgent DB;

        //public FoldersDataViewModel(Database db) { DB = db; }
        /*public FoldersDataViewModel(DataWorkerOld DB)
        {
            this.DB = DB;
        }*/
        public FoldersDataViewModel(Database db)
        {
            DB = db;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //private class ModelItems
        public ObservableCollection<MyItemDirectoryOrFile> TestModelItems => new ObservableCollection<MyItemDirectoryOrFile>() { MyItemDirectoryOrFile.Def() };

        public ObservableCollection<MyRootPlace> AllRoots// => new ObservableCollection<MyRoot>();
        {
            get
            {
                return DB.AllRoots;
            }
        }

        //public ObservableCollection<MyRootDrive> ModelHarddrives => DB.GetAllUnits();
        public ObservableCollection<MyRootDrive> ModelHarddrives => DataWorkerAgent.GetAllDiscs(DB);
        public ObservableCollection<MyRootPlace> ModelRoots => DataWorkerAgent.GetAllRoots(DB); // panel 1/4
        public ObservableCollection<MyItemDirectoryOrFile> ModelRoots_Sub => TestModelItems; //DB.GetDirectoryChildren_RootsSub(); // panel 2/4
        public ObservableCollection<MyRootPlace> ModelChosenRoots => DataWorkerAgent.GetAllRoots(DB);// DB.GetChosenRoots(); // panel 3/4
        public ObservableCollection<MyItemDirectoryOrFile> ModelChosen_Sub => DataWorkerAgent.GetDirectoryChildren_ChosenSub(); // panel 4/4
    }
}
