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
    public Dictionary<Stat, int> Stats { get; private set; } //Creating the Dictionnary with our stats (private so it won't change inside the pokemon class. <Key, value> to easily get the key, with just the value
    public Dictionary<Stat, int> StatBoosts { get; private set; } //Creating a dictionnary for Stats Boosting
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>(); //Queue is used to store a list of strings we can take out and they'll be in the order we added them, so it'll be easier

    public void Init() //Constructor of our pokemons, pBase = Pokemon Base, pLevel = Pokemon Level
    {

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

        CalculateStats(); //Calling function to get the stats

        HP = MaxHp;
    }

    void CalculateStats() //Function to calculate stats at a specific level
    {
        //Using the formula used in pokemon games to get the stats
        //FloorToInt is used to get rid of the decimal point

        Stats = new Dictionary<Stat, int>(); //Initialize the dictionnary
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack) / 100f) + 5); //Setting the values, by calculating it, then setting it to the Attack key
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense) / 100f) + 5); //Same with defense, etc etc
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp) / 100f) + 10;

        ResetStatBoost();
    }

    void ResetStatBoost() //Function to set back the stat
    {
        StatBoosts = new Dictionary<Stat, int>() //Initializing the dictionnary
        {
            {Stat.Attack, 0 },
            {Stat.Defense, 0 },
            {Stat.SpAttack, 0 },
            {Stat.SpDefense, 0 },
            {Stat.Speed, 0 },
        };
    }

    int GetStat(Stat stat) //Calculating the actual value of the stat, after it may as changed
    {
        int statValue = Stats[stat]; //Calling the dictionary to get the stat

        //Applying the logic of stat boosting before returning it
        int boost = StatBoosts[stat]; //Getting the boost from our dictionnary
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f }; //These are all the value we'll use on calculation

        //First checking if boost is negative or positive
        if (boost >= 0) //If the boost is positive, we just multiply
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]); //Getting the value of our boost, applying it to our Stat Value, and making sure it's an int
        else //If it's negative we juste divide
            statValue= Mathf.FloorToInt(statValue / boostValues[-boost]);

        return statValue;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts) //Function to apply the boosts
    {
        foreach (var statBoost in statBoosts)
        {
            //Store value of stat and boost in 2 vars
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6); //Set the new value as the stat value + the boost (With a limit of 6)

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public int Attack
    {
        //Calling the function to get the actual stat
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); } 
    }
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }
    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    //MaxHp use a slightly different formula
    public int MaxHp { get; private set; }

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

        //We check if the move we're about to get hit by is Special, physical, or a stats move
        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack; //Checking the Category of the move before using the stat according to it
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

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

    public Move GetRandomMove() //Function to get a random move for the enemy to use
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public void OnBattleOver() //Calling this when the battle is over
    {
        ResetStatBoost();
    }
}

public class DamageDetails //Class we'll use to display a message if there was a critical hit, super effective or not..
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }

}
