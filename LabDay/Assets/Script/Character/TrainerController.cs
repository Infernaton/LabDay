using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Trainer controller script
public class TrainerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation; //Reference the exclamation point
    [SerializeField] GameObject fov; //Reference the fov

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }
    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection); //Call the function to change the fov
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
            GameController.Instance.StartTrainerBattle(this);
        }));
    }

    //Set the fov direction
    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f; //Angle of the fov
        //Set the angle according to the trainer rotation
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;
        else if (dir == FacingDirection.Down)
            angle = 0f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle); //Set the rotation on, the Z axis, as the angle defined earlier
    }

    //Properties to expose names and sprites
    public string Name {
        get => name;
    }
    public Sprite Sprite {
        get => sprite;
    }
}