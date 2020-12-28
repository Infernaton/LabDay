using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    public void Interact() //We implement the function bc we used the interface
    {
        Debug.Log("Interact");
    }
}
