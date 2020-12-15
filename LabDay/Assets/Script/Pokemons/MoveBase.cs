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
    [SerializeField] bool isSpecial;

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

    public bool IsSpecial //Creating Special and Physical moves
    {
        get
        {
            return isSpecial;
        }
    }
}
