using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CleanDisk24
{
    [DebuggerDisplay("{WholePath} {Abb}")]
    public abstract class MyItemDirectoryOrFile
    {
        public string Name { get; set; }
        /// <summary>Abbreviation of this type</summary> 
        //public static char Abb { get => 'I'; }
        //public const char Abb = 'I';
        public char Abb;// = 'I';
        public DateTime DateOfCreation { get; set; }

        private long _size;
        public long Size
        {
            get => _size;
            set
            {
                _size = value;
                _colorForSize = ColorCreator.BrushForSize(value);
            }
        }
        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                _colorForProgress = ColorCreator.BrushForProgress(value);
            }
        }
        public MyPlace Parent { get; set; }
        private bool _isSizeCountedProperly = false;
        public bool IsSizeCountedProperly
        {
            get => _isSizeCountedProperly;
            set
            {
                _isSizeCountedProperly = value;
                if (value && this is MyPlace)
                {
                    foreach (MyItemDirectoryOrFile subItem in (this as MyPlace).Items)
                    {
                        if (!subItem.IsSizeCountedProperly)
                        {
                            _isSizeCountedProperly = false;
                            break;
                        }
                    }
                }
            }
        }
        private Brush _colorForProgress;
        public Brush ColorForProgress { get => _colorForProgress; }
        private Brush _colorForSize;
        public Brush ColorForSize { get => _colorForSize; }
        internal static MyRootDrive Def() => new MyRootDrive("C:\\", DriveType.Unknown, true, null, null);//{ Name = "C:\\" };
        //public LinearGradientBrush DefSize_GradientBrush => ColorCreator.GradientBrush_Size;
        //public LinearGradientBrush DefProgress_GradientBrush => ColorCreator.GradientBrush_Progress;
        /// <summary>Default (rainbowy) gradient brush.</summary>
        public LinearGradientBrush SizeBr => ColorCreator.GradientBrush_Size;
        /// <summary>Default (rainbowy) gradient brush.</summary>
        public LinearGradientBrush ProgBr => ColorCreator.GradientBrush_Progress;
        //public readonly string Path;
        public string WholePath
        {
            get
            {
                if (Parent != null) return System.IO.Path.Combine(Parent.WholePath, Name);
                else if (this is MyRootPlace) return ((MyRootPlace)this).WholePath;
                else throw new NotImplementedException("WholePath - reading eror, caused by wrong Type.");
            }
        }
        /*
        =>
        Parent == null ?
        Name :
        System.IO.Path.Combine(this.Parent?.WholePath, this.Name);
        */

        public MyItemDirectoryOrFile()
        {
            Progress = 0;
            Size = 0;
        }
        public override string ToString() => base.ToString() + WholePath;
    }

    public abstract class MyPlace : MyItemDirectoryOrFile
    {
        //new public const char Abb = 'P';
        //new public char Abb = 'P';
        public List<MyItemDirectoryOrFile> Items { get; set; }
        public MyDirectory GetSubDir(string name)
        {
            foreach (MyDirectory md in Items) if (md.Name == name) return md;
            return null;
        }
        public MyPlace() : base()
        {
            this.Items = new List<MyItemDirectoryOrFile>();
        }
        public override string ToString() => base.ToString() + WholePath;
    }

    public abstract class MyRootPlace : MyPlace
    {
        public new string WholePath
        {
            get
            {
                if (this.GetType() == typeof(MyRootDirectory)) return (this as MyRootDirectory).WholePath;
                else if (this is MyRootDrive) return (this as MyRootDrive).WholePath;
                else throw new NotImplementedException("WholePath - reading eror, caused by wrong Type.");
            }
        }
        public override string ToString() => base.ToString() + WholePath;
    }

    //[DebuggerDisplay("{Path} " + (new MyItemDirectoryOrFile() as object).GetType().ToString())]

    public class MyRootDrive : MyRootPlace
    {
        //private new string Path => Name;
        public MyRootDrive(string name, DriveType driveType, bool isReady, DirectoryInfo rootDirectory, DriveInfo di)
        {
            Name = name;
            this.Abb = 'H';
        }
        public new string WholePath => System.IO.Path.Combine(Name);
        public override string ToString() => base.ToString() + WholePath;
    }

    public class MyRootDirectory : MyRootPlace
    {
        //new public char Abb = 'M';
        //new public const char Abb = 'M';
        private readonly string Path;
        private MyRootDirectory(string name)
        {
            this.Name = name;
            this.Abb = 'M';
        }

        public MyRootDirectory(string name, string path) : this(name)
        {
            this.Path = path;
        }
        //private string _path;
        public new string WholePath => Path;
        //public new string WholePath { get => Name; }
        //private new MyPlace Parent = null;//{ get; set; }
        //public MyDirectory(string name, DirectoryInfo rootDirectory) : base(name, null, null, rootDirectory, null)        {         }
    }

    public class MyDirectory : MyPlace//, IHasParent
    {
        //new public const char Abb = 'D';
        //new public char Abb = 'D';
        
        private MyDirectory(MyPlace parent)
        {
            this.Parent = parent;
            this.Parent.Items.Add(this);
            this.Abb = 'D';
        }

        // <summary>Creates my image - (Where, DirectoryInfo of the original.) </summary>  
        ///<summary>This constructor should be followed by Adding this directory to DB</summary>  
        public MyDirectory(MyPlace parent, DirectoryInfo original) : this(parent)
        {
            Name = original.Name;
            DateOfCreation = original.CreationTime;
            string message = "Not sure if we have all data, creating:" + original.FullName;// !!
            //System.Windows.MessageBox.Show(message);
            //throw new Exception(message);
        }
        private string Wp => WholePath;// <--- test,delete it
    }

    internal interface IHasNoParent
    {
        //public string WholePath => Name;
        string Name { get; set; }
    }
    internal interface IHasParent
    {
        /*
        public constructor(MyItemDirectoryOrFile)this(MyPlace parent)
            {
            this.Parent = partent;
            this.Parent.Items.Add(this);
            }*/
    }

    public class MyFile : MyItemDirectoryOrFile//, IHasParent
    {
        //new public const char Abb = 'F';
        //new public char Abb = 'F';
        public string FileExtension { get; set; }

        public MyFile(MyPlace parent)
        {
            this.Parent = parent;
            this.Parent.Items.Add(this);
            this.Abb = 'F';
        }

        public MyFile(MyPlace parent, FileInfo fileinfo) : this(parent)
        {
            this.Name = fileinfo.Name;
            this.FileExtension = fileinfo.Extension;
            this.DateOfCreation = fileinfo.CreationTime;
            this.Size = fileinfo.Length;
            //throw new NotImplementedException();
        }
    }
    ///<summary>MyPlace + DirectoryInfo</summary>
    public struct DuoDirInfo
    {
        public MyPlace MyPlace;
        public DirectoryInfo DirectoryInfo;

        public DuoDirInfo(MyPlace myPlace, DirectoryInfo directoryInfo)
        {
            MyPlace = myPlace ?? throw new ArgumentNullException(nameof(myPlace));
            DirectoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));
        }
    }
}
