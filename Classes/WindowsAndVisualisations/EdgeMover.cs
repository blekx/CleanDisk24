using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CleanDisk24
{
    public delegate void DragMoveDelegate(object sender, MouseButtonEventArgs e);
    public struct DataForLastTick
    {
        public Rectangle sender; public MouseEventArgs e;

        public DataForLastTick(Rectangle sender, MouseEventArgs e)
        {
            this.sender = sender;
            this.e = e;
        }
    }

    internal class EdgeMover
    {

        private static Window ParentWindow { get; set; }
        private static ILoggable WindowForCommunication;
        //private static Grid grid;
        private static bool DraggingEdgeNow { get; set; } //= false;
        private static bool LastTimerTick { get; set; } //= false;
        private static DataForLastTick DataForLastTick { get; set; }
        private static string errorMessage = "This part of program didn't manage to find the GRID (<Grid x:Name=\"mainGrid\" ) or something with it is wrong.";
        private static SolidColorBrush color = new SolidColorBrush(Color.FromRgb(
            //200, 250, 100));
            200, 50, 100));
        private Rectangle[] edges;
        private static DispatcherTimer timer;
        private static Stopwatch stopwatch = new Stopwatch();
        private static Rectangle ColorChangingEdge { get; set; }
        private static Rectangle DraggedEdge { get; set; }
        private const int msToFullyShowEdge = 1000;
        private const double fullOpacityIsOnly = 0.8;
        private readonly DragMoveDelegate dragMoveDelegate;
        private static WindowPosition positionBefore;

        public EdgeMover()
        {
            timer = new DispatcherTimer();
            timer.Interval = System.TimeSpan.FromMilliseconds(15);
            timer.Tick += TimerTick;
        }

        public static void CreateEdgesForWindow(Window window, Grid grid, DragMoveDelegate dragMoveDelegate)
        {
            ParentWindow = window;
            if (WindowForCommunication == null) WindowForCommunication = (ILoggable)window;
            if (grid is Grid) { } else throw new System.Exception(errorMessage);
            CreateEdges(grid, window, dragMoveDelegate);
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            //(WindowForCommunication as MainWindow).bg.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCF7F7A1"));
            long timeOnEdge = stopwatch.ElapsedMilliseconds;
            if (ColorChangingEdge != null)
            {
                ColorChangingEdge.Opacity = CountOpacity(timeOnEdge);
                WindowForCommunication.Log(timeOnEdge.ToString() + " ms spent on edge.");
            }
            if (LastTimerTick)
            {
                stopwatch.Reset();
                //ColorChangingEdge = null;
                DataForLastTick.sender.Opacity = 0.05;
                timer.Stop();
                
                if (DraggingEdgeNow)
                {
                    //DraggingEdgeNow = false;
                    //CountWindowPosition(DataForLastTick.sender, DataForLastTick.e);
                }
            }
        }

        /// <param name="timeOnEdge">ms</param>
        private static double CountOpacity(long timeOnEdge)
        {
            if (timeOnEdge > msToFullyShowEdge)
                return 1;
            else
            { double smallValue = (double)timeOnEdge / msToFullyShowEdge;
                if (smallValue < 0.05) smallValue = 0.05;
                return smallValue;
            }
        }

        /// <summary> gpt </summary>
        private static Grid FindGrid(DependencyObject parent)
        {
            if (parent == null)
                return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Grid grid)
                {
                    return grid;
                }

                // Recursively search in the children
                Grid foundGrid = FindGrid(child);
                if (foundGrid != null)
                {
                    return foundGrid;
                }
            }

            return null;
        }

        public static void CreateEdges(Grid grid, Window window, DragMoveDelegate dragMoveDelegate)
        {
            int numRows = grid.RowDefinitions.Count;
            int numCols = grid.ColumnDefinitions.Count;
            Rectangle[] edges = new Rectangle[8];

            // Create and position rectangles for edges
            edges[0] = CreateEdgeRectangle(); // top edge
            edges[1] = CreateEdgeRectangle(); // top-right corner
            edges[2] = CreateEdgeRectangle(); // right edge
            edges[3] = CreateEdgeRectangle(); // bottom-right corner
            edges[4] = CreateEdgeRectangle(); // bottom edge
            edges[5] = CreateEdgeRectangle(); // bottom-left corner
            edges[6] = CreateEdgeRectangle(); // left edge
            edges[7] = CreateEdgeRectangle(); // top-left corner

            // Set positions and sizes for edges
            SetEdgePosition(edges[0], 1, 0, numCols - 2, 1);
            SetEdgePosition(edges[1], numCols - 1, 0, 1, 1);
            SetEdgePosition(edges[2], numCols - 1, 1, 1, numRows - 2);
            SetEdgePosition(edges[3], numCols - 1, numRows - 1, 1, 1);
            SetEdgePosition(edges[4], 1, numRows - 1, numCols - 2, 1);
            SetEdgePosition(edges[5], 0, numRows - 1, 1, 1);
            SetEdgePosition(edges[6], 0, 1, 1, numRows - 2);
            SetEdgePosition(edges[7], 0, 0, 1, 1);

            // Add edges to the grid
            for (int i = 0; i < 8; i++)
            {
                var edge = edges[i];
                grid.Children.Add(edge);
                edge.Name = ((EdgeName)i).ToString();

                edge.MouseDown += EdgeMouseDown;
                edge.MouseUp += EdgeMouseUp;
                //edge.MouseDown += (edge, e) => dragMoveDelegate();
                edge.MouseDown += (edge, e) => dragMoveDelegate(edge, e);
                edge.MouseEnter += EdgeMouseEnter;
                edge.MouseLeave += EdgeMouseLeave;
            }
        }

        private static Rectangle CreateEdgeRectangle()
        {
            Rectangle currentEdge = new Rectangle
            {
                Fill = color,
                Opacity = 0.01,//fullOpacityIsOnly,
            };
            //currentEdge.PreviewMouseDown += Edge_PreviewMouseDown;//not working
            //currentEdge.MouseDown += EdgeMouseDown; //ok/2
            return currentEdge;
        }
        private static void SetEdgePosition(Rectangle edge, int column, int row, int columnSpan, int rowSpan)
        {
            Grid.SetColumn(edge, column);
            Grid.SetRow(edge, row);
            Grid.SetColumnSpan(edge, columnSpan);
            Grid.SetRowSpan(edge, rowSpan);
        }
        private static void Edge_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        private static void EdgeMouseDownOld(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            // sender.parentWindow.DragMove(); //no
            {
                DependencyObject depObj = sender as DependencyObject;
                if (depObj != null)
                {
                    Window parentWindow = Window.GetWindow(depObj);
                    if (parentWindow != null)
                    {
                        parentWindow.DragMove();
                    }
                }
            } // ok/2
        }
        private static void EdgeMouseDown(object sender, MouseButtonEventArgs e)
        {
            Log((sender as Rectangle).Name.ToString() + " EdgeMouseDown");
            if (!DraggingEdgeNow)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    DependencyObject depObj = sender as DependencyObject;
                    if (depObj != null)
                    {
                        ParentWindow = Window.GetWindow(depObj);
                        if (ParentWindow != null)
                        {
                            DraggingEdgeNow = true;
                            DraggedEdge = sender as Rectangle;
                            SaveWindowPosition(sender as Rectangle, ParentWindow, e);
                            //***ParentWindow.DragMove();
                        }
                        else { MessageBox.Show("parent window error", "edge mouse down", MessageBoxButton.OK, MessageBoxImage.Error); }
                    }
                    else { MessageBox.Show("clicked on something which doesnt exist", "edge mouse down", MessageBoxButton.OK, MessageBoxImage.Information); }
                }
                else { MessageBox.Show("wrong button", "edge mouse down", MessageBoxButton.OK, MessageBoxImage.Information); }
            }
        }
        private static void EdgeMouseUp(object sender, MouseButtonEventArgs e)
        {
            Log((sender as Rectangle).Name.ToString() + " EdgeMouseUp");
            CountWindowPosition(sender as Rectangle, e);
        }

        private static void EdgeMouseEnter(object sender, MouseEventArgs e)
        {
            Log((sender as Rectangle).Name.ToString() + " EdgeMouseEnter");
            LastTimerTick = false;
            //(WindowForCommunication as MainWindow).bg
            (sender as Rectangle).Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCFF2233"));
            stopwatch.Start();
            ColorChangingEdge = sender as Rectangle;
            timer.Start();
        }
        private static void EdgeMouseLeave(object sender, MouseEventArgs e)
        {
            Log((sender as Rectangle).Name.ToString() + " EdgeMouseLeave");
            //(WindowForCommunication as MainWindow).bg.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC22FF33"));
            DataForLastTick = new DataForLastTick(sender as Rectangle, e);
            LastTimerTick = true;

            /*
            if (sender == DraggedEdge)
            {
                LastTimerTick = true;
                DataForLastTick = new DataForLastTick(sender as Rectangle, e);
            }
            */

            /*
            stopwatch.Reset();
            CountWindowPosition(sender as Rectangle, e);
            CountWindowPosition(sender as Rectangle, e);
            moverIsFree = true;
            activeEdge = null;
            ((Rectangle)sender).Opacity = 0.01;
            */
        }
        private static void SaveWindowPosition(Rectangle sender, Window parentWindow, MouseButtonEventArgs e)
        {
            /*
            positionBefore = new WindowPosition(parentWindow.Left, parentWindow.Top, parentWindow.Width, parentWindow.Height,
                //e.GetPosition(null));
                //((UIElement)sender).PointToScreen(e.GetPosition(null)));
                MouseHooking.GetMousePosition());
            */
        }
        private static void CountWindowPosition(Rectangle edge, MouseEventArgs e)
        {
            //double x = e.GetPosition(null).X - positionBefore.MousePos.X;
            //double y = e.GetPosition(null).Y - positionBefore.MousePos.Y;
            Point MouseNow = MouseHooking.GetMousePosition();
            double x = MouseNow.X - positionBefore.MousePos.X;
            double y = MouseNow.Y - positionBefore.MousePos.Y;


            string edgeName = edge.Name;

            //if (edgeName.Contains("Top")) Top(y);
            if (edgeName.Contains("Right")) Right(x);
            //if (edgeName.Contains("Bottom")) Bottom(y);
            //if (edgeName.Contains("Left")) Left(x);
            #region trasah
            /*
            switch (edgeName)
            {
                case nameof(EdgeName.TopEdge):
                    Top(y);
                    break;
                case nameof(EdgeName.TopRightCorner):
                    // Call submethod(s) for top-right corner
                    break;
                case nameof(EdgeName.RightEdge):
                    // Call submethod(s) for right edge
                    break;
                case nameof(EdgeName.BottomRightCorner):
                    // Call submethod(s) for bottom-right corner
                    break;
                case nameof(EdgeName.BottomEdge):
                    // Call submethod(s) for bottom edge
                    break;
                case nameof(EdgeName.BottomLeftCorner):
                    // Call submethod(s) for bottom-left corner
                    break;
                case nameof(EdgeName.LeftEdge):
                    // Call submethod(s) for left edge
                    break;
                case nameof(EdgeName.TopLeftCorner):
                    // Call submethod(s) for top-left corner
                    break;
                default:
                    // Handle default case if necessary
                    break;
            }
            */
            #endregion
            static void Left(double x) { ParentWindow.Left += x; ParentWindow.Width -= x; }
            static void Right(double x) { ParentWindow.Width += x; }
            static void Top(double y) { ParentWindow.Top += y; ParentWindow.Width -= y; }
            static void Bottom(double y) { ParentWindow.Width += y; }
        }

        private static void Log(string message)
        {
            WindowForCommunication.Log(message);
        }

        private static readonly Dictionary<int, string> edgeNames = new Dictionary<int, string>
        {
            {0, "Top Edge"},
            {1, "Top-Right Corner"},
            {2, "Right Edge"},
            {3, "Bottom-Right Corner"},
            {4, "Bottom Edge"},
            {5, "Bottom-Left Corner"},
            {6, "Left Edge"},
            {7, "Top-Left Corner"}
        };
    }
    public enum EdgeName
    {
        TopEdge,
        TopRightCorner,
        RightEdge,
        BottomRightCorner,
        BottomEdge,
        BottomLeftCorner,
        LeftEdge,
        TopLeftCorner
    }
    public struct WindowPosition
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        //public int MouseLeft { get; set; }
        //public int MouseTop { get; set; }
        public Point MousePos { get; set; }

        public WindowPosition(double left, double top, double width, double height, Point mousePos)//int mouseLeft, int mouseTop)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            //MouseLeft = mouseLeft;
            //MouseTop = mouseTop;
            MousePos = mousePos;
        }
        
    }
}