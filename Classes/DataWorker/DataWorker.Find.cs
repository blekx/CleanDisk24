using CleanDisk24.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanDisk24.DataWorker
{
    public static partial class DataWorkerAgent
    {
        /// <summary> wrong </summary>
        public static MyDirectory FindDirectory(DB database, MyDirectory myDirectory)
        {
            //MyRootDrive rootDrive = DataWorkerAgent.FindOrCreateRootDrive(myDirectory);

            // database.AllRootDrives.FirstOrDefault(D => D.Name == ;


            MyRootDrive rootDrive = database.AllRootDrives.FirstOrDefault(
                root => root.Name == DataWorkerAgent.FindOrCreateRootDrive(new DirectoryInfo(myDirectory.WholePath), database).Name);
            MyDirectory result = null;

            if (result != null) { return result as MyDirectory; }
            else
            {
                return null;
            }
            return myDirectory;
        }


    }
}