namespace FG.Diagnostics.AutoLogger.Model
{
    public static class DefaultExtensionMethods
    {
        public static string AsJson(this object that)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(that);
        }
    }
}