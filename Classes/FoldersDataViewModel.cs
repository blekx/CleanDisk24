using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CleanDisk24
{
    public class FoldersDataViewModel : INotifyPropertyChanged
    {
        private Database DB;
        private DataWorkerAgent dw;

        //public FoldersDataViewModel(Database db) { DB = db; }
        public FoldersDataViewModel(DataWorkerOld dw)
        {
            this.dw = dw;
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

        //public ObservableCollection<MyRootDrive> ModelHarddrives => dw.GetAllUnits();
        public ObservableCollection<MyRootDrive> ModelHarddrives => dw.GetAllDiscs();
        public ObservableCollection<MyRootPlace> ModelRoots => dw.GetAllRoots(); // panel 1/4
        public ObservableCollection<MyItemDirectoryOrFile> ModelRoots_Sub => TestModelItems; //dw.GetDirectoryChildren_RootsSub(); // panel 2/4
        public ObservableCollection<MyRootPlace> ModelChosenRoots => dw.GetAllRoots();// dw.GetChosenRoots(); // panel 3/4
        public ObservableCollection<MyItemDirectoryOrFile> ModelChosen_Sub => dw.GetDirectoryChildren_ChosenSub(); // panel 4/4
    }
}
