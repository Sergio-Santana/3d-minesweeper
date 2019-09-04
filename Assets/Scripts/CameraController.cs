using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float speed = 2.0f;
    void Update()
    {
        Vector3 translation = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
            translation.x = Time.deltaTime * -speed;
        else if (Input.GetKey(KeyCode.D))
            translation.x = Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.W))
            translation.y = Time.deltaTime * speed;
        else if (Input.GetKey(KeyCode.S))
            translation.y = Time.deltaTime * -speed;

        translation.z =  Input.mouseScrollDelta.y;
        Camera.main.transform.Translate(translation);
    }
}
