using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Trainer controller script
public class TrainerController : MonoBehaviour
{
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation; //Reference the exclamation point

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        //Show exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        //Walk trainer towards player
        var diff = player.transform.position - transform.position; //Get the difference of vector between the trainer and the player position, to make it move right next to the player
        var moveVec = diff - diff.normalized; //Substract one tile to get the right position
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y)); //Since we work with tiles, we want it to be an int, and not a float

        yield return character.Move(moveVec);

        //Show dialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            //Start the battle
            Debug.Log("Start trainer Battle");
        }));
    }
}
