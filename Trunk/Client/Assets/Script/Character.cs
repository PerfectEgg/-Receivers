using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum CharacterType : short
    {
        None = 0,
        User = 1,
        Monster = 2,
        Max,
    }

    public enum State : short
    {
        None = 0,
        Idle = 1,
        Run = 2,
        Dead = 3,
        Max,
    }

    private GameObject character;
    private AddressableTeat AT;
    protected SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> sprites;
    private int spriteNum = 0;

    private CharacterType characterType;
    private State state;

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

    public void SpriteSet()
    {
        string key = StateKey(state);

        if (key == "error")
            return;

        ++spriteNum;

        key += " " + spriteNum;

        Sprite value = null;
        if(sprites.TryGetValue(key, out value) == false)
        {
            key = key[..^1];
            key += 0;
            spriteNum = 0;
            sprites.TryGetValue(key, out value);
        }


        spriteRenderer.sprite = value;
    }

    public void Init(short type, GameObject gameObject, float speed)
    {
        GameObject ATObject = new GameObject("AT");
        AT = ATObject.AddComponent<AddressableTeat>();

        characterType = (CharacterType)type;
        state = State.Idle;

        character = gameObject;
        spriteRenderer = character.AddComponent<SpriteRenderer>();
        character.SetActive(true);
        character.transform.position = Vector3.zero;
        Transform = character.transform;

        this.speed = speed;

        sprites = new Dictionary<string, Sprite>();

        AT.Init(sprites);

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

        string key = StateKey(state);

        if (key == "error")
            return;

        key += " " + spriteNum;

        Sprite value = null;
        sprites.TryGetValue(key, out value);

        spriteRenderer.sprite = value;
    }

    public IEnumerator SetRenderer()
    {
        while (AT.isLoaded == false)
            yield return null;
    }
    public string StateKey(State state)
    {
        switch(state)
        {
            case State.Idle:
                return "Stand";
            case State.Run:
                return "Run";
            case State.Dead:
                return "Dead";
        }

        return "error";
    }
}
