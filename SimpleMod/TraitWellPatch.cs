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
    namespace TraitWellPatchMain
    {
        [HarmonyPatch]
        internal class PatchExe 
        {
            //TraitWell.OnBlend実行時にも瓶を還元する
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitWell), "OnBlend")]
            internal static void TraitWellPatch(TraitWell __instance, Thing t, Chara c)
            {
                if(PatchMain.configDebugLogging){Debug.Log("[PBR]Blend->" + t.id.ToString());}
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
            }
        }
    }
}