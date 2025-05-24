using System;
using System.IO;
using System.Diagnostics;
using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;

//using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649PBR.Main;


namespace s649PBR
{//>begin namespaceMain
    namespace MixIngPatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            
            private static int ReturnBottleIngredient(Thing t) { return PatchMain.ReturnBottleIngredient(t); }
            private static string DoRecycleBottle(Thing t, Card c, int at, bool broken = false) { return PatchMain.DoRecycleBottle(t, c, at, broken); }
            private static bool Func_Craft_Allowed => PatchMain.Cf_Allow_Craft;

            //[HarmonyPostfix]
            //[HarmonyPatch(typeof(TraitWell), "OnBlend")]

            /*
            [HarmonyPostfix]
            [HarmonyPatch(typeof(CraftUtil), "MixIngredients", new Type[] { typeof(Card), typeof(List<Thing>), typeof(CraftUtil.MixType), typeof(int), typeof(Chara) })]
            private static void MixIngPostPatch(Card product, List<Thing> ings, CraftUtil.MixType type, int maxQuality, Chara crafter)
            {//begin method:TraitDrinkPatch
                if (Func_Craft_Allowed)
                {//>>>>>if
                    string text = "[PBR:MixIng]";
                    string result = "";
                    var recycleList = new List<RecycleThing>();
                    foreach (Thing t in ings)
                    {
                        text += "[ings]";
                        int prodType = ReturnBottleIngredient(t);
                        int prodNum = (prodType != 0) ? t.Num : 0;

                        if (prodType != 0)
                        {
                            result = DoRecycleBottle(t, crafter, ActType.Craft);
                            PatchMain.AddThingToList(recycleList, new(result, prodNum));
                        }
                        text += "N:" + t.id.ToString() + "/T:" + prodType.ToString() + "/n:" + prodNum.ToString();
                    }
                    int rt = ReturnBottleIngredient((Thing)product);
                    text += "[result]N:" + ((Thing)product).NameSimple;
                    text += "/T:" + rt.ToString();
                    text += "/n:" + ((Thing)product).Num.ToString();
                    PatchMain.Log(text, 1);
                    PatchMain.RemoveFromList(recycleList, DoRecycleBottle((Thing)product, crafter, ActType.Craft), ((Thing)product).Num);
                    PatchMain.ExeRecycle(recycleList, crafter);

                    //PatchMain.Log(text);
            
                }//<<<<end method:TraitDrinkPatch
            }*/
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain