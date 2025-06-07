using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.BIClass;
using s649PBR.Main;
using System;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.UIElements.UxmlAttributeDescription;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using static s649PBR.Main.PatchMain;
using s649ElinLog;
using static s649ElinLog.ElinLog;
namespace s649PBR
{//>begin namespaceMain
    namespace BlendPatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {
            private static readonly string modSubNS = "BP";
            //static string title;// = "[PBR:IOBOP]";
            //static string p_phase;

            private static void CommonProcessOnBlend(string title, Trait instTrait, Thing argThing, Chara argChara)
            {
                ClearLogStack();
                LogStack("[" + modSubNS + "/" + title + "]");
                if (!Cf_Allow_Blend) { LogOther("'Use' not Allowed"); return; }

                //argCheck;
                List<string> checkThings = new();
                string checktext = "";
                BottleIngredient bottleIng;
                try
                {
                    checkThings = new List<string> { GetStr(instTrait), GetStr(argThing), GetStr(argChara) };
                    checktext = string.Join("/", checkThings);
                }
                catch (NullReferenceException ex)
                {
                    LogError("Arg check Failed for NullPo");
                    LogError(checktext);
                    Debug.Log(ex.Message);
                    Debug.Log(ex.StackTrace);
                    return;
                }
                LogDeep("ArgCheck:" + checktext);

                //BIgenerate
                bottleIng = TryCreateBottleIng(new ActType(ActType.Blend), argThing, argChara);
                if (bottleIng == null) { LogDeep("BI was not generated.", LogTier.Deep); return; }

                //DoRecycle
                bool result = DoRecycle(bottleIng, argChara);
                LogDeepTry(result);

            }
            //Harmony-----------------------------------------------------------------------------------------
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDye), "OnBlend")]
            private static void TraitDye_OnBlendPatch(TraitDye __instance, Thing t, Chara c)
            {   //アイテムに混ぜた時
                string title = "TDye.OnB";
                CommonProcessOnBlend(title, __instance, t, c);
                //Log(title + "/FookCheck");
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitWell), "OnBlend")]
            private static void TraitWell_OnBlendPatch(TraitWell __instance, Thing t, Chara c)
            {   //FookCheck:井戸に対して混ぜた時
                string title = "TWell.OnB";
                CommonProcessOnBlend(title, __instance, t, c);
                //Log(title + "/FookCheck");
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDrink), "OnBlend")]
            private static void TraitDrink_OnBlendPatch(TraitDrink __instance, Thing t, Chara c)
            {
                string title = "TDrink.OnB";
                CommonProcessOnBlend(title, __instance, t, c);
                //Log(title + "/FookCheck");
            }

        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain



/////trash////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*
             * Fook出来てない挙動
             * 染料を持ってオブジェクトなどに使った時->Useなのでそっちで
             * 
             */
/*
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Trait), "OnBlend")]
            private static bool TraitOnBlendPrePatch(Trait __instance, Thing t, Chara c, ref BottleIngredient __state)
            {
                string title = "[PBR:T.OB/Pre]";
                if (Cf_Allow_Blend)
                {
                    BottleIngredient bi;
                    if (__instance.owner == null) { Log(title + "*Error* NoOwner"); return true; }
                    Chara c_blender = __instance.owner.Chara;
                    Thing thing = __instance.owner.Thing;
                    bi = TryCreateBI(thing, c_blender, new ActType(ActType.Blend));
                    __state = bi;
                    Log(title + "PreFinish", 2);
                }
                return true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Trait), "OnBlend")]
            private static void TraitOnBlendPostPatch(Trait __instance, Thing t, Chara c, BottleIngredient __state)
            {
                string title = "[PBR:T.OB/Post]";
                if (Cf_Allow_Blend)
                {
                    string text = "";
                    if (__instance.owner == null) { Log(title + "*Error* NoOwner"); return; }
                    Chara c_blender = __instance.owner.Chara;
                    BottleIngredient bi = __state;
                    if (bi == null) { Log(title + "NoBI", 1); return; }
                    bool result = DoRecycle(bi, c_blender);
                    text += result ? "Done!" : "Not Done";

                    PatchMain.Log(title + text, 2);
                }
                else { Log(title + "'Use' not Allowed", 3); }
            }
            */
/*
[HarmonyPostfix]
[HarmonyPatch(typeof(TraitWell), "OnBlend")]
private static void TraitWellPostPatch(TraitWell __instance, Thing t, Chara c)
{//>>>>begin method:TraitDrinkPatch
    string title = "[PBR:TW.OB]";
    if (t.trait == null) { Log(title + "*Error* NoTrait", 1); return; }
    bool b = TryBlend(t.trait, c);
    if (b)
    {
        Log(title + "Success", 1);
    }
    else
    { Log(title + "NotDone", 1); }
}//<<<<end method:TraitDrinkPatch

[HarmonyPostfix]
[HarmonyPatch(typeof(TraitDrink), "OnBlend")]
private static void OnBlendPostPatch(TraitDrink __instance, Thing t, Chara c)
{//>>>>begin method:OnUsePostPatch
    string title = "[PBR:TDye.OB]";
    bool b = TryBlend(__instance, c);
    if (b)
    {
        Log(title + "Success", 1);
    }
    else
    { Log(title + "NotDone", 1); }
}//<<<<end method:OnUsePostPatch
*/
/*
[HarmonyPrefix]
[HarmonyPatch(typeof(InvOwnerBlend), "_OnProcess")]
private static bool IOBOnProcessPrePatch(InvOwnerBlend __instance, Thing t, ref BottleIngredient __state)
{   //fookinfo:インベントリのアイテムを混ぜた時全般？//well/dye/poison

    if (Cf_Allow_Blend)
    {
        p_phase = "Pre/";
        BottleIngredient bi;
        //if (__instance.owner == null) { Log(title + "*Error* NoOwner"); return true; }
        Chara c_blender = EClass.pc; // __instance.owner.Chara;
        Thing thing = t;// __instance.owner.Thing;
        bi = TryCreateBI(thing, c_blender, new ActType(ActType.Blend));
        __state = bi;
        Log(title + p_phase + "Finish", 2);
    }
    return true;

    return true;
}
[HarmonyPostfix]
[HarmonyPatch(typeof(InvOwnerBlend), "_OnProcess")]
private static void IOBOnProcesdPostPatch(InvOwnerBlend __instance, Thing t, BottleIngredient __state)
{
    //title = "[PBR:IOBOP]";
    //Log(title + "FookCheck");

    if (Cf_Allow_Blend)
    {
        p_phase = "Post/";
        string text = "";
        //if (__instance.owner == null) { Log(title + "*Error* NoOwner"); return; }
        Chara c_blender = EClass.pc; // __instance.owner.Chara;
        BottleIngredient bi = __state;
        if (bi == null) { Log(title + "NoBI", 1); return; }
        bool result = DoRecycle(bi, c_blender);
        text += result ? "Done!" : "Not Done";

        PatchMain.Log(title + p_phase + text, 2);
    }
    else { Log(title + "'Blend' not Allowed", 3); }

}
*/

//IsThrown = true;
//lastThrower = c.Chara;
//lastThrownThing = t;
//lastCreatedBI = CreateBI(t);
//bool tryBrake = bi.TryBrake();
//text += "/tB:" + GetStr(tryBrake) + "/";
//Thing thing = t.Thing;
//if (t.isThing == false) { Log(title + "*Error* t is not Thing"); return; }
//if (t == null) { Log(title + "*Error* NoCard"); return; }
//Thing t = __instance
//if (t.isThing == false) { Log(title + "*Error* t is not Thing"); return true; }
//if (t == null) { Log(title + "*Error* NoCard"); return true; }
/*
                if (PatchMain.Cf_Allow_Blend)
                {
                    string title = "[PBR:TW.OB]";
                    bool b = PatchMain.TryRecycle(t, c, ActType.Blend);
                    if (b)
                    {
                        PatchMain.Log(title + "Success", 2);
                    }
                }
                */
/*
                if (PatchMain.Cf_Allow_Blend)
                {
                    string title = "[PBR:WellBlend]";
                    Thing usedT = t;
                    PatchMain.Log(title + "Blend->" + usedT.NameSimple + " :by " + c.NameSimple, 1);
                    bool result = PatchMain.DoRecycleBottle(usedT, c, ActType.Blend);
                    if (result)
                    {
                        PatchMain.Log(title + "Success", 1);
                    }
                }
                */
/*
 * 
 * 
        {//>>>begin class:PatchExe
            //private static Thing DoRecycleBottle(Thing t, Chara c, int at, bool broken = false) { return PatchMain.DoRecycleBottle(t, c, at, broken); }

            //TraitWell.OnBlend実行時にも瓶を還元する

                PatchMain.Log("[PBR:WellBlend]Used->" + t.NameSimple + " /C: " + c.NameSimple, 1);
                Thing result = DoRecycleBottle(t, c, ActType.Blend);
                if (result != null)
                {
                    PatchMain.Log("[PBR:WellBlend]result->" + result.NameSimple, 1);
                }
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

//if(PatchMain.configDebugLogging){Debug.Log("[PBR]Blend->" + t.id.ToString());}
//if(Func_Blend_Allowed)
//{
//Thing usedT = t;
//Thing prodT = DoRecycleBottle(usedT);

//Thing result;
//if (prodT != "")
//{
//    result = ThingGen.Create(prodT);
//    if (c.IsPC) { c.Pick(result); } else { EClass._zone.AddCard(result, c.pos); }
//    PatchMain.Log("[PBR:WellBlend]result[" + result.NameSimple + " -> " + c.NameSimple + "]");
//}
//DoRecycleBottle(t, c, ActType.Blend);

//Thing usedT = __instance.owner.Thing;
//PatchMain.Log("[PBR:Drink]Used->" + usedT.NameSimple + " :by " + c.NameSimple, 1);

//Thing usedT = __instance.owner.Thing;
//string prodT = DoRecycleBottle(t, c, ActType.Blend);