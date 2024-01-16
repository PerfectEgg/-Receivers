using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using UnityEngine;

public class Character : MonoBehaviour
{
    enum CharacterType : short
    {
        None = 0,
        User = 1,
        Monster = 2,
        Max,
    }
    private GameObject character;
    private AddressableTeat AT;
    protected SpriteRenderer spriteRenderer;
    private CharacterType characterType;

    public Transform Transform;

    [SerializeField]
    private float speed;

    ~Character()
    {
        AT.Release();

    }

    public void MoveUpdate()
    {
        // 오른쪽
        if(Input.GetKey(KeyCode.RightArrow))
        {
            Transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        // 왼쪽
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
        // 위
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
        // 아래
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

    public void Init(short type, GameObject gameObject, float speed)
    {
        GameObject ATObject = new GameObject("AT");
        AT = ATObject.AddComponent<AddressableTeat>();

        characterType = (CharacterType)type;
        character = gameObject;
        spriteRenderer = character.AddComponent<SpriteRenderer>();
        character.SetActive(true);
        character.transform.position = Vector3.zero;
        Transform = character.transform;

        this.speed = speed;

        AT.Init(spriteRenderer);

        string loadAddressName = null;

        switch(characterType)
        {
            case CharacterType.User:
                loadAddressName = "tempUser";
                break;
            case CharacterType.Monster:
                loadAddressName = "tempMonster";
                break;
        }

        AT.Load(loadAddressName);
    }

    public IEnumerator SetRenderer()
    {
        while (AT.isLoaded == false)
            yield return null;
    }
}
