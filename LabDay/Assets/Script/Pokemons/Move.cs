using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Here are the data that will change during a battle, or within the game itself
public class Move 
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.Pp;
    }
}
