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
    [SerializeField] bool alwaysHits; //This bool is for moves that will always hit their target
    [SerializeField] int pp;
    [SerializeField] int priority; //Creating mvoes with priority
    [SerializeField] int nbHit = 1; //If a move hits multiple times
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaries; //Since a move can have mutliple secondary effects, we use a list
    [SerializeField] MoveTarget target; //We use enum instead of bool to easyly expends it later with double battles

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
    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }
    public int Pp
    {
        get { return pp; }
    }
    public int Priority
    {
        get { return priority; }
    }
    public int NumberHit
    {
        get { return nbHit; }
    }
    public MoveCategory Category //Property to get the move Category
    {
        get { return category; }
    }
    public MoveEffects Effects
    {
        get { return effects; }
    }
    public List<SecondaryEffects> Secondaries{
        get { return secondaries; }
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

[System.Serializable]//We can set it in Unity
public class SecondaryEffects : MoveEffects //Class for moves that have another effects (like Ember, has 10% chance to burn instead of JUST inflicting damage. It inherit from the MoveEffects class
{
    [SerializeField] int chance; //Often, secondary effect have only a small % of chance to appear, this is the chance to cause the effect
    [SerializeField] MoveTarget target; //Sometimes the secondary effects of moves can be on the target, or the source unit

    public int Chance //Properties to exposes em
    {
        get { return chance; }
    }
    public MoveTarget Target
    {
        get { return target; }
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