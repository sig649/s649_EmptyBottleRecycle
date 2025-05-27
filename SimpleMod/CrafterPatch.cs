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
        
        
        public class RecycleQueue 
        {
            string title = "[PBR:RQ]";
            List<RecycleThing> queue;
            int num;

            public RecycleQueue(List<RecycleThing> queue, int num = 1)
            {
                this.queue = queue;
                this.num = num;
            }
            public void ExeRecycle()
            {
                if (num > 0 && queue.Count > 0)
                {
                    PatchMain.Log(title + "ExeRecycle", 1);
                    string text = "[recycle]";
                    foreach (RecycleThing rthing in queue)
                    {
                        Thing t = ThingGen.Create(rthing.name, rthing.GetNum() * num);
                        EClass._zone.AddCard(t, EClass.pc.pos);
                        text += "rt:" + rthing.name + "." + rthing.GetNum().ToString() +"*" + num.ToString() +  "/";
                        text += "N:" + t.id + "." + t.Num.ToString();
                    }
                    PatchMain.Log(text);
                }
            }
            public void RemoveAll()
            {
                queue.Clear();
                this.num = 1;
                PatchMain.Log("[PBR:RQ]RecycleQueue:Cleared", 1);
            }
        }

    [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            static readonly string title = "[PBR:Craft]";
            static RecycleQueue recycleQueue;
            internal static Thing lastMixedThing;

            [HarmonyPostfix]
            [HarmonyPatch(typeof(CraftUtil), "MixIngredients", new Type[] { typeof(string), typeof(List<Thing>), typeof(CraftUtil.MixType), typeof(int), typeof(Chara)})]
            private static void CraftUtilMI_PostPatch(CraftUtil __instance, Thing __result)
            {
                PatchMain.Log(title + "MI_Called", 1);
                lastMixedThing = __result;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(AI_UseCrafter), "OnStart")]
            private static void AI_UseCrafterOnStartPostPatch(AI_UseCrafter __instance)
            {
                lastMixedThing = null;
                TraitCrafter trait = __instance.crafter;
                bool isFactory = trait is TraitFactory;
                PatchMain.Log(title + "AI_UC/OnStart/iF:" + isFactory.ToString() + "/t:" + trait.ToString());
                if (isFactory)
                {
                    //PatchMain.Log("[PBR:craft]Crafter is TraitFactory", 1);
                    Recipe recipe = __instance.recipe;
                    List<Ingredient> ingredients = (recipe != null) ? recipe.ingredients : null;
                    //checkRecipesource
                    if (recipe != null && ingredients != null)
                    {
                        var id = recipe.id;
                        if (!EClass.sources.things.map.ContainsKey(id)) { PatchMain.Log(title + "*error* idが無いよ"); return; }
                        var category = EClass.sources.things.map[id].category;
                        var unit = EClass.sources.things.map[id].unit;
                        var resultBI = PatchMain.GetBottleIngredient(id, category, unit);
                        var recycleList = new List<RecycleThing>();
                        int num = __instance.num;//製作個数？
                        //List<Thing> ings = __instance.ings;
                        
                        
                        string text = "";
                        
                        text += "recipe:" + recipe.GetName();
                        text += "/id:" + id;
                        text += "/category:" + category;
                        text += "/unit:" + unit;
                        text += "/num:" + num.ToString();
                        PatchMain.Log(title + text, 1);
                        text = "/ingre:";
                        foreach (Ingredient ing in ingredients)
                        {
                            if (ing != null)
                            {
                                text += ing.id + "." + ing.req + ":";
                                //var ingbi = PatchMain.ReturnBottleIngredient(ing.thing);
                                var bi = PatchMain.ReturnBottleIngredient(ing.thing);
                                text += "bi:" + bi.ToString();
                                if (bi != BottleIngredient.None)
                                {
                                    PatchMain.AddThingToList(recycleList, new(PatchMain.GetBottleIngredient(ing.thing), ing.req));
                                }
                                text += "/";
                            }
                        }
                        //RecycleThing resultRT = new RecycleThing(resultBI);
                        //PatchMain.RemoveFromList(recycleList, resultRT);

                        //text += "/rBI:" + resultBI.ToString();
                        PatchMain.Log(title + text, 1);

                        string recycleString = "recycle:";
                        if (recycleList.Count > 0)
                        {
                            foreach (RecycleThing rt in recycleList)
                            {
                                if(rt.IsValid())
                                {
                                    recycleString += rt.ToString() + "/";
                                }
                            }
                        }
                        else { recycleString += "-"; }
                        PatchMain.Log(title + recycleString, 1);

                        //queueに追加する
                        if (recycleList.Count > 0)
                        {
                            recycleQueue = new(recycleList);
                            PatchMain.Log(title + "NewRecycleQueue", 1);
                        }
                        else { PatchMain.Log(title + "NoRecycleQueue......", 1); }
                    }
                    

                }
                else //notFactory
                {
                
                }
            }

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
                    /*
                    if (recycleQueue != null)
                    {
                        if(lastMixedThing != null) 
                        {
                            
                        }
                        recycleQueue.ExeRecycle();
                        recycleQueue.RemoveAll();
                        PatchMain.Log(title + "RecycleDone!", 1);
                    } else { PatchMain.Log(title + "QueueNothing", 1); }
                    */
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
else { text += "/ing:-"; }

 [HarmonyPostfix]
            [HarmonyPatch(typeof(Recipe), "Craft")]
            private static void RecipeCraftPostPatch(Thing __result, BlessedState blessed, bool sound, List<Thing> ings, TraitCrafter crafter, bool model)
            {
                PatchMain.Log("[PBR:craft]Fook:Recipe/Craft");
            }
 
 */
//if (source != "") { text += "/sourceid:" + source; } else { text += "/sourceid:-"; }


//text += "/cat:" + cat;

