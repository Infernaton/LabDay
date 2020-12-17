using System.Collections;
using System.Collections.Generic;
using System.Linq; //To us the where function
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons; //List of our pokemons in the team

    private void Start()
    {
        foreach (var pokemon in pokemons) //Looping throught each pokemons in our party
        {
            pokemon.Init();//And initialize every one of them
        }
    }

    public Pokemon GetHealthyPokemon() //Function to return a pokemon not fainted
    {
        return pokemons.Where(x => x.HP > 0).FirstOrDefault(); //Where loops throught all our pokemons in order, and when the condition is clear, returns the first not fainted
    }
}
