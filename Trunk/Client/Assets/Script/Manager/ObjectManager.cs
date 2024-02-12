using Assets.Script.LazySingleton;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : LazySingleton<ObjectManager>
{
    private Queue<GameObject> Objects;
    private GameObject originalObject;
    public void Init(int MaxObject)
    {
        Objects = new Queue<GameObject>();

        originalObject = new GameObject();
        for (int i = 0; i < MaxObject; ++i)
        {
            GameObject instance = Object.Instantiate(originalObject);
            instance.SetActive(false);

            Objects.Enqueue(instance);
        }
    }

    public GameObject GetObject(string name = null)
    {
        if (Objects.Count > 0)
        {
            GameObject gameObject = Objects.Dequeue();

            if(name != null)
                gameObject.name = name;

            gameObject.SetActive(true);

            return gameObject;
        }
        else
        {
            GameObject instance = Object.Instantiate(originalObject);
            instance.SetActive(false);

            return instance;
        }
    }

    public void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);
        Object.Destroy(gameObject);
    }
}
