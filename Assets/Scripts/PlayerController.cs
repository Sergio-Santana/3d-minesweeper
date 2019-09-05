using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GridGenerator grid;
    public Text endGameText;
    public Text minesLeftText;

    private const float ROTATION_LIMIT = 45.0f;
    private const float ROTATION_SPEED = 0.3f;
    private const float ZOOM_SPEED = 1.1f;
    private bool gameFinished = false;
    private bool firstClick = true;
    private uint minesLeft;

    private const string MINES_TEXT = "Mines: ";
    private const string WIN_TEXT = "YOU WON!";
    private const string LOSE_TEXT = "YOU LOST!";
    private void Start()
    {
        gameFinished = false;
        minesLeft = GridGenerator.MINES;
        endGameText.text = "";
        minesLeftText.text = MINES_TEXT + minesLeft;
    }

    private Vector3? lastMousePosition;
    void Update()
    {
        if(Input.GetMouseButton(2))
        {
            Vector3 current = Input.mousePosition;
            Vector3 last = lastMousePosition ?? current;
            lastMousePosition = current;

            Vector3 difference = (last - current) * ROTATION_SPEED;
            Camera.main.transform.RotateAround(grid.gridCentre, Vector3.up, -difference.x);

            float verticalRotation = Camera.main.transform.rotation.eulerAngles.x;
            
            if (((difference.y > 0) && (verticalRotation < ROTATION_LIMIT || verticalRotation > 180.0f)) || ((difference.y < 0) && (verticalRotation > (360.0f-ROTATION_LIMIT) || verticalRotation < 180.0f)))
            {
                Camera.main.transform.RotateAround(grid.gridCentre, transform.TransformDirection(Vector3.right), difference.y);
            }
        }
        else
        {
            lastMousePosition = null;
        }
        Camera.main.transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * ZOOM_SPEED);

        if (!gameFinished)
        {
            bool leftClick = Input.GetMouseButtonDown(0);
            bool rightClick = Input.GetMouseButtonDown(1);
            if (leftClick || rightClick)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    CellController controller = hit.transform.GetComponent<CellController>();
                    if (firstClick)
                    {
                        firstClick = false;
                        grid.PlantMines(controller);
                    }
                    if (leftClick)
                    {
                        if (!controller.Reveal())
                        {
                            endGameText.text = LOSE_TEXT;
                            gameFinished = true;
                        }
                    }
                    else if (rightClick)
                    {
                        if (controller.DeactivateMine())
                        {
                            minesLeft -= 1;
                            minesLeftText.text = MINES_TEXT + minesLeft;
                            gameFinished = (minesLeft == 0);
                            if (gameFinished)
                                endGameText.text = WIN_TEXT;
                        }
                        else
                        {
                            endGameText.text = LOSE_TEXT;
                            gameFinished = true;
                        }
                    }
                }
            }
        }
    }
}
