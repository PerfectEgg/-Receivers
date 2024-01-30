using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableTest : MonoBehaviour
{
    public Dictionary<string, Sprite> sprites;
    public RuntimeAnimatorController animator;

    public int loadCount = 0;

    public void Init(Dictionary<string, Sprite> lsprites, RuntimeAnimatorController animator)
    {
        sprites = lsprites;
        this.animator = animator;

        loadCount = 2;
    }

    public void Load(string address1, string address2)
    {
        Addressables.LoadAssetAsync<Sprite[]>(address1).Completed +=
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
                --loadCount;
            };

        Addressables.LoadAssetAsync<RuntimeAnimatorController>(address2).Completed +=
            (AsyncOperationHandle<RuntimeAnimatorController> obj) =>
            {
                switch (obj.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        {
                            animator = obj.Result;
                        }
                        break;
                    case AsyncOperationStatus.Failed:
                        {
                            Debug.Log("애니메이터 로드 실패");
                        }
                        break;
                }
                --loadCount;
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
