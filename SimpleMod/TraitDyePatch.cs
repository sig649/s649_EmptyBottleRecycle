using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649PBR.Main;
using static UnityEngine.UIElements.UxmlAttributeDescription;


namespace s649PBR
{//>begin namespaceMain
    namespace TraitDyePatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe  //v0.1.1 new
        {//>>>begin class:PatchExe
            private static bool Func_Use_Allowed => PatchMain.Cf_Allow_Use;
            private static bool Use_PC_Allowed => PatchMain.Cf_Reg_Use_PC;
            private static bool Use_NPC_Allowed => PatchMain.Cf_Reg_Use_NPC;

            private static bool Func_Blend_Allowed => PatchMain.Cf_Allow_Blend;
            private static bool Blend_PC_Allowed => PatchMain.Cf_Reg_Blend_PC;
            private static bool Blend_NPC_Allowed => PatchMain.Cf_Reg_Blend_NPC;
            //internal int TypeContainsPotionBottle(Thing t){return PatchMain.TypeContainsPotionBottle(t);}
            private static Thing DoRecycleBottle(Thing t){return PatchMain.DoRecycleBottle(t);}
            //private static bool Func_Allowed => PatchMain.cf_Allow_F10_TraitDye;
            //private static bool PC_Allowed => PatchMain.cf_F01_PC_CBWD;

            
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDye), "OnUse")]
            private static void OnUsePostPatch(TraitDye __instance, Chara c)
            {//>>>>begin method:OnUsePostPatch
                Thing usedT = __instance.owner.Thing;
                Thing prodT = null;
                if (Func_Use_Allowed)
                {//>5begin if(Func_Allowed)
                    
                    prodT = DoRecycleBottle(usedT);
                    if(prodT != null)
                    {   
                        if(c.IsPC && Use_PC_Allowed)
                        {
                            //t = ThingGen.Create(prod);
                            c.Pick(prodT);
                            PatchMain.Log("[PBR]DyeUse:->" + usedT.GetName(NameStyle.Simple) +"/Prod->" + prodT.GetName(NameStyle.Simple) + " :by " + c.GetName(NameStyle.Simple));
                        }      
                    }
                }//<5end if(Func_Allowed)
            }//<<<<end method:OnUsePostPatch

            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDye), "OnBlend")]
            private static void OnBlendPostPatch(TraitDye __instance, Chara c)
            {//>>>>begin method:OnUsePostPatch
                Thing usedT = __instance.owner.Thing;
                Thing prodT = null;
                if (Func_Blend_Allowed)
                {//>5begin if(Func_Allowed)
                    //Thing usedT = __instance.owner.Thing;
                    prodT = DoRecycleBottle(usedT);
                    if(prodT != null)
                    {   
                        if(c.IsPC && Blend_PC_Allowed)
                        {
                            //t = ThingGen.Create(prod);
                            c.Pick(prodT);
                            PatchMain.Log("[PBR]DyeBlend:->" + usedT.GetName(NameStyle.Simple) +"/Prod->" + prodT.GetName(NameStyle.Simple) + " :by " + c.GetName(NameStyle.Simple));
                        }      
                    }
                }//<5end if(Func_Allowed)
            }//<<<<end method:OnUsePostPatch


        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain




//trash box



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
                    