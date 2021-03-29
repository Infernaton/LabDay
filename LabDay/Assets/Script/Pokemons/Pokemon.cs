using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// !!! THIS SCRIPT IS GOING TO USE THE DATA FROM THE SCRIPTABLE OBJECT TO CREATE THE REAL POKEMON, WITH LEVEL, AND HOW IT'S GROWING IN REAL TIME IN THE GAME!!!
// THESES DATA ARE MEANT TO CHANGE IN THE GAME

[System.Serializable] //To show a class in unity we need to set this
public class Pokemon
{
    [SerializeField] PokemonBase _base; //Setting a serialized field of it to acces it in unity
    [SerializeField] int level;

    private int lastDamage;

    public Pokemon(PokemonBase pBAse, int pLevel) //Create an instance of the pokemon, moslty use with wild pokemon, to catch a copy of the wild pokemon, and not THE pokemon itself
    {
        _base = pBAse;
        level = pLevel;

        Init();
    }

    public PokemonBase Base //we call this variable Name bc we use it as a property. This variable is going to use the base data
    {
        get { return _base; }
    } 
    public int Level //We set theses two as public to acces them outside this class (in the BattleHub for example.)
    {
        get { return level; }
    }

    public int Exp { get; set; }

    public int HP { get; set; }

    public List<Move> Moves { get; set; } //This is a reference to the List of move the pokemon will have in game
    public Move CurrentMove { get; set; } //Reference to the move selected in the battle system
    public Dictionary<Stat, int> Stats { get; private set; } //Creating the Dictionnary with our stats (private so it won't change inside the pokemon class. <Key, value> to easily get the key, with just the value
    public Dictionary<Stat, int> StatBoosts { get; private set; } //Creating a dictionnary for Stats Boosting
    
    public Condition Status { get; private set; }
    public int StatusTime { get; set; } //We'll primarly use it to track how many turns the pokemon should sleep
    
    public Condition VolatileStatus { get; private set; } //These are the volatile status like confusion, love..
    public int VolatileStatusTime { get; set; }

    public Queue<string> StatusChanges { get; private set; } //Queue is used to store a list of strings we can take out and they'll be in the order we added them, so it'll be easier
    public bool HpChanged { get; set; }

    public event System.Action OnStatusChanged; //Track our status condition

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
            if (Moves.Count >= PokemonBase.MaxNumberOfMoves)
                break;
        }

        Exp = Base.GetExpForLevel(Level);

        CalculateStats(); //Calling function to get the stats
        HP = MaxHp;

        StatusChanges = new Queue<string>();
        ResetStatBoost(); 
        Status = null;
        VolatileStatus = null;
    }

    void CalculateStats() //Function to calculate stats at a specific level
    {
        //Using the formula used in pokemon games to get the stats
        //FloorToInt is used to get rid of the decimal point

        Stats = new Dictionary<Stat, int>(); //Initialize the dictionnary
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5); //Setting the values, by calculating it, then setting it to the Attack key
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5); //Same with defense, etc etc
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;

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
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 },
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

            if (stat != Stat.Hp)
            {
                StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6); //Set the new value as the stat value + the boost (With a limit of 6)

                if (boost > 0)
                    StatusChanges.Enqueue($"{stat} de {Base.Name} augmente!");
                else
                    StatusChanges.Enqueue($"{stat} de {Base.Name} diminue!");

                Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}.");
            }
            else
            {
                int damage = MaxHp * boost / 100;
                
                UpdateHP(damage);
                StatusChanges.Enqueue($"{Base.name} perd quelque PV en contrecoup.");
            }
        }
    }

    public bool CheckForLevelUp() //Bool to know if the pokemon level up
    {
        //Return true if the pkmn lvl up
        if (Exp > Base.GetExpForLevel(level + 1)) //If the actual xp is greater than the exp needed, level up
        {
            ++level; //Increase by 1 the lvl
            return true;
        }

        return false;
    }

    //If there multiple move that the pokemon can learn on 1 level, we set an array
    public LearnableMoves[] GetLearnableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).ToArray(); //Level is the current level, and level is the level needed for the move. This get the first item of the list
    }

    public void LearnMove(LearnableMoves moveToLearn) //Add a new move to the current list of moves
    {
        if (Moves.Count > PokemonBase.MaxNumberOfMoves)
            return;
        
        Moves.Add(new Move(moveToLearn.Base)); //Add the move by creating a new instance of the move to learn
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
        float d = a * move.Base.Power * (attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        lastDamage = damage;
        Debug.Log($"LastDamage done: {lastDamage}");

        //After that we substract the damage to the actual life of the pokemon, and check if he died or no
        UpdateHP(-damage); //Simply call a function to update the Hp before returning
        

        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP + damage, 0, MaxHp);
        Debug.Log($"Update PV: {damage}");
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionId) //Function we'll call to set the status on a pokemon
    {
        if (Status != null) return;

        Status = ConditionsDB.Conditions[conditionId]; //Get the key of a status to set it on a pokemon
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.SartMessage}");
        OnStatusChanged?.Invoke();
    }
    public void CureStatus() //Calling this to clear a status when needed
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId) //Function we'll call to set the Volatile status on a pokemon
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.Conditions[conditionId]; //Get the key of a Volatile status to set it on a pokemon
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.SartMessage}");
        //here is not the Invoke, since we don't need to show them in the hud 
    }
    public void CureVolatileStatus() //Calling this to clear a Volatilestatus when needed
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove() //Function to get a random move for the enemy to use
    {
        //Prevent the enemy for using a move with no Pp
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList(); //Convert moves with Pp to a list

        int r = Random.Range(0, movesWithPP.Count); //Take a random move that still has Pp
        return movesWithPP[r];
    }
    public void OnAfterTurn() //Function to be called when the turn is over, before the next turn beggin, so it'll be easy to call this for every conditions etc
    {
        Status?.OnAfterTurn?.Invoke(this); //Call the action only if OnAfterTurn is not null, and the pokemon has a status
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
    public bool OnBeforeMove()
    {
        bool canPerformMove = true;

        if(Status?.OnBeforeMove != null) //Check if there is a status playing before the move happen
        {
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;
        }
        if (VolatileStatus?.OnBeforeMove != null) //Check if there is a Volatile status playing before the move happen
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }
        return canPerformMove;
    }

    public void OnBattleOver() //Calling this when the battle is over
    {
        ResetStatBoost();
        VolatileStatus = null;
    }
    public int LastDamage
    {
        get => lastDamage;
    }
}

public class DamageDetails //Class we'll use to display a message if there was a critical hit, super effective or not..
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

