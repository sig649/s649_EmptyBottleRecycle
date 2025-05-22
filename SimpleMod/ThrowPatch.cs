using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;
using System.Collections.Generic;
using s649PBR.Main;


namespace s649PBR
{//>begin namespaceMain
    namespace ThrowPatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            //internal int TypeContainsPotionBottle(Thing t){return PatchMain.TypeContainsPotionBottle(t);}
            private static string DoRecycleBottle(Thing t, Card c, int at, bool broken = false) { return PatchMain.DoRecycleBottle(t, c, at, broken); }
            

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ActThrow), "Throw", new Type[] { typeof(Card), typeof(Point), typeof(Card), typeof(Thing), typeof(ThrowMethod) })]
            private static void ThrowPostPatch(ActThrow __instance, Card c, Point p, Card target, Thing t, ThrowMethod method)
            {//begin method:TraitDrinkPatch
                
                //Thing usedT = t;
                //Chara usedC = __instance.Act.CC;
                string prodT = DoRecycleBottle(t, c, ActType.Throw, true);
                Thing result;
                //int prodN = TypeContainsPotionBottle(usedT);
                if (prodT != "")
                {
                    result = ThingGen.Create(prodT);
                    EClass._zone.AddCard(result, c.pos);
                    PatchMain.Log("[PBR:Throw]Used->" + t.NameSimple + "/Prod->" + prodT + " :by " + c.NameSimple);
                }
                
            }//<<<<end method:TraitDrinkPatch
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain