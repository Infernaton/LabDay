using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox; //Reference the dialog box
    [SerializeField] Text dialogText; //Reference the text in the game
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog; //Create two action for closing and oppening the dialog box
    public event Action OnCloseDialog;

    public static DialogManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    Dialog dialog; //Reference the dialog object so we can use it in this script
    int currentLine = 0; //The first line is the 0
    bool isTyping; //Var to track what happen in the dialog box

    public bool IsShowing { get; private set; }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame(); //This is to avoid issues with key pressed while interacting and not being in the dialog state 

        OnShowDialog?.Invoke(); //Call the action to show dialog

        IsShowing = true;
        this.dialog = dialog;
        dialogBox.SetActive(true); //First we active the dialogBox
        StartCoroutine(TypeDialog(dialog.Lines[0])); //This will show the first line of the dialog
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (!isTyping)
            {
                ++currentLine; //increase the number of thel line we show
                if (currentLine < dialog.Lines.Count) //If there still is some lines to show
                {
                    StartCoroutine(TypeDialog(dialog.Lines[currentLine])); //Write the current line
                }
                else //If there is no more lines to show, we close the dialog
                {
                    currentLine = 0; //Set back to 0, so the next dialog will start from 0
                    IsShowing = false;
                    dialogBox.SetActive(false); //Deactivate the dialog box
                    OnCloseDialog?.Invoke(); //Call the close dialog action, to change back the gameState
                }
            }
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true; //Set typing to true at the beggining

        dialogText.text = "";
        foreach (var letter in line.ToCharArray()) //For each letter in our dialog text
        {
            dialogText.text += letter; //Add them 1 by 1
            yield return new WaitForSeconds(1f / lettersPerSecond); //We can choose how many letter per second we want, in Unity itself
        }

        isTyping = false; //Set it back to false at the end
    }
}
