using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject //Voir PokemonBase
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target; //We use enum instead of bool to easyly expends it later 

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return Description; }
    }
    public PokemonType Type
    {
        get { return type; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public int Pp
    {
        get { return pp; }
    }

    public MoveCategory Category //Property to get the move Category
    {
        get { return category; }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
//New class we'll use to get the effects a move can apply
public class MoveEffects 
{
    [SerializeField] List<StatBoost> boosts; //Since Unity can't Serialize a dictionnary, we create another class called StatBoost, and call it as a List
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;
    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
    public ConditionID Status //Property to expose it
    {
        get { return status; }
    }
    public ConditionID VolatileStatus //same as status
    {
        get { return volatileStatus; }
    }
}

[System.Serializable]
public class StatBoost
{
    //Two var for the Stat and the boos
    public Stat stat;
    public int boost;
}

public enum MoveCategory //Differents categories of moves, Physical, Special, and StatsBoosting
{
    Physical, Special, Status
}

public enum MoveTarget //Target of the move
{
    Foe, Self
}