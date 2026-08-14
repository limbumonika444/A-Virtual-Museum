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
            Debug.LogError("No GOInteraction attached to this object.");
            return;
        }

        
        foreach (GameObject child in Children)
        {
            child.SetActive(false);
        }
    }

    void Update()
    {
        if (myGOI.LampInteraction)
        {

            foreach (GameObject child in Children)
            {
                child.SetActive(!child.activeSelf);
            }

            myGOI.LampInteraction = false;
        }
    }
}