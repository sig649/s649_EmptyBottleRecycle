using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.Main;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;


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
                PatchMain.Log("[PBR:Throw]Used->" + t.NameSimple + "/prod:" + prodT.ToString() + " :by " + c.NameSimple, 1);

                Thing result;
                //int prodN = TypeContainsPotionBottle(usedT);
                if (prodT != "")
                {
                    result = ThingGen.Create(prodT);
                    EClass._zone.AddCard(result, p);
                    PatchMain.Log("[PBR:Throw]Used->" + result.NameSimple + " :-> " + p.ToString());
                }
                
            }//<<<<end method:TraitDrinkPatch
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain