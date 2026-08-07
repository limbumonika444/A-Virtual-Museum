using System.Collections;
using UnityEngine;

/// <summary>
/// Swings a door leaf open/closed around a hinge pivot.
///
/// WHY THIS SCRIPT USES A "PIVOT" CHILD OBJECT INSTEAD OF ROTATING THE MESH DIRECTLY:
/// In acant.fbx, the door leaf nodes (canat.dr, canat.st, canat.dr1, canat.st1, canat.dr001,
/// canat.st001) have a non-zero "GeometricTranslation" baked into the FBX. Unity's FBX
/// importer absorbs that offset into the mesh data on import, which means the leaf's own
/// Transform pivot is NOT guaranteed to sit exactly on the hinge edge. If you rotate the
/// leaf's own Transform, the door will most likely swing around its center or an odd point
/// instead of around the hinge — this is the #1 cause of "my door flies across the room"
/// bugs with doors exported from Blender/3ds Max.
///
/// The fix used here: put an empty GameObject exactly at the hinge line, make it the PARENT
/// of the door mesh, and rotate the empty. This works no matter where the FBX's internal
/// pivot ended up, so you don't have to fight the import.
///
/// See the setup guide for exactly how to build that hierarchy in the Unity Editor.
/// </summary>
[DisallowMultipleComponent]
public class DoorOpener : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The empty GameObject positioned at the hinge line, with the door mesh parented under it. If left empty, this script's own transform is used (only correct if THIS object already sits exactly on the hinge).")]
    [SerializeField] private Transform hingePivot;

    [Header("Rotation Settings")]
    [Tooltip("How far the door swings open, in degrees.")]
    [SerializeField] private float openAngle = 90f;

    [Tooltip("Which local axis the door rotates around. For a standard upright door hinged on its side, this is almost always Y (0,1,0).")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Tooltip("Flip this if the door opens the wrong way (e.g. into a wall) after testing.")]
    [SerializeField] private bool invertDirection = false;

    [Header("Animation")]
    [Tooltip("Seconds to fully open or close.")]
    [SerializeField] private float duration = 1.0f;

    [Tooltip("Smooths the start/end of the swing instead of constant speed.")]
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Interaction")]
    [Tooltip("If true, calling Interact() toggles open/closed. If false, use OpenDoor()/CloseDoor() directly from your own input or trigger code.")]
    [SerializeField] private bool toggleOnInteract = true;

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    public bool IsOpen { get; private set; }
    public bool IsAnimating { get; private set; }

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine activeRoutine;

    private void Awake()
    {
        // Fallback so the script never silently no-ops if you forgot to assign hingePivot,
        // but this fallback only gives correct results if this object's own transform is
        // already positioned at the hinge — see the guide for the recommended setup.
        if (hingePivot == null)
        {
            hingePivot = transform;
            Debug.LogWarning($"[DoorOpener] '{gameObject.name}' has no Hinge Pivot assigned. " +
                              "Falling back to this object's own transform, which is only correct if " +
                              "it already sits exactly on the hinge edge. See the setup guide.", this);
        }

        closedRotation = hingePivot.localRotation;

        float angle = invertDirection ? -openAngle : openAngle;
        openRotation = closedRotation * Quaternion.AngleAxis(angle, rotationAxis.normalized);
    }

    /// <summary>Call this from a trigger, raycast interact script, UI button, etc.</summary>
    public void Interact()
    {
        if (IsAnimating) return;

        if (toggleOnInteract)
        {
            if (IsOpen) CloseDoor();
            else OpenDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (IsOpen || IsAnimating) return;
        StartRotation(openRotation, true);
    }

    public void CloseDoor()
    {
        if (!IsOpen || IsAnimating) return;
        StartRotation(closedRotation, false);
    }

    private void StartRotation(Quaternion target, bool willBeOpen)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(RotateRoutine(target, willBeOpen));
    }

    private IEnumerator RotateRoutine(Quaternion target, bool willBeOpen)
    {
        IsAnimating = true;

        if (audioSource != null)
        {
            AudioClip clip = willBeOpen ? openSound : closeSound;
            if (clip != null) audioSource.PlayOneShot(clip);
        }

        Quaternion start = hingePivot.localRotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            float eased = easeCurve.Evaluate(normalized);
            hingePivot.localRotation = Quaternion.Slerp(start, target, eased);
            yield return null;
        }

        hingePivot.localRotation = target;
        IsOpen = willBeOpen;
        IsAnimating = false;
        activeRoutine = null;
    }

    // Lets you see the swing arc in the Scene view without pressing Play.
    private void OnDrawGizmosSelected()
    {
        Transform pivot = hingePivot != null ? hingePivot : transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(pivot.position, 0.03f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(pivot.position, pivot.TransformDirection(rotationAxis.normalized) * 0.5f);
    }
}