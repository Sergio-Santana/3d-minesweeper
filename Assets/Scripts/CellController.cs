using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public TextMesh rightSide;
    public TextMesh leftSide;
    public TextMesh top;

    private Material material;
    private bool revealed, flagged;

    public bool hasMine;
    public Vector3 position;
    void Start()
    {
        material = GetComponent<Renderer>().material;
        revealed = flagged = hasMine = false;

        rightSide.text = leftSide.text = top.text = "";
        material.color = Color.gray;
    }

    public void Reveal(uint number = 0)
    {
        if (flagged || revealed) return;

        revealed = true;

        if (hasMine)
        {
            material.color = Color.red;
            return;
        }


        if (number == 0)
            this.gameObject.SetActive(false);

        rightSide.text = leftSide.text = top.text = number.ToString();
        material.color = Color.white;
    }

    public void ToogleFlag()
    {
        if (revealed) return;

        flagged = !flagged;
        material.color = flagged ? Color.yellow : Color.grey;
    }
}
