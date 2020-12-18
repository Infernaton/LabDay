using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots; //Creating an array of our memberSlots

    public void Init() //Function to not use a SerializedField, but to assign pokemons automaticly
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(); //Will return every children components attached in the PartyScreen
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        for (int i = 0; i < memberSlots.Length; i++) //For every memberSlots in our array, we will apply this function to get the data of each one of them
        {
            if (i < pokemons.Count) //Befor calling the function we check how many pokemon we actually have
                memberSlots[i].SetData(pokemons[i]);
            else
                memberSlots[i].gameObject.SetActive(false); //If we don't get 6, we deactivate the last spots unused
        }

        messageText.text = "Choose a Pokemon";
    }
}
