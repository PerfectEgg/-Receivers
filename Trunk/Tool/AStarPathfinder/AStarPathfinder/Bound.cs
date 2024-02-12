using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarPathfind
{
    public class Bound
    {
        int MinX;
        int MinY;

        int MaxX;
        int MaxY;

        int SizeX;
        int SizeY;

        public Bound() { }

        public Bound(int minX, int minY, int maxX, int maxY, int sizeX, int sizeY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            SizeX = sizeX;
            SizeY = sizeY;
        }

        public void AllSet(int minX, int minY, int maxX, int maxY, int sizeX, int sizeY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            SizeX = sizeX;
            SizeY = sizeY;
        }

        public void GetMinPos(ref int minX, ref int minY)
        {
            minX = MinX;
            minY = MinY;
        }

        public void GetMaxPos(ref int maxX, ref int maxY)
        {
            maxX = MaxX;
            maxY = MaxY;
        }

        public void GetSizePos(ref int sizeX, ref int sizeY)
        {
            sizeX = SizeX;
            sizeY = SizeY;
        }
    }
}
