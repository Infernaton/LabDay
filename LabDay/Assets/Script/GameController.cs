using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We'll use GameStates to switch beetween Scenes (Overworld, Battle, etc)

public enum GameState { FreeRoam, Battle, Dialog} //For now we have just two states
public class GameController : MonoBehaviour
{
    GameState state;//Reference to our GameState
    [SerializeField] PlayerController playerController;//Reference to the PlayerController script
    [SerializeField] BattleSystem battleSystem;//Reference to the BattleSystem Script 
    [SerializeField] Camera worldCamera; //Reference to our Camera

    private void Awake()
    {
        ConditionsDB.Init();
    }

    //On the first frame we check if we enable our Overworld script, or the battle one
    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

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

        battleSystem.StartBattle(playerParty, wildPokemon); //Call our StartBattle, so every fight are not the same
    }
    //Change our battle state, camera active, and gameobject of the Battle System
    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
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
