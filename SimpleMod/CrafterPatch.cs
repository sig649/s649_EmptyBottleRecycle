using System;
using System.IO;
using System.Diagnostics;
using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;

using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649PBR.Main;

namespace s649PBR
{//>begin NamespaceMain
    namespace CrafterPatchMain
    {//>>begin NamespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            private static Thing DoRecycleBottle(Thing t){return PatchMain.DoRecycleBottle(t);}
            private static bool Func_Allowed => PatchMain.cf_Allow_F03_Craft;
            //private static bool PC_Allowed => PatchMain.cf_F01_PC_CBWD;

            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitCrafter), "Craft")]
            private static void PostPatch(TraitCrafter __instance, 	AI_UseCrafter ai)
            {//>>>>begin method:PostPatch
                if(Func_Allowed)
                {//>>>>>if
                    List<Thing> ings = ai.ings;
                    string text = "[PBR]";
                    Point pos = ai.owner.pos;
                    foreach(Thing t in ings)
                    {   
                        text += "[ings:" + t.id.ToString() +"]";
                        //bottleをcreateしてposにadd
                        Thing prodT = DoRecycleBottle(t);
                        if(prodT != null)
                        {
                            text += "[prod:" + prodT.ToString() +"]";
                            EClass._zone.AddCard(prodT, pos);
                        }
                    }
                    if(PatchMain.configDebugLogging){Debug.Log(text);} 
                }//<<<<<end if
            }//<<<<end method:PostPatch
            /*
            private bool CanRecycleBottle(Thing t)
            {//>>>>begin method:DoRecycleBottle
                if(PatchMain.TypeContainsPotionBottle(t) == 1)
                {
                    return true;
                } else {
                    return false;
                }
            }//<<<<end method:DoRecycleBottle
            */
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain

///trashbox//////////////////////////////////////////////////////////////////
/// 
/*
/////////////////////////////////////////////////

////////////////////////////////////////////////
*/