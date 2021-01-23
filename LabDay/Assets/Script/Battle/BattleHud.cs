using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //We use this library bc we actually work with UI

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;

    //Set all the text colors we'll use
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Pokemon _pokemon; //reference to our PokemonClass (_pokemon) is bc the name pokemon is already taken

    //Store the colors in a dictionnary
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Pokemon pokemon) //function to set the value of our two texts and HPBar
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        SetLevel();
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp );
        SetExp();

        statusColors = new Dictionary<ConditionID, Color>() //Initialize the dictionnary
        {
            {ConditionID.psn, psnColor },
            {ConditionID.brn, brnColor },
            {ConditionID.slp, slpColor },
            {ConditionID.par, parColor },
            {ConditionID.frz, frzColor }
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;//Link the SetStatuxText with the OnStatusChanged, to know when we should change the status
    }

    public void SetStatusText() //Function to set the status text (psn, frz, slp..)
    {
        if (_pokemon.Status == null)
            statusText.text = "";
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper(); //call the ID of the status, to know what we should print in the StatusText, and convert it to string
            statusText.color = statusColors[_pokemon.Status.Id]; //Set the color of the text from our dictionnary
        }
    }

    public void SetLevel() //Set the lvl in a function that we can call on a lvl up
    {
        levelText.text = "Lvl " + _pokemon.Level;
    }

    public void SetExp() //Set the xp bar
    {
        if (expBar == null) return; //Make sure only the player hud has an Exp Bar

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset=false) //Set the xp bar smoothly when a pokemon gain xp. Take a bool as parameter to know if we need to reset the exp bar
    {
        if (expBar == null) yield break; //Make sure only the player hud has an Exp Bar

        if (reset == true)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1f).WaitForCompletion(); //Use DotWeen to make it look good
    }

    //Calculate the normalize exp, to make it fit in the xp bar (It has to be between 0 and 1, for the scale)
    float GetNormalizedExp()
    {
        int currLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);

        float normalizedExp = (float)(_pokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp); //Formula to normalize the current exp
        return Mathf.Clamp01(normalizedExp);
    }

    //We use coroutine here to set the Hp after a hit was taken
    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
            _pokemon.HpChanged = false;
        }
    }
}
