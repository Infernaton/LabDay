using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuOption { Off, Main, PokemonParty, }

public class MenuController : MonoBehaviour, IMenuController
{

    [SerializeField] Color highlightedColor;
    [SerializeField] List<Text> menuChoices;
    [SerializeField] MenuPokemonParty menuPokemonParty;

    int currentSelection = 0;

    MenuOption state = MenuOption.Main;

    public static MenuController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public void NotVisible()
    {
        gameObject.SetActive(false);
    }

    public void HandleUpdate(PlayerController playerController)
    {
        switch (state)
        {
            case MenuOption.Main:
                menuPokemonParty.NotVisible();
                Action<int> onChoiceSelected = (choiceIndex) => //This part is all a reference to the action used in the HandleMoveSelection
                {
                    switch (choiceIndex)
                    {
                        //Open the Pokemon Party menu
                        case 0:
                            state = MenuOption.PokemonParty;
                            break;
                        //Open the Pokedex
                        /*case 1:
                            break;
                        //Open the backPack
                        case 2:
                            break;
                        //Save the game
                        case 3:
                            break;
                        //Open option Menu
                        case 4:
                            break;*/
                        default:
                            Debug.Log($"Coming Soon {choiceIndex}");
                            break;
                    }
                };
                HandleChoiceSelection(onChoiceSelected);
                break;

            case MenuOption.PokemonParty:
                NotVisible();
                var playerParty = playerController.GetComponent<PokemonParty>();
                menuPokemonParty.HandleUpdate(playerParty);
                break;

            default:
                break;
        }
    }

    public void ReturnToMainMenu()
    {
        Instance.gameObject.SetActive(true);
        state = MenuOption.Main;
    }

    public void HandleChoiceSelection(Action<int> onSelected) //Same logic as every other Handle*** 
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, menuChoices.Capacity);

        UpdateMenuUISelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            onSelected?.Invoke(currentSelection);
        }
    }

    public void UpdateMenuUISelection(int selection) //Same logic as UpdateMoveSelection in BattleSystem.cs
    {
        for (int i = 0; i < menuChoices.Capacity; i++)
        {
            if (i == selection)
            {
                menuChoices[i].color = highlightedColor;
                if (menuChoices[i].text[0] != '>')
                    menuChoices[i].text = "> " + menuChoices[i].text;
            }
            else
            {
                menuChoices[i].color = Color.black;
                if (menuChoices[i].text[0] == '>')
                    menuChoices[i].text = menuChoices[i].text.Substring(2);
            }
        }
    }
}
