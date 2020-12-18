using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Here is where we manage our battle system, by calling every needed function, that we create in other classes

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy, PartyScreen} //We will use different state in our BattleSystem, and show what need to be shown in a specific state

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver; //Add an action happening when the battle ended (Action<bool> is to add a bool to the Action)

    BattleState state;

    //These var will be used to navigate throught selection screens
    int currentAction; //Actually, 0 is Fight, 1 is Bag, 2 is Party, 4 is Run
    int currentMove; //We'll have 4 moves
    int currentMember; //We have 6 pokemons

    PokemonParty playerParty;
    Pokemon wildPokemon;

    //We want to setup everything at the very first frame of the battle
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty; //this. is to use our variable and not the parameter
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle()); //We call our SetupBattle function
    }

    public IEnumerator SetupBattle() //We use the data created in the BattleUnit and BattleHud scripts
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        //We return the function Typedialog
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared."); //With the $, a string can show a special variable in it

        //This is the function where the player choose a specific action
        PlayerAction(); 
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction; //Change the state to PlayerAction
        dialogBox.SetDialog("Choose an action"); //Then write a text
        dialogBox.EnableActionSelector(true); //Then allow player to choose an Action
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen; //Change the battle state to party screen
        partyScreen.SetPartyData(playerParty.Pokemons); //Set the data of our actual pokemons
        partyScreen.gameObject.SetActive(true); //Set active and visible our party screen
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove; //Change the state to PlayerMove
        dialogBox.EnableActionSelector(false); //Then disable player to choose an action, and allow it to choose a move
        dialogBox.EnableDialogText(false); //Disable the DialogText
        dialogBox.EnableMoveSelector(true); //Enable the MoveSelector
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;//We set Busy so the player can not move in the UI

        var move = playerUnit.Pokemon.Moves[currentMove]; //we store in a variable, the actual move selected
        move.PP--; //Redcing PP of the move on use
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}"); //We write to the player that it's pokemon used a move

        playerUnit.PlayAttackAnimation(); //Calling the attack animation right after displaying a message
        yield return new WaitForSeconds(0.75f); //Then wait for a second before reducing HP

        enemyUnit.PlayHitAnimation();

        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHP(); //Calling the function to show damages taken
        yield return ShowDamageDetails(damageDetails);

        //If the enemy died, we display a message, else we call it's attack
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"The {enemyUnit.Pokemon.Base.Name} enemy fainted");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true); //True is to know that the player won
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
    IEnumerator EnemyMove() 
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        move.PP--; //Redcing PP of the move on use
        yield return dialogBox.TypeDialog($" The enemy {enemyUnit.Pokemon.Base.Name} used {move.Base.Name}"); //We write to the player that the enemy pokemon used a move

        enemyUnit.PlayAttackAnimation(); //Calling the attack animation right after displaying a message
        yield return new WaitForSeconds(0.75f); //Then wait for a second before reducing HP

        playerUnit.PlayHitAnimation();

        var damageDetails = playerUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon); //Check our DamageDetails var to know if the Player pokemon died
        yield return playerHud.UpdateHP(); //Calling the function to show damages taken
        yield return ShowDamageDetails(damageDetails);

        //If the enemy died, we display a message, else we call it's attack
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"Your {playerUnit.Pokemon.Base.Name} fainted");
            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

            var nextPokemon = playerParty.GetHealthyPokemon(); //Store in a var out next pokemon
            if (nextPokemon != null) //we open the party screen when a pokemon of us fainted, and we still have at least one healthy pokemon
            {
                OpenPartyScreen();
            }
            else
            {
                OnBattleOver(false); //False is to know that the player lost
            }
        }
        else
        {
            PlayerAction();
        }
    }
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f) //Check the value of Critical to show a message saying we had a critical hit
            yield return dialogBox.TypeDialog("A critical hit!");

        if(damageDetails.TypeEffectiveness > 1)
            yield return dialogBox.TypeDialog("That's super effective!");
        else if (damageDetails.TypeEffectiveness < 1)
            yield return dialogBox.TypeDialog("That was not very effective..");

    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection(); //Function to make the player able to choose an action
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3); //Since we have 4 actions we want to loop throught each one of them and not going beyond 3

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
                //Bag
            }
            else if (currentAction == 2)
            {
                //Pokemon party
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //Run
            }
        }
    }

    //We create a way to move freely between every moves our Creature actually has.
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1); //Since we have ""player unit" number of moves we want to loop throught each one of them and not going beyond

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        //Here we'll make the move happen
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            //First we change the box to dialogText
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);

            StartCoroutine(PerformPlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            PlayerAction();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 3;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 3;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            var selectedMember = playerParty.Pokemons[currentMember]; //Creating a var of the actual pokemon we are on
            if (selectedMember.HP <= 0) //Making sure the actual pokemon selected ain't fainted
            {
                partyScreen.SetMessageText("You can't send out a fainted pokemon!");
                return;
            }
            if (selectedMember == playerUnit.Pokemon) //Making sure the actual selected pokemon is not the same as the one in the battle
            {
                partyScreen.SetMessageText("This pokemon is already in the battle.");
                return;
            }

            partyScreen.gameObject.SetActive(false); //Changing the actual view on the screen
            state = BattleState.Busy; //State is changed to busy so player won't mess with the UI
            StartCoroutine(SwitchPokemon(selectedMember)); //Finally, calling coroutine to switch pokemons
        }
        else if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            partyScreen.gameObject.SetActive(false);
            PlayerAction();
        }
    }

    IEnumerator SwitchPokemon (Pokemon newPokemon) //Coroutine to make the switch happen
    {
        if (playerUnit.Pokemon.HP > 0) //This will play ONLY if the player change pokemon by choosing the action, if the pokemon fainted and we have to send another one, this will not play
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}"); //First we change the message
            playerUnit.PlayFaintAnimation(); //Then we play the faint animation to show that our pokemon came back
            yield return new WaitForSeconds(1f); //Then we wait before it ends
        }

        playerUnit.Setup(newPokemon);
        playerHud.SetData(newPokemon);

        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        StartCoroutine(EnemyMove());
    }
}
