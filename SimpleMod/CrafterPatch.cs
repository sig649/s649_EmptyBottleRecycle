using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static UnityEditor.MaterialProperty;
using Debug = UnityEngine.Debug;
//using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace s649PBR
{//>begin NamespaceMain
    namespace CrafterPatchMain
    {//>>begin NamespaceSub
        
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            
            private static int ReturnBottleIngredient(Thing t){return PatchMain.ReturnBottleIngredient(t);}
            private static string GetBottleIngredient(Thing t) { return PatchMain.GetBottleIngredient(t); }
            private static void DoRecycleBottle(Thing t, Chara c, int at, bool broken = false) { PatchMain.DoRecycleBottle(t, c, at, broken); }
            private static bool Func_Craft_Allowed => PatchMain.Cf_Allow_Craft;
            
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Recipe), "Craft")]
            private static void RecipeCraftPostPatch(Thing __result, BlessedState blessed, bool sound, List<Thing> ings, TraitCrafter crafter, bool model)
            {
                PatchMain.Log("[PBR:craft]Fook:Recipe/Craft/product:" + __result.NameSimple + "/Num:" + __result.Num.ToString(), 1);
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
                //PatchMain.Log("[PBR:craft]Fook:RecipeCard/Craft", 1);
                PatchMain.Log("[PBR:craft]Fook:RecipeCard/Craft/product:" + __result.NameSimple + "/Num:" + __result.Num.ToString(), 1);
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
            [HarmonyPatch(typeof(TraitCrafter), "Craft")]
            private static void TraitCrafterCraftPostPatch(TraitCrafter __instance, AI_UseCrafter ai, Thing __result)
            {//>>>>begin method:PostPatch
                PatchMain.Log("[PBR:craft]Fook:TraitCrafter/Craft", 1);
                if (Func_Craft_Allowed)
                {
                    string text = "[PBR:craft]ings:";
                    //string resultBI = "";
                    var recycleList = new List<RecycleThing>();
                    List<Thing> ings = ai.ings;
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
/////////////////////////////////////////////////

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