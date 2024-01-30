using UnityEditor;
using UnityEngine;
using AStarPathfind;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.Mathematics;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

namespace Assets.Script.WindowEditor
{
    public class TileMapExporter : EditorWindow
    {
        [MenuItem("DearMyBrother/TileMapExport")]
        static void Init()
        {
            TileMapExporter tileMapExporter = (TileMapExporter)EditorWindow.GetWindow(typeof(TileMapExporter));
            tileMapExporter.Show();
        }

        string saveAddress = Application.dataPath + "\\Data\\MapData\\";
        string saveDataAddress = Application.dataPath + "\\Resources\\Prefab\\MapPrefab";
        string saveFormat = "json";
        private void OnGUI()
        {
            GUILayout.Label("----- 저장 주소 -----");
            saveAddress = EditorGUILayout.TextField("주소", saveAddress);
            GUILayout.Label("----- 저장 포멧 -----");
            saveFormat = EditorGUILayout.TextField("포멧", saveFormat);

            GUILayout.Label("----- 해당 Scene에 있는 MapData를 Export합니다. -----");
            if (GUILayout.Button("Export"))
            {
                if(GetTile() == false)
                    return;
            }
        }

        private bool GetTile()
        {
            DirectoryInfo di = new DirectoryInfo(saveDataAddress);

            foreach (FileInfo file in di.GetFiles())
            {
                string fileName = file.Name;
                if (fileName.Contains(".meta") == true)
                    continue;
                string saveName = fileName.Substring(0, fileName.LastIndexOf(".", fileName.Length - 1, fileName.Length));

                Grid grid = Resources.Load<GameObject>("Prefab\\MapPrefab\\" + saveName).GetComponent<Grid>();

                if (grid == null)
                {
                    EditorUtility.DisplayDialog("실패", "Grid 객체를 확인할 수 없습니다.", "확인");
                    return false;
                }

                Tilemap[] tiles = grid.GetComponentsInChildren<Tilemap>();
                Tilemap SaveTileMap = null;
                foreach(Tilemap tile in tiles)
                {
                    if(tile.name == "WallTilemap")
                    {
                        SaveTileMap = tile;
                        break;
                    }
                }

                if(SaveTileMap == null)
                {
                    EditorUtility.DisplayDialog("실패", "WallTilemap 객체를 확인할 수 없습니다.", "확인");
                    return false;
                }

                // bound
                AStarPathfind.Bound bound = new AStarPathfind.Bound(SaveTileMap.cellBounds.min.x, SaveTileMap.cellBounds.min.y,
                    SaveTileMap.cellBounds.max.x, SaveTileMap.cellBounds.max.y,
                    SaveTileMap.cellBounds.size.x, SaveTileMap.cellBounds.size.y);

                // node
                AStarPathfind.Node[,] nodes = new AStarPathfind.Node[SaveTileMap.cellBounds.size.x, SaveTileMap.cellBounds.size.y];

                for (int i = 0; i < SaveTileMap.cellBounds.size.x; ++i)
                {
                    for (int j = 0; j < SaveTileMap.cellBounds.size.y; ++j)
                    {
                        bool isWall = false;
                        foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + SaveTileMap.cellBounds.min.x, j + SaveTileMap.cellBounds.min.y), 0.4f))
                            if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                        nodes[i, j] = new AStarPathfind.Node(isWall, false, i + SaveTileMap.cellBounds.min.x, j + SaveTileMap.cellBounds.min.y);
                    }
                }

                string saveData;
                SaveAndLoad.Save(bound, nodes, out saveData);

                int c1 = 0;
                foreach (Node n in nodes)
                {
                    if (n.IsWall == true)
                        c1++;
                }

                Debug.Log("Befor GridName : " + fileName + " WallCount : " + c1);

                File.WriteAllText(saveAddress + saveName + "." + saveFormat, saveData);

                string str = null;
                using (StreamReader sr = new System.IO.StreamReader(saveAddress + saveName + "." + saveFormat))
                {
                    str = sr.ReadToEnd();
                    sr.Close();
                }

                AStarPathfind.Bound tb = null;
                AStarPathfind.Node[,] tn = null;
                SaveAndLoad.Load(str, out tb, out tn);

                int c2 = 0;
                foreach(Node n in tn)
                {
                    if (n.IsWall == true)
                        c2++;
                }

                Debug.Log("After GridName : " + fileName + " WallCount : " + c2);
            }

            return true;
        }
    }
}
