using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Text endGameText;
    public Text deactivatorsLeft;

    private const float ROTATION_LIMIT = 45.0f;
    private const float ROTATION_SPEED = 0.3f;
    private const float ZOOM_SPEED = 1.1f;
    private bool gameFinished = false;
    private bool firstClick = true;
    private uint deactivators;

    private const string MINES_TEXT = " deactivators left";
    private const string WIN_TEXT = "YOU WON!";
    private const string LOSE_TEXT = "YOU LOST!";
    private void Start()
    {
        gameFinished = false;
        deactivators = GridGenerator.MINES;
        endGameText.text = "";
        deactivatorsLeft.text = deactivators + MINES_TEXT;
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
            Vector3 gridCentre = GridGenerator.GetCentre();
            Camera.main.transform.RotateAround(gridCentre, Vector3.up, -difference.x);

            float verticalRotation = Camera.main.transform.rotation.eulerAngles.x;
            
            if (((difference.y > 0) && (verticalRotation < ROTATION_LIMIT || verticalRotation > 180.0f)) || ((difference.y < 0) && (verticalRotation > (360.0f-ROTATION_LIMIT) || verticalRotation < 180.0f)))
            {
                Camera.main.transform.RotateAround(gridCentre, transform.TransformDirection(Vector3.right), difference.y);
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
                        GridGenerator.PlantMines(controller);
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
                        deactivators -= 1;
                        if (controller.DeactivateMine())
                        {
                            deactivatorsLeft.text = deactivators + MINES_TEXT;
                            gameFinished = (deactivators == 0);
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
