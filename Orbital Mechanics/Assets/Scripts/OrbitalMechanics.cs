using UnityEngine;

public class OrbitalMechanics : MonoBehaviour
{
    private OrbitalMechanics[] celestialBodies;

    [SerializeField] private Rigidbody rb;
    
    public Vector3 initialVelocity;
    

    private void Start()
    {
        celestialBodies = FindObjectsOfType<OrbitalMechanics>();
        rb = gameObject.GetComponent<Rigidbody>();

        rb.velocity = initialVelocity;
    }

    private void FixedUpdate()
    {
        foreach(OrbitalMechanics body in celestialBodies)
        {
            if (body == this) continue;

            Rigidbody otherRb = body.GetComponent<Rigidbody>();

            float distance = Vector3.Distance(transform.position, body.transform.position);
            Vector3 direction = (body.transform.position - transform.position).normalized;

            Vector3 gravitationalForce = direction * (Constants.G * (rb.mass * otherRb.mass / Mathf.Pow(distance, 2)));

            rb.AddForce(gravitationalForce, ForceMode.Force);
        }
    }
}
