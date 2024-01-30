
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


    }

}