using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static Recipe;
//using static UnityEditor.MaterialProperty;
//using static UnityEngine.ParticleSystem;
using Debug = UnityEngine.Debug;
//using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace s649PBR
{//>begin NamespaceMain
    namespace CrafterPatchMain
    {//>>begin NamespaceSub
        
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            [HarmonyPostfix]
            [HarmonyPatch(typeof(AI_UseCrafter), "OnStart")]
            private static void AI_UseCrafterOnStartPostPatch(AI_UseCrafter __instance)
            {
                TraitCrafter trait = __instance.crafter;
                bool isFactory = trait is TraitFactory;
                PatchMain.Log("[PBR:craft]AI_UC/OnStart/iF:" + isFactory.ToString() + "/t:" + trait.ToString());
                if (isFactory)
                {
                    //PatchMain.Log("[PBR:craft]Crafter is TraitFactory", 1);
                    Recipe recipe = __instance.recipe;
                    //checkRecipesource
                    if (recipe != null)
                    {
                        var id = recipe.id;
                        if (!EClass.sources.things.map.ContainsKey(id)) { PatchMain.Log("[PBR:Craft]*error* idが無いよ"); return; }
                        var category = EClass.sources.things.map[id].category;
                        var unit = EClass.sources.things.map[id].unit;
                        var resultBI = PatchMain.GetBottleIngredient(id, category, unit);
                        var recycleList = new List<RecycleThing>();
                        int num = __instance.num;//１回の作業で作る数
                        //List<Thing> ings = __instance.ings;
                        List<Ingredient> ingredients = (recipe != null) ? recipe.ingredients : null;
                        
                        string text = "[PBR:craft]";
                        
                        text += "recipe:" + recipe.GetName();
                        text += "/id:" + id;
                        text += "/category:" + category;
                        text += "/unit:" + unit;
                        if (ingredients != null)
                        {
                            text += "/ingre:";
                            foreach (Ingredient ing in ingredients)
                            {
                                if (ing != null) 
                                { 
                                    text += ing.id + "." + ing.req + ":";
                                    //var ingbi = PatchMain.ReturnBottleIngredient(ing.thing);
                                    var bi = PatchMain.GetBottleIngredient(ing.thing);
                                    PatchMain.AddThingToList(recycleList, new(bi, ing.req));
                                }
                            }
                            PatchMain.RemoveFromList(recycleList, resultBI, num);
                        }
                        text += "/num:" + num.ToString();
                        text += "/rBI:" + resultBI.ToString();
                        PatchMain.Log(text, 1);
                    }
                    

                }
                else //notFactory
                {
                
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(AI_UseCrafter), "OnEnd")]
            private static void AI_UseCrafterOnEndPostPatch(AI_UseCrafter __instance)
            {
                PatchMain.Log("[PBR:craft]AI_UseCrafter/OnEnd");
                //List<Thing> ings = __instance.ings;
                TraitCrafter trait = __instance.crafter;
                //string tC = __instance.crafter.ToString();
                if (trait is TraitFactory)
                {   //作業台など
                    
                }
                else 
                {   //加工機械
                    //PatchMain.Log("[PBR:craft]Crafter is not TraitFactory", 1);
                }
                
                //Chara owner = __instance.owner;
                //PatchMain.Log("[PBR:craft]chara:" + owner.NameSimple, 1);
            }



            

            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitCrafter), "Craft")]
            private static void TraitCrafterCraftPostPatch(TraitCrafter __instance, AI_UseCrafter ai, Thing __result)
            {//>>>>begin method:PostPatch
                PatchMain.Log("[PBR:craft]Fook:TraitCrafter/Craft");
                /*
                if (__result == null)
                {
                    PatchMain.Log("[PBR:Craft]NoResult");
                    return;
                }
                //PatchMain.Log("[PBR:craft]Fook:TraitCrafter/Craft", 1);

                if (Func_Craft_Allowed)
                {
                    string text = "[PBR:craft]ings:";
                    //string resultBI = "";
                    var recycleList = new List<RecycleThing>();
                    List<Thing> ings = ai.ings;
                    if (ings == null) 
                    {
                        PatchMain.Log("[PBR:Craft]NoIngs");
                        return;
                    }
                    foreach (Thing thing in ings)
                    {
                        string bi = GetBottleIngredient(thing);
                        text += "[" + thing.NameSimple + ":" + bi.ToString() + ":" + thing.Num + "]";
                        if (bi != "")
                        {
                            //result = DoRecycleBottle(t, ai.owner, ActType.Craft);
                            PatchMain.AddThingToList(recycleList, new(bi, thing.Num));
                        }
                    }
                    PatchMain.Log(text, 1);

                    string resultBI = GetBottleIngredient(__result);
                    //productにbottleingが含まれている場合はリターンを減産する
                    PatchMain.RemoveFromList(recycleList, resultBI, __result.Num);
                    PatchMain.ExeRecycle(recycleList, EClass.pc);
                }*/

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
            }//<<<<end method:PostPatch
        }//<<<end class:PatchExe
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
            string bi = GetBottleIngredient(thing);
            text += "[" + thing.NameSimple + ":" + bi.ToString() + ":" + thing.Num + "]";
            if (bi != "")
            {
                //result = DoRecycleBottle(t, ai.owner, ActType.Craft);
                PatchMain.AddThingToList(recycleList, new(bi, thing.Num));
            }
        }
        PatchMain.Log(text, 1);

        string resultBI = GetBottleIngredient(__result);
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
            string bi = GetBottleIngredient(thing);
            text += "[" + thing.NameSimple + ":" + bi.ToString() + ":" + thing.Num + "]";
            if (bi != "")
            {
                //result = DoRecycleBottle(t, ai.owner, ActType.Craft);
                PatchMain.AddThingToList(recycleList, new(bi, thing.Num));
            }
        }
        PatchMain.Log(text, 1);

        string resultBI = GetBottleIngredient(__result);
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
else { text += "/ing:-"; }*/
//if (source != "") { text += "/sourceid:" + source; } else { text += "/sourceid:-"; }


//text += "/cat:" + cat;

