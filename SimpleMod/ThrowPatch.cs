using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.Main;
using System;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.UIElements.UxmlAttributeDescription;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using s649PBR.BIClass;
using static s649PBR.Main.PatchMain;


namespace s649PBR
{//>begin namespaceMain
    namespace ThrowPatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            [HarmonyPostfix]
            [HarmonyPatch(typeof(ActThrow), "Throw", new Type[] { typeof(Card), typeof(Point), typeof(Card), typeof(Thing), typeof(ThrowMethod) })]
            private static void ThrowPostPatch(ActThrow __instance, Card c, Point p, Card target, Thing t, ThrowMethod method)
            {//begin method:TraitDrinkPatch
                string title = "[PBR:AT.T]IsThrownSet";
                IsThrown = true;
                lastThrower = c.Chara;
                lastThrownThing = t;
                //lastCreatedBI = CreateBI(t);

                PatchMain.Log(title + "Done", 2);
            }//<<<<end method:TraitDrinkPatch
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain

//Thing usedT = t;
//Chara usedC = c.Chara;
//bool result = PatchMain.TryRecycle(usedT, usedC, ActType.Throw, true, p);
//if (result)
//{
//    PatchMain.Log(title + "Success", 1);
//}
// else { PatchMain.Log(title + "NotDone", 1); }
//Thing result;
//int prodN = TypeContainsPotionBottle(usedT);
//if (prodT != "")
//{
//    result = ThingGen.Create(prodT);
//    EClass._zone.AddCard(result, p);
//    PatchMain.Log("[PBR:Throw]Used->" + result.NameSimple + " :-> " + p.ToString());
//}

//string text = "Throw->" + usedT.NameSimple;
//text += " :by " + usedC.NameSimple;
//text += " :pos: " + p.ToString();
//PatchMain.Log(title + text, 1);
/*
 * 
            //internal int TypeContainsPotionBottle(Thing t){return PatchMain.TypeContainsPotionBottle(t);}
            //private static void DoRecycleBottle(Thing t, Chara c, int at, bool broken = false, Point p = null) { PatchMain.DoRecycleBottle(t, c, at, broken, p); }
            private static Thing DoRecycleBottle(Thing t, Chara c, int at, bool broken = false, Point p = null) { return PatchMain.DoRecycleBottle(t, c, at, broken, p); }

                //Thing usedT = t;
                Chara usedC = c.Chara;
                PatchMain.Log("[PBR:Throw]Used->" + t.NameSimple + " :by " + usedC.NameSimple, 1);
                Thing result = DoRecycleBottle(t, usedC, ActType.Throw, true, p);
                if (result != null)
                {
                    PatchMain.Log("[PBR:Dye]result->" + result.NameSimple, 1);
                }

 * 
 */