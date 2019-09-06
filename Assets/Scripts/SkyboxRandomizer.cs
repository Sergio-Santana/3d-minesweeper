using UnityEngine;

public class SkyboxRandomizer : MonoBehaviour
{
    void Start()
    {
        float r = Random.Range(0.7f, 1f);
        float g = Random.Range(0.7f, 1);
        float b = Random.Range(0.7f, 1);
        GetComponent<Camera>().backgroundColor = new Color(r, g, b, 0f);
        GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
    }
}
