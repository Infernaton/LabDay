using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPokemonParty : MonoBehaviour, IMenuController
{
    [SerializeField] List<Text> options;
    [SerializeField] Color highlightedColor;

    PokemonParty pokemonParty;
    PartyMemberUI[] memberSlots; //Creating an array of our memberSlots
    List<Pokemon> pokemons;

    int currentSelection = 0;

    /*public void Init() //Function to not use a SerializedField, but to assign pokemons automaticly
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true); //Will return every children components attached in the PartyScreen
    }*/

    public void HandleUpdate(PokemonParty playerParty)
    {
        this.pokemonParty = playerParty;
        this.gameObject.SetActive(true);
        SetPartyData(pokemonParty.Pokemons);
        Action<int> onChoiceSelected = (choiceIndex2) =>
        {            
            if (choiceIndex2 == 0)
            {
                MenuController.Instance.ReturnToMainMenu();
            }
        };
        HandleChoiceSelection(onChoiceSelected);
    }

    private void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons; //Assigning variable as pokemons
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        for (int i = 0; i < memberSlots.Length; i++) //For every memberSlots in our array, we will apply this function to get the data of each one of them
        {
            if (i < pokemons.Count) //Befor calling the function we check how many pokemon we actually have
            {
                memberSlots[i].gameObject.SetActive(true); //Active every member slot anyway so we won't have an isssue whil catching a new pkmn
                memberSlots[i].SetData(pokemons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false); //If we don't get 6, we deactivate the last spots unused
        }
    }

    public void NotVisible()
    {
        gameObject.SetActive(false);
    }

    public void HandleChoiceSelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, options.Capacity-1);

        UpdateMenuUISelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            onSelected?.Invoke(currentSelection);
        }
        else if(Input.GetKeyDown(KeyCode.I))
        {
            onSelected?.Invoke(currentSelection);
            NotVisible();
        }
    }

    public void UpdateMenuUISelection(int selection) //Same logic as UpdateMoveSelection in BattleSystem.cs
    {
        for (int i = 0; i < options.Capacity; i++)
        {
            if (i == selection)
            {
                options[i].color = highlightedColor;
                /*if (moveTexts[i].text[0] != '>')
                    moveTexts[i].text = "> " + moveTexts[i].text;*/
            }
            else
            {
                options[i].color = Color.black;
                /*if (moveTexts[i].text[0] == '>')
                    moveTexts[i].text = moveTexts[i].text.Substring(2);*/
            }
        }
    }
}
