using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Here we manage the wilds pokemon of an area
public class MapArea : MonoBehaviour
{
    [SerializeField] int minLevel;  //Level min and max that the encounter Pokemon would have
    [SerializeField] int maxLevel;
    [SerializeField] List<Pokemon> wildPokemons; //List of the wild pokemons in an area

    public Pokemon GetRandomWildPokemon() //Function to get a random pokemon within a list
    {
        int level = Random.Range(minLevel, maxLevel + 1);
        var wildPokemon = wildPokemons[Random.Range(0, wildPokemons.Count)]; //In a range from 0 to our maximum number of pokemon, we store in a var one pokemon randomly
        wildPokemon.Level = level;
        wildPokemon.Init();//Then initialize it
        return wildPokemon;
    }
}
