using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class will be used in every moving character, including NPC
public class Character : MonoBehaviour
{
    public float moveSpeed; // Speed value

    CharacterAnimator animator;
    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver=null) //We can check for an action only when needed (encounter for player, battle for trainers..)
    {
        animator.MoveX = moveVec.x; //Link the moveX from the animator with the moveVec.x of the code
        animator.MoveY = moveVec.y; //Same for the Y

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        if (!IsWalkable(targetPos))
            yield break;

        //We set IsMoving to true in the beggining and false at the end
        animator.IsMoving = true;

        //this part get us a smooth movement, instead of juste moving tile to tile
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        animator.IsMoving = false;

        OnMoveOver?.Invoke(); //We use this to check for encounter ONLY in the end of our Player movement, not the npc
    }

    //Function to know if the target tile is a solid objects or if we can walk on it, we get the target pos in the Update.
    private bool IsWalkable(Vector3 targetPos)
    {
        //if the targetPos tile is a solid object or Interactable tile, within a radius of 0.2f, on the SolidObjects layer
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null) //Radius is 2f so with the Player position, it will look better before colliding with a solid object
        {
            return false;
        }
        else return true;
    }

    public CharacterAnimator Animator { get => animator; }
}