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
{//>begin namespaceMain
    namespace TraitWellPatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe 
        {//>>>begin class:PatchExe
            private static string DoRecycleBottle(Thing t, Chara c, int at, bool broken = false) { return PatchMain.DoRecycleBottle(t, c, at, broken); }
            //private static bool Func_Use_Allowed => PatchMain.Cf_Allow_Use;
            //private static bool Use_PC_Allowed => PatchMain.Cf_Reg_Use_PC;
            //private static bool Use_NPC_Allowed => PatchMain.Cf_Reg_Use_NPC;

            //private static bool Func_Blend_Allowed => PatchMain.Cf_Allow_Blend;
            //private static bool Blend_PC_Allowed => PatchMain.Cf_Reg_Blend_PC;
            //private static bool Blend_NPC_Allowed => PatchMain.Cf_Reg_Blend_NPC;

            //TraitWell.OnBlend実行時にも瓶を還元する
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitWell), "OnBlend")]
            private static void TraitWellPostPatch(TraitWell __instance, Thing t, Chara c)
            {//>>>>begin method:TraitDrinkPatch
                Thing usedT = __instance.owner.Thing;
                string prodT = DoRecycleBottle(usedT, c, ActType.Blend);
                Thing result;
                //if(PatchMain.configDebugLogging){Debug.Log("[PBR]Blend->" + t.id.ToString());}
                //if(Func_Blend_Allowed)
                //{
                //Thing usedT = t;
                //Thing prodT = DoRecycleBottle(usedT);

                if (prodT != "")
                {
                    result = ThingGen.Create(prodT);
                    if (c.IsPC) { c.Pick(result); } else { EClass._zone.AddCard(result, c.pos); }
                    PatchMain.Log("[PBR:WellBlend]Used->" + usedT.NameSimple + "/Prod->" + prodT + " :by " + c.NameSimple);
                }
                // }
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
            }//<<<<end method:TraitDrinkPatch
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain