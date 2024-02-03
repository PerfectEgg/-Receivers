using Assets.Script.AStartPathfinder;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ObjectManager objectManager;
    private AStarPathfinderManager aStarPathfinderManager;

    Character user;

    private void Start()
    {
        aStarPathfinderManager = new AStarPathfinderManager();

        GameObject gameObject = new GameObject("objectManager");
        objectManager = gameObject.AddComponent<ObjectManager>();

        GameObject gameObjectUser = new GameObject("user");
        user = gameObject.AddComponent<Character>();

        objectManager.Init(50);

        user.Init(1, objectManager.GetObject(), 3.0f);

        Camera.main.GetComponent<CameraMove>().SetPlayerTransfrom(user.Transform);
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (user.AnimatorSet() == false)
            return;

        user.MoveUpdate();
    }
}
