using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Here is where we manage our battle system, by calling every needed function, that we create in other classes

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver} //We will use different state in our BattleSystem, and show what need to be shown in a specific state

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
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

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        //We return the function Typedialog
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared."); //With the $, a string can show a special variable in it

        //This is the function where the player choose a specific action
        ChooseFirstTurn(); 
    }

    void ChooseFirstTurn() //Function that will use the speed to determine who will play first
    {
        if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed) //This will change with prioritie moves etc
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
    }

    void BattleOver(bool won) //Function to know if the battle is over or not
    {
        state = BattleState.BattleOver; //Set the state
        playerParty.Pokemons.ForEach(p => p.OnBattleOver()); //Reset the stats of all our pokemons in a ForEach loop
        OnBattleOver(won); //Calling the event to notify the GameController that the battle is Over
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection; //Change the state to ActionSelection
        dialogBox.SetDialog("Choose an action"); //Then write a text
        dialogBox.EnableActionSelector(true); //Then allow player to choose an Action
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen; //Change the battle state to party screen
        partyScreen.SetPartyData(playerParty.Pokemons); //Set the data of our actual pokemons
        partyScreen.gameObject.SetActive(true); //Set active and visible our party screen
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection; //Change the state to MoveSelection
        dialogBox.EnableActionSelector(false); //Then disable player to choose an action, and allow it to choose a move
        dialogBox.EnableDialogText(false); //Disable the DialogText
        dialogBox.EnableMoveSelector(true); //Enable the MoveSelector
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove; //We set PerformMove so the player can not move in the UI

        var move = playerUnit.Pokemon.Moves[currentMove]; //we store in a variable, the actual move selected
        yield return RunMove(playerUnit, enemyUnit, move); //Calling the function to run the move selected

        //If the battle state was not changed by the RunMove, go to next step
        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove()); //Then the enemy attack
    }
    IEnumerator EnemyMove() 
    {
        state = BattleState.PerformMove; //We set PerformMove so the player can not move in the UI

        var move = enemyUnit.Pokemon.GetRandomMove(); //we store in a variable, a random move selected

        yield return RunMove(enemyUnit, playerUnit, move); //Calling the function to run the move selected

        //If the battle state was not changed by the RunMove, go to next step
        if (state == BattleState.PerformMove)
            ActionSelection(); //Then it's back to the player Action Selection phase
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move) //Creating a function with the logic of the moves, to easily change it later and make our code more clear
    {
        move.PP--; //Redcing PP of the move on use
        if (targetUnit == playerUnit) //If statement to show if the pokemon using a move is the player's one of the enemy
            yield return dialogBox.TypeDialog($"The enemy {sourceUnit.Pokemon.Base.Name} used {move.Base.Name}"); //We write to the player that a pokemon used a move
        else
            yield return dialogBox.TypeDialog($"Your {sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        sourceUnit.PlayAttackAnimation(); //Calling the attack animation right after displaying a message
        yield return new WaitForSeconds(0.75f); //Then wait for a second before reducing HP
        targetUnit.PlayHitAnimation();

        if (move.Base.Category == MoveCategory.Status)
        {
            yield return RuneMoveEffects(move, sourceUnit.Pokemon, targetUnit.Pokemon);
        }
        else
        {
            var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
            yield return targetUnit.Hud.UpdateHP(); //Calling the function to show damages taken
            yield return ShowDamageDetails(damageDetails);
        }

        //If a pokemon died, we display a message, then check if the battle is over or not
        if (targetUnit.Pokemon.HP <= 0) //Since with status move the pokemon can faint at mostly any moment, we don't check DamageDetails.fainted
        {
            if (targetUnit == enemyUnit)
             yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} enemy fainted");
            else
                yield return dialogBox.TypeDialog($"Your {targetUnit.Pokemon.Base.Name} fainted");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }
    }

    IEnumerator RuneMoveEffects(Move move, Pokemon source, Pokemon target) //Creating a function of the Effects move, so we'll call it easyly
    {
        var effects = move.Base.Effects; //Easier to call the effects

        if (effects.Boosts != null) //Call for stat boost
        {
            if (move.Base.Target == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        if (effects.Status != ConditionID.none) //Check from the dictionnary if there are any status condition, and call for status
        {
            target.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon) //Check if there are any messages in the Status changes queue, then show all of them in dialogBox
    {
        while (pokemon.StatusChanges.Count > 0) //Means that there is a message
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver (BattleUnit faintedUnit) //Logic to know if the fainted pokemon is the player's one or the enemy one, and so if the battle is over or not
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon(); //Store in a var out next pokemon
            if (nextPokemon != null) //we open the party screen when a pokemon of us fainted, and we still have at least one healthy pokemon
                OpenPartyScreen();
            else
                BattleOver(false); //False is when the player lost
        }
        else
            BattleOver(true); //True when the player won
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
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection(); //Function to make the player able to choose an action
        }
        else if (state == BattleState.MoveSelection)
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
                MoveSelection();
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

            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
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
            ActionSelection();
        }
    }

    IEnumerator SwitchPokemon (Pokemon newPokemon) //Coroutine to make the switch happen
    {
        bool currentPokemonFainted = true; //Bool we'll use to determine if we switch bc of a pokemon faint, or bc it's a choice

        if (playerUnit.Pokemon.HP > 0) //This will play ONLY if the player change pokemon by choosing the action, if the pokemon fainted and we have to send another one, this will not play
        {
            currentPokemonFainted = false;
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}"); //First we change the message
            playerUnit.PlayFaintAnimation(); //Then we play the faint animation to show that our pokemon came back
            yield return new WaitForSeconds(1f); //Then we wait before it ends
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        if (currentPokemonFainted)
            ChooseFirstTurn();
        else
            StartCoroutine(EnemyMove());
    }
}