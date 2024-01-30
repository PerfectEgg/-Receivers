
namespace AStarPathfind
{
    public class Node
    {
        public Node(bool _isWall, bool _isMoveAbleObj, int _x, int _y)
        {
            isWall = _isWall;
            isMoveAbleObj = _isMoveAbleObj;
            x = _x;
            y = _y;
        }

        // 벽인가.
        bool isWall;
        // 움직일 수 있는 오브젝트인가.
        bool isMoveAbleObj;
        // 부모 노드(이동하게 되면 자신 이전에 가야하는 노드).
        Node parentNode;
        // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시라며 목표까지의 거리, F : G + H
        public int x, y, G, H;
        public int F { get { return G + H; } }

        public bool IsWall { get { return isWall; } }
        public bool IsMoveAbleObj { get {  return isMoveAbleObj; } }
    }
}
