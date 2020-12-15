using System;//Added this library to use Observer Design Pattern
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed; // Speed value
    public LayerMask solidObjectsLayer; //Reference our SolidObjects layer
    public LayerMask grassLayer; //Reference our LongGrass layer

    public event Action OnEncountered; //Creating an action with "using. System"

    private bool isMoving; // To know if the player is currently moving
    private Vector2 input; // For getting the Input

    private Animator animator;

    //With this void Awake, we set the Animator so it plays the animation of the direction the player asked
    private void Awake()
    {
        animator = GetComponent<Animator>(); //We set the Animator as our variable animator
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Get rid of Diagonal movement
            if (input.x != 0) input.y = 0;
            if (input.y != 0) input.x = 0;

            //While the player is not moving, we read the input, and move the player in the choosen direction
            if (input != Vector2.zero)
            {
                //if (directionCheck != )

                animator.SetFloat("moveX", input.x); //Link the moveX from the animator with the input.x of the code
                animator.SetFloat("moveY", input.y); //Same for the Y

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;


                if (IsWalkable(targetPos)) //once we checked the input, and if the tile is walkable, our player will move
                {
                    StartCoroutine(Move(targetPos)); 
                }
            }
        }
        animator.SetBool("isMoving", isMoving); //Link the Animator "isMoving" to the script "isMoving"
    }

    //This coroutine get us a smooth movement
    IEnumerator Move(Vector3 targetPos)
    {
        //We set isMoving to true in the beggining and false at the end
        isMoving = true;

        //this part get us a smooth movement, instead of juste moving tile to tile
        while ((targetPos - transform.position).sqrMagnitude >Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;

        CheckForEncounter();
    }

    //Function to know if the target tile is a solid objects or if we can walk on it, we get the target pos in the Update.
    private bool IsWalkable(Vector3 targetPos)
    {
        //Get to know if the targetPos tile is a solid object, within a radius of 0.2f, on the SolidObjects layer
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null) //Radius is 2f so with the Player position, it will look better before colliding with a solid object
        {
            return false;
        }

        else return true;
    }

    //Function to know if the player walk on a grass tile
    private void CheckForEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null) //Same as IsWalkable
        {
            if (UnityEngine.Random.Range(1, 101) <= 10) //If, within a range of 1 to 100, we hit below 10 (10% chances), we will encounter a creature
            {
                animator.SetBool("isMoving", false); //Link the Animator "isMoving" to the script "isMoving", and set it to false
                OnEncountered();//We call our BattleSystem by changing the GameState to battle
            }
        }
    }
}
