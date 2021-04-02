using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    [SerializeField] bool Healer;

    NPCState state;
    float idleTimer; //Keep track of the time
    int currentMovementPattern = 0;

    Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact(Transform initiator) //We implement the function bc we used the interface
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog; //Change the state

            character.LookTowards(initiator.position); //Turn the npc towards the player

            if (Healer)
            {
                GameController.Instance.HealPlayerTeam();
            }

            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
                idleTimer = 0;
                state = NPCState.Idle; //Change back the state, when the action finish the dialog happen
            }));
        }
    }

    private void Update()
    {
        if (state == NPCState.Idle) //We want the idle to long a certain time, then the NPC will move, and not be able to move while a dialog is happening
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

        var oldPos = transform.position; //Keep track of the previous pos

        yield return character.Move(movementPattern[currentMovementPattern]);

        if (transform.position != oldPos)
            currentMovementPattern = (currentMovementPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walking, Dialog }