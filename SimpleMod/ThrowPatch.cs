using BepInEx;
//using BepInEx.Configuration;
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
            private static readonly string modSubNS = "TP";
            
            static BottleIngredient stateBottleIng;
            static Chara c_thrower;
            static Thing throwed_t;
            static Point throwed_p;
            static Card target_state;
            static bool isPreCheckSuccess;
            static bool isCreateSuccess;
            //static bool isRecycleDone;
            static bool trySetAllow;
            static ThrowType stateThrowType;
            //static bool needDoRecycleSkip;

            private static void InitState() 
            {
                stateBottleIng = null;
                c_thrower = null;
                throwed_t = null;
                throwed_p = null;
                target_state = null;

                isPreCheckSuccess = false;
                isCreateSuccess = false;
                trySetAllow = false;
                stateThrowType = ThrowType.Default;
                //needDoRecycleSkip = false;
            }
            
            private static void CommonThrowProcessPre(Card card_thrower, Point point, Card target, Thing thing, ThrowMethod method) 
            {
                //BottleIngredient stateBottleIng;
                ThrowType throwtype;
                List<string> checkThings;
                string checktext = "";
                try
                {
                    throwtype = thing.trait.ThrowType;
                    checkThings = new List<string> { GetStr(throwtype), GetStr(card_thrower.Chara), GetStr(point), GetStr(target), GetStr(thing) };
                    checktext = string.Join("/", checkThings);
                }
                catch (NullReferenceException ex)
                {
                    LogError("ThrowType check Failed for NullPo");
                    LogError(checktext);
                    Debug.Log(ex.Message);
                    Debug.Log(ex.StackTrace);
                    return;
                }
                LogDeep("Throwtype:" + checktext);
                c_thrower = card_thrower.Chara;
                throwed_t = thing;
                throwed_p = point;
                target_state = target;
                stateThrowType = throwtype;

                if (IsInProhibitionList(thing.id)) { LogDeep("Prohibition Item"); return; }
                bool trySetAllow = false;
                if (throwtype == ThrowType.Potion || throwtype == ThrowType.Vase || throwtype == ThrowType.Default || thing.trait is TraitDye)
                {
                    trySetAllow = true;
                }
                if (!trySetAllow) { LogOther("Throwed thing is cannot recycle"); return; }
                isPreCheckSuccess = true;
                //CommonProcessThrowPost(stateBottleIng, target, card_thorwer.Chara, point);
            }

            private static void CommonThrowProcessPhaseCreate() 
            {
                if (!isPreCheckSuccess || isCreateSuccess) { return; }
                try
                {
                    // var args = new List<string> { GetStr(c), GetStr(p), GetStr(target), GetStr(t), GetStr(throwtype) };
                    //var argtext = string.Join("/", args);
                    //LogDeep("Start/Arg:" + argtext, LogTier.Deep);
                    //if (!card_thorwer.isChara || card_thorwer.Chara == null) { { LogError("*Warn* c is invalid"); return; } }
                    //c_thrower = card_thorwer.Chara;
                    //throwed_t = thing;
                    //throwed_p = point;

                    //Thing thing = card.Thing;
                    stateBottleIng = TryCreateBottleIng(new ActType(ActType.Throw), throwed_t, c_thrower);
                    if (stateBottleIng == null) { LogDeep("BI was not generated.", LogTier.Deep); return; }
                    bool tryBrake = stateBottleIng.TryBrake();
                    LogDeepTry(tryBrake, "tryBrake");
                    //isCheckSuccess = true;
                    //return = bi;
                    //LogDeepTry(stateBottleIng != null);
                    //isCheckSuccess = true;

                }
                catch (NullReferenceException ex)
                {
                    LogError("Error for NullPo");
                    Debug.Log(ex.Message);
                    Debug.Log(ex.StackTrace);
                    return;
                }
                isCreateSuccess = true;
            }

            private static void CommonThrowProcessPost()
            {
                //ClearLogStack();
                //string title = "HM:AT.T/Post";
                //LogStack("[" + modSubNS + "/" + title + "]");

                //if (!isCheckSuccess) { LogTweet("Through Card.Die"); return; }

                //string text = "";
                if (!isCreateSuccess) { return; }
                if (!(stateBottleIng.GetOrgTrait() is TraitDrink)) 
                {
                    /*
                     ����������擾����K�v�L��
                     */
                    if (target_state != null || throwed_p.HasObj) 
                    {
                        //will brake
                        //bool tryBrake = argBI.TryBrake();
                        //LogDeepTry(tryBrake, "tryBrake");
                    } 
                    else
                    {
                        //won't brake cannnot recycle
                        LogDeep("Just Dropped");
                        return;
                    }
                }
                else
                {
                    //will brake
                    //bool tryBrake = argBI.TryBrake();
                    //LogDeepTry(tryBrake, "tryBrake");
                }
                    
                //text += "/tB:" + GetStr(tryBrake) + "/";

                bool result = DoRecycle(stateBottleIng, c_thrower, throwed_p);
                LogDeepTry(result);
                LogOther("The recycle has been completed.");
                InitState();
                //isRecycleDone = true;
                //InitState();
            }


            //harmony--------------------------------------------------------------------------------------------------
            [HarmonyPrefix]
            [HarmonyPatch(typeof(ActThrow), "Throw", new Type[] { typeof(Card), typeof(Point), typeof(Card), typeof(Thing), typeof(ThrowMethod) })]
            private static bool ThrowPrePatch(ActThrow __instance, Card c, Point p, Card target, Thing t, ThrowMethod method)
            {//begin method
                
                InitState();
                ClearLogStack();
                string title = "AT.T";
                LogStack("[" + modSubNS + "/" + title + "]");
                if (!Cf_Allow_Throw) { LogOther("'Throw' not Allowed", LogTier.Other); return true; }
                //if(method != ThrowType.Potion)
                //bool isCheckSuccess = false;
                //throwtyoe check //potion�ȊO�����O
                CommonThrowProcessPre(c, p, target, t, method);
                //if (throwtype != ThrowType.Potion && !(t.trait is TraitDye)) { LogOther("Throwed Thing is not Potion."); return; }
                
                //if (t.trait is TraitDye)
                //{
                //    trySetAllow = true;
                //    //needDoRecycleSkip = true;
                //}
                //argcheck
                
                
                //if (!needDoRecycleSkip) { LogOther("TraitDye"); }
                
                //text += result ? "Done!" : "Not Done";


                //LogStackDump();
                return true;
                
            }//end method



            [HarmonyPostfix]
            [HarmonyPatch(typeof(ActThrow), "Throw", new Type[] { typeof(Card), typeof(Point), typeof(Card), typeof(Thing), typeof(ThrowMethod) })]
            private static void ThrowPostPatch(ActThrow __instance, Card c, Point p, Card target, Thing t, ThrowMethod method)
            {
                //
                CommonThrowProcessPost();
            }


            [HarmonyPostfix]
            [HarmonyPatch(typeof(Card), "Die")]//TraitDye�̃t�b�N
            internal static void Card_Die_PostPatch() 
            {
                CommonThrowProcessPhaseCreate();
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Card), "Destroy")]//TraitDye�̃t�b�N
            internal static void Card_Destroy_PostPatch() 
            {
                CommonThrowProcessPhaseCreate();
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Trait), "OnThrowGround")]
            internal static void TraitDrink_OnThrowGround_PostPatch(Chara c, Point p) 
            {
                CommonThrowProcessPhaseCreate();
            }

            /*
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Card), "Die")]//TraitDye�̃t�b�N
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
            
            [HarmonyPostfix]//TraitDye�ȊO��potion�̃t�b�N
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
            }*/

        }//<<<end class:PatchExe

    }//<<end namespaceSub
}//<end namespaceMain


/////trash***************************************************************************************
///


/*
             * �s����
             * �����𓊂��Ēn�ʂɗ��������͉��Ȃ����A�K���X���Ҍ�����Ă��܂�
             * ActThrow���s���ɂ͓������ď����Ă��邩�m�肵�Ă��Ȃ��̂�
             * ������ւ�̒ǉ��������K�v
             * OnDrink�i�����݂�r���K�{�j��OnthrowGround�ŊҌ����ׂ����H
             * ��������Ҍ������K���X�����̍ގ����Q��
             * 
             */
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