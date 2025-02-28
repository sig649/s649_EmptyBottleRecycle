using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649PBR.Main;


namespace s649PBR
{//>begin namespaceMain
    namespace TraitDrinkPatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            //internal int TypeContainsPotionBottle(Thing t){return PatchMain.TypeContainsPotionBottle(t);}
            private static Thing DoRecycleBottle(Thing t){return PatchMain.DoRecycleBottle(t);}
            private static bool Func_Allowed => PatchMain.cf_Allow_F01_Use;
            private static bool PC_Allowed => PatchMain.cf_F01_PC_CBWD;

            
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDrink), "OnDrink")]
            private static void TraitDrinkPostPatch(TraitDrink __instance, Chara c)
            {//>>>>begin method:TraitDrinkPatch
                if(Func_Allowed)
                {//>5begin if(Func_Allowed)
                    Thing usedT = __instance.owner.Thing;
                    
                    //int prodN = TypeContainsPotionBottle(usedT);
                    Thing prodT = DoRecycleBottle(usedT);
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
                    if(prodT != null)
                    {   
                        if(c.IsPC && PC_Allowed)
                        {
                            //t = ThingGen.Create(prod);
                            c.Pick(prodT);
                            if(PatchMain.configDebugLogging){Debug.Log("[PBR]Used->" + usedT.ToString() +"/Prod->" + prodT.ToString() + " :by " + c.GetName(NameStyle.Simple));}
                        } 
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
                        
                    }
                    
                    //if(PatchMain.configDebugLogging){Debug.Log("[PBR]Drinked->" + usedT.id.ToString());}
                    
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
                    
                    
                }//<5end if(Func_Allowed)
            }//<<<<end method:TraitDrinkPatch
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain