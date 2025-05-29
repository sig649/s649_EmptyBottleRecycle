using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.BIClass;
using s649PBR.Main;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;


namespace s649PBR
{//>begin namespaceMain
    namespace TraitDyePatch
    {//>>begin namespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDye), "OnUse")]
            private static void OnUsePostPatch(TraitDye __instance, Chara c)
            {//>>>>begin method:OnUsePostPatch
                if (PatchMain.Cf_Allow_Use)
                {
                    string title = "[PBR:TDye.OU]";
                    Thing usedT = __instance.owner.Thing;
                    bool b = PatchMain.TryRecycle(usedT, c, ActType.Use);
                    if (b)
                    {
                        PatchMain.Log(title + "Success", 2);
                    }
                }
            }//<<<<end method:OnUsePostPatch

            [HarmonyPostfix]
            [HarmonyPatch(typeof(TraitDye), "OnBlend")]
            private static void OnBlendPostPatch(TraitDye __instance, Chara c)
            {//>>>>begin method:OnUsePostPatch
                if (PatchMain.Cf_Allow_Blend)
                {
                    string title = "[PBR:TDye.OB]";
                    Thing usedT = __instance.owner.Thing;
                    bool b = PatchMain.TryRecycle(usedT, c, ActType.Blend);
                    if (b)
                    {
                        PatchMain.Log(title + "Success", 2);
                    }
                }
            }//<<<<end method:OnUsePostPatch
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain




//trash box
/*
                if (PatchMain.Cf_Allow_Blend)
                {
                    string title = "[PBR:Dye]";
                    Thing usedT = __instance.owner.Thing;
                    PatchMain.Log(title + "Blend->" + usedT.NameSimple + " :by " + c.NameSimple, 1);
                    bool result = PatchMain.DoRecycleBottle(usedT, c, ActType.Blend);
                    if (result)
                    {
                        PatchMain.Log(title + "Success", 1);
                    }
                }*/
/*
                if (PatchMain.Cf_Allow_Use)
                {
                    string title = "[PBR:Dye]";
                    Thing usedT = __instance.owner.Thing;
                    PatchMain.Log(title + "Used->" + usedT.NameSimple + " :by " + c.NameSimple, 1);
                    bool result = PatchMain.DoRecycleBottle(usedT, c, ActType.Use);
                    if (result)
                    {
                        PatchMain.Log(title + "Success", 1);
                    }
                }*/
//Thing usedT = __instance.owner.Thing;
//PatchMain.Log("[PBR:Dye]Used->" + usedT.NameSimple + " :by " + c.NameSimple, 1);
//DoRecycleBottle(usedT, c, ActType.Use);


//if(PatchMain.configDebugLogging){Debug.Log("[PBR]Drinked->" + usedT.id.ToString());}

////sitaya old
/*
 * 
                Thing usedT = __instance.owner.Thing;
                PatchMain.Log("[PBR:Dye]Used->" + usedT.NameSimple + " :by " + c.NameSimple, 1);
                Thing result = DoRecycleBottle(usedT, c, ActType.Use);
                if (result != null)
                {
                    PatchMain.Log("[PBR:Dye]result->" + result.NameSimple, 1);
                }

 * 

                Thing usedT = __instance.owner.Thing;
                PatchMain.Log("[PBR:Dye]Blend->" + usedT.NameSimple + " :by " + c.NameSimple, 1);
                Thing result = DoRecycleBottle(usedT, c, ActType.Blend);
                if (result != null)
                {
                    PatchMain.Log("[PBR:Dye]result->" + result.NameSimple, 1);
                }
 * 
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


//private static bool Func_Use_Allowed => PatchMain.Cf_Allow_Use;
//private static bool Use_PC_Allowed => PatchMain.Cf_Reg_Use_PC;
//private static bool Use_NPC_Allowed => PatchMain.Cf_Reg_Use_NPC;

//private static bool Func_Blend_Allowed => PatchMain.Cf_Allow_Blend;
//private static bool Blend_PC_Allowed => PatchMain.Cf_Reg_Blend_PC;
//private static bool Blend_NPC_Allowed => PatchMain.Cf_Reg_Blend_NPC;
//internal int TypeContainsPotionBottle(Thing t){return PatchMain.TypeContainsPotionBottle(t);}


//if (Func_Blend_Allowed)
//{//>5begin if(Func_Allowed)
//Thing usedT = __instance.owner.Thing;
//prodT = DoRecycleBottle(usedT);