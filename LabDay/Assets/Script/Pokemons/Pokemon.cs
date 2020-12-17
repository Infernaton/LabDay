using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// !!! THIS SCRIPT IS GOING TO USE THE DATA FROM THE SCRIPTABLE OBJECT TO CREATE THE REAL POKEMON, WITH LEVEL, AND HOW IT'S GROWING IN REAL TIME IN THE GAME!!!
// THESES DATA ARE MEANT TO CHANGE IN THE GAME

[System.Serializable] //To show a class in unity we need to set this
public class Pokemon
{
    [SerializeField] PokemonBase _base; //Setting a serialized field of it to acces it in unity
    [SerializeField] int level;

    public PokemonBase Base //we call this variable Name bc we use it as a property. This variable is going to use the base data
    {
        get { return _base; }
    } 
    public int Level //We set theses two as public to acces them outside this class (in the BattleHub for example.)
    {
        get { return level; }
    }

    public int HP { get; set; }

    public List<Move> Moves { get; set; } //This is a reference to our List of move the pokemon will have in game

    public void Init() //Constructor of our pokemons, pBase = Pokemon Base, pLevel = Pokemon Level
    {
        HP = MaxHp;

        //This genretae a move
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves) //For each move in LearnableMove, if the Level of the pokemon is greater than the level of the Learnable move, we acces the Moves and add 1, but if there is already 4 moves or more, we don't.
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }
            if (Moves.Count >= 4)
                break;
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack) / 100f) + 5; } //This is the formula used in pokemon games to get the stats at any level
        //FloorToInt is used to get rid of the decimal point
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense) / 100f) + 5; } 
    }
    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack) / 100f) + 5; }
    }
    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense) / 100f) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed) / 100f) + 5; }
    }

    //MaxHp use a slightly different formula
    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp) / 100f) + 10; }
    }

    //Create a function called when taking damage
    public DamageDetails TakeDamage(Move move, Pokemon attacker) //Take in reference the move used, and the attacking pokemon
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.52) //Creating critical hits
            critical = 2f; //If the random number is under 6.52 we'll set the critical value to 2, so in the modifiers, there will be a *2

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2); //Calculate the type effectiveness

        var damageDetails = new DamageDetails() //Creating an object of our class
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        //We check if the move we're about to get it by is Special or not
        float attack = (move.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack; //if isSpecial is true we'll get SpAttack, else we'll get the attack normal
        float defense = (move.Base.IsSpecial) ? SpDefense : Defense;

        //Here is the actual formula used in the pokemons game, might change later
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        //After that we substract the damage to the actual life of the pokemon, and check if he died or no
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true; //Set Fainted to true if he died
        }

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails //Class we'll use to display a message if there was a critical hit, super effective or not..
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }

}
