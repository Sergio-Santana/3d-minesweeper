using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public TextMesh rightSide;
    public TextMesh leftSide;
    public TextMesh top;

    private Material material;
    private bool revealed = false;

    private const float fadeSpeed = 3.0f;
    private bool fadingOut = false;

    public bool hasMine = false;
    public Vector3 position;
    void Start()
    {
        material = GetComponent<Renderer>().material;

        rightSide.text = leftSide.text = top.text = "";
        material.color = Color.gray;
    }

    public bool Reveal(uint number = 0)
    {
        if (revealed) return true;

        revealed = true;

        if (hasMine)
        {
            material.color = Color.red;
            return false;
        }


        if (number == 0)
            this.gameObject.SetActive(false);

        rightSide.text = leftSide.text = top.text = number.ToString();
        material.color = Color.white;
        return true;
    }

    public bool DeactivateMine()
    {
        if (revealed) return true;

        revealed = true;

        if (hasMine)
        {
            material.color = Color.green;
            StartFadeOut();
        }
        else
        {
            material.color = Color.red;
        }

        return hasMine;
    }

    public void StartFadeOut()
    {
        if (!fadingOut)
            StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        fadingOut = true;
        for (Color color = material.color;
            color.a > 0;
            color.a -= Time.deltaTime * fadeSpeed)
        {
            material.color = color;
            yield return null;
        }

        GetComponent<Renderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
    }
}
