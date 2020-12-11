using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// !!! THIS SCRIPT IS GOING TO USE THE DATA FROM THE SCRIPTABLE OBJECT TO CREATE THE REAL POKEMON, WITH LEVEL, AND HOW IT'S GROWING IN REAL TIME IN THE GAME!!!
// THESES DATA ARE MEANT TO CHANGE IN THE GAME
public class Pokemon
{
    public PokemonBase Base {get; set;} //we call this variable Name bc we use it as a property. This variable is going to use the base data
    public int Level { get; set; } //We set theses two as public to acces them outside this class (in the BattleHub for example.)

    public int HP { get; set; }

    public List<Move> Moves { get; set; } //This is a reference to our List of move the pokemon will have in game

    public Pokemon(PokemonBase pBase, int pLevel) //Constructor of our pokemons, pBase = Pokemon Base, pLevel = Pokemon Level
    {
        Base = pBase;
        Level = pLevel;
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
}
