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
{//>begin NamespaceMain
    namespace TaskCraftPatchMain
    {//>>begin NamespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TaskCraft), "OnProgressComplete")]
            private static void PostPatch(TaskCraft __instance)
            {//>>>>begin method:OnProgressComplete
                List <Ingredient> ingL = __instance.recipe.ingredients;
                if(PatchMain.configDebugLogging)
                {
                    string text = "[PBR]";
                    text += "[ingL:" + ingL.ToString() +"]";
                    Debug.Log(text);
                } 
            }//<<<<end method:OnProgressComplete
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain
