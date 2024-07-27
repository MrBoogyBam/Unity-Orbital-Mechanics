using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float smoothZoomSpeed;
    [SerializeField] private float smoothSwitchSpeed;

    [Header("Camera Control")]
    [SerializeField] private float zoomAmount;
    [SerializeField] private float sensitivity;
    [SerializeField] private float maxDistance;

    private CameraVariables selectedPlanet;

    private Transform oldTarget;

    private Vector3 smoothPosition;
    private Vector3 virtualTargetPosition;

    private Quaternion rotation;
    private Quaternion smoothRotation;

    private float desiredDistance;
    private float xRotation;
    private float yRotation;

    private void Start()
    {
        oldTarget = target;
        desiredDistance = distance;
        selectedPlanet = target.GetComponent<CameraVariables>();

        Vector3 angles = transform.eulerAngles;

        xRotation = angles.x;
        yRotation = angles.y;
    }

    private void Update()
    {
        distance = Mathf.Lerp(distance, desiredDistance, smoothZoomSpeed);
        Debug.Log(Vector3.Distance(virtualTargetPosition, target.position));
        virtualTargetPosition = Vector3.Distance(virtualTargetPosition, target.position) > 5 ? Vector3.Lerp(virtualTargetPosition, target.position, smoothSwitchSpeed) : target.position;
        smoothPosition = smoothRotation * new Vector3(0, 0, -distance) + virtualTargetPosition;
        smoothRotation = Quaternion.Lerp(transform.rotation, rotation, smoothSpeed);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.transform != target)
                {
                    oldTarget = target;
                    target = hit.transform;

                    selectedPlanet = target.GetComponent<CameraVariables>();

                    desiredDistance = Mathf.Clamp(desiredDistance, selectedPlanet.minDistance, maxDistance);
                }
            }
        }

        if(Input.mouseScrollDelta.y != 0)
        {
            desiredDistance += desiredDistance <= selectedPlanet.slowDownDistance ? -Input.mouseScrollDelta.y * (zoomAmount / selectedPlanet.slowDownAmount) : -Input.mouseScrollDelta.y * zoomAmount;

            desiredDistance = Mathf.Clamp(desiredDistance, selectedPlanet.minDistance, maxDistance);
        }

        if(Input.GetMouseButton(1))
        {
            xRotation += Input.GetAxis("Mouse X") * sensitivity * 0.02f;
            yRotation -= Input.GetAxis("Mouse Y") * sensitivity * 0.02f;

            yRotation = Mathf.Clamp(yRotation, -90, 90);

            rotation = Quaternion.Euler(yRotation, xRotation, 0);
        }
    }

    private void FixedUpdate()
    {
        transform.rotation = smoothRotation;
        transform.position = smoothPosition;
    }
}
