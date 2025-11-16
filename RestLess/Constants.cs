namespace RestLess
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Filter constants
        /// </summary>
        public static class Query
        {
            public const string Select = "$select";
            public const string Expand = "$expand";
            public const string Filter = "$filter";
            public const string Top = "$top";
            public const string OrderBy = "$orderby";
            public const string Desc = "desc";
            public const string And = " and ";
        }
    }
}
