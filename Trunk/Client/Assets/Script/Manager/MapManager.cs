using Assets.Script.AStartPathfinder;
using Assets.Script.LazySingleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Script.Manager
{
    public class MapManager : LazySingleton<MapManager>
    {
        private string mapName = "TGrid";
        private GameObject map;

        private bool isMapChange = false;
        public bool IsMapChange => isMapChange;

        public void Init()
        {
            Addressables.LoadAssetAsync<GameObject>(mapName).Completed +=
            (AsyncOperationHandle<GameObject> obj) =>
            {
                switch (obj.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        {
                            map = UnityEngine.Object.Instantiate(obj.Result);
                            isMapChange = true;
                        }
                        break;
                    case AsyncOperationStatus.Failed:
                        {
                            Debug.Log("맵 로드 실패");
                        }
                        break;
                }
            };
        }

        public bool Update(ref GameObject tileMap)
        {
            if (isMapChange == false)
                return false;
            ObjectManager.Instance.Destroy(tileMap);
            tileMap = map;

            isMapChange = false;

            return true;
        }
    }


}
