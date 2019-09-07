using UnityEngine;
using System;

public class GridGenerator : MonoBehaviour
{
    private static GridGenerator instance = null;

    public GameObject CellPrefab;

    [HideInInspector]
    public CellController[,,] cells;
    [HideInInspector]
    private Vector3 gridCentre;

    public const uint MINES = 10;
    public const uint MAX_X = 5;
    public const uint MAX_Y = 5;
    public const uint MAX_Z = 5;

    private const float CUBE_DIMENSIONS = 1f;
    private const float SPACE_BETWEEN_CELLS = 0.05f;

    void Start()
    {
        if (instance == null)
        {
            Screen.fullScreen = false;
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        cells = new CellController[MAX_X, MAX_Y, MAX_Z];

        float cubeOffSet = CUBE_DIMENSIONS + SPACE_BETWEEN_CELLS;
        Vector3Int position = Vector3Int.zero;
        for (position.x = 0; position.x < MAX_X; ++position.x)
        {
            for (position.y = 0; position.y < MAX_Y; ++position.y)
            {
                for (position.z = 0; position.z < MAX_Z; ++position.z)
                {
                    GameObject go = Instantiate(CellPrefab, (Vector3)position * cubeOffSet, Quaternion.identity, this.transform);
                    CellController cc = go.GetComponent<CellController>();
                    cc.position = position;
                    cells[position.x, position.y, position.z] = go.GetComponent<CellController>();
                }
            }
        }

        Vector3 cameraPosition = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(cameraPosition.x, cameraPosition.y + MAX_Y*cubeOffSet, cameraPosition.z);
        gridCentre = new Vector3(MAX_X, MAX_Y, MAX_Z);
        gridCentre = (gridCentre * (CUBE_DIMENSIONS + SPACE_BETWEEN_CELLS)) / 2f;
        Camera.main.transform.LookAt(gridCentre);
    }

    public static void PlantMines(CellController initialCell)
    {
        ref CellController[,,] cells = ref instance.cells;

        for (int n = 0; n < MINES;)
        {
            int x = UnityEngine.Random.Range(0, (int)MAX_X);
            int y = UnityEngine.Random.Range(0, (int)MAX_Y);
            int z = UnityEngine.Random.Range(0, (int)MAX_Z);
            if ((initialCell.position != new Vector3(x, y, z)) && (!cells[x,y,z].hasMine))
            {
                cells[x, y, z].hasMine = true;
                ++n;
                ApplyToNeighbours(x, y, z, c => ++c.neighbouringMines);
            }
        }
    }

    public static Vector3 GetCentre()
    {
        return instance.gridCentre;
    }

    public static void ApplyToNeighbours(int x, int y, int z, Action<CellController> f)
    {
        ref CellController[,,] cells = ref instance.cells;

        for (int i = x - 1; i <= x + 1; ++i)
        {
            if ((i < 0) || (i >= MAX_X)) continue;

            for (int j = y - 1; j <= y + 1; ++j)
            {
                if ((j < 0) || (j >= MAX_Y)) continue;
                for (int k = z - 1; k <= z + 1; ++k)
                {
                    if ((k < 0) || (k >= MAX_Z)) continue;
                    if (i == x && j == y && k == z) continue;
                    f.Invoke(instance.cells[i, j, k]);
                }
            }
        }
    }

    public static void RevealMines()
    {
        ref CellController[,,] cells = ref instance.cells;
        foreach (CellController cc in cells)
        {
            cc.ShowMineContent();
        }
    }

    public static void ClearCells()
    {
        ref CellController[,,] cells = ref instance.cells;
        foreach (CellController cc in cells)
        {
            cc.Clear();
        }
    }
}
