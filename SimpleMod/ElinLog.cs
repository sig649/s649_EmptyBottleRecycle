

namespace s649ElinLog
{//begin namespaceMain
    public class ElinLog 
    {
        public static string ConvertToString(object input)
        {
            switch (input) 
            {
                case bool:
                    return ToTF((bool)input);
                case int:
                    return ((int)input).ToString();
                case string:
                    return (string)input;
                case Card:
                    return ((Card)input).NameSimple;
                case Recipe:
                    return ((Recipe)input).GetName();
                default:
                    return input?.ToString() ?? "";
            }
        }

        private static string ToTF(bool b) { return b ? "T" : "F"; }
        /*
        public static string GetStr(bool b)
        {
            return ToTF(b);
        }
        public static string GetStr(int arg)
        {
            return (arg != 0) ? arg.ToString() : "0";
        }
        public static string GetStr(string s)
        {
            return s;
        }
        public static string GetStr(Point arg)
        {
            return (arg != null) ? arg.ToString() : "-";
        }
        public static string GetStr(Trait arg)
        {
            return (arg != null) ? arg.ToString() : "-";
        }
        public static string GetStr(Card arg)
        {
            return (arg != null) ? arg.NameSimple : "-";
        }*/
    }
}//end namespaceMain