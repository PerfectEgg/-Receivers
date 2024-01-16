using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private Queue<GameObject> Objects;
    private GameObject originalObject;
    public void Init(int MaxObject)
    {
        Objects = new Queue<GameObject>();

        originalObject = new GameObject();
        for (int i = 0; i < MaxObject; ++i)
        {
            GameObject instance = Instantiate(originalObject);
            instance.SetActive(false);

            Objects.Enqueue(instance);
        }
    }

    public GameObject GetObject()
    { 
        if(Objects.Count > 0)
        {
            GameObject gameObject = Objects.Dequeue();
            return gameObject;
        }
        else
        {
            GameObject instance = Instantiate(originalObject);
            instance.SetActive(false);

            return instance;
        }
    }
}
