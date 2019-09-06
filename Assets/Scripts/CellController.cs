using System;
using System.Collections;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public TextMesh interiorMesh;

    private Material material;
    private bool revealed = false;

    private const float fadeSpeed = 3.0f;
    private const float transparentAlpha = 0.1f;

    private readonly Color GRAY_SOLID = new Color(0.45f, 0.45f, 0.45f, 1f);
    private readonly Color DARKGRAY_SOLID = new Color(0.2f, 0.2f, 0.2f, 1f);
    private readonly Color RED_TRANSPARENT = new Color(1, 0, 0, transparentAlpha);
    private readonly Color GREEN_TRANSPARENT = new Color(0, 1, 0, transparentAlpha);
    private readonly Color WHITE_TRANSPARENT = new Color(1, 1, 1, transparentAlpha);


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

    public bool Reveal()
    {
        if (revealed) return true;
        revealed = true;
        GetComponent<BoxCollider>().enabled = false;

        if (hasMine)
        {
            material.color = RED_TRANSPARENT;
            return false;
        }

        material.color = WHITE_TRANSPARENT;
        if (neighbouringMines == 0)
        {
            StartCoroutine(FadeOutFromFull());
            interiorMesh.text = "";
        }
        else
        {
            interiorMesh.text = neighbouringMines.ToString();
        }
        return true;
    }

    public bool DeactivateMine()
    {
        if (revealed) return true;
        revealed = true;
        GetComponent<BoxCollider>().enabled = false;

        if (hasMine)
        {
            material.color = GREEN_TRANSPARENT;
            GridGenerator.ApplyToNeighbours(position.x, position.y, position.z, x => x.DecreaseCounter());
            StartCoroutine(FadeOutFromFull());
        }
        else
        {
            material.color = RED_TRANSPARENT;
        }

        return hasMine;
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

    IEnumerator FadeOutFromFull()
    {
        Color color = material.color;
        color.a = 1f;
        for ( ; color.a > 0; color.a -= Time.deltaTime * fadeSpeed)
        {
            material.color = color;
            yield return null;
        }

        GetComponent<Renderer>().enabled = false;
    }
}
