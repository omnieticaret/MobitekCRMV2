namespace MobitekCRMV2.Entity.Enums
{
    public static class CountryCodeDictionary
    {
        public static Dictionary<string, string> List { get; set; } = new Dictionary<string, string> {
                        {"tr", "Türkiye" },
                        {"us", "Amerika" },
                        {"uk", "İngiltere" },
                        {"de", "Almanya" },
                        {"fr", "Fransa" },
                        {"es", "İspanya" },
                    };
    }
}
