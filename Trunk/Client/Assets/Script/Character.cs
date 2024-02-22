using Assets.Script.AStartPathfinder;
using Assets.Script.Manager;
using AStarPathfind;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

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
    private Character target;

    private Vector2Int destination;

    protected SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> sprites;
    private Animator animator;

    private Dictionary<State, Action> actionHandlers;

    private CharacterType characterType;
    private State state;

    public Transform Transform;
    public Vector2 Position2D => new Vector2(Transform.position.x, Transform.position.y);

    //public Vector2Int Vector2IntPosition => new Vector2Int((int)Transform.position.x, (int)Transform.position.y);
    public Vector2Int Vector2IntPosition => new Vector2Int((int)Math.Round(Transform.position.x), (int)Math.Round(Transform.position.y));
   
    private float delayTime;

    private Node nextNode;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float moveDelayTime = 2.4f;

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

        switch (characterType)
        {
            case CharacterType.User:
                {
                    sSprite += "Undead Survivor\\Sprites\\Farmer 3";
                    sAnimator += "Undead Survivor\\Animations\\Player\\playerAnimatorController";
                }
                break;
            case CharacterType.Monster:
                {
                    destination = Vector2Int.zero;
                    target = GameObject.Find("user").GetComponent<Character>();
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
        if (characterType == CharacterType.User)
            animator.SetBool("isMove", true);
    }

    // ���߿� enemy������ ���� ���� �Ѵ�.
    public void EnemyUpdate()
    {
        //for (int i = 0; i < Transform.childCount; i++)
        //{
        //    Transform.GetChild(i).GetComponent<Character>().actionHandlers[state]?.Invoke();
        //}
        actionHandlers[state]?.Invoke();
        Debug.Log(state);
    }

    private void SetAction()
    {
        if (characterType == CharacterType.User)
            return;

        actionHandlers = new Dictionary<State, Action>();
        actionHandlers.Add(State.Spawn, Spwan);
        actionHandlers.Add(State.Idle, Idle);
        actionHandlers.Add(State.Tracking, Tracking);
        actionHandlers.Add(State.Run, Run);

    }

    private void Spwan()
    {
        bool isPos = MapManager.Instance.RandomPos(out var pos);
        if (isPos == false)
            return;

        Vector3 tPos = new Vector3(pos.x, pos.y, Transform.position.z);

        Transform.position = tPos;
        character.SetActive(true);

        state = State.Idle;
    }
    private void Idle()
    {
        if (Vector2.Distance(Position2D, target.Position2D) <= 2.0f)
        {
            delayTime = 0.0f;
            state = State.Tracking;
        }
        else
        {
            delayTime += Time.deltaTime;
            if (delayTime < moveDelayTime)
                return;

            state = State.Run;

            bool isPos = MapManager.Instance.RandomPos(out var pos);
            if (isPos == false)
                return;

            destination.x = (int)pos.x;
            destination.y = (int)pos.y;

            delayTime = 0.0f;
        }
    }

    private void Tracking()
    {
        if (Vector2.Distance(Position2D, target.Position2D) > 2.0f)
            state = State.Idle;
        else
        {
            List<Node> finalNodeList = new List<Node>();
            AStarPathfinderManager.Instance.Pathfind(MapManager.Instance.MapName, Vector2IntPosition, target.Vector2IntPosition, ref finalNodeList);
            
            nextNode = finalNodeList.Count >= 2 ? finalNodeList[1] : finalNodeList[0];

            var nextPos = new Vector2((float)nextNode.x, (float)nextNode.y);
            nextPos -= Position2D;
            nextPos.Normalize();

            float angle = Vector2.Angle(Position2D, nextPos);

            short flipX;

            if (angle > 90.0f || angle < -90.0f)
                flipX = 0;
            else
                flipX = 1;

            Move(nextPos, flipX);
        }
    }

    private void Run()
    {
        if (Vector2.Distance(Position2D, target.Position2D) <= 2.0f)
        {
            delayTime = 0.0f;
            state = State.Tracking;
        }
        else
        {
            List<Node> finalNodeList = new List<Node>();
            AStarPathfinderManager.Instance.Pathfind(MapManager.Instance.MapName, Vector2IntPosition, destination, ref finalNodeList);
            
            nextNode = finalNodeList.Count >= 2 ? finalNodeList[1] : finalNodeList[0];

            var nextPos = new Vector2((float)nextNode.x, (float)nextNode.y);
            nextPos -= Position2D;
            nextPos.Normalize();

            float angle = Vector2.Angle(Position2D, nextPos);

            short flipX;

            if (angle > 90.0f || angle < -90.0f)
                flipX = 0;
            else
                flipX = 1;

            Move(nextPos, flipX);

            if(Vector2Int.Distance(destination, Vector2IntPosition) < 0.01f)
                state = State.Idle;
        }
    }
}
