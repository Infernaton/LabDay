using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //We use this library bc we actually work with UI


public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;


    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTexts; //We use List bc we want the player to choose between several actions
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    //This function will set the dialog when we call it
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    //This coroutine will make it looks like the dialog is writing ahead.
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray()) //For each letter in our dialog text
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond); //We can choose how many letter per second we want, in Unity itself
        }
    }
    //This will change wich dialog we should see, either Dialog, Move selector, etc
    public void EnableDialogText (bool enabled) 
    {
        dialogText.enabled = enabled;
    }
    public void EnableActionSelector (bool enabled)
    {
        actionSelector.SetActive(enabled); //Since actionSelector is a GameObject and not a text, we have to use the SetActive() function
    }
    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled); //Since moveSelector is a GameObject and not a text, we have to use the SetActive() function
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction) //This will actually change the color of the actionSelector selected
    {
        for (int i=0; i<actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;     
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move) //This function is the same as UpdateActionSelection, but for the moves
    {
        for (int i=0; i<moveTexts.Count; ++i)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP}/{move.Base.Pp}"; //We show how many Pp are lefts
        typeText.text = move.Base.Type.ToString(); //We also convert our enumType in a String, to show wich type is the actual Move
    }

    public void SetMoveNames(List<Move> moves)
    {
        //Loop throught our MovesText
        for (int i=0; i<moveTexts.Count; ++i) //We usually have 4 mouves, but sometimes we can have less
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.Name; //Set the move in our Selector, to the name of our move
            }
            else
                moveTexts[i].text = "-";
        }
    }
}
