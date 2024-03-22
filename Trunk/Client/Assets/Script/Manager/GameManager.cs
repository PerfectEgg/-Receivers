using Assets.Script.AStartPathfinder;
using Assets.Script.Manager;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private AStarPathfinderManager aStarPathfinderManager;
    Character user;
    public Character enemy;
    Character another_user;

    private GameObject map;

    private int enemyId = 0;

    private void Start()
    {
        aStarPathfinderManager = new AStarPathfinderManager();

        GameObject gameObject = new GameObject("objectManager");

        ObjectManager.Instance.Init(50);

        //user = ObjectManager.Instance.GetObject("user").AddComponent<Character>();
        //user.Init(1, ObjectManager.Instance.GetObject("player"), user, 3.0f);

        enemy = ObjectManager.Instance.GetObject("enemy").AddComponent<Character>();
        enemy.Init(2, ObjectManager.Instance.GetObject("nomal"), enemy, 2.0f, Vector2.zero, enemyId++);

        //another_user = ObjectManager.Instance.GetObject("another_user").AddComponent<Character>();
        //another_user.Init(3, ObjectManager.Instance.GetObject("player"), another_user, 3.0f);

        //Camera.main.GetComponent<CameraMove>().SetPlayerTransfrom(user.Transform);

        map = ObjectManager.Instance.GetObject("tileMap");
        MapManager.Instance.Init();
    }

    private void Update()
    {
        if (MapManager.Instance.Update(ref map) == true)
            Camera.main.GetComponent<CameraMove>().SetTileData(map);
    }

    private void FixedUpdate()
    {
        if (user != null) user.MoveUpdate();
        if (enemy != null) enemy.EnemyUpdate();
        if (another_user != null) another_user.anotherMoveUpdate();
    }

    public Character MakeMyPlayer(Vector2 pos ,int playerId)
    {
        user = ObjectManager.Instance.GetObject("user").AddComponent<Character>();
        user.Init(1, ObjectManager.Instance.GetObject("player"), user, 3.0f, pos, playerId);
        return user;
    }

    public Character MakeMyAnotherPlayer(Vector2 pos, int playerId)
    {
        another_user = ObjectManager.Instance.GetObject("another_user").AddComponent<Character>();
        another_user.Init(1, ObjectManager.Instance.GetObject("player"), another_user, 3.0f, pos, playerId);
        return another_user;
    }
}
