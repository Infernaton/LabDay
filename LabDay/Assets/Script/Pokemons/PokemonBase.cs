using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// !!! THIS SCRIPT IS THE BASE FROM ALL POKEMON, WITH STATS, TYPES, SPRITES, ETC, NOT THE ACTUAL POKEMONS !!!

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")] //We create a menu in Unity to acces this blueprint
public class PokemonBase : ScriptableObject //Changed from "MonoBehavior" to "ScriptableObject" because we'll do some blueprints with theses datas
{
    // Inside we'll create variables to store the data of the pokemons

    // SerializedField is used instead of public, so we can use them in some other classes more easily, and modify them in Unity, or others scripts
    [SerializeField]string name;

    [TextArea] //This will give us some space to write a description
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    //Base stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMoves> learnableMoves;

    //Here we'll use Properties instead of Functions so it will not look like
    // public string GetName()
    //{
    //  return name;
    //}
    //But like :

    public string Name
    {
        get { return name; } //Set a getter of the name so we could just call pBase.name in the Pokemon script.
    }
    public string Description
    {
        get { return description; }
    }
    public int MaxHp
    {
        get { return maxHp; }
    }
    public int Attack
    {
        get { return attack; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int SpAttack
    {
        get { return spAttack; }
    }
    public int SpDefense
    {
        get { return spDefense; }
    }
    public int Speed
    {
        get { return speed; }
    }
    public List<LearnableMoves> LearnableMoves
    {
        get { return learnableMoves; }
    }
}

//Here we set the moves a pokemon can learn
[System.Serializable]
public class LearnableMoves
{
    [SerializeField] MoveBase moveBase; //This is a reference to the MoveBase script
    [SerializeField] int level;

    //here we get the Move that will be learnable, and the level at wich it'll be learnable
    public MoveBase Base
    {
        get { return moveBase; } 
    }
    public int Level
    {
        get { return level; }
    }
}

public enum PokemonType //Using an enum to acces all the pokemon types easily
{
    None,
    Bug,
    Dark,
    Dragon,
    Electric,
    Fairy,
    Fighting,
    Fire,
    Flying,
    Ghost,
    Grass,
    Ground,
    Ice,
    Normal,
    Poison,
    Psychic,
    Rock,
    Steel,
    Water,
}
