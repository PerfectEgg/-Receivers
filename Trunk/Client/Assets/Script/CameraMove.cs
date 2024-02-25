using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMove : MonoBehaviour
{
    private Transform playerTransform;
    private Camera mainCamera;

    private Vector2 moveMin;
    private Vector2 moveMax;
    private Vector2 mapSize;

    private float height;
    private float width;
    private float cameraMoveSpeed = 0.5f;

    private bool isInTilemap = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void SetTileData(GameObject tileMap)
    {
        Transform tile = null;

        Transform[] allChildren = tileMap.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if(child.name == "WallTilemap")
            {
                tile = child;
                break;
            }
        }

        if (tile == null)
        {
            return;
        }

        Tilemap t = tile.GetComponent<Tilemap>();

        var min = t.cellBounds.min;
        moveMin = new Vector2(min.x, min.y);
        var max = t.cellBounds.max;
        moveMax = new Vector2(max.x, max.y);
        var size = t.cellBounds.size;
        mapSize = new Vector2(size.x, size.y);

        height = mainCamera.orthographicSize;
        width = height * Screen.width / Screen.height;

        isInTilemap = true;
    }

    void FixedUpdate()
    {
        LimitCameraArea();
    }

    public void SetPlayerTransfrom(Transform transform)
    {
        if (this.playerTransform == null)
            this.playerTransform = transform;
    }

    private void LimitCameraArea()
    {
        if (isInTilemap == false)
            return;

        transform.position = Vector3.Lerp(transform.position,
            playerTransform.position, cameraMoveSpeed);

        float lx = (mapSize.x * 0.5f) - width;
        float clampX = Mathf.Clamp(transform.position.x, -lx, lx);
        float ly = (mapSize.y * 0.5f) - height;
        float clampY = Mathf.Clamp(transform.position.y, -ly, ly);

        transform.position = new Vector3(clampX, clampY, -10f);
    }
}
