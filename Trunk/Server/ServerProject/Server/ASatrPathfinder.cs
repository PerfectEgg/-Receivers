using AStarPathfind;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Server
{
    public class ASatrPathfinder
    {
        public static ASatrPathfinder Instance { get; } = new ASatrPathfinder();

        string path = "../../../../../../../Data/MapData";
        Dictionary<string, AStarPathfinder> dicAStarts = new Dictionary<string, AStarPathfinder>();

        public ASatrPathfinder()
        {
            Load();
        }

        private void Load()
        {
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (var file in di.GetFiles())
            {
                var fullName = file.Name;

                var extensionIndex = fullName.LastIndexOf('.');

                var extention = fullName.Substring(extensionIndex + 1, fullName.Length - (extensionIndex + 1));


                if (extention.CompareTo("json") != 0)
                    continue;

                var key = fullName.Substring(0, extensionIndex);

                string jsonData = File.ReadAllText(file.FullName);

                SaveAndLoad.Load(jsonData, out var bound, out var nodes);

                AStarPathfinder aStarPathfinder = new AStarPathfinder(bound, nodes);

                dicAStarts.Add(key, aStarPathfinder);
            } // end of foreach(var file in di.GetFiles())
        }



        public bool Pathfind(string key, Vector2Int stratPos, Vector2Int endPos, ref List<Node> finalNodeList)
        {
            if (!dicAStarts.TryGetValue(key, out var map))
                return false;

            map.PathFinding(stratPos, endPos, ref finalNodeList);

            return true;
        }

        public Vector2 RandomPos(string key)
        {
            var result = Vector2Int.zero;
            if (!dicAStarts.TryGetValue(key, out var map))
                return result;

            do
            {
                int x = UnityEngine.Random.Range(map.topRight.x, map.bottomLeft.x);
                int y = UnityEngine.Random.Range(map.topRight.y, map.bottomLeft.y);

                result = new Vector2Int(x, y);

            } while (map.IsCollision(result, true) == false);

            return new Vector2(result.x, result.y);
        }

        public string GetNameToIndex(int index)
        {
            return dicAStarts.Keys.ElementAt(index);
        }
    }
}
