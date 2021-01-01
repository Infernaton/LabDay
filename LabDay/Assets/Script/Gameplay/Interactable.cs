using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This interface is here so we don't have to call everything we interact with in the PlayerController script, we just call this interface
public interface Interactable //We use interface so we can create classes that implement this interface, and all the functions within
{
    void Interact(Transform initiator);
}
