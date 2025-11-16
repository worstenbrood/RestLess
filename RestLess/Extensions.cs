using System;
using System.Web;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using System.Threading.Tasks;

<<<<<<< TODO: Unmerged change from project 'RestLesser (netstandard2.0)', Before:
=======
using RestLess;
using RestLesser;
>>>>>>> After

namespace RestLesser
{
    /// <summary>
    /// Class containing extension methods
    /// </summary>
    public static class Extensions
    {
        #region Expression

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

        #endregion
        #region Enum

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

        #endregion
        #region Task

        /// <summary>
        /// Get sync result from async method
        /// </summary>
        /// <param name="t"></param>
        /// <exception cref="TimeoutException"></exception>
        public static void SyncResult(this Task t)
        {
            try
            {
                t.ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (TaskCanceledException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                if (ex.CancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException("A timeout occurred.", ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Get sync result from async method
        /// </summary>
        /// <param name="t"></param>
        /// <exception cref="TimeoutException"></exception>
        public static T SyncResult<T>(this Task<T> t)
        {
            try
            {
                return t.ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (TaskCanceledException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                if (ex.CancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException("A timeout occurred.", ex);
                }

                throw;
            }
        }

        #endregion
        #region HttpRequestMessage

        /// <summary>
        /// Set token header
        /// </summary>
        /// <param name="r"></param>
        /// <param name="token"></param>
        public static void SetToken(this HttpRequestMessage r, string token)
        {
            r.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Set custom header
        /// </summary>
        /// <param name="r"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetHeader(this HttpRequestMessage r, string name, string value)
        {
            r.Headers.Add(name, value);
        }

        /// <summary>
        /// Set basic authentication header
        /// </summary>
        /// <param name="r"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public static void SetBasic(this HttpRequestMessage r, string user, string pass)
        {
            r.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}")));
        }

        /// <summary>
        /// Set url query parameters
        /// </summary>
        /// <param name="r"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetUrlAuthentication(this HttpRequestMessage r, string name, string value)
        {
            NameValueCollection httpValueCollection = HttpUtility.ParseQueryString(r.RequestUri.Query);
            httpValueCollection.Add(name, value);

            UriBuilder ub = new UriBuilder(r.RequestUri)
            {
                Query = httpValueCollection.ToString()
            };

            r.RequestUri = ub.Uri;
        }

        #endregion
    }
}
