using System;
using System.Collections.Generic;
using System.IO;
using Assets.Script.LazySingleton;
using AStarPathfind;
using UnityEngine;


namespace Assets.Script.AStartPathfinder
{
    public class AStarPathfinderManager : LazySingleton<AStarPathfinderManager>
    {
        Dictionary<string, AStarPathfinder> dicAStarts;

        private readonly string path;
        public AStarPathfinderManager()
        {
            dicAStarts = new Dictionary<string, AStarPathfinder>();

            var p = UnityEngine.Application.dataPath;
            path = p + "/Data/MapData/";

            Load();
        }

        private void Load()
        {
            DirectoryInfo di = new DirectoryInfo(path);

            foreach(var file in di.GetFiles())
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

        public bool GetMapMinMaxPos(string key, out int minX, out int minY, out int maxX, out int maxY)
        {
            minX = 0;
            minY = 0;
            maxX = 0;
            maxY = 0;

            if (!dicAStarts.TryGetValue(key, out var map))
                return false;

            minX = map.bottomLeft.x;
            minY = map.bottomLeft.y;
            maxX = map.topRight.x;
            maxY = map.topRight.y;

            return true;
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
    }
}
