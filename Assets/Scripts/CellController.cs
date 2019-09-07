using System.Collections;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public TextMesh interiorMesh;

    private Material material;
    private bool revealed = false;

    private const float fadeSpeed = 3.0f;
    private const float transparentAlpha = 0.1f;

    private static readonly Color GRAY_SOLID = new Color(0.45f, 0.45f, 0.45f, 1f);
    private static readonly Color DARKGRAY_SOLID = new Color(0.2f, 0.2f, 0.2f, 1f);
    private static readonly Color RED_SOLID = new Color(1, 0, 0, 1f);
    private static readonly Color GREEN_SOLID = new Color(0, 1, 0, 1f);
    private static readonly Color YELLOW_SOLID = new Color(1, 1, 0, 1f);
    private static readonly Color WHITE_SOLID = new Color(1, 1, 1, transparentAlpha);

    [HideInInspector]
    public uint neighbouringMines = 0;
    public bool hasMine = false;
    public Vector3Int position;

    void Start()
    {
        material = GetComponent<Renderer>().material;

        interiorMesh.text = "";
        if (position.x == 0 || position.y == 0 || position.z == 0 ||
            position.x == (GridGenerator.MAX_X-1) || position.y == (GridGenerator.MAX_Y-1) || position.z == (GridGenerator.MAX_Z-1))
            material.color = DARKGRAY_SOLID;
        else
            material.color = GRAY_SOLID;
    }

    private void Update()
    {
        if (interiorMesh.text != "")
        {
            Vector3 direction = interiorMesh.transform.position - Camera.main.transform.position;
            interiorMesh.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void ShowText()
    {
        if (neighbouringMines == 0)
        {
            StartCoroutine(FadeOutFromFull());
            interiorMesh.text = "";
        }
        else
        {
            StartCoroutine(FadeToAlpha(transparentAlpha));
            interiorMesh.text = neighbouringMines.ToString();
        }
    }

    public bool Reveal()
    {
        if (revealed) return true;
        revealed = true;
        GetComponent<BoxCollider>().enabled = false;

        if (hasMine)
        {
            material.color = RED_SOLID;
            return false;
        }

        material.color = WHITE_SOLID;
        ShowText();
        return true;
    }

    public bool DeactivateMine()
    {
        if (revealed) return true;
        revealed = true;
        GetComponent<BoxCollider>().enabled = false;

        if (hasMine)
        {
            hasMine = false;
            material.color = GREEN_SOLID;
            GridGenerator.ApplyToNeighbours(position.x, position.y, position.z, x => x.DecreaseCounter());
            ShowText();
            return true;
        }
        else
        {
            material.color = YELLOW_SOLID;
            return false;
        }
    }

    private void DecreaseCounter()
    {
        --neighbouringMines;
        if (!revealed) return;

        if (neighbouringMines == 0)
        {
            interiorMesh.text = "";
            if (revealed)
                StartCoroutine(FadeOutFromFull());
        }
        else
        {
            interiorMesh.text = neighbouringMines.ToString();
        }
    }

    public void Clear()
    {
        GetComponent<BoxCollider>().enabled = false;
        revealed = true;
        StartCoroutine(FadeOutFromFull());
    }

    public void ShowMineContent()
    {
        if (!revealed)
        {
            revealed = true;
            if (hasMine)
            {
                material.color = RED_SOLID;
            }
            else
            {
                StartCoroutine(FadeToAlpha(transparentAlpha));
            }
        }
    }

    IEnumerator FadeOutFromFull()
    {
        Color color = material.color;
        color.a = 1f;
        for (; color.a > 0; color.a -= Time.deltaTime * fadeSpeed)
        {
            material.color = color;
            yield return null;
        }

        GetComponent<Renderer>().enabled = false;
    }

    IEnumerator FadeToAlpha(float alpha)
    {
        Color color = material.color;
        color.a = 1f;
        for (; color.a > alpha; color.a -= Time.deltaTime * fadeSpeed)
        {
            material.color = color;
            yield return null;
        }

        color.a = alpha;
        material.color = color;
    }
}
