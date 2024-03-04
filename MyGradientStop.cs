using System;
using System.Windows.Media;

namespace CleanDisk24
{
    /// <summary>
    /// 
    /// </summary>
    public class MyGradientStop
    {
        public readonly MyColor Color;
        /// <summary>
        /// quantity, at which the measured property reaches this Gradient color
        /// </summary>
        public readonly long Position;

        public MyGradientStop(MyColor color, long position)
        {
            Color = color;
            Position = position;
        }

        //public bool Equals(MyGradientStop comparingGradientStop)
        public override bool Equals(object comparingGradientStop)
        {
            return (
                this.Color.Equals((comparingGradientStop as MyGradientStop).Color)
                &&
                this.Position == (comparingGradientStop as MyGradientStop).Position
                );
        }

        public static bool operator ==(MyGradientStop operand1, MyGradientStop op2) => operand1.Equals(op2);
        public static bool operator !=(MyGradientStop op1, MyGradientStop op2) => !op1.Equals(op2);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MyColor
    {
        public MyColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte R { get; }
        public byte G { get; private set; }
        public byte B { get; private set; }

        public static MyColor FromRgb(byte r, byte g, byte b) => new MyColor(r, g, b);        
    }
}
