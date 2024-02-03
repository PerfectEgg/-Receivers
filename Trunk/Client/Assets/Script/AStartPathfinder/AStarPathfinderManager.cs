using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Script.LazySingleton;
using AStarPathfind;


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
    }
}
