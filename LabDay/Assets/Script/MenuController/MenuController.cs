using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [SerializeField] Color highlightedColor;
    [SerializeField] List<Text> menuChoices;

    int currentSelection = 0;

    SeePokemonParty seePokemonParty;

    //public void Awake(){}
    public void HandleUpdate()
    {
        //TODO : Gérer le menu
        
        Action<int> onChoiceSelected = (choiceIndex) => //This part is all a reference to the action used in the HandleMoveSelection
        {
            switch (choiceIndex)
            {
                //Open the Pokemon Party menu
                case 0:
                    seePokemonParty = GameObject.Find("SeePkmnParty").GetComponent<SeePokemonParty>();
                    seePokemonParty.HandleUpdate();
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
                    Debug.Log("Coming Soon");
                    break;
            }
        };
        HandleMoveSelection(onChoiceSelected);
    }

    public void HandleMoveSelection(Action<int> onSelected) //Same logic as every other Handle*** 
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, PokemonBase.MaxNumberOfMoves);

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
