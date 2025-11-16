using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RestLess
{
     public static class Extensions
    {
        /// <summary>
        /// Get member name of expression <paramref name="p"/>
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetMemberName<TDelegate>(this Expression<TDelegate> p)
            where TDelegate : class, Delegate
        {
            switch (p.Body)
            {
                case MemberExpression expression:
                    return ((PropertyInfo)expression.Member).Name;

                case UnaryExpression expression:
                    return ((PropertyInfo)((MemberExpression)expression.Operand).Member).Name;

                default:
                    throw new Exception("Unhandled expression cast.");
            }
        }

        /// <summary>
        /// Join expressions
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="p"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinMembers<TDelegate>(this Expression<TDelegate>[] p, string separator = ",")
            where TDelegate : class, Delegate
        {
            return string.Join(separator, p.Select(t => t.GetMemberName()));
        }

        /// <summary>
        /// Convert an enmu value to its string value and lower case
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string Lower<T>(this T e)
            where T : struct, Enum
        {
            return e.ToString("f").ToLower();
        }
    }
}
