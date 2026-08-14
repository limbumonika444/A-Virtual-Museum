using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClicker : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;

    private bool mousePress = false;

    void Start()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }

        if (m_Camera == null)
        {
            Debug.LogError("No Camera found!");
        }
    }

    void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            mousePress = true;
        }
    }

    void FixedUpdate()
    {
        if (mousePress)
        {
            mousePress = false;

            Mouse mouse = Mouse.current;

            if (mouse == null || m_Camera == null)
            {
                return;
            }

            Vector3 mousePosition = mouse.position.ReadValue();

            Ray ray = m_Camera.ScreenPointToRay(mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 2f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Clicked on: " + hit.collider.gameObject.name);

                GOInteraction aGOI =
                    hit.collider.gameObject.GetComponent<GOInteraction>();

                if (aGOI != null)
                {
                    aGOI.Interaction = true;
                    Debug.Log("Interaction triggered!");
                }
            }
        }
    }
}