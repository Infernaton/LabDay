using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is to write the actual dialog
[System.Serializable]
public class Dialog //We use a class bc in the future we want the NPC to give items, quest, etc.. and it'll be easier to add
{
    [SerializeField] List<string> lines; //This will be the dialog that we'll show

    public List<string> Lines
    {
        get { return lines; }
    }
}
