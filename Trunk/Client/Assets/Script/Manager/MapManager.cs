using Assets.Script.AStartPathfinder;
using Assets.Script.LazySingleton;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Script.Manager
{
    public class MapManager : LazySingleton<MapManager>
    {
        private string mapName = "Prefab\\MapPrefab\\TGrid";
        private GameObject map;

        private bool isMapChange = false;
        public bool IsMapChange => isMapChange;

        public void Init()
        {
            map = Resources.Load<GameObject>(mapName);
            isMapChange = true;
        }

        public bool Update(ref GameObject tileMap)
        {
            if (isMapChange == false)
                return false;
            if(tileMap.transform.childCount > 0)
            {
                while (tileMap.transform.childCount <= 0)
                    ObjectManager.Instance.Destroy(tileMap.transform.GetChild(0).gameObject);
            }
            var ins = GameObject.Instantiate(map, tileMap.transform);
            ins.SetActive(true);

            isMapChange = false;

            return true;
        }

        public Vector2 RandomPos(string _mapName = "")
        {
            if (_mapName.CompareTo("") == 0)
                _mapName = map.name;

            return AStarPathfinderManager.Instance.RandomPos(_mapName);
        }
    }

}
