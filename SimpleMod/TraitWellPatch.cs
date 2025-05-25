using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.Main;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.UIElements.UxmlAttributeDescription;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace s649PBR
{//>begin namespaceMain
    namespace TraitWellPatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe 
        {//>>>begin class:PatchExe
            private static Thing DoRecycleBottle(Thing t, Chara c, int at, bool broken = false) { return PatchMain.DoRecycleBottle(t, c, at, broken); }

            //TraitWell.OnBlend実行時にも瓶を還元する
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitWell), "OnBlend")]
            private static void TraitWellPostPatch(TraitWell __instance, Thing t, Chara c)
            {//>>>>begin method:TraitDrinkPatch
                PatchMain.Log("[PBR:WellBlend]Used->" + t.NameSimple + " /C: " + c.NameSimple, 1);
                Thing result = DoRecycleBottle(t, c, ActType.Blend);
                if (result != null)
                {
                    PatchMain.Log("[PBR:WellBlend]result->" + result.NameSimple, 1);
                }
            }//<<<<end method:TraitDrinkPatch
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain



/*
                string prod = "";
                switch(t.id)
                {
                    case "snow" or "drug" or "334": break;
                    case "water" or "water_dirty" or "potion" or "perfume" or "lovepotion" or "mercury" or "blood_angel": prod = "potion_empty";
                    break;
                    case "330" or "331" or "335" or "336" or "338" or "928" or "1142" or "1163" or "1165" : prod = "potion_empty";
                    break;
                    case "bucket" : prod = "bucket_empty";
                    break;
                    case "milk" : prod = "726";//akibin
                    break;
                    case "504" or "505" or "718" : prod = "1170";//akikan
                    break;
                    default : prod = "726";//akibin
                    break;
                }
                if(PatchMain.configDebugLogging && prod != ""){Debug.Log("[PBR]Prod->" + prod + " :by " + c.GetName(NameStyle.Simple));}
                if(c.IsPC && prod != "")
                {
                    t = ThingGen.Create(prod);
                    c.Pick(t);
                }
                */

//if(PatchMain.configDebugLogging){Debug.Log("[PBR]Blend->" + t.id.ToString());}
//if(Func_Blend_Allowed)
//{
//Thing usedT = t;
//Thing prodT = DoRecycleBottle(usedT);

//Thing result;
//if (prodT != "")
//{
//    result = ThingGen.Create(prodT);
//    if (c.IsPC) { c.Pick(result); } else { EClass._zone.AddCard(result, c.pos); }
//    PatchMain.Log("[PBR:WellBlend]result[" + result.NameSimple + " -> " + c.NameSimple + "]");
//}
//DoRecycleBottle(t, c, ActType.Blend);

//Thing usedT = __instance.owner.Thing;
//PatchMain.Log("[PBR:Drink]Used->" + usedT.NameSimple + " :by " + c.NameSimple, 1);

//Thing usedT = __instance.owner.Thing;
//string prodT = DoRecycleBottle(t, c, ActType.Blend);