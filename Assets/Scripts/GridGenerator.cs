using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject CellPrefab;
    private GameObject[,,] cells;

    private const int MAX_X = 5;
    private const int MAX_Y = 2;
    private const int MAX_Z = 5;

    private const float CUBE_DIMENSIONS = 1f;
    private const float SPACE_BETWEEN_CELLS = 0.1f;

    void Start()
    {
        cells = new GameObject[MAX_X, MAX_Y, MAX_Z];

        float cubeOffSet = CUBE_DIMENSIONS + SPACE_BETWEEN_CELLS;
        for (uint x = 0; x < MAX_X; ++x)
        {
            for (uint y = 0; y < MAX_Y; ++y)
            {
                for (uint z = 0; z < MAX_Z; ++z)
                {
                    Vector3 v = new Vector3(x, y, z);
                    v *= cubeOffSet;
                    GameObject go = Instantiate(CellPrefab, v, Quaternion.identity, this.transform);
                    cells[x, y, z] = go;
                }
            }
        }
        Vector3 cameraPosition = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(cameraPosition.x, cameraPosition.y + MAX_Y*cubeOffSet, cameraPosition.z);

        StartCoroutine(PlantMines());
    }

    // TODO: This coroutine is not necessary, we will start planting after the user does the first movement
    private IEnumerator PlantMines()
    {
        yield return 0;
        cells[0, 0, 0].GetComponent<CellController>().hasMine = true;
        cells[MAX_X - 1, MAX_Y - 1, MAX_Z - 1].GetComponent<CellController>().hasMine = true;

    }

    private uint testCounter = 0;
    public void Update()
    {
        // NOTE: Test code, move to another file
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if (leftClick || rightClick)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                CellController controller = hit.transform.GetComponent<CellController>();
                if (leftClick)
                {
                    // TODO: Count how many mines are nearby. This won't work right now because of inactive cells (??)
                    controller.Reveal(testCounter++);
                }
                else if (rightClick)
                {
                    controller.ToogleFlag();
                }
            }
        }
    }
}
