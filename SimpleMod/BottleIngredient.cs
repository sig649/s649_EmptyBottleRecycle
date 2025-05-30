using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using static s649PBR.Main.PatchMain;
//using s649PBR.Main;

namespace s649PBR
{//>begin namespaceMain
    namespace BIClass
    {//>>begin namespaceSub
        //--nakami----------------------
        
        public class ActType
        {//class:ActType
            //public int value;
            public const int None = 0;
            public const int Use = 1;
            public const int Blend = 2;
            public const int Throw = 3;
            public const int Craft = 4;
            public int id { get; private set; }
            public ActType(int arg = 0)
            {
                id = arg;
            }
            public bool IsValid() 
            {
                if (id > 0 && id <= 4)
                { return true; }
                else { return false; }
            }
            public override string ToString()
            {
                string text = "";
                switch (id)
                {
                    case ActType.None:
                        text = "None";
                        break;
                    case ActType.Use:
                        text = "Use";
                        break;
                    case ActType.Blend:
                        text = "Blend";
                        break;
                    case ActType.Throw:
                        text = "Throw";
                        break;
                    case ActType.Craft:
                        text = "Craft";
                        break;
                    default:
                        text = "";
                        break;
                }
                return text;
            }
        }//class:ActType
        public class BottleIngredient
        {//class:BottleIng
            public const int None = 0;
            public const int Bottle_Empty = 1;
            public const int Bucket_Empty = 2;
            public const int Junk_Bottles = -1;
            public const int Junk_Can = -2;
            public const int Can = -3;
            public const int Drug = -4;
            public const int Junk_Glass = -5;
            public const int Junk_Bowl = -6;
            public const int Junk_Cup = -7;
            public const int Other = -999;

            public string id { get; private set; }
            public string resultID { get; private set; }
            //public string orgID { get; private set; }
            // Change the `idIngredient` property to include a private setter to allow assignment within the class.
            public int idIngredient { get; private set; }
            public string orgCategory { get; private set; }
            public string orgUnit { get; private set; }
            public Thing orgThing;
            public int num;
            public bool isBroken;//壊れた？
            public bool isConsumed;//消費された？
            public bool isProhibition;//還元禁止・ing用
            //public bool ConsumeIng;//還元禁止・result用

            public bool isJunk { get; private set; }

            //private bool isJunk { get; }
            //private Thing orgThing { get; }

            public BottleIngredient(Thing thing, int n1 = 1)
            {
                //orgThing = thing;
                //orgID = thing.id;
                //orgTrait = thing.trait;
                orgCategory = thing.sourceCard.category;
                orgUnit = thing.source.unit;
                orgThing = thing;
                isBroken = false;
                isConsumed = false;
                
                int intID = GetIDIngredient(thing);
                idIngredient = intID;
                id = GetStringID(intID);
                resultID = id;
                isJunk = (intID < 0) ? true : false;
                num = n1;
                isProhibition = IsProhibition();
                //isJunk = false;
            }
            /*
            public BottleIngredient(string argID, string argCategory, string argUnit, int n1 = 1)
            {
                //orgThing = thing;
                orgID = argID;
                //orgTrait = thing.trait;
                orgCategory = argCategory;
                orgUnit = argUnit;
                int intID = GetIDIngredient(argID);
                idIngredient = intID;
                id = GetStringID(intID);
                isJunk = (intID < 0) ? true : false;
                num = n1;
                //isJunk = false;
            }*/
            private bool IsProhibition() 
            {
                return false;
                /*
                var trait = orgThing.trait;
                if (trait is TraitAlchemyBench || trait is TraitToolWaterPot)
                {
                    return true;
                }
                else*/
            }
            private int GetIDIngredient(Thing t)
            {
                if (t == null) { return 0; }
                //Trait trait = t.trait;
                //string category = t.sourceCard.category;
                //string unit = t.source.unit;
                return GetIDIngredient(t.id, t.trait);
            }

            private int GetIDIngredient(string tid, Trait trait = null)
            {
                string category = orgCategory;
                string unit = orgUnit;
                //string category = orgThing.sourceCard.category;
                //string unit = orgThing.source.unit;
                if (tid == "") { return 0; }

                if (trait != null)
                {   //trait持ち
                    if (trait is TraitSnow) { return BottleIngredient.None; }
                    if (trait is TraitDye) { return BottleIngredient.Bottle_Empty; }
                    if (trait is TraitPotion || trait is TraitPotionRandom || trait is TraitPotionEmpty)
                    {
                        if (category == "drug") { return BottleIngredient.Drug; } 
                        else { return BottleIngredient.Bottle_Empty; }
                        //return 1;
                    }
                    if (trait is TraitPerfume) { return BottleIngredient.Junk_Bottles; }
                    if (trait is TraitDrinkMilk || trait is TraitDrinkMilkMother)
                    {
                        if (unit == "cup") { return BottleIngredient.Junk_Bottles; }//kofi:718 etc
                        if (unit == "bowl") { return BottleIngredient.Junk_Bowl; }//coconut j:789 etc
                        return BottleIngredient.None;
                    }
                    if (trait is TraitDrink)
                    {
                        if (category == "booze")
                        {
                            if (unit == "jug") { return BottleIngredient.Junk_Glass; }//ビア:58
                            if (unit == "glass") { return BottleIngredient.Junk_Glass; }//ワイン:732 etc
                            return BottleIngredient.Junk_Bottles;
                        }
                        else if (category == "_drink")
                        {
                            if (unit == "bucket") { return BottleIngredient.Bucket_Empty; }
                            if (unit == "pot") { return BottleIngredient.Bottle_Empty; }
                            if (unit == "bottle") { return BottleIngredient.Junk_Bottles; }
                            if (unit == "jar") { return BottleIngredient.Junk_Bottles; }//ビン:59
                            if (unit == "cup") { return BottleIngredient.Junk_Cup; }//お茶:503
                            if (unit == "can") { return BottleIngredient.Junk_Can; }//缶ジュース:504,505
                        }
                        //else if (category == "milk") { }
                    }
                    return BottleIngredient.None;
                }
                else //ThingがCreateされておらず、idから呼び出す時に使う。ThingVには使えない？Foodは対応する
                {   //traitナシ・TraitFactoryから呼び出すときのみ・ThingやFoodなど
                    /*
                    switch (category)
                    {
                        case "_drink"://雪・汚水・水・水バケツ・ポーション・香水・飲料
                            if (unit == "bucket") { return BottleIngredient.Bucket_Empty; }
                            if (unit == "handful") { return BottleIngredient.None; }
                            if (unit == "pot") { return BottleIngredient.Bottle_Empty; }
                            if (unit == "bottle") { return BottleIngredient.Junk_Bottles; }
                            if (tid == "perfume") { return BottleIngredient.Junk_Bottles; }
                            return BottleIngredient.Bottle_Empty;//random potion
                        case "drug"://薬
                            return BottleIngredient.Drug;
                        case "milk"://乳・ミルク缶・
                            return BottleIngredient.None;
                        default:
                            return BottleIngredient.None;
                    }*/
                    return 0;
                }
            }
            //private static bool IsJunk
            //{
            //    return idIngredient < 0;
            //}
            
            private string GetStringID(int bi)
            {
                string resultid = "";

                switch (bi)
                {
                    case BottleIngredient.Bottle_Empty:
                        resultid = "potion_empty";
                        break;
                    case BottleIngredient.Bucket_Empty:
                        resultid = "bucket_empty";
                        break;
                    case BottleIngredient.None://nothing
                        break;
                    case BottleIngredient.Junk_Bottles:
                        resultid = GetRandomJunkBottle();//bottle
                        break;
                    case BottleIngredient.Junk_Can:
                        resultid = GetRandomJunkCan();//can
                        break;
                    case BottleIngredient.Can:
                        resultid = "";//can not junk
                        break;
                    case BottleIngredient.Drug:
                        resultid = "";//(!broken) ? "231" : "";//drug bin
                        break;
                    case BottleIngredient.Junk_Glass:
                        resultid = GetRandomJunkBottle();//(!broken) ? "231" : "";//drug bin
                        break;
                    case BottleIngredient.Junk_Bowl:
                        resultid = "176";//(!broken) ? "231" : "";
                        break;
                    case BottleIngredient.Junk_Cup:
                        resultid = "202";//(!broken) ? "231" : "";
                        break;
                    default:
                        resultid = "";
                        break;
                }
                return resultid;
            }
            public string GetChangedID() 
            {
                return resultID;
            }
            //public bool IsEnableRecycle()
            //{
            //    return IsValid();
            //}
            public bool TryBrake() 
            {
                if (!IsValid()) { return false; }
                switch (idIngredient)
                {
                    case BottleIngredient.Bottle_Empty:
                        isBroken = true;
                        resultID = "glass";
                        return true;
                    case BottleIngredient.Bucket_Empty:
                        break;
                    case BottleIngredient.None:
                        break;
                    case BottleIngredient.Junk_Bottles:
                        isBroken = true;
                        resultID = "fragment";
                        return true;
                    case BottleIngredient.Junk_Can:
                        break;
                    case BottleIngredient.Can:
                        break;
                    case BottleIngredient.Drug:
                        isBroken = true;
                        isConsumed = true;
                        resultID = "";
                        return true;
                    case BottleIngredient.Junk_Glass:
                        isBroken = true;
                        resultID = "glass";
                        return true;
                    default:
                        break;
                }
                return false;
            }
            public bool TryConsume() 
            {
                if (idIngredient == BottleIngredient.Drug)
                {
                    isConsumed = true;
                    return true;
                }
                return false;
            }
            public bool IsEnableRecycle() 
            {
                //string title = "[PBR:BI.IER]";
                //bool b1 = IsValid();
                //bool b2 = num >= 1;
                //PatchMain.Log(title + GetStr(b1) + "/" + GetStr(b2), 2);
                return IsValid() && num >= 1;
            }
            public bool IsValid() 
            {
                return (id != "" && idIngredient != 0) ? true : false;
            }
            public bool IsEqualID(BottleIngredient bi)
            {
                if (!bi.IsValid()) { return false; }
                return this.IsValid() && IsEqualID(bi.idIngredient);
            }
            public bool IsEqualID(int biID)
            {
                return (biID == this.idIngredient) ? true : false;
            }
            public void AddNum(BottleIngredient bi)
            {
                if (!bi.IsValid()) { return; }
                AddNum(bi.num);
                //this.num += t.num; 
            }
            private void AddNum(int n1)
            {
                this.num += n1;
            }
            public void SetMultiNum(int n1)
            {
                this.num *= n1;
            }
            public void Decrease(BottleIngredient bi)
            {
                if (this.IsValid() && bi.IsValid() && IsEqualID(bi))
                {
                    AddNum(-bi.num);
                    //if (this.num <= 0) { isValid = false; }
                }
            }
            //private static List<string> JunkBottleList = new List<string> { "726", "727", "728" };
            //private static List<string> JunkCanList = new List<string> { "236", "529", "1170" };
            public string GetRandomJunkBottle()
            {
                //List<string> sList = JunkBottleList;
                List<string> JunkBottleList = new List<string> { "726", "727", "728" };
                return JunkBottleList[Random.Range(0, JunkBottleList.Count)];
            }
            public string GetRandomJunkCan()
            {
                //List<string> sList = JunkCanList;
                List<string> JunkCanList = new List<string> { "236", "529", "1170" };
                return JunkCanList[Random.Range(0, JunkCanList.Count)];
            }
            
        }//class:BottleIngredient
        /*
        public class RecycleThing
        {
            private static readonly string title = "[RQ]";
            //public string name { get; }
            private BottleIngredient bottleIngredient;
            private int num { get; set; }
            private bool isValid { get; set; }

            public RecycleThing(string name, int num)
            {
                this.name = name;
                this.num = num;
                isValid = true;
            }
            public RecycleThing(string name)
            {
                this.name = name;
                this.num = 1;
                isValid = true;
            }
            public int GetNum()
            {
                return num;
            }

            public void SetMulti(int n1)
            {
                if (IsValid()) { this.num *= n1; }
            }
            

            
            public bool IsNotValid()
            {
                return !IsValid();
            }
            public bool IsValid()
            {
                //PatchMain.Log(title + "Str:" + ToString());
                return (name != "") && (num > 0) && isValid;
            }
            public override string ToString()
            {
                //if (IsNotValid()) { return ""; }
                string text = name;
                text += "." + this.num.ToString();
                return text;
            }
        }

        public class RecycleQueue
        {
            private readonly string title = "[PBR:RQ]";
            //private List<RecycleThing> queue;
            //private Dictionary<BottleIngredient, int> queue;
            private BottleIngredient bottleIngredient;
            private int num;

            //private int num;
            public RecycleQueue(BottleIngredient bi, int num) 
            {
                bottleIngredient = bi;
                this.num = num;
            }

            //public RecycleQueue(List<RecycleThing> queue, int num = 1)
            //{
            //    this.queue = queue;
            //    this.num = num;
            //}
            public override string ToString()
            {
                var text = "";
                if (queue.Count > 0)
                {
                    foreach (RecycleThing rt in queue)
                    {
                        text += rt.ToString() + "/";
                    }
                    text += num.ToString();
                }
                return text;
            }
            public bool IsValid(List<RecycleThing> listRT)
            {
                foreach (RecycleThing rt in listRT)
                {
                    if (rt.IsValid())
                    {
                        return true;
                    }
                }
                return false;
            }
            public void ExeRecycle()
            {
                PatchMain.Log(title + "ExeRecycle:" + PatchMain.GetStrings(queue), 1);
                if (num > 0 && queue.Count > 0)
                {
                    //string text = "[recycle]";
                    if (!IsValid(queue))
                    {
                        PatchMain.Log(title + "queue is Not Valid......", 1);
                        return;
                    }
                    else { PatchMain.Log(title + "queue is Valid", 1); }

                    foreach (RecycleThing rthing in queue)
                    {
                        PatchMain.Log(title + "rthing:" + rthing.ToString() + "/V:" + rthing.IsValid().ToString(), 1);
                        if (rthing.IsValid())
                        {
                            PatchMain.Log(title + "rthingisValid", 1);
                            Thing t = ThingGen.Create(rthing.name).SetNum(rthing.GetNum() * num);
                            PatchMain.Log(title + "CreateThing:" + t.id + "." + t.Num.ToString(), 1);
                            EClass._zone.AddCard(t, EClass.pc.pos);
                            //text += "rt:" + rthing.name + "." + rthing.GetNum().ToString() + "*" + num.ToString() + "/";
                            //text += "Thing:" + t.id + "." + t.Num.ToString();
                        }
                        else { PatchMain.Log(title + "rthingisNotValid", 1); }
                    }


                }
                else { PatchMain.Log(title + "ExeRecycle:Skip", 1); }
                PatchMain.Log(title + "ExeRecycle:Done", 1);
            }
            public void SetQueueNum(int n1)
            {
                this.num = n1;
            }
            public void RemoveBI(RecycleThing rt, int num = 1)
            {
                PatchMain.Log(title + "RemoveBI", 1);
                if (num > 0) { rt.SetMulti(num); }
                if (queue.Count > 0)
                {
                    foreach (RecycleThing queueRT in queue)
                    {
                        if (queueRT.IsEqualID(rt))
                        {
                            queueRT.Decrease(rt);
                            return;
                        }
                    }
                }
                PatchMain.Log(title + "RemoveDone:" + this.ToString(), 1);
            }
            public void RemoveAll()
            {
                queue.Clear();
                this.num = 1;
                PatchMain.Log("[PBR:RQ]RecycleQueue:Cleared", 1);
            }
        }*/
    }//<<end namespaceSub
}//>end namespaceMain









/*
            private static bool GetRegulation(Chara c, int at)
            {
                int wcc = 0;
                int tcp = TypeCharaPlaying(c);
                //bool allow_Func;
                bool reg_chara;
                bool reg_junk = PatchMain.Cf_Reg_JunkBottle && (IsJunk) ? (tcp <= PatchMain.CE_WhichCharaCreatesJunkBottles.Value) : true;
                switch (at)
                {
                    case ActType.None: return false;
                    case ActType.Use:
                        wcc = CE_WhichCharaCreatesWhenUse.Value;
                        //allow_Func = Cf_Allow_Use;
                        break;
                    case ActType.Blend:
                        wcc = CE_WhichCharaCreatesWhenBlend.Value;
                        //allow_Func = Cf_Allow_Blend;
                        break;
                    case ActType.Throw:
                        wcc = CE_WhichCharaCreatesWhenThrow.Value;
                        //allow_Func = Cf_Allow_Throw;
                        break;
                    case ActType.Craft: return Cf_Allow_Craft;
                    default: return false;
                }
                reg_chara = TypeCharaPlaying(c) <= wcc;

                bool result = (!isJunk) ? reg_chara : (reg_chara && reg_junk);
                string text = "[PBR:GR]";
                text += "TCP:" + tcp.ToString() + "/";
                //text += "Func:" + ToTF(allow_Func) + "/";
                text += "RC:" + ToTF(reg_chara) + "/";
                text += (isJunk) ? ("RJ:" + ToTF(reg_junk) + "/") : "RJ:-/";
                text += "rs:" + ToTF(result) + "/";
                //Log(text, 1);
                return result;
            }
            */