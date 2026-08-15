using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClicker : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;
    private bool mousePress = false;

    void Awake()
    {
        m_Camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            mousePress = true;
        }
    }

    // FixedUpdate is called on physics frame updates
    void FixedUpdate()
    {
        if (mousePress)
        {
            mousePress = false;
            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                Vector3 mousePosition = mouse.position.ReadValue();
                Ray ray = m_Camera.ScreenPointToRay(mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Debug.Log("Clicked on: " + hit.collider.gameObject.name);

                    // Try to get the GOInteraction component on the hit object
                    GOInteraction aGOI = hit.collider.gameObject.GetComponent<GOInteraction>();
                    if (aGOI != null)
                    {
                        aGOI.Interaction = true;
                    }
                }
            }
        }
    }
}