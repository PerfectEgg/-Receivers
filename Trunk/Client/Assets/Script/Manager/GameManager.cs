using Assets.Script.AStartPathfinder;
using Assets.Script.Manager;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private AStarPathfinderManager aStarPathfinderManager;
    Character user;
    Character enemy;
    private GameObject map;


    private void Start()
    {
        aStarPathfinderManager = new AStarPathfinderManager();

        GameObject gameObject = new GameObject("objectManager");

        ObjectManager.Instance.Init(50);

        user = ObjectManager.Instance.GetObject("user").AddComponent<Character>();
        user.Init(1, ObjectManager.Instance.GetObject("player"), user, 3.0f);

        enemy = ObjectManager.Instance.GetObject("enemy").AddComponent<Character>();
        enemy.Init(2, ObjectManager.Instance.GetObject("nomal"), enemy, 2.0f);

        Camera.main.GetComponent<CameraMove>().SetPlayerTransfrom(user.Transform);

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
        user.MoveUpdate();
        enemy.EnemyUpdate();
    }
}
