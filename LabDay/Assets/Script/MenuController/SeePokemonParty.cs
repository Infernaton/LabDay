using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeePokemonParty : MonoBehaviour
{
    [SerializeField] List<Text> options;
    [SerializeField] Color highlightedColor;

    int currentSelection = -1;

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

    public void HandleChoiceSelection(Action<int> onSelected) //Same logic as every other Handle***
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, options.Capacity);

        UpdateMoveUISelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            onSelected?.Invoke(currentSelection);
        }
    }

    public void UpdateMoveUISelection(int selection) //Same logic as UpdateMoveSelection in BattleSystem.cs
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
