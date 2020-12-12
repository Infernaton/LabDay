using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Here is where we manage our battle system, by calling every needed function, that we create in other classes

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy} //We will use different state in our BattleSystem, and show what need to be shown in a specific state

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction; //Actually, 0 is Fight, 1 is Run
    int currentMove; //We'll have 4 moves

    //We want to setup everything at the very first frame of the battle
    private void Start()
    {
        StartCoroutine(SetupBattle()); //We call our SetupBattle function
    }

    public IEnumerator SetupBattle() //We use the data created in the BattleUnit and BattleHud scripts
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        //We return the function Typedialog
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared."); //With the $, a string can show a special variable in it
        yield return new WaitForSeconds(1f);

        //This is the function where the player choose a specific action
        PlayerAction(); 
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction; //Change the state to PlayerAction
        StartCoroutine(dialogBox.TypeDialog("Choose an action")); //Then write a text
        dialogBox.EnableActionSelector(true); //Then allow player to choose an Action
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove; //Change the state to PlayerMove
        dialogBox.EnableActionSelector(false); //Then disable player to choose an action, and allow it to choose a move
        dialogBox.EnableDialogText(false); //Disable the DialogText
        dialogBox.EnableMoveSelector(true); //Enable the MoveSelector
    }

    private void Update()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection(); //Function to make the player able to choose an action
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow)) //For now we have two choices so we just want to increase or decrease the int currentAction
        {
            if (currentAction == 0)
                ++currentAction; 
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction == 1)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentAction == 0)
            {
                //Fight*
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                //Run
            }
        }
    }


    //We create a way to move freely beyond every moves our Creature actually has.
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) //The left and right keys moves by one, but the Up and Down have to move by 2
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);
    }
}
