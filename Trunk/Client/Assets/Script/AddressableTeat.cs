using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableTeat : MonoBehaviour
{
    public Dictionary<string, Sprite> sprites;

    public bool isLoaded = false;

    public void Init(Dictionary<string, Sprite> lsprites)
    {
        sprites = lsprites;
    }

    public void Load(string address)
    {
        Addressables.LoadAssetAsync<Sprite[]>(address).Completed +=
            (AsyncOperationHandle<Sprite[]> obj) =>
            {
                switch (obj.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        {
                            for (int i = 0; i < obj.Result.Length; i++)
                            {
                                sprites.Add(obj.Result[i].name, obj.Result[i]);
                            }
                        }
                        break;
                    case AsyncOperationStatus.Failed:
                        {
                            Debug.Log("스프라이트 로드 실패");
                        }
                        break;
                }

                isLoaded = true;
            };
    }

    public void Release()
    {
        Addressables.Release(sprites);
    }

    public Sprite GetSprite()
    {
        if (sprites == null)
            return null;

        return sprites.First().Value;
    }
}
