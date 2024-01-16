using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableTeat : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public bool isLoaded = false;

    public void Init(SpriteRenderer sr)
    {
        spriteRenderer = sr;
    }

    public void Load(string address)
    {
        Addressables.LoadAssetAsync<Sprite>(address).Completed +=
            (AsyncOperationHandle<Sprite> obj) =>
            {
                switch (obj.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        {
                            spriteRenderer.sprite = obj.Result;
                        }
                        break;
                    case AsyncOperationStatus.Failed:
                        {
                            Debug.Log("��������Ʈ �ε� ����");
                        }
                        break;
                }

                isLoaded = true;
            };
    }

    public void Release()
    {
        Addressables.Release(spriteRenderer);
    }

    public Sprite GetSprite()
    {
        if (spriteRenderer == null)
            return null;

        return spriteRenderer.sprite;
    }
}
