using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649PBR.Main;

namespace s649PBR
{
    namespace TraitDrinkPatchMain
    {
        [HarmonyPatch]
        internal class PatchExe{
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDrink), "OnDrink")]
            internal static void TraitDrinkPatch(TraitDrink __instance, Chara c){
                if(PatchMain.configDebugLogging){Debug.Log("[PBR]Drinked->" + __instance.owner.id.ToString());}
                Thing t = null;
                string prod = "";
                //Thing t = ThingGen.Create("potion_empty");
                //Debug.Log("[PBR]Akibin:" + __instance.owner.id.ToString());
                int num;
                if(int.TryParse(__instance.owner.id, out num))
                {//owner.idが数字
                    switch(num)
                    {
                        case 928 : prod = "potion_empty";//horumon
                        break;
                        case 718 : prod = "1170";//coffee -> akikan
                        break;
                        case >= 504 and <= 505 : prod = "1170";//-> akikan
                        break;
                        case >= 48 and <= 59 : prod = "726";//akibin
                        break;
                        case >= 501 and <= 508 : prod = "726";
                        break;
                        case >= 718 and <= 1134 : prod = "726";
                        break;
                        default : prod = "potion_empty";
                        break;
                    }
                } else 
                {
                    switch(__instance.owner.id)
                    {//owner.idが数字以外
                        case "bucket" : prod = "bucket_empty";
                        break;
                        case "crimAle" : prod = "726";
                        break;
                        case "milk" or "milkcan" : prod = "726";
                        break;
                        default : prod = "potion_empty";
                        break;
                    }
                
                }
                if(prod != "")
                {   
                    if(c.IsPC)
                    {
                        t = ThingGen.Create(prod);
                        c.Pick(t);
                        if(PatchMain.configDebugLogging){Debug.Log("[PBR]Prod->" + prod + " :by " + c.GetName(NameStyle.Simple));}
                    } else 
                    {
                        if(PatchMain.cf_F01_NCBWD)
                        {
                            t = ThingGen.Create(prod);
                            if(prod == "potion_empty")
                            {
                                if(EClass.rnd(2) == 0 || (c.ChildrenWeight > c.WeightLimit))
                                    {EClass._zone.AddCard(t, c.pos);} 
                                else 
                                    {c.Pick(t);}
                            } else 
                            {
                                if(EClass.rnd(9) == 0 || (c.ChildrenWeight > c.WeightLimit))
                                    {EClass._zone.AddCard(t, c.pos);} 
                                else 
                                    {c.Pick(t);}
                            }
                            if(PatchMain.configDebugLogging){Debug.Log("[PBR]Prod->" + prod + " :by " + c.GetName(NameStyle.Simple));}
                        }
                    }
                }
            }
        }
    }
}