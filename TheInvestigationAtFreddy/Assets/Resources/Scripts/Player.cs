using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class Player : MonoBehaviour
{
    public KeyCode forward, backward, right, left, sprint;
    public CharacterController characterController;
    public GameObject player;
    public Transform groundCheck;

    public float groundDistance = 0.4f;
    public float speed = 1f;
    public float gravity = -9.81f;
    public float stamina = 10f;

    bool isGrounded;

    public LayerMask groundMask;
    Vector3 velocity;

    public int GetAxis(KeyCode A, KeyCode B)
    {
        if (Input.GetKey(A))
        {
            return 1;
        }
        if (Input.GetKey(B))
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -0.5f;
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        float x = GetAxis(forward, backward);
        float y = GetAxis(right, left);

        Vector3 move = transform.right * y + transform.forward * x;

        characterController.Move(move * speed * Time.deltaTime);

        if(isGrounded && Input.GetKey(sprint) && Input.GetKey(forward))
        {
            speed = 1.25f;
            stamina -= 1 * Time.deltaTime;
            if (stamina <= 0) Debug.Log("no stamina"); speed = 1;
        }
        else
        {
            speed = 1;
        }
    }
}
