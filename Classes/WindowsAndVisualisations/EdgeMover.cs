using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CleanDisk24
{
    internal class EdgeMover
    {
        private Window parentWindow;
        private static ILoggable WindowForCommunication;
        private Grid grid;
        private string errorMessage = "This part of program didn't manage to find the GRID (<Grid x:Name=\"mainGrid\" ) or something with it is wrong.";
        private static SolidColorBrush color = new SolidColorBrush(Color.FromRgb(
            //200, 250, 100));
            200, 50, 100));
        private Rectangle[] edges;
        private DispatcherTimer timer;
        private static Stopwatch stopwatch = new Stopwatch();
        private static Rectangle activeEdge;
        private const int msToFullyShowEdge = 1000;
        private const double fullOpacityIsOnly = 0.8;

        public EdgeMover(Window window, Grid grid)
        {
            parentWindow = window;
            if (WindowForCommunication == null) WindowForCommunication = (ILoggable)window;
            //this.grid = FindGrid(window);
            this.grid = grid;
            if (grid is Grid) { } else throw new System.Exception(errorMessage);
            CreateEdges(grid);
            timer = new DispatcherTimer();
            timer.Interval = System.TimeSpan.FromMilliseconds(15);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            long timeOnEdge = stopwatch.ElapsedMilliseconds;
            if (activeEdge != null)
            {
                activeEdge.Opacity = CountOpacity(timeOnEdge);
                WindowForCommunication.Log(timeOnEdge.ToString() + " ms spent on edge.");

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeOnEdge">ms</param>
        /// <returns></returns>
        private static double CountOpacity(long timeOnEdge)
        {
            if (timeOnEdge > msToFullyShowEdge)
                return 1;
            else
                return (double)timeOnEdge / msToFullyShowEdge;
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

        public static void CreateEdges(Grid grid)
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
            foreach (var edge in edges)
            {
                grid.Children.Add(edge);
            }
        }

        private static Rectangle CreateEdgeRectangle()
        {
            Rectangle currentEdge;
            currentEdge = new Rectangle
            {
                Fill = color,
                Opacity = fullOpacityIsOnly,
            };
            currentEdge.MouseEnter += EdgeMouseEnter;
            currentEdge.MouseLeave += EdgeMouseLeave;
            currentEdge.PreviewMouseDown += Edge_PreviewMouseDown;//not working

            return currentEdge;
        }
        private static void Edge_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        private static void EdgeMouseEnter(object sender, MouseEventArgs e)
        {
            stopwatch.Start();
            activeEdge = sender as Rectangle;
        }
        private static void EdgeMouseLeave(object sender, MouseEventArgs e)
        {
            stopwatch.Reset();
            activeEdge = null;
            ((Rectangle)sender).Opacity = 0.01;
        }

        private static void SetEdgePosition(Rectangle edge, int column, int row, int columnSpan, int rowSpan)
        {
            Grid.SetColumn(edge, column);
            Grid.SetRow(edge, row);
            Grid.SetColumnSpan(edge, columnSpan);
            Grid.SetRowSpan(edge, rowSpan);
        }
    }
}