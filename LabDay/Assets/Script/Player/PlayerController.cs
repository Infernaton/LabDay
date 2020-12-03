using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed; //Speed value
    private bool isMoving; //Boolean to know if the player is currently moving
    private Vector3 origPos, targetPos; //Vector that we'll use to know what direction has been choosen
    private float timeToMove = 0.2f;

    private void Update()
    {
        //While the player is not moving, we read the input, and move the player in the good direction
        if (!isMoving)
        {
            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
            {
                StartCoroutine(MovePlayer(Vector3.up));
            }

            else if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
            {
                StartCoroutine(MovePlayer(Vector3.left));
            }

            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                StartCoroutine(MovePlayer(Vector3.down));
            }

            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                StartCoroutine(MovePlayer(Vector3.right));
            }
        }
    }

    void GoUp()
    { }

    //This coroutine get us a smooth movement
    private IEnumerator MovePlayer(Vector3 direction)
    {
        //We set isMoving to true in the beggining and false at the end
        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        //this part get us a smooth movement, instead of juste moving tile to tile
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //We want to be sure that in the end, the player is on the targetPos, and this will rarely occur, this will just happen on lags
        transform.position = targetPos;

        isMoving = false;
    }
}
