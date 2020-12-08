using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// !!! THIS SCRIPT IS GOING TO USE THE DATA FROM THE SCRIPTABLE OBJECT TO CREATE THE REAL POKEMON, WITH LEVEL, AND HOW IT'S GROWING !!!
public class Pokemon
{
    PokemonBase _base; //_ is bc we can't name a variable name. This variable is going to use the base data
    int level;

    public Pokemon(PokemonBase pBase, int pLevel) //Constructor of our pokemons, pBase = Pokemon Base, pLevel = Pokemon Level
    {
        _base = pBase;
        level = pLevel;
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((_base.Attack) / 100f) + 5; } //This is the formula used in pokemon games to get the stats at any level
        //FloorToInt is used to get rid of the decimal point
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((_base.Defense) / 100f) + 5; } 
    }
    public int SpAttack
    {
        get { return Mathf.FloorToInt((_base.SpAttack) / 100f) + 5; }
    }
    public int SpDefense
    {
        get { return Mathf.FloorToInt((_base.SpDefense) / 100f) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((_base.Speed) / 100f) + 5; }
    }

    //MaxHp use a slightly different formula
    public int MaxHp
    {
        get { return Mathf.FloorToInt((_base.Defense) / 100f) + 10; }
    }
}
