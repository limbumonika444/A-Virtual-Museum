using UnityEngine;

public class GOInteraction : MonoBehaviour
{
    public bool Interaction = false;
    public bool LampInteraction = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void TriggerInteraction()
    {
        Debug.Log("Interaction triggered!");
        Interaction = true;
    }

    public void TriggerLampInteraction()
    {
        Debug.Log("Lamp Interaction triggered!");
        LampInteraction = true;
    }
}