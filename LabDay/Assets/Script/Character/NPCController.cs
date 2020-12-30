using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    public void Interact() //We implement the function bc we used the interface
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
