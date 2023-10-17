using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Transform cam;
    private Vector3 pivotEulerAngles = Vector3.zero;

    // 마우스 키보드 조작용 변수들
    [Range(1.0f, 20.0f)]
    public float cameraMoveSpeed = 3.5f;
    [Range(0.1f, 20.0f)]
    public float rotateSpeedRate = 3.0f;
    [Range(1.0f, 20.0f)]
    public float accelerationRate = 4.0f;

    private Vector3 mouseLastPos = Vector3.zero;
    private Vector3 screenCenterPos = Vector3.zero;

    private float horizontalCamMove = 0f;
    private float verticalCamMove = 0f;
    private float heightCamMove = 0f;


    
    void Start()
    {
        cam = Camera.main.transform;
        pivotEulerAngles = cam.eulerAngles;
    }
    
    void Update()
    {
        horizontalCamMove = 0f;
        verticalCamMove = 0f;
        heightCamMove = 0f;

        if (Input.GetKey(KeyCode.W) && Input.GetMouseButton(1))
            verticalCamMove = 1f;
        else if (Input.GetKey(KeyCode.S) && Input.GetMouseButton(1))
            verticalCamMove = -1f;

        if (Input.GetKey(KeyCode.D) && Input.GetMouseButton(1))
            horizontalCamMove = 1f;
        else if (Input.GetKey(KeyCode.A) && Input.GetMouseButton(1))
            horizontalCamMove = -1f;

        if (Input.GetKey(KeyCode.E) && Input.GetMouseButton(1))
            heightCamMove = 1f;
        else if (Input.GetKey(KeyCode.Q) && Input.GetMouseButton(1))
            heightCamMove = -1f;

        float moveSpeed = cameraMoveSpeed * ((Input.GetKey(KeyCode.LeftShift)) ? accelerationRate : 1.0f);

        cam.transform.Translate(new Vector3(horizontalCamMove * moveSpeed * Time.deltaTime, 0f, verticalCamMove * moveSpeed * Time.deltaTime));
        cam.transform.position += new Vector3(0f, heightCamMove * moveSpeed * Time.deltaTime, 0f);

        if (Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                pivotEulerAngles = cam.eulerAngles;

                Vector3 delta = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0.0f);
                pivotEulerAngles.x -= delta.y * rotateSpeedRate * 0.2f;
                pivotEulerAngles.y += delta.x * rotateSpeedRate * 0.2f;

                cam.transform.eulerAngles = pivotEulerAngles;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
