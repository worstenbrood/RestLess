using System;

namespace RestLess
{
    /// <summary>
    /// Class to work with relative urls
    /// </summary>
    public class RelativeUri : Uri
    {
        private readonly static Uri Dummy = new Uri("http://dummy");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeUri"></param>
        public RelativeUri(string relativeUri) : base(Dummy, relativeUri)
        {
        }

        /// <summary>
        /// Return path
        /// </summary>
        public string Path => GetLeftPart(UriPartial.Path);
       
        /// <summary>
        /// Build PathAndQuery
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Query))
            {
                return $"{Path}?{Query}";
            }

            return Path;
        }
    }
}
