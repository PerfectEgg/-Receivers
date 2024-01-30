using Newtonsoft.Json.Linq;

namespace AStarPathfind
{
    public class SaveAndLoad
    {
        public static void Save(Bound bound, Node[,] node, out string saveDataString)
        {
            JObject saveData = new JObject();

            int MinX = 0;
            int MinY = 0;

            int MaxX = 0;
            int MaxY = 0;

            int SizeX = 0;
            int SizeY = 0;
            bound.GetMinPos(ref MinX, ref MinY);
            bound.GetMaxPos(ref MaxX, ref MaxY);
            bound.GetSizePos(ref SizeX, ref SizeY);

            JObject inBound = new JObject(
                new JProperty("MinX", MinX),
                new JProperty("MinY", MinY),
                new JProperty("MaxX", MaxX),
                new JProperty("MaxY", MaxY),
                new JProperty("SizeX", SizeX),
                new JProperty("SizeY", SizeY)
                );
            saveData.Add("Bound", inBound);

            JObject inNode = new JObject();
            for (int x = 0; x < SizeX; ++x)
            {
                for(int y = 0; y < SizeY; ++y)
                {
                    JObject oneNode = new JObject(
                        new JProperty("IsWall", node[x, y].IsWall),
                        new JProperty("IsMoveAbleObj", node[x, y].IsMoveAbleObj),
                        new JProperty("X", node[x, y].x),
                        new JProperty("Y", node[x, y].y)
                        );

                    inNode.Add(x + "," + y, oneNode);
                }
            }

            saveData.Add("Node", inNode);

            saveDataString = saveData.ToString();
        }

        public static void Load(string saveDataString, out Bound bound, out Node[,] nodes)
        {
            bound = null;
            nodes = null;

            JObject jRoot = JObject.Parse(saveDataString);

            JToken jBToken = jRoot["Bound"];
            int MinX = (int)jBToken["MinX"];
            int MinY = (int)jBToken["MinY"];

            int MaxX = (int)jBToken["MaxX"];
            int MaxY = (int)jBToken["MaxY"];

            int SizeX = (int)jBToken["SizeX"];
            int SizeY = (int)jBToken["SizeY"];

            bound = new Bound(MinX, MinY, MaxX, MaxY, SizeX, SizeY);

            nodes = new Node[SizeX, SizeY];

            JToken jNToken = jRoot["Node"];
            for (int x = 0; x < SizeX; ++x)
            {
                for (int y = 0; y < SizeY; ++y)
                {
                    JToken jInNToken = jNToken[x + "," + y];
                    nodes[x, y] = new Node((bool)jInNToken["IsWall"], (bool)jInNToken["IsMoveAbleObj"], (int)jInNToken["X"], (int)jInNToken["Y"]);
                }
            }

        }
    }
}
