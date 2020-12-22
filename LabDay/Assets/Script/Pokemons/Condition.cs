using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Condition //This script is for
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string SartMessage { get; set; }
    public Action<Pokemon> OnStart { get; set; }
    public Func<Pokemon, bool> OnBeforeMove { get; set; } //Since this is an Action that will return a value (bool), we use Func
    public Action<Pokemon> OnAfterTurn { get; set; } //Create an action AND a property to be called at the end of a turn
}
