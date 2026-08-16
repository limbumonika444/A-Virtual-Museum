using UnityEngine;

public class DirectWalker : MonoBehaviour
{
    public Transform[] points; 
    public float speed = 2.0f;
    public float yOffset = 0.0f; 
    private int currentPointIndex = 0;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void Update()
    {
        if (points == null || points.Length == 0) return;

        Transform target = points[currentPointIndex];
        if (target == null) return;

        Vector3 targetPos = target.position;
        targetPos.y = transform.position.y; 

        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPos.x, 0, targetPos.z)) < 0.5f)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Length;
            return;
        }


        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        Vector3 direction = (targetPos - transform.position);
        direction.y = 0; 

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}