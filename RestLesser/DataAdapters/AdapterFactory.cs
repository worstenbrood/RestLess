using System;

namespace RestLesser.DataAdapters
{
    /// <summary>
    /// Data adapter factory
    /// </summary>
    public static class AdapterFactory<T>
    {
        private static readonly IDataAdapter PrimitiveAdapter =
#if DEBUG
            new DebugAdapter(new PrimitiveAdapter());
#else
        new PrimitiveAdapter();
#endif
        /// <summary>
        /// Type of T
        /// </summary>
        public static readonly Type Type = typeof(T);

        /// <summary>
        /// Return correcr adapter based on type
        /// </summary>
        /// <param name="supplied"></param>
        /// <returns></returns>
        public static IDataAdapter Get(IDataAdapter supplied) =>
            Type.IsPrimitive || Type == typeof(string) ?
                // Use the primitive adapter
                PrimitiveAdapter :
                // Use the supplied adapter
                supplied;
    }
}
