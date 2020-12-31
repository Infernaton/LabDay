using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    NPCState state;
    float idleTimer; //Keep track of the time
    int currentMovementPattern = 0;

    Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact() //We implement the function bc we used the interface
    {
        if (state == NPCState.Idle)
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    private void Update()
    {
        if (DialogManager.Instance.IsShowing) return; //If the NPC is talking, we don't want it to move

        if (state == NPCState.Idle) //We want the idle to long a certain time, then the NPC will move
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        yield return character.Move(movementPattern[currentMovementPattern]);
        currentMovementPattern = (currentMovementPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }

}

public enum NPCState { Idle, Walking }