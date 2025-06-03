using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649PBR.Main;
using s649PBR.BIClass;
using static s649PBR.Main.PatchMain;
using BepInEx.Logging;

namespace s649PBR
{//>begin namespaceMain
    namespace DrinkPatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            private static BottleIngredient stateBottleIng;
            private static void CharaDrinkPatchPreExe(Chara chara, Card card) 
            {
                
                LogStack("[C.D/Pre]");
                CharaDrinkPPre(chara, card);
                LogStackDump();
            }
            private static void CharaDrinkPatchPostExe(Chara chara, Card card)
            {
                LogStack("[C.D/Post]");
                CharaDrinkPPost(chara, card);
                LogStackDump();
            }
            private static void CharaDrinkPPre(Chara chara, Card card)
            {
                Log("Start", LogTier.Other);
                //BottleIngredient bi;
                Chara c_drinker = chara;
                if (card == null) { LogError("NoCard"); return; }
                Thing thing = card.Thing;
                stateBottleIng = TryCreateBottleIng(new ActType(ActType.Use), thing, c_drinker);
                //return = bi;
                Log("PreFinish", LogTier.Other);
            }
            private static void CharaDrinkPPost(Chara chara, Card card) 
            {
                Log("Start", LogTier.Other);
                string text = "";
                if (card == null) { LogError("NoCard"); return; }
                Chara c_drinker = chara;
                BottleIngredient bi = stateBottleIng;
                if (bi == null) { Log("NoBI", LogTier.Info); return; }
                bool result = DoRecycle(bi, c_drinker);
                text += result ? "Done!" : "Not Done";

                PatchMain.Log(text, LogTier.Info);
            }
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Chara), "Drink")]
            private static bool CharaDrinkPrePatch(Chara __instance, Card t) 
            {
                ClearLogStack();
                stateBottleIng = null;
                if (Cf_Allow_Use) { CharaDrinkPatchPreExe(__instance, t); }
                    
                /*
                LogStack("[C.D/Pre]");//string title = "[PBR:C.D/Pre]";
                if (Cf_Allow_Use)
                {
                    Log("Start", LogTier.Other);
                    BottleIngredient bi;
                    Chara c_drinker = __instance;
                    if (t == null) { LogError("NoCard"); goto MethodEnd; }
                    Thing thing = t.Thing;
                    bi = TryCreateBI(thing, c_drinker, new ActType(ActType.Use));
                    __state = bi;
                    Log("PreFinish", LogTier.Other);
                }
            MethodEnd:
                LogStackDump();
                */
                return true;
                
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Chara), "Drink")]
            private static void CharaDrinkPostPatch(Chara __instance, Card t)
            {
                if (Cf_Allow_Use) { CharaDrinkPatchPostExe(__instance, t); }
                else { Log("'Use' not Allowed", LogTier.Other); }
                /*
                LogStack("[C.D/Pre]");//string title = "[PBR:C.D/Post]";
                if (Cf_Allow_Use)
                {
                    Log("Start", LogTier.Other);
                    string text = "";
                    if (t == null) { LogError("NoCard"); goto MethodEnd; }
                    Chara c_drinker = __instance;
                    BottleIngredient bi = __state;
                    if (bi == null) { Log("NoBI", LogTier.Info); goto MethodEnd; }
                    bool result = DoRecycle(bi, c_drinker);
                    text += result ? "Done!" : "Not Done";

                    PatchMain.Log(text, LogTier.Info);
                }
                else { Log("'Use' not Allowed", LogTier.Other); }
            MethodEnd:
                LogStackDump();
                */
            }
            
            

            
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain


//trash

//.owner.Chara;//t.Chara;
//if (t.isThing == false) { Log(title + "*Error* t is not Thing"); return; }
//if (t == null) { Log(title + "*Error* NoCard"); return; }
//.owner.Chara;//t.Chara;
//Thing thing = t.Thing;
//bool tryBrake = bi.TryBrake();
//text += "/tB:" + GetStr(tryBrake) + "/";
//IsThrown = true;
//lastThrower = c.Chara;
//lastThrownThing = t;
//lastCreatedBI = CreateBI(t);
//Thing t = __instance
/*
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDrink), "OnDrink")]
            private static void TraitDrinkPostPatch(TraitDrink __instance, Chara c)
            {//>>>>begin method:TraitDrinkPatch
                if (!IsThrown)
                {
                    string title = "[PBR:TD.OD:Drinked]:";
                    bool b = TryUse(__instance, c);
                    if (b)
                    {
                        Log(title + "Success", 1);
                    }
                    else
                    { Log(title + "NotDone", 1); }
                    
                }
                else //投げられて飲まされた判定
                {
                    string title = "[PBR:TD.OD:Thrown]";
                    bool b = TryThrown(__instance, lastThrower, c.pos, true);
                    if (b)
                    {
                        Log(title + "Success", 1);
                    }
                    else
                    { Log(title + "NotDone", 1); }
                }
                //return true;
            }//<<<<end method:TraitDrinkPatch
            */
/*
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDrink), "OnThrowGround")]
            private static void OnThrowGroundPostPatch(TraitDrink __instance, Chara c, Point p)
            {//begin method:TraitDrinkPatch
                string title = "[PBR:TD.OTG]";
                Log(title + "Start", 2);
                bool b = TryThrown(__instance, c, p, true);
                if (b)
                {
                    Log(title + "Success", 1);
                }
                else
                { Log(title + "NotDone", 2); }
            }//<<<<end method:TraitDrinkPatch
            
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Trait), "OnThrowGround")]
            private static void OnThrowGroundPostPatch(Trait __instance, Chara c, Point p)
            {//begin method:TraitDrinkPatch
                string title = "[PBR:TD.OTG]";
                Log(title + "Start", 2);
                bool b = TryThrown(__instance, c, p, true);
                if (b)
                {
                    Log(title + "Success", 1);
                }
                else
                { Log(title + "NotDone", 2); }
            }//<<<<end method:TraitDrinkPatch
            */
//if (__instance.owner == null) { Log(title + "*Error* NoOwner"); return true; }
//if (t.isThing == false) { Log(title + "*Error* t is not Thing"); return true; }

/*
                    
                    //string text = "";
                    if (Cf_Allow_Use)
                    {
                        if (__instance == null) { Log(title + "*Error* NoInstance");  return; }
                        if (__instance.owner == null) { Log(title + "*Error* NoOwner"); return; }
                        Thing usedT = __instance.owner.Thing;
                        Log(title + "Try/" + GetStr(usedT) +":C" + GetStr(c), 1);
                        bool b = TryRecycle(usedT, c, ActType.Use);
                        if (b)
                        {
                            Log(title + "Success", 1);
                        }
                        else
                        { Log(title + "NotDone", 1); }
                    } */
/*
                if (PatchMain.Cf_Allow_Throw)
                {
                    string title = "[PBR:TD.OTG]";
                    Thing usedT = __instance.owner.Thing;
                    //Chara usedC = c;
                    bool result = PatchMain.TryRecycle(usedT, c, ActType.Throw, true, p);
                    if (result)
                    {
                        PatchMain.Log(title + "Success", 1);
                    }
                    else { PatchMain.Log(title + "NotDone", 1); }
                }
                return true;
                */
//if (PatchMain.Cf_Allow_Throw)
//{
/*
//Thing usedT = __instance.owner.Thing;
Chara c_thrower = lastThrower;
if (c_thrower == null) { Log(title + "*Error* NoThrower"); return; }
BottleIngredient bi = lastCreatedBI;
if (bi == null) { Log(title + "NoBI"); return; }
bi.TryBrake();
//Chara thrownC = PatchMain.lastThrower;
//bool result = PatchMain.TryRecycle(usedT, PatchMain.lastThrower, ActType.Throw, true, c.pos);
bool result = DoRecycle(bi, c_thrower, ActType.Throw, c.pos);
if (result)
{
    PatchMain.Log(title + "Success!", 1);
}
else { PatchMain.Log(title + "NotDone", 1); }
*/

//}

/*
                    string title = "[PBR:Drink]";
                    Thing usedT = __instance.owner.Thing;
                    PatchMain.Log(title + "Used->" + usedT.NameSimple + " :by " + c.NameSimple, 1);
                    bool result = PatchMain.DoRecycleBottle(usedT, c, ActType.Use);
                    if (result)
                    {
                        PatchMain.Log(title + "Success", 1);
                    }
                    */
/*
                   string prod = "";
                   switch(prodN){
                       case 1 : prod = "potion_empty";
                       break;
                       case 2 : prod = "bucket_empty";
                       break;
                       case 0 : prod = "";
                       break;
                       case -1 : prod = "";//bottle
                       break;
                       case -2 : prod = "";//can
                       break;
                       case -3 : prod = "";//can not junk
                       default : prod = "qqq";
                   }
                   */
////sitaya old
/*
string prod = "";
//Thing t = ThingGen.Create("potion_empty");
//Debug.Log("[PBR]Akibin:" + __instance.owner.id.ToString());
int num;
if(int.TryParse(__instance.owner.id, out num))
{//owner.idが数字
    switch(num)
    {
        case 928 : prod = "potion_empty";//horumon
        break;
        case 718 : prod = "1170";//coffee -> akikan
        break;
        case >= 504 and <= 505 : prod = "1170";//-> akikan
        break;
        case >= 48 and <= 59 : prod = "726";//akibin
        break;
        case >= 501 and <= 508 : prod = "726";
        break;
        case >= 718 and <= 1134 : prod = "726";
        break;
        default : prod = "potion_empty";
        break;
    }
} else 
{
    switch(__instance.owner.id)
    {//owner.idが数字以外
        case "bucket" : prod = "bucket_empty";
        break;
        case "crimAle" : prod = "726";
        break;
        case "milk" or "milkcan" : prod = "726";
        break;
        default : prod = "potion_empty";
        break;
    }

}
*/

/*
                            if(PatchMain.cf_F02_NPC_CBWD)
                            {
                                t = ThingGen.Create(prod);
                                if(prod == "potion_empty")
                                {
                                    if(EClass.rnd(2) == 0 || (c.ChildrenWeight > c.WeightLimit))
                                        {EClass._zone.AddCard(t, c.pos);} 
                                    else 
                                        {c.Pick(t);}
                                } else 
                                {
                                    if(EClass.rnd(9) == 0 || (c.ChildrenWeight > c.WeightLimit))
                                        {EClass._zone.AddCard(t, c.pos);} 
                                    else 
                                        {c.Pick(t);}
                                }
                                if(PatchMain.configDebugLogging){Debug.Log("[PBR]Prod->" + prod + " :by " + c.GetName(NameStyle.Simple));}
                            }
                            */

//private static bool Func_Use_Allowed => PatchMain.Cf_Allow_Use;
//private static bool Use_PC_Allowed => PatchMain.Cf_Reg_Use_PC;
//private static bool Use_NPC_Allowed => PatchMain.Cf_Reg_Use_NPC;

//private static bool Allow_CreateJunkBottle => PatchMain.Cf_Reg_JunkBottle;
//private static bool CJB_PC_Allowed => PatchMain.Cf_Reg_CJB_PC;
//private static bool CJB_NPC_Allowed => PatchMain.Cf_Reg_CJB_NPC

//if (c.IsPC && Use_PC_Allowed)
//    {
//       prodT = DoRecycleBottle(usedT);
//        c.Pick(prodT);
//        PatchMain.Log("[PBR]Used->" + usedT.NameSimple + "/Prod->" + prodT.NameSimple + " :by " + c.NameSimple);
//    }
//    else if(!c.IsPC && Use_NPC_Allowed)
//    {
//        prodT = DoRecycleBottle(usedT);
//        EClass._zone.AddCard(prodT, c.pos);
//        PatchMain.Log("[PBR]Used->" + usedT.NameSimple + "/Prod->" + prodT.NameSimple + " :by " + c.NameSimple);
//    } 
//}//<5end if(Func_Use_Allowed)


//Thing result;
//if (prodT != "") 
//{
//    result = ThingGen.Create(prodT);
//    if (c.IsPC) { c.Pick(result); } else { EClass._zone.AddCard(result, c.pos); }
//    PatchMain.Log("[PBR:Drink]result:" + result.NameSimple + "  -> " + c.NameSimple);
//}
