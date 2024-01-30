using System.Collections.Generic;
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
            Move(Vector2.right, 0);
        }
        // 왼쪽
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Move(Vector2.left, 1);
        }
        // 위
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Move(Vector2.up);
        }
        // 아래
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Move(Vector2.down);
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

        switch (characterType)
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
        switch (state)
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

    private bool IsCollision(Vector2 nextPos)
    {
        return Physics2D.OverlapBox(nextPos, new Vector2(1, 1), 0.0f) == null ? false : true;
    }

    private Vector2 ToVector2(Vector3 Pos)
    {
        return new Vector2(Pos.x, Pos.y);
    }

    // flipX == 0 오른쪽을 본다 == 1 왼쪽을 본다 그 외엔 신경쓰지 않는다.
    private void Move(Vector2 destination, short flipX = 3)
    {
        Vector2 nextPos = ToVector2(Transform.position) + (destination * speed * Time.deltaTime);
        if (IsCollision(nextPos) == false)
        {
            Transform.Translate(destination * speed * Time.deltaTime);

            switch (flipX)
            {
                case 0:
                    spriteRenderer.flipX = false;
                    break;
                case 1:
                    spriteRenderer.flipX = true;
                    break;
            }
        }

        state = State.Run;
        animator.SetBool("isMove", true);
    }
}
