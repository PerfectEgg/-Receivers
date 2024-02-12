
using UnityEngine;

namespace AStarPathfind
{
    public class AStarPathfinder
    {
        public Vector2Int bottomLeft, topRight, startPos, targetPos;
        public List<Node> finalNodeList;
        public bool allowDigonal, dontCrossCorner;

        int sizeX, sizeY;
        Node[,] nodeArray;
        Node startNode, targetNode, curNode;
        List<Node> openList, closedList;

        public AStarPathfinder(Bound _bound, Node[,] _nodeArray)
        {
            int x = 0, y = 0;
            _bound.GetMinPos(ref x, ref y);
            this.bottomLeft = new Vector2Int(x, y);

            _bound.GetMaxPos(ref x, ref y);
            this.topRight = new Vector2Int(x, y);

            _bound.GetSizePos(ref sizeX, ref sizeY);

            this.nodeArray = new Node[sizeX, sizeY];

            for (int indexX = 0; indexX < sizeX; ++indexX)
            {
                for(int indexY = 0; indexY < sizeY; ++indexY)
                {
                    this.nodeArray[indexX, indexY] = _nodeArray[indexX, indexY];
                }
            }

            startPos = new Vector2Int();
            targetPos = new Vector2Int();

            openList = new List<Node>() ;
            closedList = new List<Node>();
            finalNodeList = new List<Node>();

            startNode = targetNode = curNode = nodeArray[0, 0];
        }

        public void PathFinding(Vector2Int _stratPos, Vector2Int _endPos, ref List<Node> path)
        {
            if (!IsInSize(_stratPos) || !IsInSize(_endPos))
                return;

            startNode = nodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
            targetNode = nodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

            openList.Clear();
            openList.Add(startNode);

            closedList.Clear();
            //finalNodeList.Clear();

            while (openList.Count > 0)
            {
                curNode = openList[0];
                for (int i = 1; i < openList.Count; ++i)
                {
                    if (openList[i].F <= curNode.F && openList[i].H >= curNode.H)
                        curNode = openList[i];
                }

                openList.Remove(curNode);
                closedList.Add(curNode);

                if (curNode == targetNode)
                {
                    Node TargetCurNode = targetNode;
                    while (TargetCurNode != startNode)
                    {
                        //finalNodeList.Add(TargetCurNode);
                        path.Add(TargetCurNode);

                        if (TargetCurNode.parentNode != null)
                            TargetCurNode = TargetCurNode.parentNode;
                    }
                    //finalNodeList.Add(startNode);
                    //finalNodeList.Reverse();
                    path.Add(startNode);
                    path.Reverse();

                    //for (int i = 0; i < FinalNodeList.Count; ++i)
                    //{
                    //    print(i + "번째는 " + FinalNodeList[i].x + "," + FinalNodeList[i].y);
                    //}

                    return;
                }

                // ↗↖↙↘
                if (allowDigonal)
                {
                    OpenListAdd(curNode.x + 1, curNode.y + 1);
                    OpenListAdd(curNode.x - 1, curNode.y + 1);
                    OpenListAdd(curNode.x - 1, curNode.y - 1);
                    OpenListAdd(curNode.x + 1, curNode.y - 1);
                }

                // ↑→↓←
                OpenListAdd(curNode.x, curNode.y + 1);
                OpenListAdd(curNode.x + 1, curNode.y);
                OpenListAdd(curNode.x, curNode.y - 1);
                OpenListAdd(curNode.x - 1, curNode.y);
            }
        }

        void OpenListAdd(int checkX, int checkY)
        {
            if (checkX >= bottomLeft.x && checkX < topRight.x + 1 &&
                checkY >= bottomLeft.y && checkY < topRight.y + 1 &&
                !nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].IsWall &&
                !closedList.Contains(nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
            {
                if (allowDigonal)
                {
                    if (nodeArray[curNode.x - bottomLeft.x, checkY - bottomLeft.y].IsWall && nodeArray[checkX - bottomLeft.x, curNode.y - bottomLeft.y].IsWall)
                        return;
                }

                if (dontCrossCorner)
                {
                    if (nodeArray[curNode.x - bottomLeft.x, checkY - bottomLeft.y].IsWall || nodeArray[checkX - bottomLeft.x, curNode.y - bottomLeft.y].IsWall)
                        return;
                }

                Node NeighbourNode = nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
                int MoveCost = curNode.G + (curNode.x - checkX == 0 || curNode.y - checkY == 0 ? 10 : 14);

                if (MoveCost < NeighbourNode.G || !openList.Contains(NeighbourNode))
                {
                    NeighbourNode.G = MoveCost;
                    NeighbourNode.H = (Mathf.Abs(NeighbourNode.x - targetNode.x) + Mathf.Abs(NeighbourNode.y - targetNode.y)) * 10;
                    NeighbourNode.parentNode = curNode;

                    openList.Add(NeighbourNode);
                }
            }
        }

        //private void OnDrawGizmos()
        //{
        //    if (FinalNodeList.Count != 0)
        //    {
        //        for (int i = 0; i < FinalNodeList.Count - 1; ++i)
        //        {
        //            Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
        //        }
        //    }
        //}

        private bool IsInSize(Vector2Int checkPos)
        {
            if (checkPos.x < bottomLeft.x || checkPos.x > topRight.x)
                return false;
            if(checkPos.y < bottomLeft.y || checkPos.y > topRight.y)
                return false;

            return true;
        }
    }

}