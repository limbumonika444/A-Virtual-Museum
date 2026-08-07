using UnityEngine;

/// <summary>
/// Put this on a separate trigger collider near the door (NOT on the door mesh itself).
/// Opens the door automatically when the player enters the zone, closes it when they leave.
/// If you'd rather require a key press (e.g. "E to open"), set requireKeyPress = true.
/// </summary>
public class DoorTrigger : MonoBehaviour
{
    [Tooltip("Drag the GameObject that has the DoorOpener script on it (the hinge pivot object).")]
    [SerializeField] private DoorOpener door;

    [Tooltip("Only objects with this tag will trigger the door. Default 'Player'.")]
    [SerializeField] private string playerTag = "Player";

    [Header("Interaction Mode")]
    [Tooltip("If true, player must press a key while inside the trigger. If false, door opens automatically on enter.")]
    [SerializeField] private bool requireKeyPress = false;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInRange;

    private void Reset()
    {
        // Auto-configure the collider as a trigger when this script is first added.
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        if (requireKeyPress && playerInRange && Input.GetKeyDown(interactKey))
        {
            door.Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = true;

        if (!requireKeyPress)
        {
            door.OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;

        if (!requireKeyPress)
        {
            door.CloseDoor();
        }
    }
}