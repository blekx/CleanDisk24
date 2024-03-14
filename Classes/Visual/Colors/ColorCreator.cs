using System.Linq;
using System.Windows.Media;

namespace CleanDisk24.Classes.Visual.Colors
{
    public static class ColorCreator
    {
        const int k = 1024;
        const int M = k * k;
        const int G = k * k * k;
        /*
        const int darkGreen = k;
        const int yellow = 10 * k;
        const int red = 10 * M;
        */
        /*
        private static LinearGradientBrush SizeColor = new LinearGradientBrush(new GradientStopCollection() {
            // 0
            new GradientStop(Color.FromRgb(0, 30,0), 0), 
            // dark Green
            new GradientStop(Color.FromRgb(0,127,0), k), 
            // Yellow
            new GradientStop(Color.FromRgb(127,127,0), k),  
            // Red
            new GradientStop(Color.FromRgb(127,0,0), k),  
            // Blue
            new GradientStop(Color.FromRgb(0,0,127), k),
            // Light Blue
            new GradientStop(Color.FromRgb(0,127,255), 10*G),
        });
        */

        //private const Color c = Color.FromRgb(10, 20, 100);
        private static readonly MyGradientStop[] SortedSizeColors = new MyGradientStop[]
        {            
            // 0
            new MyGradientStop(MyColor.FromRgb(0, 30,0), 0), 
            // dark Green      
            new MyGradientStop(MyColor.FromRgb(0,127,0), k), 
            // Yellow          
            new MyGradientStop(MyColor.FromRgb(127,127,0), 100*k),  
            // Red             
            new MyGradientStop(MyColor.FromRgb(255,0,0), 10*M),
            // Purple          
            new MyGradientStop(MyColor.FromRgb(127,0,127), 100*M),
            // Blue            
            new MyGradientStop(MyColor.FromRgb(0,0,127), 1*G),
            // Light Blue      
            new MyGradientStop(MyColor.FromRgb(0,127,255), (long)10*G),
            // Light Blue End  
            new MyGradientStop(MyColor.FromRgb(0,255,255), (long)200*G),
        };
        private static readonly MyGradientStop[] ProgressColors2 = new MyGradientStop[]
        {
            new MyGradientStop(MyColor.FromRgb(255,0,0), 0),
            new MyGradientStop(MyColor.FromRgb(160,0,0), 12),
            new MyGradientStop(MyColor.FromRgb(127,30,0), 25),
            new MyGradientStop(MyColor.FromRgb(191,150,0),38),
            new MyGradientStop(MyColor.FromRgb(191,191,0),50),
            new MyGradientStop(MyColor.FromRgb(150,191,0),62),
            new MyGradientStop(MyColor.FromRgb(30,127,0), 75),
            new MyGradientStop(MyColor.FromRgb(0,160,0), 88),
            new MyGradientStop(MyColor.FromRgb(0,255,0), 100),
        };
        private static readonly MyGradientStop[] ProgressColors = new MyGradientStop[]
        {
            new MyGradientStop(MyColor.FromRgb(255,0,0), 0),
            new MyGradientStop(MyColor.FromRgb(127,30,0), 25),
            new MyGradientStop(MyColor.FromRgb(191,191,0),50),
            new MyGradientStop(MyColor.FromRgb(30,127,0), 75),
            new MyGradientStop(MyColor.FromRgb(0,255,0), 100),
        };

        public static SolidColorBrush BrushForProgress(int progress) => new SolidColorBrush(ColorForProgress(progress));
        private static Color ColorForProgress(int progress) => CreateColor(ProgressColors, progress);

        public static SolidColorBrush BrushForSize(long sizeInBytes) => new SolidColorBrush(ColorForSize(sizeInBytes));
        private static Color ColorForSize(long sizeInBytes) => CreateColor(SortedSizeColors, sizeInBytes);

        /// <summary>Returns parameter T between 0 and 1, or just 1 to avoid error.</summary>
        /// <param name="visualisedValue"> parameter between L and H </param>
        /// <!--
        /// Whenever parameter (sizeInBytes) is out of range =>
        /// automatically l & h must be both the same gradient stop.
        /// manually we set t: t=1;
        /// -->
        private static Color CreateColor(MyGradientStop[] arrayGS, long visualisedValue)
        {
            byte r, g, b;

            MyGradientStop lowGradientPoint = arrayGS.LastOrDefault(point => point.Position <= visualisedValue)
                ?? arrayGS[0];
            MyGradientStop highGradientPoint = arrayGS.FirstOrDefault(point => point.Position >= visualisedValue)
                ?? arrayGS[arrayGS.Length - 1];

            long l = lowGradientPoint.Position;  // t = 0
            long h = highGradientPoint.Position; // t = 1
            double t = CheckRangeAndCountParameter(visualisedValue, l, h);

            byte B = 255;
            r = (byte)((1 - t) * lowGradientPoint.Color.R + t * highGradientPoint.Color.R);
            g = (byte)((1 - t) * lowGradientPoint.Color.G + t * highGradientPoint.Color.G);
            b = (byte)((1 - t) * lowGradientPoint.Color.B + t * highGradientPoint.Color.B);

            return Color.FromRgb(r, g, b);
        }

        private static double CheckRangeAndCountParameter(long visualisedValue, long l, long h)
        {
            return h == l ? 1 :
                (double)(visualisedValue - l) / (h - l);
        }


        public static LinearGradientBrush GradientBrush_Progress => Give_GradientBrush(ProgressColors);
        public static LinearGradientBrush GradientBrush_Size => Give_GradientBrush(SortedSizeColors);

        private static LinearGradientBrush Give_GradientBrush(MyGradientStop[] arrayOfColors)
        {
            int lenght = arrayOfColors.Count();
            GradientStopCollection gsc = new GradientStopCollection();
            for (int i = 0; i < lenght; i++)
            {
                MyColor mc = arrayOfColors[i].Color;
                Color c = Color.FromRgb(mc.R, mc.G, mc.B);
                gsc.Add(new GradientStop(c, 1 / (double)(lenght - 1) * i));
            }
            return new LinearGradientBrush(gsc, 0.0);
        }
        /*{
            GradientStopCollection gsc = new GradientStopCollection();
            int lenght = ProgressColors.Count();
            //foreach (MyGradientStop mgs in ProgressColors) gsc.Add(new GradientStop( mgs);
            for (int i = 0; i < lenght; i++)
            {
                gsc.Add(new GradientStop(ProgressColors[i].Color, 1 / (double)(lenght - 1) * i));
            }
            LinearGradientBrush br = new LinearGradientBrush(gsc, 0.0);
            return br;
        }*/
    }
}
