using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanDisk24
{
    public static class StringSplitter
    {
        public static int PositionOfBackslash(string path)
        {
            int position = path.Length;
            for (int i = 1; i < path.Length; i++)
            {
                if (path[i] == '\\')
                {
                    position = i;
                    return position;
                    //break;
                }
            }
            return -0;
        }
        ///<summary>Returns FALSE if there is no Backslash in the middle. (They must be only in middle.)</summary>
        public static bool Split(ref string firstPart, ref string remainingPart, string path)
        {
            int position = PositionOfBackslash(path);
            firstPart = path.Substring(0, position);
            remainingPart = path.Substring(position + 1);
            return (position != -0);
        }

        public static string Split_GetRemainingPart(string path)
        {
            string firstDirectory = "", remainingPart = "";
            Split(ref firstDirectory, ref remainingPart, path);
            return remainingPart;
        }
    }
}
