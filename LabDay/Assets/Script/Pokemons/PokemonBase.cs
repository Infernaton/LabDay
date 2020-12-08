using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")] //We create a menu in Unity to acces this blueprint
public class PokemonBase : ScriptableObject //Changed from "MonoBehavior" to "ScriptableObject" because we'll do some blueprints
{
    // Inside we'll create variables to store the data of the pokemons

    // SerializedField is used instead of public, so we can use them in some other classes more easily, and modify them in Unity.
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
    Water,
}
