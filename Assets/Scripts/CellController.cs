using System.Collections;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public TextMesh interiorMesh;

    private Material material;
    private bool revealed = false;

    private const float fadeSpeed = 3.0f;
    private const float transparentAlpha = 0.3f;
    private readonly Color RED_TRANSPARENT = new Color(1, 0, 0, transparentAlpha);
    private readonly Color GREEN_TRANSPARENT = new Color(0, 1, 0, transparentAlpha);
    private readonly Color WHITE_TRANSPARENT = new Color(1, 1, 1, transparentAlpha);


    [HideInInspector]
    public uint neighbouringMines = 0;
    public bool hasMine = false;
    public Vector3 position;
    void Start()
    {
        material = GetComponent<Renderer>().material;

        interiorMesh.text = "";
        material.color = Color.gray;
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
            StartCoroutine(FadeOut());
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
            //StartCoroutine(FadeOut());
        }
        else
        {
            material.color = RED_TRANSPARENT;
        }

        return hasMine;
    }

    IEnumerator FadeOut()
    {
        for (Color color = material.color;
            color.a > 0;
            color.a -= Time.deltaTime * fadeSpeed)
        {
            material.color = color;
            yield return null;
        }

        GetComponent<Renderer>().enabled = false;
    }
}
