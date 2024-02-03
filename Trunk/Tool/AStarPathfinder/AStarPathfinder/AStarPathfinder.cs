
using UnityEngine;

namespace AStarPathfind
{
    public class AStarPathfinder
    {
        public Vector2Int bottomLeft, topRight, startPos, targetPos;
        public List<Node> FinalNodeList;
        public bool allowDigonal, dontCrossCorner;

        int sizeX, sizeY;
        Node[,] NodeArray;
        Node StartNode, TargetNode, CurNode;
        List<Node> OpentList, ClosedList;

        public AStarPathfinder(Bound bound, Node[,] nodeArray)
        {
            int x = 0, y = 0;
            bound.GetMinPos(ref x, ref y);
            this.bottomLeft = new Vector2Int(x, y);

            bound.GetMaxPos(ref x, ref y);
            this.topRight = new Vector2Int(x, y);

            bound.GetSizePos(ref sizeX, ref sizeY);

            NodeArray = new Node[sizeX, sizeY];

            for (int indexX = 0; indexX < sizeX; ++indexX)
            {
                for(int indexY = 0; indexY < sizeY; ++indexY)
                {
                    NodeArray[indexX, indexY] = nodeArray[indexX, indexY];
                }
            }
        }
    }

}