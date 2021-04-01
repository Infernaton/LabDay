using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeePokemonParty : MonoBehaviour, IMenuController
{
    [SerializeField] List<Text> options;
    [SerializeField] Color highlightedColor;

    int currentSelection = 0;

    public void HandleUpdate()
    {
        this.gameObject.SetActive(true);
        Action<int> onChoiceSelected = (choiceIndex2) =>
        {            
            if (choiceIndex2 == 0)
            {
                MenuController.Instance.ReturnToMainMenu();
            }
        };
        HandleChoiceSelection(onChoiceSelected);
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

        currentSelection = Mathf.Clamp(currentSelection, 0, options.Capacity);

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
