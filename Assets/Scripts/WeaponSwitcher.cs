using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
   public GameObject innerWeapon;

   public void SwitchWeapon ()
   {
       if(innerWeapon.activeSelf == true)
       {
            innerWeapon.SetActive(false);
       } 
       else
       {
            innerWeapon.SetActive(true);
       }       
   }  
}
