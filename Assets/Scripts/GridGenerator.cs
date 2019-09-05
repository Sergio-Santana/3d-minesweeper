using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject CellPrefab;
    [HideInInspector]
    public CellController[,,] cells;

    public const uint MINES = 2;
    private const uint MAX_X = 5;
    private const uint MAX_Y = 2;
    private const uint MAX_Z = 5;

    private const float CUBE_DIMENSIONS = 1f;
    private const float SPACE_BETWEEN_CELLS = 0.1f;

    void Start()
    {
        cells = new CellController[MAX_X, MAX_Y, MAX_Z];

        float cubeOffSet = CUBE_DIMENSIONS + SPACE_BETWEEN_CELLS;
        for (uint x = 0; x < MAX_X; ++x)
        {
            for (uint y = 0; y < MAX_Y; ++y)
            {
                for (uint z = 0; z < MAX_Z; ++z)
                {
                    Vector3 v = new Vector3(x, y, z);
                    GameObject go = Instantiate(CellPrefab, v * cubeOffSet, Quaternion.identity, this.transform);
                    cells[x, y, z] = go.GetComponent<CellController>();
                    cells[x, y, z].position = v;
                }
            }
        }
        // TODO: Move the camera a bit further away
        Vector3 cameraPosition = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(cameraPosition.x, cameraPosition.y + MAX_Y*cubeOffSet, cameraPosition.z);

        StartCoroutine(PlantMines());
    }

    // TODO: This coroutine is not necessary, we will start planting after the user does the first movement
    private IEnumerator PlantMines()
    {
        yield return 0;
        cells[0, 0, 0].hasMine = true;
        cells[MAX_X - 1, MAX_Y - 1, MAX_Z - 1].hasMine = true;

    }
}
