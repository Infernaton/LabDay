using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB //Db stands for Database
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() //Static so we can acces without creating an instance of this class
    { //In here is the dictionnary (first brackets)
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                SartMessage = "has been poisoned",
                OnAfterTurn = (Pokemon pokemon) => //This create the function right into the Condition
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} got hurt by poison");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                SartMessage = "has been burned",
                OnAfterTurn = (Pokemon pokemon) => //This create the function right into the Condition
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} got hurt by burn");
                }
            }
        }

    }; //In here is the dictionnary (last brackets)
}

public enum ConditionID //Key of a dictionnary with all the conditions we have
{
    none, psn, brn, slp, par, frz //Poison, Burn, Sleep, Paralized, Frozen
}
