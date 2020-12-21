using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB //Db stands for Database
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() //Static so we can acces without creating an instance of this class
    { //In here is the dictionnary (first brackets)

        { //In here we call each conditions (first brackets)

            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                SartMessage = "has been poisoned"
            }

        } //In here we call each conditions (last brackets)

    }; //In here is the dictionnary (last brackets)

}

public enum ConditionID //Key of a dictionnary with all the conditions we have
{
    none, psn, brn, slp, par, frz //Poison, Burn, Sleep, Paralized, Frozen
}
