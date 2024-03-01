using Assets.Script.Manager;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoButtenEvent : MonoBehaviour
{
    TMP_InputField _textMeshPro;
    private readonly string saveDataAddress = Application.dataPath + "/Resources/Prefab/MapPrefab";
    // Start is called before the first frame update
    void Awake()
    {
        _textMeshPro = GameObject.Find("MapName").GetComponent<TMP_InputField>();
        //GameObject mapObject = GameObject.Find("MapName");

        //Component[] components = mapObject.GetComponents(typeof(Component));
        //foreach (Component component in components)
        //{
        //    Debug.Log(component.ToString());
        //}
    }

    public void IntoMap()
    {
        var mapName = _textMeshPro.text;
        if (mapName.CompareTo("") == 0)
        {
            Debug.Log("map name is null or empty");
            return;
        }

        DirectoryInfo di = new DirectoryInfo(saveDataAddress);
        foreach (FileInfo file in di.GetFiles())
        {
            string fileName = file.Name;
            if (fileName.Contains(".meta") == true)
                continue;

            if (fileName.Contains(mapName) == false)
                continue;

            string saveName = fileName.Substring(0, fileName.LastIndexOf(".", fileName.Length - 1, fileName.Length));

            string ChangeMapName = "Prefab\\MapPrefab\\" + saveName;

            MapManager.Instance.SetMapName(ChangeMapName);

            SceneManager.LoadScene("SampleScene");

            return;
        }

        Debug.Log("map name is null or empty");
    }
}
