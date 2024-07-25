using UnityEngine;

[ExecuteInEditMode]
public class OrbitLines : MonoBehaviour
{
    [SerializeField] private bool renderLine = false;
    [SerializeField] private int iterations;
    [SerializeField] private float timeStep;

    private void Start()
    {
        if(Application.isPlaying)
        {
            OrbitalMechanics[] celestialBodies = FindObjectsOfType<OrbitalMechanics>();

            foreach(OrbitalMechanics body in celestialBodies)
            {
                body.GetComponent<LineRenderer>().enabled = false;
                Destroy(this);
            }
        }
    }

    private void Update()
    {
        if (!renderLine)
        {
            return;
        }

        if (iterations < 0)
        {
            iterations = 0;
        }

        OrbitalMechanics[] celestialBodies = FindObjectsOfType<OrbitalMechanics>();
        VirtualBody[] virtualBodies = new VirtualBody[celestialBodies.Length];
        Vector3[][] points = new Vector3[celestialBodies.Length][];

        for (int i = 0; i < virtualBodies.Length; i++)
        {
            virtualBodies[i] = new VirtualBody(celestialBodies[i]);
            points[i] = new Vector3[iterations];
        }

        for(int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < virtualBodies.Length; j++)
            {
                virtualBodies[j].velocity += calculateAcceleration(j, virtualBodies);
            }

            for (int j = 0; j < virtualBodies.Length; j++)
            {
                Vector3 newPos = virtualBodies[j].position + virtualBodies[j].velocity * timeStep;

                virtualBodies[j].position = newPos;

                points[j][i] = newPos;
            }
        }

        for(int i = 0; i < celestialBodies.Length; i++)
        {
            LineRenderer orbitLine = celestialBodies[i].GetComponent<LineRenderer>();

            orbitLine.positionCount = points[i].Length;
            orbitLine.SetPositions(points[i]);
        }
    }

    private Vector3 calculateAcceleration(int i, VirtualBody[] virtualBodies)
    {
        Vector3 acceleration = Vector3.zero;

        for(int j = 0; j < virtualBodies.Length; j++)
        {
            if (i == j) continue;

            Vector3 direction = (virtualBodies[j].position - virtualBodies[i].position).normalized;

            float sqrDist = (virtualBodies[j].position - virtualBodies[i].position).sqrMagnitude;

            acceleration += direction * Constants.G * virtualBodies[j].mass / sqrDist * timeStep;
        }

        return acceleration;
    }

    private class VirtualBody
    {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;

        public VirtualBody(OrbitalMechanics body)
        {
            position = body.transform.position;
            velocity = body.initialVelocity;
            mass = body.GetComponent<Rigidbody>().mass;
        }
    }
}
