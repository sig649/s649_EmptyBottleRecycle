using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.Main;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Net.NetworkInformation;
using UnityEngine;
using static Recipe;
using Debug = UnityEngine.Debug;
using s649PBR.BIClass;
using static s649PBR.Main.PatchMain;

namespace s649PBR
{//>begin NamespaceMain
    namespace CrafterPatchMain
    {//>>begin NamespaceSub

        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            static string title = "[PBR:Craft]";
            static List<BottleIngredient> recycleQueues;
            static Thing lastMixedThing;
            static int lastCraftCount;
            static int recycleSet;

            private static bool ExeRecycle()
            {
                string text = "[recycle]";
                bool b = false;
                foreach (BottleIngredient bi in recycleQueues)
                {
                    if (bi.IsEnableRecycle()) 
                    {
                        Thing t = ThingGen.Create(bi.GetID()).SetNum(bi.num);
                        EClass._zone.AddCard(t, EClass.pc.pos);
                        text += "N:" + t.NameSimple + "/n:" + t.Num.ToString();
                        b = true;
                    }
                }
                PatchMain.Log(text, 1);
                return b;
            }
            private static bool AddBIToQueues(BottleIngredient bi) 
            {
                //if (biList.Count == 0) { biList.Add(bottleIngredient); return; }
                if (!bi.IsEnableRecycle()) { return false; }
                foreach (BottleIngredient bottleIngredient in recycleQueues) 
                {
                    if (bottleIngredient.IsEqualID(bi) && bi.IsEnableRecycle()) 
                    {
                        bottleIngredient.AddNum(bi);
                        return true;
                    }
                }
                recycleQueues.Add(bi);
                return true;
            }
            private static bool RemoveBIFromQueues(BottleIngredient bi) 
            {
                if (!bi.IsEnableRecycle() || !bi.isProhibition) { return false; }
                foreach (BottleIngredient bottleIngredient in recycleQueues)
                {
                    if (bottleIngredient.IsEqualID(bi) && bi.IsEnableRecycle())
                    {
                        bottleIngredient.Decrease(bi);
                        return true;
                    }
                }
                return false;
            }
            private static void SetMultiNum(int argNum)
            {
                if (recycleQueues.Count <= 0) { return; }
                foreach (BottleIngredient bi in recycleQueues)
                {
                    bi.SetMultiNum(argNum);
                }
            }
            private static string GetStrings(List<BottleIngredient> biList) 
            {
                string text = "";
                if (biList.Count > 0)
                {
                    foreach (BottleIngredient bi in biList)
                    {
                        if (bi.IsEnableRecycle())
                        {
                            text += bi.ToString() + "/";
                        }
                    }
                }
                else { text += "-"; }
                return text;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(AI_UseCrafter), "OnStart")]
            private static void AI_UseCrafterOnStartPostPatch(AI_UseCrafter __instance)
            {
                title = "[PBR:Craft/AIUC.OS]";
                recycleQueues = null;
                lastMixedThing = null;
                lastCraftCount = 0;
                recycleSet = 0;
                //isProhibition = false;
                //processRecycleList = null;
                if (!PatchMain.Cf_Allow_Craft) { return; }
                
                recycleQueues = new List<BottleIngredient>();
                TraitCrafter trait = __instance.crafter;
                bool isFactory = trait is TraitFactory;
                PatchMain.Log(title + "iF:" + isFactory.ToString() + "/t:" + trait.ToString());

                if (isFactory)
                {
                    //PatchMain.Log("[PBR:craft]Crafter is TraitFactory", 1);
                    Recipe recipe = __instance.recipe;
                    List<Ingredient> ingredients = (recipe != null) ? recipe.ingredients : null;
                    //checkRecipesource
                    if (recipe != null && ingredients != null)
                    {   

                        string text = "";
                        text += "recipe:" + recipe.GetName();
                        //text += "/id:" + id;
                        //text += "/category:" + category;
                        //text += "/unit:" + unit;
                        //text += "/num:" + num.ToString();
                        PatchMain.Log(title + text, 1);

                        text = "ingre:";
                        foreach (Ingredient ing in ingredients)
                        {
                            var bi = new BottleIngredient(ing.thing, ing.req);

                            if (bi.IsEnableRecycle())
                            {
                                text += "bi:" + GetStr(bi) ;
                                AddBIToQueues(bi);
                            }
                            text += "/";
                        }
                        //RecycleThing resultRT = new RecycleThing(resultBI);
                        //PatchMain.RemoveFromList(recycleList, resultRT);

                        //text += "/rBI:" + resultBI.ToString();
                        PatchMain.Log(title + text, 1);

                        string recycleString = "queue:" + GetStrings(recycleQueues);
                        
                        PatchMain.Log(title + recycleString, 1);

                        //queueに追加する
                        ///if (recycleList.Count > 0)
                        //{
                       //     recycleQueue = new(recycleList);
                       //     PatchMain.Log(title + "NewRecycleQueue:" + recycleQueue.ToString(), 1);
                       // }
                       // else { PatchMain.Log(title + "NoRecycleQueue......", 1); }
                    }
                    

                }
                //else //notFactory
                //{
                    
                    //PatchMain.Log(title + "processor:" + lastCraftCount.ToString(), 1);
                    //PatchMain.Log(title + "recycleList:" + GetStr(recycleList), 1);
                //}
            }
            
            [HarmonyPostfix]
            [HarmonyPatch(typeof(RecipeCard), "Craft")]
            private static void RecipeCardCraftPostPatch(Thing __result, BlessedState blessed, bool sound, List<Thing> ings, TraitCrafter crafter, bool model)
            {
                //PatchMain.Log(title + "RecipeCard/Craft");
                if (__result != null)
                {
                    lastMixedThing = __result;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(TraitCrafter), "Craft")]
            private static bool TraitCrafterCraftPrePatch(TraitCrafter __instance, AI_UseCrafter ai)
            {
                title = "[PBR:Craft/TC.C/Pre]";
                if (!PatchMain.Cf_Allow_Craft) { return true; }
                
                if (recycleSet == PhaseRecycle.None)
                {
                    PatchMain.Log(title + "SetStart", 1);
                   // string text = "ings:";
                    //string resultBI = "";
                    //recycleQueue = new List<RecycleThing>();
                    if (ai == null)
                    {
                        PatchMain.Log(title + "NoAI");
                        return true;
                    }
                    else { PatchMain.Log(title + "AI:Found", 1); }
                    List<Thing> ings = ai.ings;
                    if (ings == null || ings.Count <= 0)
                    {
                        PatchMain.Log(title + "NoIngs");
                        return true;
                    }
                    else { PatchMain.Log(title + "ai.ings:Found", 1); }
                    foreach (Thing thing in ings)
                    {
                        if (thing == null) { break; }

                        var bi = new BottleIngredient(thing);
                        PatchMain.Log(title + "BIcheck:" + thing.NameSimple + "/bi:" + bi.ToString(), 1);
                        //text += "[" + thing.NameSimple + ":" + bi + ":" + thing.Num + "]";
                        if (bi.IsEnableRecycle())
                        {
                            //string thingBI = PatchMain.GetStringID(thing);
                            //result = DoRecycleBottle(t, ai.owner, ActType.Craft);
                            bool b = AddBIToQueues(bi);
                            if (b) 
                            {
                                PatchMain.Log(title + "processRLSet:Success/" + GetStrings(recycleQueues), 1);
                            } else 
                            {
                                PatchMain.Log(title + "processRLSet:Failed....../" + GetStrings(recycleQueues), 1);
                            }
                            
                        }
                        else { PatchMain.Log(title + "processRLSet:Canceled......", 1); }
                        //PatchMain.Log(text, 1);
                    }
                    recycleSet++;
                }
                else { PatchMain.Log(title + "SetSkipped", 2); }
                return true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitCrafter), "Craft")]
            private static void TraitCrafterCraftPostPatch(TraitCrafter __instance, AI_UseCrafter ai, Thing __result)
            {//>>>>begin method:PostPatch
                title = "[PBR:TC.C:Post]";
                if (!PatchMain.Cf_Allow_Craft) { return; }
                //PatchMain.Log("[PBR:craft]Fook:TraitCrafter/Craft");
                if (__result == null)
                {
                    PatchMain.Log(title + "NoResult");
                    return;
                }
                if (ai == null)
                {
                    PatchMain.Log(title + "NoAI");
                    return;
                }
                //PatchMain.Log("[PBR:craft]Fook:TraitCrafter/Craft", 1);

                if (recycleSet == PhaseRecycle.IngSet)
                {
                    
                    if (recycleQueues.Count > 0)
                    {
                        //string resultBI = PatchMain.GetStringID(__result);
                        lastMixedThing = __result;
                        var bi = new BottleIngredient(__result, __result.trait.CraftNum);
                        PatchMain.Log(title + "BIcheck:" + __result.NameSimple + "/bi:" + bi.ToString(), 1);
                        //productにbottleingが含まれている場合はリターンを減産する
                        if (bi.IsEnableRecycle())
                        {
                            bool b = RemoveBIFromQueues(bi);
                            if (b)
                            {
                                PatchMain.Log(title + "Remove:Success" + GetStrings(recycleQueues), 1);
                            }
                            else { PatchMain.Log(title + "Remove:Fail" + GetStrings(recycleQueues), 1); }
                        }
                        else
                        { PatchMain.Log(title + "Result:NoBI" + GetStrings(recycleQueues), 1); }

                    }
                    else { PatchMain.Log(title + "NoRecycleQueue" + GetStrings(recycleQueues), 1); }
                    
                    recycleSet++;
                    //PatchMain.ExeRecycle(recycleList, EClass.pc);
                }
                else { PatchMain.Log(title + "Skipped:", 2); }
                lastCraftCount++;
                PatchMain.Log(title + "Count:" + lastCraftCount.ToString(), 2);

            }//<<<<end method:PostPatch

            [HarmonyPostfix]
            [HarmonyPatch(typeof(AI_UseCrafter), "OnEnd")]
            private static void AI_UseCrafterOnEndPostPatch(AI_UseCrafter __instance)
            {
                title = "[PBR:Craft/AIUC.OE:Post]";
                if (!PatchMain.Cf_Allow_Craft) { return; }
                PatchMain.Log(title + "AI_UseCrafter/OnEnd", 1);
                TraitCrafter trait = __instance.crafter;
                
                if (trait is TraitFactory)
                {   //作業台など
                    if (lastMixedThing != null && recycleQueues != null && recycleQueues.Count > 0)
                    {
                        //craftしたThingからBIを作成しつつRemove
                        PatchMain.Log(title + "lastMixed:" + lastMixedThing.NameSimple + "/num:" + lastMixedThing.Num);
                        var bi = new BottleIngredient(lastMixedThing, lastMixedThing.trait.CraftNum);
                        if (bi.IsEnableRecycle()) //還元除外用
                        {
                            //recycleQueue.RemoveBIFromQueues(new (PatchMain.GetStringID(lastMixedThing), lastMixedThing.trait.CraftNum));
                            RemoveBIFromQueues(bi);
                        } else { PatchMain.Log(title + "Result:NoBI"); }
                        SetMultiNum(__instance.num);//回数分倍化
                        //bool consumed = ConsumeIng(lastMixedThing);
                        ExeRecycle();
                        //recycleQueue.ExeRecycle();
                        //recycleQueue.RemoveAll();
                        PatchMain.Log(title + "RecycleDone!", 1);
                    } else { PatchMain.Log(title + "Canceled:RemoveFromResult", 1); }
                    
                }
                else 
                {   //加工機械
                    //PatchMain.Log("[PBR:craft]Crafter is not TraitFactory", 1);
                    if (lastCraftCount != 0 && recycleQueues != null && recycleSet == PhaseRecycle.Done) 
                    {
                        PatchMain.Log(title + "lastCraftCount:" + lastCraftCount.ToString());
                        SetMultiNum(lastCraftCount);
                        PatchMain.Log(title + "RQ:" + GetStrings(recycleQueues));
                        //bool consumed = ConsumeIng(lastMixedThing);
                        ExeRecycle();
                        //recycleQueue.RemoveAll();
                        PatchMain.Log(title + "RecycleDone!", 1);
                    }
                    else { PatchMain.Log(title + "RecycleNotExecuted", 1); }
                }
                
                //Chara owner = __instance.owner;
                //PatchMain.Log("[PBR:craft]chara:" + owner.NameSimple, 1);
            }
        }//<<<end class:PatchExe

        public class PhaseRecycle
        {//class:ActType
            //public int value;
            public const int None = 0;
            public const int IngSet = 1;
            public const int Done = 2;
            //public const int Throw = 3;
            //public const int Craft = 4;

        }//class:ActType
    }//<<end namespaceSub
}//<end namespaceMain

///trashbox//////////////////////////////////////////////////////////////////
/// 
/*
//           trash box
//
////////////////////////////////////////////////
*/


/*
                        if (recycleQueues.Count > 0)
                        {
                            foreach (BottleIngredient bi in recycleQueues)
                            {
                                if(bi.IsEnableRecycle())
                                {
                                    recycleString += bi.id + "." + bi.num.ToString() + "/";
                                }
                            }
                        }
                        else { recycleString += "-"; }*/
//string resultBI = PatchMain.GetStringID(__result);
//productにbottleingが含まれている場合はリターンを減産する
//PatchMain.RemoveFromList(recycleList, new RecycleThing(resultBI), __result.Num);
//recycleQueue = new RecycleQueue(recycleList);
//PatchMain.Log(title + "QueueSet:" + recycleQueue.ToString(), 1);
//PatchMain.ExeRecycle(recycleList, EClass.pc);
//static bool isProhibition;
//static bool ConsumeIng;

//static List<RecycleThing> processRecycleList;
//private static bool ConsumeIng(Thing t) 
//{
//    if (t.id == "tool_alchemy" || t.id == "waterPot") { return true; } else { return false; }
//}
//craftしたThingをうまくFook出来なかったのでrecipeからBIを生成
//RecipeCardからfook出来た
/*
var id = recipe.id;
if (!EClass.sources.things.map.ContainsKey(id)) { PatchMain.Log(title + "*error* idが無いよ"); return; }
var category = EClass.sources.things.map[id].category;
var unit = EClass.sources.things.map[id].unit;
var traits = EClass.sources.things.map[id].trait;
int num = __instance.num;//製作回数
var resultBI = new BottleIngredient(id, category, unit, traits, num);
*/
//if (PatchMain.IsValid(recycleQueues))
//{
//    recycleQueues = new RecycleQueue(recycleQueues);
//    PatchMain.Log(title + "QueueSet:" + recycleQueues.ToString(), 1);
//}
//else { PatchMain.Log(title + "QueueSet:Fail", 1); }
/*
                    if(resNum > 0)
                        {
                            if(resT != null)
                            {
                                resT.SetNum(resNum);
                                text += "[res:" + resT.ToString() +"]";
                                text += "[resN:" + resNum.ToString() +"]";
                                EClass._zone.AddCard(resT, pos);
                            }
                        } else 
                        {
                            if(resT != null){resT.Destroy();}
                        }

 [HarmonyPostfix]
            [HarmonyPatch(typeof(AI_Craft), "OnSuccess")]
            internal static void APostPatch()
            {
                PatchMain.Log("[PBR]AI_craft/OnSucess/Fook");
            }
 */

//private static bool PC_Allowed => PatchMain.cf_F01_PC_CBWD;

/*
[HarmonyPostfix]
[HarmonyPatch(typeof(AI_UseCrafter), "OnSuccess")]
internal static void OnSuccessPostPatch(AI_UseCrafter __instance)
{
    PatchMain.Log("[PBR:craft]Fook:AI_UseCrafter/OnSuccess", 1);
    List<Thing> ings = __instance.ings;
    string text = "";
    foreach(Thing thing in ings)
    {
        text += thing.NameSimple + "/";
    }
    PatchMain.Log("[PBR:craft]ings:" + text, 1);
    Chara owner = __instance.owner;
    PatchMain.Log("[PBR:craft]chara:" + owner.NameSimple, 1);
}
*/

//string[] traits;
/*
if (EClass.sources.things._rows.ContainsKey(recipe.id))
{
traits = EClass.sources.things._rows[recipe.id].trait;
} else if(EClass.sources.thingV.trait.ContainsKey(recipe.id))
{

}*/

//var traits = source.trait;
//var sourceRow = source.row;
//Thing result = recipe.Craft(BlessedState.Normal, false, ings, trait);
//List<Ingredient> ingredients = (recipe != null) ? recipe.ingredients : null;

//string text = "";
//foreach (Thing thing in ings)
//{
//    text += thing.NameSimple + "/";
//}
/*
[HarmonyPostfix]
[HarmonyPatch(typeof(Recipe), "Craft")]
private static void RecipeCraftPostPatch(Thing __result, BlessedState blessed, bool sound, List<Thing> ings, TraitCrafter crafter, bool model)
{
    //PatchMain.Log("[PBR:craft]Fook:Recipe/Craft");
    /*
    if (__result == null)
    {
        PatchMain.Log("[PBR:Craft]NoResult");
        return;
    }
    PatchMain.Log("[PBR:craft]product:" + __result.NameSimple + "/Num:" + __result.Num.ToString(), 1);
    if (ings == null)
    {
        PatchMain.Log("[PBR:Craft]NoIngs");
        return;
    }
    if (Func_Craft_Allowed)
    {
        string text = "[PBR:craft]ings:";
        //string resultBI = "";

        var recycleList = new List<RecycleThing>();
        foreach (Thing thing in ings)
        {
            string bi = GetStringID(thing);
            text += "[" + thing.NameSimple + ":" + bi.ToString() + ":" + thing.Num + "]";
            if (bi != "")
            {
                //result = DoRecycleBottle(t, ai.owner, ActType.Craft);
                PatchMain.AddThingToList(recycleList, new(bi, thing.Num));
            }
        }
        PatchMain.Log(text, 1);

        string resultBI = GetStringID(__result);
        //productにbottleingが含まれている場合はリターンを減産する
        PatchMain.RemoveFromList(recycleList, resultBI, __result.Num);
        PatchMain.ExeRecycle(recycleList, EClass.pc);

    }
}

[HarmonyPostfix]
[HarmonyPatch(typeof(RecipeCard), "Craft")]
private static void RecipeCardCraftPostPatch(Thing __result, BlessedState blessed, bool sound, List<Thing> ings, TraitCrafter crafter, bool model)
{
    //PatchMain.Log("[PBR:craft]Fook:RecipeCard/Craft");
    //if(__result != null) { PatchMain.Log("[PBR:craft]product:" + __result.NameSimple + "/Num:" + __result.Num.ToString(), 1); }
    // PatchMain.Log("[PBR:craft]blessed:" + blessed.ToString(), 1);
    //PatchMain.Log("[PBR:craft]sound:" + sound.ToString(), 1);
    //if (ings != null) { PatchMain.Log("[PBR:craft]ings:" + ings.ToString(), 1); }
    //if (crafter != null) { PatchMain.Log("[PBR:craft]crafter:" + crafter.ToString(), 1); }
    //PatchMain.Log("[PBR:craft]model:" + model.ToString(), 1);


    /*
    if (__result == null)
    {
        PatchMain.Log("[PBR:Craft]NoResult");
        return;
    }
    //PatchMain.Log("[PBR:craft]Fook:RecipeCard/Craft", 1);
    PatchMain.Log("[PBR:craft]product:" + __result.NameSimple + "/Num:" + __result.Num.ToString(), 1);

    if (Func_Craft_Allowed)
    {
        if (ings == null)
        {
            PatchMain.Log("[PBR:Craft]NoIngs");
            return;
        }
        string text = "[PBR:craft]ings:";
        //string resultBI = "";

        var recycleList = new List<RecycleThing>();

        foreach (Thing thing in ings)
        {
            string bi = GetStringID(thing);
            text += "[" + thing.NameSimple + ":" + bi.ToString() + ":" + thing.Num + "]";
            if (bi != "")
            {
                //result = DoRecycleBottle(t, ai.owner, ActType.Craft);
                PatchMain.AddThingToList(recycleList, new(bi, thing.Num));
            }
        }
        PatchMain.Log(text, 1);

        string resultBI = GetStringID(__result);
        //productにbottleingが含まれている場合はリターンを減産する
        PatchMain.RemoveFromList(recycleList, resultBI, __result.Num);
        PatchMain.ExeRecycle(recycleList, EClass.pc);
    }
}*/


/*if (ings != null)
{
    text += "/ing:";
    foreach (Thing ing in ings)
    {
        if (ing != null) { text += ing.id + "." + ing.Num + ":"; }
    }
}
else { text += "/ing:-"; }

 [HarmonyPostfix]
            [HarmonyPatch(typeof(Recipe), "Craft")]
            private static void RecipeCraftPostPatch(Thing __result, BlessedState blessed, bool sound, List<Thing> ings, TraitCrafter crafter, bool model)
            {
                PatchMain.Log("[PBR:craft]Fook:Recipe/Craft");
            }
 lastMixedThing.sourceCard.category != "_tool"
 */
//if (source != "") { text += "/sourceid:" + source; } else { text += "/sourceid:-"; }


//text += "/cat:" + cat;
/*
               [HarmonyPostfix]
               [HarmonyPatch(typeof(AI_UseCrafter), "Run")]
               private static void AI_UseCrafterRunPostPatch(AI_UseCrafter __instance)
               {
                   PatchMain.Log("[PBR:craft]AI_UseCrafter/Run");
                   TraitCrafter trait = __instance.crafter;
                   if (trait is TraitFactory) 
                   {
                       if (recycleQueue != null && lastMixedThing != null)
                       {
                           PatchMain.Log(title + "created:" + lastMixedThing.id + "." + lastMixedThing.Num + ":bi" + PatchMain.ReturnBottleIngredient(lastMixedThing).ToString());
                       }
                   }
                   else
                   {

                   }
               }
               */


/*
            [HarmonyPostfix]
            [HarmonyPatch(typeof(CraftUtil), "MixIngredients", new Type[] { typeof(string), typeof(List<Thing>), typeof(CraftUtil.MixType), typeof(int), typeof(Chara)})]
            private static void CraftUtilMI_PostPatch(CraftUtil __instance, Thing __result)
            {
                PatchMain.Log(title + "MI_Called", 1);
                lastMixedThing = __result;
            }
            */
//description
//ai.ings ：素材リスト
//ingsの要素Thing tにおいて、TCPBがtrueを返すならresNumをtの個数分加算
//__resultにもTCPBがtrueを返すなら、resNumを__result.Num分減算
//resNum > 0ならクリエイトしてスポーンさせる
/*
if (Func_Craft_Allowed)
{//>>>>>if
    List<Thing> ings = ai.ings;
    string text = "[PBR:craft]";
    string result = "";
    var recycleList = new List<RecycleThing>();
    foreach (Thing t in ings)
    {   
        int prodType = ReturnBottleIngredient(t);
        int prodNum = (prodType != 0) ? t.Num : 0;
        text += "[ings]";
        if (prodType != 0)
        {
            result = DoRecycleBottle(t, ai.owner, ActType.Craft);
            PatchMain.AddThingToList(recycleList, new(result, prodNum));
        }
        text += "N:" + t.id.ToString() + "/T:" + prodType.ToString() + "/n:"  + prodNum.ToString();
    }
    int rt = ReturnBottleIngredient(__result);
    text += "[result]N:" + __result.NameSimple;
    text += "/T:" + rt.ToString();
    text += "/n:" + __result.Num.ToString();
    PatchMain.Log(text, 1);
    //productにbottleingが含まれている場合はリターンを減産する
    PatchMain.RemoveFromList(recycleList, DoRecycleBottle(__result, ai.owner, ActType.Craft), __result.Num);
    PatchMain.ExeRecycle(recycleList, ai.owner);
    //string text2 = "[PBR:return]" + recycleList.ToString() + "[C:" + ai.owner.NameSimple + "]";
    //PatchMain.Log(text2, 1); 
}//<<<<<end if
*/
//var recycleList = new List<RecycleThing>();

//Thing thing = __instance.ings[0];
//Thing thing2 = ((trait.numIng > 1) ? __instance.ings[1] : null);
//List<Thing> ings = __instance.ings;
//if (ings == null)
//{
//    PatchMain.Log(title + "[process]NoIngs");
//    return;
//}
//string text = "ings:";
//text += (thing != null) ? thing.ToString() : "";
//text += "/";
//text += (thing2 != null) ? thing2.ToString() : "";
//foreach (Thing thing in ings)
//{
//    text += "[" + thing.NameSimple + ":" + bi.ToString() + ":" + thing.Num + "]";
//   if (bi != "")
//    {
//        
//        //result = DoRecycleBottle(t, ai.owner, ActType.Craft);
//        PatchMain.AddThingToList(recycleList, new(bi, thing.Num));
//    }
//}

//string text = "ings:";
//string resultBI = "";
//var recycleList = new List<RecycleThing>();

//List<Thing> ings = ai.ings;
/*
if (ings == null || ings.Count <= 0)
{
    PatchMain.Log(title + "NoIngs");
    return;
}
foreach (Thing thing in ings)
{
    if (thing == null) { break; }
    string bi = PatchMain.GetStringID(thing);
    text += "[" + thing.NameSimple + ":" + bi.ToString() + ":" + thing.Num + "]";
    if (bi != "")
    {
        //result = DoRecycleBottle(t, ai.owner, ActType.Craft);
        PatchMain.AddThingToList(recycleList, new(bi, thing.Num));
    }
}
*/
//recycleSet = true;
//PatchMain.Log(text, 1);