using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Simple Script For Handling Character Movement
public class CameraMovement : MonoBehaviour
{
    //Initialize Variables For Movement
    public int speed;
    public float mouseSensitivity;
    public GameObject BoardCamRef;
    public GameObject TextCameraRef;
    public GameObject CameraNode;
    float mouseX;
    float mouseY;
    float xRotation = 0f;

    // Initialize Variables
    void Start()
    {
        //Moves Camera to New Position and Locks Cursor
        transform.position = CameraNode.transform.position;
        transform.rotation = CameraNode.transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Changes and Moves Camera
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        GetComponent<CharacterController>().Move(move * speed * Time.deltaTime);


        if (Input.GetKey(KeyCode.E))
        {
            this.transform.position += Vector3.up * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.position += Vector3.down * speed * Time.deltaTime;
        }


        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        BoardCamRef.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        TextCameraRef.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
