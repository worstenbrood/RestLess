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
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetMemberName<TDelegate>(this Expression<TDelegate> expression)
            where TDelegate : class, Delegate => expression.Body switch
            {
                MemberExpression m => ((PropertyInfo)m.Member).Name,
                UnaryExpression u => ((PropertyInfo)((MemberExpression)u.Operand).Member).Name,
                _ => throw new Exception("Unhandled expression cast."),
            };
        
        /// <summary>
        /// Join expressions
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="expressions"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinMembers<TDelegate>(this Expression<TDelegate>[] expressions, string separator = Constants.Query.ParameterSeparator)
            where TDelegate : class, Delegate
        {
            return string.Join(separator, expressions.Select(t => t.GetMemberName()));
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
        public static void Sync(this Task t)
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
        public static T Sync<T>(this Task<T> t)
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
        /// <param name="request"></param>
        /// <param name="token"></param>
        public static void SetToken(this HttpRequestMessage request, string token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Set custom header
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetHeader(this HttpRequestMessage request, string name, string value)
        {
            request.Headers.Add(name, value);
        }

        /// <summary>
        /// Set basic authentication header
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public static void SetBasic(this HttpRequestMessage request, string user, string pass)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", 
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}")));
        }

        /// <summary>
        /// Set url query parameters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetUrlAuthentication(this HttpRequestMessage request, string name, string value)
        {
            // Add query parameter
            NameValueCollection httpValueCollection = HttpUtility.ParseQueryString(request.RequestUri.Query);
            httpValueCollection[name] = value;

            // Construct new uri
            UriBuilder ub = new(request.RequestUri) { Query = httpValueCollection.ToString() };
            request.RequestUri = ub.Uri;
        }

        #endregion
    }
}
