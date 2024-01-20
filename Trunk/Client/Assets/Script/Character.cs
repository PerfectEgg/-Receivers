using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    private AddressableTest AT;
    protected SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> sprites;
    private Animator animator;

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
        // 죽었다면 움직임 불가.
        if (state == State.Dead)
            return;

        state = State.Idle;

        // 움직였다면 이동.
        // 좌우라면 해당 위치에 맞게 flipX조정.
        // 움직였다면 상태 변경.

        // 오른쪽
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Transform.Translate(Vector2.right * speed * Time.deltaTime);
            spriteRenderer.flipX = false;
            Move();
        }
        // 왼쪽
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Transform.Translate(Vector2.left * speed * Time.deltaTime);
            spriteRenderer.flipX = true;
            Move();
        }
        // 위
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Transform.Translate(Vector2.up * speed * Time.deltaTime);
            Move();
        }
        // 아래
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Transform.Translate(Vector2.down * speed * Time.deltaTime);
            Move();
        }

        if (state == State.Idle)
            animator.SetBool("isMove", false);
    }

    public bool AnimatorSet()
    {
        if (animator.runtimeAnimatorController == null)
        {
            if (AT.loadCount > 0)
                return false;

            animator.runtimeAnimatorController = AT.animator;
        }

        return true;
    }

    public void Init(short type, GameObject gameObject, float speed)
    {
        GameObject ATObject = new GameObject("AT");
        AT = ATObject.AddComponent<AddressableTest>();

        characterType = (CharacterType)type;
        state = State.Idle;

        character = gameObject;
        animator = character.AddComponent<Animator>();
        spriteRenderer = character.AddComponent<SpriteRenderer>();
        character.SetActive(true);
        character.transform.position = Vector3.zero;
        Transform = character.transform;

        this.speed = speed;

        sprites = new Dictionary<string, Sprite>();

        AT.Init(sprites, animator.runtimeAnimatorController);

        string loadAddressName = null;
        string animatorName = null;

        switch(characterType)
        {
            case CharacterType.User:
                loadAddressName = "tempUser";
                animatorName = "playerAnimatorController";
                break;
            case CharacterType.Monster:
                loadAddressName = "tempMonster";
                break;
        }

        AT.Load(loadAddressName, animatorName);
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

    private void Move()
    {
        state = State.Run;
        animator.SetBool("isMove", true);
    }
}
