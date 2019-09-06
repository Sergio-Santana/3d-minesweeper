using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject CellPrefab;

    [HideInInspector]
    public CellController[,,] cells;
    [HideInInspector]
    public Vector3 gridCentre;

    public const uint MINES = 3;
    public const uint MAX_X = 5;
    public const uint MAX_Y = 5;
    public const uint MAX_Z = 5;

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
        Vector3 cameraPosition = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(cameraPosition.x, cameraPosition.y + MAX_Y*cubeOffSet, cameraPosition.z);
        gridCentre = new Vector3(MAX_X, MAX_Y, MAX_Z);
        gridCentre = (gridCentre * (CUBE_DIMENSIONS + SPACE_BETWEEN_CELLS)) / 2f;
        Camera.main.transform.LookAt(gridCentre);

    }

    public void PlantMines(CellController initialCell)
    {
        for (int n = 0; n < MINES;)
        {
            int x = Random.Range(0, (int)MAX_X);
            int y = Random.Range(0, (int)MAX_Y);
            int z = Random.Range(0, (int)MAX_Z);
            if ((initialCell.position != new Vector3(x, y, z)) && (!cells[x,y,z].hasMine))
            {
                cells[x, y, z].hasMine = true;
                ++n;
                for (int i = x-1; i <= x+1 ; ++i)
                {
                    if ((i < 0) || (i >= MAX_X)) continue;

                    for (int j = y-1; j <= y+1 ; ++j)
                    {
                        if ((j < 0) || (j >= MAX_Y)) continue;
                        for (int k = z-1; k <= z+1 ; ++k)
                        {
                            if ((k < 0) || (k >= MAX_Z)) continue;
                            ++cells[i, j, k].neighbouringMines;

                        }
                    }
                }
            }
        }
    }
}
