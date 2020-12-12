using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health; //We could modify our GameObject health right in Unity

    public void SetHP(float hpNormalized) //Method to set our Hp in real time while in a battle
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f); //Transform our health bar at the scale of the float of our Hp
    }

    public IEnumerator SetHPSmooth(float newHp) //HPBar will now reduce smootlhy
    {
        float curHp = health.transform.localScale.x; //The current Hp are now set as an x line.
        float changeAmt = curHp - newHp; //Get the amount of hp to reduce

        //This loop will move the life bar by a certain amount within a certain given time
        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null; //This return is made to get out of the loop
        }
        health.transform.localScale = new Vector3(newHp, 1f); //We set it equals to the new Hp anyway, in case it appears to look bad
    }
}