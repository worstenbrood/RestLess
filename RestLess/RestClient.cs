using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using RestLess.Authentication;
using RestLess.DataAdapters;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

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

        protected virtual async Task<string> HandleResponse(HttpResponseMessage response)
        {
            string body = await response.Content.ReadAsStringAsync();
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

        protected async Task<TRes> SendAsync<TRes>(string url, HttpMethod method)
        {
            using (var message = new AuthenticationRequestMessage(method, url, Authentication))
            {
                message.Headers.Accept.Add(DataAdapter.MediaTypeHeader);

                using (var result = await Client.SendAsync(message))
                {
                    return DataAdapter.Deserialize<TRes>(await HandleResponse(result));
                }
            }
        }

        protected TRes Send<TRes>(string url, HttpMethod method)
        {
            return SendAsync<TRes>(url, method).SyncResult();
        }

        protected async Task SendAsync<TReq>(string url, HttpMethod method, TReq data)
        {
            using (var message = new AuthenticationRequestMessage(method, url, Authentication))
            {
                using (var content = new RestContent<TReq>(data, DataAdapter))
                {
                    message.Content = content;

                    using (HttpResponseMessage result = await Client.SendAsync(message))
                    {
                        await HandleResponse(result);
                    }
                }
            }
        }

        protected void Send<TReq>(string url, HttpMethod method, TReq data)
        {
            SendAsync(url, method, data).SyncResult();
        }

        protected async Task<TRes> SendAsync<TReq, TRes>(string url, HttpMethod method, TReq data)
        {
            using (var message = new AuthenticationRequestMessage(method, url, Authentication))
            {
                message.Headers.Accept.Add(DataAdapter.MediaTypeHeader);

                using (var content = new RestContent<TReq>(data, DataAdapter))
                {
                    message.Content = content;

                    using (HttpResponseMessage result = await Client.SendAsync(message))
                    {
                        return DataAdapter.Deserialize<TRes>(await HandleResponse(result));
                    }
                }
            }
        }

        protected TRes Send<TReq, TRes>(string url, HttpMethod method, TReq data)
        {
            return SendAsync<TReq, TRes>(url, method, data).SyncResult();
        }

        /// <summary>
        /// Send a GET request, returning the result as an object
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url">Url</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<TRes> GetAsync<TRes>(string url)
        {
            return await SendAsync<TRes>(url, HttpMethod.Get);
        }

        public TRes Get<TRes>(string url)
        {
            return GetAsync<TRes>(url).SyncResult();
        }

        /// <summary>
        /// Send a POST request without returning an object
        /// </summary>
        /// <typeparam name="TReq">Type of object that will be serialized to the body of the request</typeparam>
        /// <param name="url">Url</param>
        /// <param name="data">Body object</param>
        /// <exception cref="HttpRequestException"></exception>
        public async Task PostAsync<TReq>(string url, TReq data)
        {
            await SendAsync(url, HttpMethod.Post, data);
        }

        public void Post<TReq>(string url, TReq data)
        {
            PostAsync(url, data).SyncResult();
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
        public async Task<TRes> PostAsync<TReq, TRes>(string url, TReq data)
        {
            return await SendAsync<TReq, TRes>(url, HttpMethod.Post, data);
        }

        public TRes Post<TReq, TRes>(string url, TReq data)
        {
            return PostAsync<TReq, TRes>(url, data).SyncResult();
        }

        /// <summary>
        /// Post parameters as FormUrlEncodedContent
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<TRes> PostAsync<TRes>(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            using (var message = new AuthenticationRequestMessage(HttpMethod.Post, url, Authentication))
            {
                message.Headers.Accept.Add(DataAdapter.MediaTypeHeader);

                using (var content = new FormUrlEncodedContent(parameters))
                {
                    message.Content = content;

                    using (HttpResponseMessage result = await Client.SendAsync(message))
                    {
                        return DataAdapter.Deserialize<TRes>(await HandleResponse(result));
                    }
                }
            }
        }

        public TRes Post<TRes>(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return PostAsync<TRes>(url, parameters).SyncResult();
        }

        /// <summary>
        /// Post a file as multipart form data without returning an object
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="path">Full path to file</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task PostFileAsync(string url, string path)
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

                            using (HttpResponseMessage result = await Client.SendAsync(message))
                            {
                                await HandleResponse(result);
                            }
                        }
                    }
                }
            }
        }

        public async void PostFile(string url, string path)
        {
            PostFileAsync(url, path).SyncResult();
        }

        /// <summary>
        /// Post a file as multipart form data with returning an object
        /// </summary>
        /// <typeparam name="TRes">Type of the object that will be deserialized from the response</typeparam>
        /// <param name="url">Url</param>
        /// <param name="path">Full path to file</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<TRes> PostFileAsync<TRes>(string url, string path)
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

                            using (HttpResponseMessage result = await Client.SendAsync(message))
                            {
                               return DataAdapter.Deserialize<TRes>(await HandleResponse(result)); 
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
        public async Task<HttpResponseMessage> GetFileAsync(string url, string mediaType)
        {
            using (var message = new AuthenticationRequestMessage(HttpMethod.Get, url, Authentication))
            {
                if (mediaType != null)
                {
                    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                }

                return await Client.SendAsync(message);
            }
        }

        public HttpResponseMessage GetFile(string url, string mediaType)
        {
            return GetFileAsync(url, mediaType).SyncResult();
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="Stream">Stream to copy to</param>
        /// <param name="mediaType"></param>
        public async Task GetFileAsync(string url, Stream output, string mediaType)
        {
            using (var response = GetFile(url, mediaType))
            {
                using (var stream = response.Content)
                {
                    await stream.CopyToAsync(output);
                }
            }
        }

        public void GetFile(string url, Stream output, string mediaType)
        {
            GetFileAsync(url, output, mediaType).SyncResult();
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="path">Path to local file</param>
        /// <param name="mediaType"></param>
        public async Task GetFileAsync(string url, string path, string mediaType)
        {
            using (var stream = File.OpenWrite(path))
            {
                await GetFileAsync(url, stream, mediaType);
            }
        }

        public void GetFile(string url, string path, string mediaType)
        {
            GetFileAsync(url, path, mediaType).SyncResult();
        }

        /// <summary>
        /// Send a PATCH request without returning an object
        /// </summary>
        /// <typeparam name="TReq">Type of object that will be serialized to the body of the request</typeparam>
        /// <param name="url">Url</param>
        /// <param name="data">Body object</param>
        public async Task PatchAsync<TReq>(string url, TReq data)
        {
            await SendAsync(url, new HttpMethod("PATCH"), data);
        }

        public void Patch<TReq>(string url, TReq data)
        {
            PatchAsync(url, data).SyncResult();
        }

        /// <summary>
        /// Send a delete
        /// </summary>
        /// <param name="url">Url</param>
        public async Task DeleteAsync(string url)
        {
            using (var message = new AuthenticationRequestMessage(HttpMethod.Delete, url, Authentication))
            {
                using (HttpResponseMessage result = await Client.SendAsync(message))
                {
                    await HandleResponse(result);
                }
            }
        }

        public void Delete(string url)
        {
            DeleteAsync(url).SyncResult();
        }

        public void Dispose()
        {            
            Client?.Dispose();
            // This will prevent derived class with a finalizer to need to re-implement dispose
            GC.SuppressFinalize(this);
        }
    }  
}
