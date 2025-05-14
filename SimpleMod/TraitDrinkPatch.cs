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
            private static string DoRecycleBottle(Thing t, Chara c, int at, bool broken = false){return PatchMain.DoRecycleBottle(t, c, at, broken);}
            //private static bool Func_Use_Allowed => PatchMain.Cf_Allow_Use;
            //private static bool Use_PC_Allowed => PatchMain.Cf_Reg_Use_PC;
            //private static bool Use_NPC_Allowed => PatchMain.Cf_Reg_Use_NPC;

            //private static bool Allow_CreateJunkBottle => PatchMain.Cf_Reg_JunkBottle;
            //private static bool CJB_PC_Allowed => PatchMain.Cf_Reg_CJB_PC;
            //private static bool CJB_NPC_Allowed => PatchMain.Cf_Reg_CJB_NPC;

            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDrink), "OnDrink")]
            private static void TraitDrinkPostPatch(TraitDrink __instance, Chara c)
            {//>>>>begin method:TraitDrinkPatch
                //if(Func_Use_Allowed)
                //{//>5begin if(Func_Use_Allowed)
                Thing usedT = __instance.owner.Thing;
                string prodT = DoRecycleBottle(usedT, c, ActType.Use);
                Thing result;
                //int prodN = TypeContainsPotionBottle(usedT);
                if (prodT != "") 
                {
                    result = ThingGen.Create(prodT);
                    if (c.IsPC) { c.Pick(result); } else { EClass._zone.AddCard(result, c.pos); }
                    PatchMain.Log("[PBR:Drink]Used->" + usedT.NameSimple + "/Prod->" + prodT + " :by " + c.NameSimple);
                }
                //if (c.IsPC && Use_PC_Allowed)
                //    {
                 //       prodT = DoRecycleBottle(usedT);
                //        c.Pick(prodT);
                //        PatchMain.Log("[PBR]Used->" + usedT.NameSimple + "/Prod->" + prodT.NameSimple + " :by " + c.NameSimple);
                //    }
                //    else if(!c.IsPC && Use_NPC_Allowed)
                //    {
                //        prodT = DoRecycleBottle(usedT);
                //        EClass._zone.AddCard(prodT, c.pos);
                //        PatchMain.Log("[PBR]Used->" + usedT.NameSimple + "/Prod->" + prodT.NameSimple + " :by " + c.NameSimple);
                //    } 
                //}//<5end if(Func_Use_Allowed)
            }//<<<<end method:TraitDrinkPatch
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain


//trash


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