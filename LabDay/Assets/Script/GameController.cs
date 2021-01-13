using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We'll use GameStates to switch beetween Scenes (Overworld, Battle, etc)
public enum GameState { FreeRoam, Battle, Dialog, Cutscene } //List every states we'll use
public class GameController : MonoBehaviour
{
    GameState state;//Reference to our GameState
    [SerializeField] PlayerController playerController;//Reference to the PlayerController script
    [SerializeField] BattleSystem battleSystem;//Reference to the BattleSystem Script 
    [SerializeField] Camera worldCamera; //Reference to our Camera

    public static GameController Instance { get; private set; } //Get reference from the game controller anywhere we want

    private void Awake()
    {
        Instance = this;
        ConditionsDB.Init();
    }

    //On the first frame we check if we enable our Overworld script, or the battle one
    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerController.OnEnterTrainersView += (Collider2D trainerCollider) => //Use this collider to get reference of the trainerController
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
        };

        DialogManager.Instance.OnShowDialog += () => //Change the state to dialog so the player won't be able to move will a dialog appears
        {
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    //Change our battle state, camera active, and gameobject of the Battle System
    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>(); //Store our party in a var
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon(); //Store a random wild pokemon FROM our map area in a var

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level); //Create a copy of the pokemon in the case the player want to catch it

        battleSystem.StartBattle(playerParty, wildPokemonCopy); //Call our StartBattle, so every fight are not the same
    }

    TrainerController trainer; //Reference the trainer

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer; //Set THIS specific trainer as our reference

        var playerParty = playerController.GetComponent<PokemonParty>(); //Store our party in a var
        var trainerParty = trainer.GetComponent<PokemonParty>(); //Store the trainer party in a var

        battleSystem.StartTrainerBattle(playerParty, trainerParty); //Call our StartBattle, so every fight are not the same
    }

    //Change our battle state, camera active, and gameobject of the Battle System
    void EndBattle(bool won)
    {
        if (trainer != null && won == true) //If it is a trainer battle, won by the player
        {
            trainer.BattleLost(); //Disable the fov, to disable the battle
            trainer = null;
        }


        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    public void ChangeTab()
    {
        Debug.Log("Change to tab 2");
    }

    private void Update()
    {
        if (state == GameState.FreeRoam) //While we are in the overworld, we use our PlayerController script
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle) //Else if we are in a battle, we'll disable our PlayerController Script
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }
}