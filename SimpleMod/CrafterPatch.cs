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
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace s649PBR
{//>begin NamespaceMain
    namespace CrafterPatchMain
    {//>>begin NamespaceSub
        public class RecycleThing
        {
            private string name { get; set; }
            private int num { get; set; }

            // Change the constructor's access modifier to 'public' to fix CS0122  
            public RecycleThing(string name, int num)
            {
                this.name = name;
                this.num = num;
            }
            public void AddNum(RecycleThing t)
            { this.num += t.num; }
            public bool IsEqualName(RecycleThing t)
            {
                if (t.name == this.name) { return true; }
                return false;
            }
                    
        }
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            private static void AddThing(List<RecycleThing> list, RecycleThing rt)
                {
                bool b = false;
                foreach (RecycleThing t in list)
                {
                    if (t.IsEqualName(rt))
                    {
                        t.AddNum(rt);
                        b = true;
                        break;
                    }
                }
                if(b == false)
                    {
                        list.Add(rt);
                    }
                }



            private static int TypeContainsPotionBottle(Thing t){return PatchMain.TypeContainsPotionBottle(t);}
            private static string DoRecycleBottle(Thing t, Chara c, int at, bool broken = false) { return PatchMain.DoRecycleBottle(t, c, at, broken); }
            private static bool Func_Craft_Allowed => PatchMain.Cf_Allow_Craft;
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

                if(Func_Craft_Allowed)
                {//>>>>>if
                    List<Thing> ings = ai.ings;
                    string text = "[PBR]";
                    Point pos = ai.owner.pos;
                    int resNum = 0;
                    string result = "";// = DoRecycleBottle(t);
                    var recycleList = new List<RecycleThing>();
                    foreach (Thing t in ings)//v0.1.0.1 edit
                    {   
                        text += "[ings:" + t.id.ToString() +"]";

                        //bottleをcreateしてposにadd
                        
                        int prodType = TypeContainsPotionBottle(t);//akibin aru?
                        if(prodType != 0){ result = DoRecycleBottle(t, ai.owner, ActType.Craft); }//akibin tukuru
                        int prodNum = (prodType != 0)? t.Num : 0;//kosuu
                        
                        if(prodType != 0)
                        {
                            //resNum += prodNum;
                            RecycleThing rt = new(result, prodNum);
                            AddThing(recycleList, rt);
                            
                        }
                    }
                    if(TypeContainsPotionBottle(__result) != 0)
                        {
                        //resNum -= __result.Num;
                        recycleList.Remove(DoRecycleBottle(__result, ai.owner, ActType.Craft), __result.Num);
                        }
                    recycleList.ExeRecycle();
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
                        }*/
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