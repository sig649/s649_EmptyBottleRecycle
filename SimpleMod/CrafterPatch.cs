using System;
using System.IO;
using System.Diagnostics;
using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;

using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649PBR.Main;

namespace s649PBR
{//>begin NamespaceMain
    namespace CrafterPatchMain
    {//>>begin NamespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            private static int TypeContainsPotionBottle(Thing t){return PatchMain.TypeContainsPotionBottle(t);}
            private static Thing DoRecycleBottle(Thing t){return PatchMain.DoRecycleBottle(t);}
            private static bool Func_Allowed => PatchMain.cf_Allow_F03_Craft;
            //private static bool PC_Allowed => PatchMain.cf_F01_PC_CBWD;

            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitCrafter), "Craft")]
            private static void PostPatch(TraitCrafter __instance, 	AI_UseCrafter ai, Thing __result)
            {//>>>>begin method:PostPatch
                //description
                    //ai.ings ：素材リスト
                    //ingsの要素Thing tにおいて、TCPBがtrueを返すならresNumをtの個数分加算
                    //__resultにもTCPBがtrueを返すなら、resNumを__result.Num分減算
                    //resNum > 0ならクリエイトしてスポーンさせる

                if(Func_Allowed)
                {//>>>>>if
                    List<Thing> ings = ai.ings;
                    string text = "[PBR]";
                    Point pos = ai.owner.pos;
                    int resNum = 0;
                    Thing resT = null;// = DoRecycleBottle(t);
                    foreach(Thing t in ings)//v0.1.0.1 edit
                    {   
                        text += "[ings:" + t.id.ToString() +"]";
                        
                        //bottleをcreateしてposにadd
                        
                        int prodType = TypeContainsPotionBottle(t);//akibin aru?
                        if(prodType == 1){resT = DoRecycleBottle(t);}//akibin tukuru
                        int prodNum = (prodType == 1)? t.Num : 0;//kosuu
                        
                        //var recycleList = new List<string>();
                        
                        if(prodType == 1 && prodNum > 0)
                        {
                            resNum += prodNum;
                            //recycleList.Add(prodT);
                            //EClass._zone.AddCard(prodT, pos);
                        }
                    }
                    if(TypeContainsPotionBottle(__result) == 1)
                        {
                            resNum -= __result.Num;
                        }
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
                    PatchMain.Log(text); 
                }//<<<<<end if
            }//<<<<end method:PostPatch
            /*
            private bool CanRecycleBottle(Thing t)
            {//>>>>begin method:DoRecycleBottle
                if(PatchMain.TypeContainsPotionBottle(t) == 1)
                {
                    return true;
                } else {
                    return false;
                }
            }//<<<<end method:DoRecycleBottle
            */
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain

///trashbox//////////////////////////////////////////////////////////////////
/// 
/*
/////////////////////////////////////////////////

////////////////////////////////////////////////
*/