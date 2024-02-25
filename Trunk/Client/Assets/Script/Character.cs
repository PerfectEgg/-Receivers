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

// 나중에 플레이어, 적 클래스 나누기.
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

        // 스프라이트
        sprites = new Dictionary<string, Sprite>();
        var rSprites = Resources.LoadAll<Sprite>(sSprite);
        foreach (var r in rSprites)
        {
            sprites.Add(r.name, r);
        }

        // 애니메이터
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
        if (characterType == CharacterType.User)
            animator.SetBool("isMove", true);
    }

    // 나중에 enemy용으로 따로 빼야 한다.
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
