using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Color highlightedColor;

    int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i<currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].Name; //Set the name of the moves already known
        }
        moveTexts[currentMoves.Count].text = newMove.Name; //Set the name of the new move
    }

    public void HandleMoveSelection(Action<int> onSelected) //Same logic as every other Handle***
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, PokemonBase.MaxNumberOfMoves);

        UpdateMoveUISelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            onSelected?.Invoke(currentSelection);
        }
    }

    public void UpdateMoveUISelection (int selection) //Same logic as UpdateMoveSelection in BattleSystem.cs
    {
        for (int i = 0; i < PokemonBase.MaxNumberOfMoves +1; i++)
        {
            if (i == selection)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }
    }
}
