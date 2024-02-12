using Assets.Script.AStartPathfinder;
using Assets.Script.Manager;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private AStarPathfinderManager aStarPathfinderManager;
    Character user;
    private GameObject map;


    private void Start()
    {
        aStarPathfinderManager = new AStarPathfinderManager();

        GameObject gameObject = new GameObject("objectManager");

        ObjectManager.Instance.Init(50);

        user = ObjectManager.Instance.GetObject("user").AddComponent<Character>();
        user.Init(1, ObjectManager.Instance.GetObject("player"), user, 3.0f);

        Camera.main.GetComponent<CameraMove>().SetPlayerTransfrom(user.Transform);

        map = ObjectManager.Instance.GetObject("tileMap");
        MapManager.Instance.Init();
    }

    private void Update()
    {
        if (MapManager.Instance.Update(ref map) == true)
            Camera.main.GetComponent<CameraMove>().SetTileData();
    }

    private void FixedUpdate()
    {
        if (user.AnimatorSet() == false)
            return;

        user.MoveUpdate();
    }
}
