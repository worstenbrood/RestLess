using System;
using System.Linq;
using System.Reflection;

namespace RestLesser.OData.Attributes
{
    /// <summary>
    /// Fields cache 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public static class Fields<TObject>
    {
        /// <summary>
        /// <see cref="Type"/> of TObject
        /// </summary>
        public static readonly Type Type = typeof(TObject);

        private static readonly MemberInfo[] _fields = Type.GetMembers(BindingFlags.Public);

        private static readonly MemberInfo _primaryKey = _fields
            .Where(f => Attribute.IsDefined(f, typeof(PrimaryKeyAttribute)))
            .FirstOrDefault();

        /// <summary>
        /// MEmber with primary key attribute
        /// </summary>
        public static MemberInfo PrimaryKey => _primaryKey;

    }
}
