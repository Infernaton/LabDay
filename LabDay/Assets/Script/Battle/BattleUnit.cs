using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //We use this library bc we actually work with UI

//This is were we manage our pokemon in the battle
public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }

    //We will know wich pokemon we should choose to show, and if we need it's back or front sprite
    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);
        if (isPlayerUnit)
        {
            GetComponent<Image>().sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            GetComponent<Image>().sprite = Pokemon.Base.FrontSprite;
        }
    }
}
