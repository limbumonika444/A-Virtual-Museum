using UnityEngine;

public class DirectWalker : MonoBehaviour
{
    public Transform[] points; 
    public float speed = 2.0f;
    public float yOffset = 1.0f; 
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
        targetPos.y += yOffset;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        Vector3 direction = targetPos - transform.position;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }

        
        if (Vector3.Distance(transform.position, targetPos) < 0.3f)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Length;
        }
    }
}