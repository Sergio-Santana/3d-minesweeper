using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Text endGameText;
    public Text deactivatorsLeft;

    private const float ROTATION_LIMIT = 45.0f;
    private const float ROTATION_SPEED = 0.3f;
    private const float ZOOM_SPEED = 1.1f;
    private enum GameState { START = 0, ONGOING, WIN, LOSE };
    private GameState gameState = GameState.START;
    private uint deactivators;

    private const string MINES_TEXT = " deactivators left";
    private const string WIN_TEXT = "YOU WON!";
    private const string LOSE_TEXT = "YOU LOST!";
    private readonly Color DARK_RED = new Color(0.5f, 0f, 0f, 1f);
    private readonly Color DARK_GREEN = new Color(0f, 0.5f, 0f, 1f);

    private void Start()
    {
        gameState = GameState.START;
        deactivators = GridGenerator.MINES;
        endGameText.text = "";
        deactivatorsLeft.text = deactivators + MINES_TEXT;
    }

    private Vector3? lastMousePosition;
    private void CameraMovement()
    {
        if (Input.GetMouseButton(2))
        {
            Vector3 current = Input.mousePosition;
            Vector3 last = lastMousePosition ?? current;
            lastMousePosition = current;

            Vector3 difference = (last - current) * ROTATION_SPEED;
            Vector3 gridCentre = GridGenerator.GetCentre();
            Camera.main.transform.RotateAround(gridCentre, Vector3.up, -difference.x);

            float verticalRotation = Camera.main.transform.rotation.eulerAngles.x;

            if (((difference.y > 0) && (verticalRotation < ROTATION_LIMIT || verticalRotation > 180.0f)) || ((difference.y < 0) && (verticalRotation > (360.0f - ROTATION_LIMIT) || verticalRotation < 180.0f)))
            {
                Camera.main.transform.RotateAround(gridCentre, transform.TransformDirection(Vector3.right), difference.y);
            }
        }
        else
        {
            lastMousePosition = null;
        }
        Camera.main.transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * ZOOM_SPEED);
    }

    private void PlayerAction()
    {
        if (gameState <= GameState.ONGOING)
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
                    if (gameState == GameState.START)
                    {
                        gameState = GameState.ONGOING;
                        GridGenerator.PlantMines(controller);
                    }
                    if (leftClick)
                    {
                        if (!controller.Reveal())
                        {
                            gameState = GameState.LOSE;
                        }
                    }
                    else if (rightClick)
                    {
                        deactivators -= 1;
                        if (controller.DeactivateMine())
                        {
                            deactivatorsLeft.text = deactivators + MINES_TEXT;
                            gameState = (deactivators == 0) ? GameState.WIN : GameState.ONGOING;
                        }
                        else
                        {
                            gameState = GameState.LOSE;
                        }
                    }
                }
            }
        }
    }

    private void ShowEndGameText()
    {
        if (gameState == GameState.WIN)
        {
            endGameText.text = WIN_TEXT;
            endGameText.color = DARK_GREEN;
            GridGenerator.ClearCells();
        }
        else if (gameState == GameState.LOSE)
        {
            endGameText.text = LOSE_TEXT;
            endGameText.color = DARK_RED;
            GridGenerator.RevealMines();
        }
    }

    void Update()
    {
        CameraMovement();
        PlayerAction();
        ShowEndGameText();
    }
}
