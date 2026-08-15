using System.Collections.Generic;
using UnityEngine;

public class LampManager : MonoBehaviour
{
    private List<GameObject> Children = new List<GameObject>();
    private GOInteraction myGOI;

    void Start()
    {
    
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Information"))
            {
                Children.Add(child.gameObject);
            }
        }

        myGOI = GetComponent<GOInteraction>();
        if (myGOI == null)
        {
            Debug.Log("No GOInteraction attached to this object.");
        }

        foreach (GameObject child in Children)
        {
            child.SetActive(false);
        }
    }

    void Update()
    {
        if (myGOI != null && myGOI.Interaction == true)
        {
            foreach (GameObject child in Children)
            {
                child.SetActive(!child.activeSelf);
            }
            myGOI.Interaction = false; // Reset state after handling
        }
    }
}