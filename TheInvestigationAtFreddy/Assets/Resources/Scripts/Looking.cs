using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looking : MonoBehaviour
{
    public float sensitivity;
    public Transform player;

    public float mouseX;
    public float mouseY;
    public float Xrotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * sensitivity;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        Xrotation -= mouseY;
        Xrotation = Mathf.Clamp(Xrotation, -80, 90);

        player.Rotate(Vector3.up * mouseX);
        transform.localRotation= Quaternion.Euler(Xrotation,0,0);
    }
}
