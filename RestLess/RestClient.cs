using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using RestLess.Authentication;
using RestLess.DataAdapters;
using System.IO;
using System.Collections.Generic;

namespace RestLess
{
    /// <summary>
    /// Rest client
    /// </summary>
    public class RestClient : IDisposable
    {
        /// <summary>
        /// Internal http client
        /// </summary>
        public readonly HttpClient Client;
       
        /// <summary>
        /// Token auth
        /// </summary>
        protected IAuthentication Authentication;

        /// <summary>
        /// Handles serialization and deserialization of content
        /// </summary>
        protected readonly IDataAdapter DataAdapter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler">Message handler</param>
        /// <param name="dataAdapter">Uses JsonAdapter by default</param>
        public RestClient(HttpMessageHandler handler = null, IDataAdapter dataAdapter = null)
        {
            Client = handler == null ? new HttpClient() : new HttpClient(handler);
            DataAdapter = dataAdapter ?? new JsonAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint">Endpoint base url</param>
        /// <param name="handler">Message handler</param>
        /// <param name="dataAdapter">Uses JsonAdapter by default</param>
        public RestClient(string endPoint, HttpMessageHandler handler = null, IDataAdapter dataAdapter = null)
        {
            Client = handler == null ? new HttpClient { BaseAddress = new Uri(endPoint) } :
                new HttpClient(handler) { BaseAddress = new Uri(endPoint) };
            DataAdapter = dataAdapter ?? new JsonAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler">Message handler</param>
        /// <param name="authentication">Token provider</param>
        /// <param name="dataAdapter">Uses JsonAdapter by default</param>
        public RestClient(IAuthentication authentication, HttpMessageHandler handler = null, IDataAdapter dataAdapter = null) : this(handler, dataAdapter)
        {
            Authentication = authentication;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint">Endpoint base url</param>
        /// <param name="authentication">Token provider</param>
        /// <param name="handler">Message handler</param>
        /// <param name="dataAdapter">Uses JsonAdapter by default</param>
        public RestClient(string endPoint, IAuthentication authentication, HttpMessageHandler handler = null, IDataAdapter dataAdapter = null) : this(endPoint, handler, dataAdapter)
        {
            Authentication = authentication;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint">Endpoint base url</param>
        /// <param name="clientCertificate"></param>
        /// <param name="authentication">Token provider</param>
        /// <param name="dataAdapter">Uses JsonAdapter by default</param>
        public RestClient(string endPoint, X509Certificate clientCertificate, IAuthentication authentication = null, IDataAdapter dataAdapter = null)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate, chain, policyErrors) => true,
                ClientCertificates = {clientCertificate},
                ClientCertificateOptions = ClientCertificateOption.Manual
            };
            Client = new HttpClient(handler) {BaseAddress = new Uri(endPoint)};
            Authentication = authentication;
            DataAdapter = dataAdapter ?? new JsonAdapter();
        }

        protected virtual string HandleResponse(HttpResponseMessage response)
        {
            string body = response.Content.ReadAsStringAsync().SyncResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP Status {response.StatusCode}: {body}");
            }         

            return body;
        }
        
        private class RestContent<T> : StringContent
        {
            public RestContent(T content, IDataAdapter adapter) : base(adapter.Serialize(content), Encoding.UTF8)
            {
                Headers.ContentType = adapter.MediaTypeHeader;
            }
        }

        private class AuthenticationRequestMessage : HttpRequestMessage
        {
            public AuthenticationRequestMessage(HttpMethod method, string url, IAuthentication authentication = null) : base(method, url)
            {
                authentication?.SetAuthentication(this);
            }
        }

        protected TRes Send<TRes>(string url, HttpMethod method)
        {
            using (var message = new AuthenticationRequestMessage(method, url, Authentication))
            {
                message.Headers.Accept.Add(DataAdapter.MediaTypeHeader);

                using (var result = Client.SendAsync(message).SyncResult())
                {
                    return DataAdapter.Deserialize<TRes>(HandleResponse(result));
                }
            }
        }

        protected void Send<TReq>(string url, HttpMethod method, TReq data)
        {
            using (var message = new AuthenticationRequestMessage(method, url, Authentication))
            {
                using (var content = new RestContent<TReq>(data, DataAdapter))
                {
                    message.Content = content;

                    using (HttpResponseMessage result = Client.SendAsync(message).SyncResult())
                    {
                        HandleResponse(result);
                    }
                }
            }
        }

        protected TRes Send<TReq, TRes>(string url, HttpMethod method, TReq data)
        {
            using (var message = new AuthenticationRequestMessage(method, url, Authentication))
            {
                message.Headers.Accept.Add(DataAdapter.MediaTypeHeader);

                using (var content = new RestContent<TReq>(data, DataAdapter))
                {
                    message.Content = content;

                    using (HttpResponseMessage result = Client.SendAsync(message).SyncResult())
                    {
                        return DataAdapter.Deserialize<TRes>(HandleResponse(result));
                    }
                }
            }
        }

        /// <summary>
        /// Send a GET request, returning the result as an object
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url">Url</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public TRes Get<TRes>(string url)
        {
            return Send<TRes>(url, HttpMethod.Get);
        }

        /// <summary>
        /// Send a POST request without returning an object
        /// </summary>
        /// <typeparam name="TReq">Type of object that will be serialized to the body of the request</typeparam>
        /// <param name="url">Url</param>
        /// <param name="data">Body object</param>
        /// <exception cref="HttpRequestException"></exception>
        public void Post<TReq>(string url, TReq data)
        {
            Send(url, HttpMethod.Post, data);
        }

        /// <summary>
        /// Send a POST request with returning an object
        /// </summary>
        /// <typeparam name="TReq">Type of object that will be serialized to the body of the request</typeparam>
        /// <typeparam name="TRes">Type of the object that will be deserialized from the response</typeparam>
        /// <param name="url">Url</param>
        /// <param name="data">Body object</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public TRes Post<TReq, TRes>(string url, TReq data)
        {
            return Send<TReq, TRes>(url, HttpMethod.Post, data);
        }

        /// <summary>
        /// Post parameters as FormUrlEncodedContent
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public TRes Post<TRes>(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            using (var message = new AuthenticationRequestMessage(HttpMethod.Post, url, Authentication))
            {
                message.Headers.Accept.Add(DataAdapter.MediaTypeHeader);

                using (var content = new FormUrlEncodedContent(parameters))
                {
                    message.Content = content;

                    using (HttpResponseMessage result = Client.SendAsync(message).SyncResult())
                    {
                        return DataAdapter.Deserialize<TRes>(HandleResponse(result));
                    }
                }
            }
        }

        /// <summary>
        /// Post a file as multipart form data without returning an object
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="path">Full path to file</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public void PostFile(string url, string path)
        {
            using (var message = new AuthenticationRequestMessage(HttpMethod.Post, url, Authentication))
            {
                using (var multipart = new MultipartFormDataContent())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        using (var content = new StreamContent(stream))
                        {
                            multipart.Add(content);
                            message.Content = multipart;

                            using (HttpResponseMessage result = Client.SendAsync(message).SyncResult())
                            {
                                HandleResponse(result);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Post a file as multipart form data with returning an object
        /// </summary>
        /// <typeparam name="TRes">Type of the object that will be deserialized from the response</typeparam>
        /// <param name="url">Url</param>
        /// <param name="path">Full path to file</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public TRes PostFile<TRes>(string url, string path)
        {
            using (var message = new AuthenticationRequestMessage(HttpMethod.Post, url, Authentication))
            {
                message.Headers.Accept.Add(DataAdapter.MediaTypeHeader);               

                using (var multipart = new MultipartFormDataContent())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        using (var content = new StreamContent(stream))
                        {
                            multipart.Add(content);
                            message.Content = multipart;

                            using (HttpResponseMessage result = Client.SendAsync(message).SyncResult())
                            {
                               return DataAdapter.Deserialize<TRes>(HandleResponse(result)); 
                            }
                        }
                    }
                }
            }                   
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="Stream">Stream to copy to</param>
        /// <param name="mediaType"></param>
        public HttpResponseMessage GetFile(string url, string mediaType)
        {
            using (var message = new AuthenticationRequestMessage(HttpMethod.Get, url, Authentication))
            {
                if (mediaType != null)
                {
                    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                }

                return Client.SendAsync(message).SyncResult();
            }
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="Stream">Stream to copy to</param>
        /// <param name="mediaType"></param>
        public void GetFile(string url, Stream output, string mediaType)
        {
            using (var response = GetFile(url, mediaType))
            {
                using (var stream = response.Content)
                {
                    stream.CopyToAsync(output).SyncResult();
                }
            }
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="path">Path to local file</param>
        /// <param name="mediaType"></param>
        public void GetFile(string url, string path, string mediaType)
        {
            using (var stream = File.OpenWrite(path))
            {
                GetFile(url, stream, mediaType);
            }
        }

        /// <summary>
        /// Send a PATCH request without returning an object
        /// </summary>
        /// <typeparam name="TReq">Type of object that will be serialized to the body of the request</typeparam>
        /// <param name="url">Url</param>
        /// <param name="data">Body object</param>
        public void Patch<TReq>(string url, TReq data)
        {
            Send(url, new HttpMethod("PATCH"), data);
        }

        /// <summary>
        /// Send a delete
        /// </summary>
        /// <param name="url">Url</param>
        public void Delete(string url)
        {
            using (var message = new AuthenticationRequestMessage(HttpMethod.Delete, url, Authentication))
            {
                using (HttpResponseMessage result = Client.SendAsync(message).SyncResult())
                {
                    HandleResponse(result);
                }
            }
        }

        public void Dispose()
        {            
            Client?.Dispose();
            // This will prevent derived class with a finalizer to need to re-implement dispose
            GC.SuppressFinalize(this);
        }
    }  

    public static class TaskExtensions
    {
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
    }

    public static class HttpRequestMessageExtensions
    {
        public static void SetToken(this HttpRequestMessage r, string token)
        {
            r.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static void SetHeader(this HttpRequestMessage r, string name, string value)
        {
            r.Headers.Add(name, value);
        }

        public static void SetBasic(this HttpRequestMessage r, string user, string pass)
        {
            r.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}")));
        }
    }
}
