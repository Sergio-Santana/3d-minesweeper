using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GridGenerator grid;
    public Text endGameText;
    public Text minesLeftText;

    private const float cameraSpeed = 2.0f;
    private bool gameFinished = false;
    private uint minesLeft;

    // TODO: Maybe this should be called deactivators, since we lose once we use one when we shouldn't
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

    void Update()
    {
        Vector3 translation = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
            translation.x = Time.deltaTime * -cameraSpeed;
        else if (Input.GetKey(KeyCode.D))
            translation.x = Time.deltaTime * cameraSpeed;

        if (Input.GetKey(KeyCode.W))
            translation.y = Time.deltaTime * cameraSpeed;
        else if (Input.GetKey(KeyCode.S))
            translation.y = Time.deltaTime * -cameraSpeed;

        translation.z =  Input.mouseScrollDelta.y;
        Camera.main.transform.Translate(translation);

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
                    if (leftClick)
                    {
                        // TODO: Count how many mines are nearby. This won't work right now because of inactive cells (??)
                        if (!controller.Reveal(1))
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
