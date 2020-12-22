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
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                SartMessage = "has been paralyzed",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1,5) ==1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s paralyzed and can not move!");
                        return false;
                    }
                    else
                        return true; //True = will performe move, false = won't
                }
            }
        },
        {
            ConditionID.frz, //Same logic that paralyzed, but may disappear instead of happening on 1 on 4 chance
            new Condition()
            {
                Name = "Freeze",
                SartMessage = "has been frozen",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1,5) ==1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s not frozen anymore!");
                        return true;
                    }
                    else
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'is frozen and can not move!");
                        return false;
                }
            }
        },
        {
            ConditionID.slp, //Same logic that paralyzed, but may disappear instead of happening on 1 on 4 chance
            new Condition()
            {
                Name = "Sleep",
                SartMessage = "has fallen asleep",
                OnStart = (Pokemon pokemon) => //Calling the action to determine how many number will the pokemon sleep
                {
                    //Sleep for a random number of turns (1-3)
                    pokemon.StatusTime = Random.Range(1, 4);
                    Debug.Log($"Will be asleep for {pokemon.StatusTime} turns");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
                    return false;
                }
            }
        },

    }; //In here is the dictionnary (last brackets)
}

public enum ConditionID //Key of a dictionnary with all the conditions we have
{
    none, psn, brn, slp, par, frz //Poison, Burn, Sleep, Paralized, Frozen
}
