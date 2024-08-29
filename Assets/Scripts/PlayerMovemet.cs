using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemet : MonoBehaviour
{
    private float xMovement;
    private float zMovement;
    [SerializeField] private Transform playerPos;

    public CharacterController controller;

    public float speed  = 12.0f;

    // Update is called once per frame
    void Update()
    {
        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");

        Vector3 move = transform.right * xMovement + transform.forward * zMovement;

        controller.Move(move * speed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerPos.position, playerPos.forward * 10);
    }
}
