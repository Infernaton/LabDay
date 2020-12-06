using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed; // Speed value
    public LayerMask solidObjectsLayer; //Reference to our SolidObjects layer

    private bool isMoving; // To know if the player is currently moving
    private Vector2 input; // For getting the Input
    private Vector2 currentDir; // For storing direction

    private Animator animator;

    //With this void Awake, we set the Animator so it plays the animation of the direction the player asked
    private void Awake()
    {
        animator = GetComponent<Animator>(); //We set the Animator as our variable animator
    }

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Get rid of Diagonal movement
            if (input.x != 0) input.y = 0;
            else if (input.y != 0) input.x = 0;

            //While the player is not moving, we read the input, and move the player in the choosen direction
            if (input != Vector2.zero)
            {
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
    }

    //Function to know if the target tile is a solid objects or if we can walk on it, we get the target pos in the Update.
    private bool IsWalkable(Vector3 targetPos)
    {
        //Get to know if the targetPos tile is a solid object, within a radius of 0,3f, on the SolidObjects layer
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null) //Radius is 2f so with the Player position, it will look better before colliding with a solid object
        {
            return false;
        }

        else return true;
    }
}
