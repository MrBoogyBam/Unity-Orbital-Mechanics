using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance;
    [SerializeField] private float smoothSpeed;

    [Header("Camera Control")]
    [SerializeField] private float zoomAmount;
    [SerializeField] private float sensitivity;

    private Vector3 smoothPosition;

    private Quaternion smoothRotation;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;

        xRotation = angles.x;
        yRotation = angles.y;
    }

    private void Update()
    {
        smoothPosition = smoothRotation * new Vector3(0, 0, -distance) + target.position;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    target = hit.transform;
                }
            }
        }

        if(Input.mouseScrollDelta.y != 0)
        {
            distance += distance <= 400 ? -Input.mouseScrollDelta.y * (zoomAmount / 5) : -Input.mouseScrollDelta.y * zoomAmount;
            if (distance < 50)
            {
                distance = 50;
                return;
            }
        }

        if(Input.GetMouseButton(1))
        {
            xRotation += Input.GetAxis("Mouse X") * sensitivity * 0.02f;
            yRotation -= Input.GetAxis("Mouse Y") * sensitivity * 0.02f;

            yRotation = Mathf.Clamp(yRotation, -90, 90);

            Quaternion rotation = Quaternion.Euler(yRotation, xRotation, 0);
            smoothRotation = Quaternion.Lerp(transform.rotation, rotation, smoothSpeed);
            smoothPosition = smoothRotation * new Vector3(0, 0, -distance) + target.position;
        }
    }

    private void FixedUpdate()
    {
        transform.rotation = smoothRotation;
        transform.position = smoothPosition;
    }
}
