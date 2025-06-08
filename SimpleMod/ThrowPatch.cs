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
using s649ElinLog;
using static s649ElinLog.ElinLog;

namespace s649PBR
{//>begin namespaceMain
    namespace ThrowPatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            /*
             * 不具合情報
             * 染料を投げて地面に落ちた時は壊れないが、ガラスが還元されてしまう
             * ActThrow実行時には当たって消費されているか確定していないので
             * ここらへんの追加調査が必要
             * OnDrink（直飲みを排除必須）やOnthrowGroundで還元すべきか？
             * 染料から還元されるガラスが元の材質を参照
             * 
             */
            private static readonly string modSubNS = "TP";
            static BottleIngredient stateBottleIng;
            static Chara c_thrower;
            static Thing throwed_t;
            static Point throwed_p;
            static bool isCheckSuccess;
            static bool trySetAllow;
            //static bool needDoRecycleSkip;

            private static void InitState() 
            {
                stateBottleIng = null;
                c_thrower = null;
                throwed_t = null;
                throwed_p = null;
                isCheckSuccess = false;
                trySetAllow = false;
                //needDoRecycleSkip = false;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(ActThrow), "Throw", new Type[] { typeof(Card), typeof(Point), typeof(Card), typeof(Thing), typeof(ThrowMethod) })]
            private static bool ThrowPostPatch(ActThrow __instance, Card c, Point p, Card target, Thing t, ThrowMethod method)
            {//begin method
                
                InitState();
                ClearLogStack();
                string title = "AT.T";
                LogStack("[" + modSubNS + "/" + title + "]");
                if (!Cf_Allow_Throw) { LogOther("'Throw' not Allowed", LogTier.Other); return true; }
                //if(method != ThrowType.Potion)
                //bool isCheckSuccess = false;
                //throwtyoe check //potion以外を除外
                ThrowType throwtype;
                List<string> checkThings = new();
                string checktext = "";
                try
                {
                    throwtype = t.trait.ThrowType;
                    checkThings.Add(GetStr(throwtype));
                    checktext = string.Join("/", checkThings);
                }
                catch (NullReferenceException ex)
                {
                    LogError("ThrowType check Failed for NullPo");
                    LogError(checktext);
                    Debug.Log(ex.Message);
                    Debug.Log(ex.StackTrace);
                    return true;
                }
                LogDeep("Throwtype:" + checktext);
                if (IsInProhibitionList(t.id)) { LogDeep("Prohibition Item"); return true; }
                //if (throwtype != ThrowType.Potion && !(t.trait is TraitDye)) { LogOther("Throwed Thing is not Potion."); return; }
                if (throwtype == ThrowType.Potion || throwtype == ThrowType.Vase || t.trait is TraitDye)
                {
                    trySetAllow = true;
                }
                //if (t.trait is TraitDye)
                //{
                //    trySetAllow = true;
                //    //needDoRecycleSkip = true;
                //}
                //argcheck
                if (!trySetAllow) { LogOther("Throwed thing is cannot recycle"); return true; }
                try
                {
                    var args = new List<string> { GetStr(c), GetStr(p), GetStr(target), GetStr(t), GetStr(throwtype) };
                    var argtext = string.Join("/", args);
                    LogDeep("Start/Arg:" + argtext, LogTier.Deep);
                    if (!c.isChara || c.Chara == null) { { LogError("*Warn* c is invalid"); return true; } }
                    c_thrower = c.Chara;
                    throwed_t = t;
                    throwed_p = p;
                    
                    //Thing thing = card.Thing;
                    stateBottleIng = TryCreateBottleIng(new ActType(ActType.Throw), throwed_t, c_thrower);
                    if (stateBottleIng == null) { LogDeep("BI was not generated.", LogTier.Deep); return true; }
                    //isCheckSuccess = true;
                    //return = bi;
                    LogOther("PreCheckDone");
                    isCheckSuccess = true;


                    

                    
                }
                catch (NullReferenceException ex)
                {
                    LogError("Error for NullPo");
                    Debug.Log(ex.Message);
                    Debug.Log(ex.StackTrace);
                    return true;
                }
                //if (!needDoRecycleSkip) { LogOther("TraitDye"); }
                
                //text += result ? "Done!" : "Not Done";


                //LogStackDump();
                return true;
                
            }//end method

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Card), "Die")]//TraitDyeのフック
            internal static void Card_Die_PostPatch() 
            {
                ClearLogStack();
                string title = "HM:C.D";
                LogStack("[" + modSubNS + "/" + title + "]");

                if (!isCheckSuccess) { LogTweet("Through Card.Die"); return; }

                //string text = "";
                bool tryBrake = stateBottleIng.TryBrake();
                LogDeepTry(tryBrake, "tryBrake");
                //text += "/tB:" + GetStr(tryBrake) + "/";
                bool result = DoRecycle(stateBottleIng, c_thrower, throwed_p);
                LogDeepTry(result);
                LogOther("The recycle has been completed.");
                InitState();
            }
            /*
            [HarmonyPostfix]//TraitDye以外のpotionのフック
            [HarmonyPatch(typeof(Card), "Die", new Type[] { typeof(Element), typeof(Card), typeof(AttackSource) })]
            internal static void Card_Die_PostPatch(Element e, Card origin, AttackSource attackSource)
            {
                ClearLogStack();
                string title = "HM:C.D+++";
                LogStack("[" + modSubNS + "/" + title + "]");
                if (!isCheckSuccess) { LogTweet("Through Card.Die"); return; }

                //string text = "";
                bool tryBrake = stateBottleIng.TryBrake();
                LogDeepTry(tryBrake, "tryB:");
                //text += "/tB:" + GetStr(tryBrake) + "/";
                bool result = DoRecycle(stateBottleIng, c_thrower, throwed_p);
                LogDeepTry(result);
                LogOther("The recycle has been completed and we will initialize and close the recycling.");
                InitState();
            }
            */
        }//<<<end class:PatchExe

    }//<<end namespaceSub
}//<end namespaceMain


/////trash***************************************************************************************
///



/*
 * 
 * 
 * [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDye), "OnThrowGround")]
            internal static void TraitDrink_OnThrowGround_PostPatch(Chara c, Point p)
            {
                string title = "[PBR:FookCheck]";
                Log(title + "TraitDrink" + "/" + "OnThrowGround");
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDrink), "OnDrink")]
            internal static void TraitDrink_OnDrink_PostPatch()
            {
                string title = "[PBR:FookCheck]";
                Log(title + "TraitDrink" + "/" + "OnDrink");
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDrink), "OnThrowGround")]
            internal static void TraitDrink_OnThrowGround_PostPatch()
            {
                string title = "[PBR:FookCheck]";
                Log(title + "TraitDrink" + "/" + "OnThrowGround");
            }
            */
//if (!c.isChara) { { LogError("NoT Chara"); return; } }
//if (t == null) { LogError("NoThing"); return; }
//if (p == null) { LogError("NoPoint"); return; }

//Log("ArgCheck:" + GetStr(c_thrower) + "/" + GetStr(t) + "/" + GetStr(p) , LogTier.Deep);
//MethodEnd:
//LogStackDump();
//if (!isCheckSuccess) { Log("recycle phase is skipped for check failure", LogTier.Deep); return; }

//ThrowPPost();
//Log("Start", LogTier.Other);
//if (__instance == null) { LogError("NoInstance"); return; }
//if (__instance.owner == null) { LogError("NoOwner"); return; }
//if (__instance.owner.Chara == null) { LogError("NoThrower"); return; }
/*
private static void ThrowPatchPreExe(ActThrow actThrow, Thing thing, Point point)
{
    LogStack("[" + header+ "/Pre]");
    c_thrower = null;
    throwed_t = null;
    throwed_p = null;
    isCheckSuccess = false;
    ThrowPPre(actThrow, thing, point);
    LogStackDump();
}
*/
/*
private static void ThrowPatchPostExe()
{
    if (!isCheckSuccess) { Log("post phase is skipped for check failure", LogTier.Other); return; }
    LogStack("[" + header + "/Post]");
    ThrowPPost();
    LogStackDump();
}
private static void ThrowPPre(ActThrow actThrow, Thing thing, Point point)
{

    //Chara c_drinker = chara;
    if (actThrow.owner == null) { LogError("NoOwner"); return; }
    Chara c_thrower = actThrow.owner.Chara;
    if (c_thrower == null) { LogError("NoThrower"); return; }
    if (thing == null) { LogError("NoThing"); return; }
    throwed_t = thing;
    if (point == null) { LogError("NoPoint"); return; }
    throwed_p = point;
    Log("ArgChecked", LogTier.Other);
    //Thing thing = card.Thing;
    stateBottleIng = TryCreateBottleIng(new ActType(ActType.Throw), throwed_t, c_thrower);
    if (stateBottleIng == null) { Log("NoBI", LogTier.Info); return; }
    isCheckSuccess = true;
    //return = bi;
    Log("PreSuccess", LogTier.Other);
}
private static void ThrowPPost() 
{
    Log("Start", LogTier.Other);
    string text = "";
    bool tryBrake = stateBottleIng.TryBrake();
    text += "/tB:" + GetStr(tryBrake) + "/";
    //if (thing == null) { LogError("NoCard"); return; }
    //Chara c_drinker = chara;
    //BottleIngredient bi = stateBottleIng;

    bool result = DoRecycle(stateBottleIng, c_thrower, throwed_p);
    text += result ? "Done!" : "Not Done";

    PatchMain.Log(text, LogTier.Info);
}
*/
//static Chara c_thrower;
//harmonyPatch----------------------------------------------------------------------
/*
[HarmonyPrefix]
[HarmonyPatch(typeof(ActThrow), "Throw", new Type[] { typeof(Card), typeof(Point), typeof(Card), typeof(Thing), typeof(ThrowMethod) })]
private static bool ThrowPrePatch(ActThrow __instance, Card c, Point p, Card target, Thing t, ThrowMethod method)
{//begin method
    //stateBottleIng = null;
    //c_thrower = null;
    //throwed_t = null;
    //throwed_p = null;
    //isCheckSuccess = false;
    return true;

}//end method
*/

//if (thing == null) { LogError("NoCard"); return; }
//Chara c_drinker = chara;
//BottleIngredient bi = stateBottleIng;
/*
                string title = "[PBR:AT.T:Post]";
                if (Cf_Allow_Throw)
                {
                    string text = "";
                    if (c == null) { Log(title + "*Error* NoCard"); return; }
                    Chara c_thrower = c.Chara;
                    BottleIngredient bi = __state;
                    if (bi == null) { Log(title + "NoBI", 1); return; }
                    bool tryBrake = bi.TryBrake();
                    text += "/tB:" + GetStr(tryBrake) + "/";
                    bool result = DoRecycle(bi, c_thrower, p);
                    text += result ? "Done!" : "Not Done";
                    //IsThrown = true;
                    //lastThrower = c.Chara;
                    //lastThrownThing = t;
                    //lastCreatedBI = CreateBI(t);

                    PatchMain.Log(title + text, 2);
                } else { Log(title + "'Throw' not Allowed", 3); }
                */
/*
                string title = "[PBR:AT.T;Pre]";
                if (Cf_Allow_Throw)
                {
                    BottleIngredient bi;
                    if (c == null) { Log(title + "*Error* NoCard"); return true; }
                    Chara c_thrower = c.Chara;
                    //Thing t = __instance
                    bi = TryCreateBI(t, c_thrower, new ActType(ActType.Throw));

                    //IsThrown = true;
                    //lastThrower = c.Chara;
                    //lastThrownThing = t;
                    //lastCreatedBI = CreateBI(t);
                    __state = bi;
                    Log(title + "PreFinish", 2);
                } 
                */
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