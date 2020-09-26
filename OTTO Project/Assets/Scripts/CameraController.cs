using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraDistanceMax = 50f;
    [SerializeField] private float cameraDistanceMin = 25f;
    [SerializeField] private float cameraDistance = 25f;
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private float rotationSpeed = 30f;

    public Cinemachine.CinemachineVirtualCamera virtualCamera;

    public static CameraController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

    }

    void Update()
    {
        if(GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                cameraDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * -1f;
                cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);

                virtualCamera.m_Lens.FieldOfView = cameraDistance;
            }

            if (Input.GetKey(KeyCode.D))
            {
                Quaternion currentRot = virtualCamera.transform.rotation;
                virtualCamera.transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                Quaternion currentRot = virtualCamera.transform.rotation;
                virtualCamera.transform.RotateAround(Vector3.zero, Vector3.up, -rotationSpeed * Time.deltaTime);
            }
        }
    }

}