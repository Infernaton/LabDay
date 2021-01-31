using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB //Db stands for Database
{
    public static void Init()
    {
        foreach (var kvp in Conditions) //Loop throught all elements in the dictionnary kvp = KeyValuePair, since we have key and value pairs in our script
        {
            var conditionId = kvp.Key; //store the key;
            var condition = kvp.Value; //then the value;

            condition.Id = conditionId; //we can later use this id easily 
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() //Static so we can acces without creating an instance of this class
    { //In here is the dictionnary (first brackets)
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                SartMessage = "est empoisonné",
                OnAfterTurn = (Pokemon pokemon) => //This create the function right into the Condition
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} souffre de son poison");
                }
            }
        },
        {
            ConditionID.brl,
            new Condition()
            {
                Name = "Brûlure",
                SartMessage = "est brûlé",
                OnAfterTurn = (Pokemon pokemon) => //This create the function right into the Condition
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} souffre de sa brûlure");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralisé",
                SartMessage = "est paralisé",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1,5) ==1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}est paralisé et ne peux plus bouger!");
                        return false;
                    }
                    else
                        return true; //True = will performe move, false = won't
                }
            }
        },
        {
            ConditionID.gel, //Same logic that paralyzed, but may disappear instead of happening on 1 on 4 chance
            new Condition()
            {
                Name = "Gel",
                SartMessage = "est gelé",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1,5) ==1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} n'est plus gelé!");
                        return true;
                    }
                    else
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} est bloqué dans la glace");
                        return false;
                }
            }
        },
        {
            ConditionID.som, //Same logic that paralyzed, but may disappear instead of happening on 1 on 4 chance
            new Condition()
            {
                Name = "Endormi",
                SartMessage = "s'est endormi",
                OnStart = (Pokemon pokemon) => //Calling the action to determine how many number will the pokemon sleep
                {
                    //Sleep for a random number of turns (2-3)
                    pokemon.StatusTime = Random.Range(2, 4);
                    Debug.Log($"sera endormi pour {pokemon.StatusTime} tours");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} s'est réveillé!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} dort profondément");
                    return false;
                }
            }
        },
        //Volatile status are down here
        {
            ConditionID.confusion, //Same logic that paralyzed, but may disappear instead of happening on 1 on 4 chance
            new Condition()
            {
                Name = "Confusion",
                SartMessage = "a été confus",
                OnStart = (Pokemon pokemon) => //Calling the action to determine how many number will the pokemon be confuse
                {
                    //Confused for a random number of turns (1-4)
                    pokemon.VolatileStatusTime = Random.Range(1, 5);
                    Debug.Log($"Sera confus pour {pokemon.VolatileStatusTime} tours");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} est sorti de sa confusion!");
                        return true;
                    }

                    pokemon.VolatileStatusTime--;
                    
                    //50% chance to do a move, or hit itself

                    //Play the move
                    if (Random.Range(1, 3) == 1)
                        return true;
                    //Hurt itself
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} est confus");
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} s'est bléssé dans sa confusion");
                    return false;
                }
            }
        },

    }; //In here is the dictionnary (last brackets)

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.som || condition.Id == ConditionID.gel)
            return 2f;
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brl)
            return 1.5f;

        return 1f;
    }
}

public enum ConditionID //Key of a dictionnary with all the conditions we have
{
    none, psn, brl, som, par, gel, //Poison, Burn, Sleep, Paralized, Frozen
    confusion
}
