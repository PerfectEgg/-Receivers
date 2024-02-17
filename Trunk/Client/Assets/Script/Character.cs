using Assets.Script.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

// ���߿� �÷��̾�, �� Ŭ���� ������.
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
        Tracking = 4,
        Spawn = 5,
        Max,
    }

    private GameObject character;
    protected SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> sprites;
    private Animator animator;

    private Dictionary<State, Action> actionHandlers;

    private CharacterType characterType;
    private State state;

    public Transform Transform;

    public Vector2Int Vector2IntPosition => new Vector2Int((int)transform.position.x, (int)transform.position.y);

    [SerializeField]
    private float speed;

    public void MoveUpdate()
    {
        // �׾��ٸ� ������ �Ұ�.
        if (state == State.Dead)
            return;

        state = State.Idle;

        // �������ٸ� �̵�.
        // �¿��� �ش� ��ġ�� �°� flipX����.
        // �������ٸ� ���� ����.

        // ������
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Move(Vector2.right, 0);
        }
        // ����
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Move(Vector2.left, 1);
        }
        // ��
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Move(Vector2.up);
        }
        // �Ʒ�
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Move(Vector2.down);
        }

        if (state == State.Idle)
            animator.SetBool("isMove", false);
    }

    public void Init(short type, GameObject gameObject, Character parent, float speed)
    {
        characterType = (CharacterType)type;
        state = characterType == CharacterType.User ? State.Idle : State.Spawn;

        character = gameObject;
        character.transform.parent = parent.transform;
        animator = character.AddComponent<Animator>();
        spriteRenderer = character.AddComponent<SpriteRenderer>();
        character.transform.position = Vector3.zero;
        Transform = character.transform;

        this.speed = speed;

        string sSprite = "";
        string sAnimator = "";

        switch(characterType)
        {
            case CharacterType.User:
                {
                    sSprite += "Undead Survivor\\Sprites\\Farmer 3";
                    sAnimator += "Undead Survivor\\Animations\\Player\\playerAnimatorController";
                }
                break;
            case CharacterType.Monster:
                {
                    sSprite += "Undead Survivor\\Sprites\\Enemy 0";
                    sAnimator += "Undead Survivor\\Animations\\Enemy\\AcEnemy 0";
                }
                break;
            default:
                {
                    Debug.Log("Character Type Error");
                }
                break;
        }

        SetAction();

        // ��������Ʈ
        sprites = new Dictionary<string, Sprite>();
        var rSprites = Resources.LoadAll<Sprite>(sSprite);
        foreach (var r in rSprites)
        {
            sprites.Add(r.name, r);
        }

        // �ִϸ�����
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(sAnimator);
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

    // flipX == 0 �������� ���� == 1 ������ ���� �� �ܿ� �Ű澲�� �ʴ´�.
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

    // ���߿� enemy������ ���� ���� �Ѵ�.
    public void EnemyUpdate()
    {
        actionHandlers[state]?.Invoke();
    }

    private void SetAction()
    {
        if (characterType == CharacterType.User)
            return;

        actionHandlers = new Dictionary<State, Action>();
        actionHandlers.Add(State.Spawn, Spwan);
        actionHandlers.Add(State.Idle, Idle);

    }

    private void Spwan()
    {
        Vector2 pos = MapManager.Instance.RandomPos();

        Vector3 tPos = new Vector3((float)pos.x, Transform.position.y, (float)pos.y);

        Transform.position = tPos;
        character.SetActive(true);

        state = State.Idle;
    }
    private void Idle()
    {

    }
}
