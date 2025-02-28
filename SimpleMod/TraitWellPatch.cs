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
            private static Thing DoRecycleBottle(Thing t){return PatchMain.DoRecycleBottle(t);}
            private static bool Func_Allowed => PatchMain.cf_Allow_F02_Blend;
            //private static bool PC_Allowed => PatchMain.cf_F01_PC_CBWD;

            //TraitWell.OnBlend実行時にも瓶を還元する
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitWell), "OnBlend")]
            private static void TraitWellPostPatch(TraitWell __instance, Thing t, Chara c)
            {//>>>>begin method:TraitDrinkPatch
                //if(PatchMain.configDebugLogging){Debug.Log("[PBR]Blend->" + t.id.ToString());}
                if(Func_Allowed)
                {
                    Thing usedT = t;
                    Thing prodT = DoRecycleBottle(usedT);

                    if(prodT != null)
                    {
                        if(c.IsPC)
                        {
                            //t = ThingGen.Create(prod);
                            c.Pick(prodT);
                            if(PatchMain.configDebugLogging){Debug.Log("[PBR]Used->" + usedT.ToString() +"/Prod->" + prodT.ToString() + " :by " + c.GetName(NameStyle.Simple));}
                        } 
                    }
                }
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