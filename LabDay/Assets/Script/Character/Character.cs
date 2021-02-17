using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class will be used in every moving character, including NPC
public class Character : MonoBehaviour
{
    public float moveSpeed; // Speed value
    public float runSpeed; // Run Speed value
    public bool IsMoving { get; private set; }
    public bool IsRunning;

    CharacterAnimator animator;
    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver=null) //We can check for an action only when needed (encounter for player, battle for trainers..)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f); //Link the moveX from the animator with the moveVec.x of the code, clamp it for the NPC 
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f); //Same for the Y

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        if (!IsPathClear(targetPos))
            yield break;

        //We set IsMoving to true in the beggining and false at the end
        IsMoving = true;

        //this part get us a smooth movement, instead of juste moving tile to tile
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            IsRunning = false;

            if (Input.GetKeyDown(KeyCode.B))
                { 
                    IsRunning = true;
                    moveSpeed = runSpeed;
                }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        IsMoving = false;

        OnMoveOver?.Invoke(); //We use this to check for encounter ONLY in the end of our Player movement, not the npc
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos) //Know if there is a solidObject, character, or other, in the path
    {
        var diff = targetPos - transform.position; //Value of the direction beetween the actual position, and the target position
        var dir = diff.normalized; //Return a new vector with the same Direction, but with a lenghht of 1 

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer) == true)
            return false;
        else
            return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        //Fin the difference beetween X and Y coordinates
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
            Debug.LogError("Error in Look Towards : You can't ask the character to look diagonaly");
    }

    public CharacterAnimator Animator { get => animator; }
}