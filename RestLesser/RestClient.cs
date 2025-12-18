using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using RestLesser.Http;
using RestLesser.Authentication;
using RestLesser.DataAdapters;

namespace RestLesser
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
#if DEBUG
            DataAdapter = new DebugAdapter(dataAdapter ?? new JsonAdapter());
#else
            DataAdapter = dataAdapter ?? new JsonAdapter();
#endif
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
#if DEBUG
            DataAdapter = new DebugAdapter(dataAdapter ?? new JsonAdapter());
#else
            DataAdapter = dataAdapter ?? new JsonAdapter();
#endif
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
#if DEBUG
            DataAdapter = new DebugAdapter(dataAdapter ?? new JsonAdapter());
#else
            DataAdapter = dataAdapter ?? new JsonAdapter();
#endif
        }

        /// <summary>
        /// Http response handler
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        protected virtual async Task<string> HandleResponse(HttpResponseMessage response)
        {
            string body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP Status {response.StatusCode}: {body}");
            }         

            return body;
        }

        /// <summary>
        /// Create an authenticated http request message
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected AuthenticationRequestMessage CreateRequest(string url, HttpMethod method) =>
            new (method, url, Authentication);

        /// <summary>
        /// Get data adaptor for type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IDataAdapter GetAdapter<T>() =>
            AdapterFactory<T>.Get(DataAdapter);

        /// <summary>
        /// Create request content
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        protected RestContent<TReq> CreateContent<TReq>(TReq data)
        {
            var adapter = GetAdapter<TReq>();
            return new RestContent<TReq>(data, adapter);
        }

        /// <summary>
        /// Get result from http message
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task<TRes> GetResult<TRes>(AuthenticationRequestMessage message)
        {
            var adapter = GetAdapter<TRes>();
            message.Headers.Accept.Add(adapter.MediaTypeHeader);
            using var result = await Client.SendAsync(message);
            return adapter.Deserialize<TRes>(await HandleResponse(result));
        }

        /// <summary>
        /// Get result from http message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task GetResult(AuthenticationRequestMessage message)
        {
            using var result = await Client.SendAsync(message);
            await HandleResponse(result);
        }

        /// <summary>
        /// Send async
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected async Task SendAsync(string url, HttpMethod method)
        {
            using var message = CreateRequest(url, method);
            message.Headers.Accept.Add(DataAdapter.MediaTypeHeader);

            using var result = await Client.SendAsync(message);
            await HandleResponse(result);
        }

        /// <summary>
        /// Send async
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected void Send(string url, HttpMethod method)
        {
            SendAsync(url, method).Sync();
        }      

        /// <summary>
        /// Send async
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected async Task<TRes> SendAsync<TRes>(string url, HttpMethod method)
        {
            using var message = CreateRequest(url, method);
            return await GetResult<TRes>(message);
        }

        /// <summary>
        /// Send sync
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected TRes Send<TRes>(string url, HttpMethod method)
        {
            return SendAsync<TRes>(url, method).Sync();
        }

        /// <summary>
        /// Send async
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task SendAsync<TReq>(string url, HttpMethod method, TReq data)
        {
            using var content = CreateContent<TReq>(data);
            using var message = CreateRequest(url, method);
            message.Content = content;
            await GetResult(message);
        }

        /// <summary>
        /// Send sync
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        protected void Send<TReq>(string url, HttpMethod method, TReq data)
        {
            SendAsync(url, method, data).Sync();
        }

        /// <summary>
        /// Send async
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task<TRes> SendAsync<TReq, TRes>(string url, HttpMethod method, TReq data)
        {
            // Request
            using var message = CreateRequest(url, method);
            using var content = CreateContent<TReq>(data);
            message.Content = content;

            // Response
            return await GetResult<TRes>(message);
        }

        /// <summary>
        /// Send sync
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected TRes Send<TReq, TRes>(string url, HttpMethod method, TReq data)
        {
            return SendAsync<TReq, TRes>(url, method, data).Sync();
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

        /// <summary>
        /// Get sync
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public TRes Get<TRes>(string url)
        {
            return GetAsync<TRes>(url).Sync();
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

        /// <summary>
        /// Post sync
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        public void Post<TReq>(string url, TReq data)
        {
            PostAsync(url, data).Sync();
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

        /// <summary>
        /// Post sync
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public TRes Post<TReq, TRes>(string url, TReq data)
        {
            return PostAsync<TReq, TRes>(url, data).Sync();
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
            using var message = CreateRequest(url, HttpMethod.Post);
            using var content = new FormUrlEncodedContent(parameters);
            message.Content = content;
            return await GetResult<TRes>(message);
        }

        /// <summary>
        /// Post sync
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public TRes Post<TRes>(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return PostAsync<TRes>(url, parameters).Sync();
        }

        /// <summary>
        /// Post file
        /// </summary>
        /// <param name="url"></param>
        /// <param name="input">Input stream</param>
        /// <returns></returns>
        public async Task PostFileAsync(string url, Stream input)
        {
            using var message = CreateRequest(url, HttpMethod.Post);
            using var multipart = new MultipartFormDataContent();
            using var content = new StreamContent(input);
            multipart.Add(content);
            message.Content = multipart;
            await GetResult(message);
        }

        /// <summary>
        /// PostFile sync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public void PostFile(string url, Stream input)
        {
            PostFileAsync(url, input).Sync();
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
            using var file = File.OpenRead(path);
            await PostFileAsync(url, file);
        }

        /// <summary>
        /// PostFile sync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        public void PostFile(string url, string path)
        {
            PostFileAsync(url, path).Sync();
        }

        /// <summary>
        /// Post a file as multipart form data with returning an object
        /// </summary>
        /// <typeparam name="TRes">Type of the object that will be deserialized from the response</typeparam>
        /// <param name="url">Url</param>
        /// <param name="input">Input stream</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<TRes> PostFileAsync<TRes>(string url, Stream input)
        {
            using var message = CreateRequest(url, HttpMethod.Post);
            using var multipart = new MultipartFormDataContent();
            var content = new StreamContent(input);
            multipart.Add(content);
            message.Content = multipart;
            return await GetResult<TRes>(message);
        }

        /// <summary>
        /// Post a file as multipart form data with returning an object
        /// </summary>
        /// <typeparam name="TRes">Type of the object that will be deserialized from the response</typeparam>
        /// <param name="url">Url</param>
        /// <param name="path">Full path to input file</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<TRes> PostFileAsync<TRes>(string url, string path)
        {
            using var file = File.OpenRead(path);
            return await PostFileAsync<TRes>(url, file);
        }

        /// <summary>
        /// PostFile sync
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public TRes PostFile<TRes>(string url, string path)
        {
            return PostFileAsync<TRes>(url, path).Sync();
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="mediaType"></param>
        public async Task<HttpResponseMessage> GetFileAsync(string url, string mediaType)
        {
            using var message = CreateRequest(url, HttpMethod.Get);
            if (mediaType != null)
            {
                message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            }

            return await Client.SendAsync(message);
        }

        /// <summary>
        /// GetFile sync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public HttpResponseMessage GetFile(string url, string mediaType)
        {
            return GetFileAsync(url, mediaType).Sync();
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="output">Stream to copy to</param>
        /// <param name="mediaType"></param>
        public async Task GetFileAsync(string url, Stream output, string mediaType)
        {
            using var response = GetFile(url, mediaType);
            using var stream = response.Content;
            await stream.CopyToAsync(output);
        }

        /// <summary>
        /// GetFile sync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="output"></param>
        /// <param name="mediaType"></param>
        public void GetFile(string url, Stream output, string mediaType)
        {
            GetFileAsync(url, output, mediaType).Sync();
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="path">Path to local file</param>
        /// <param name="mediaType"></param>
        public async Task GetFileAsync(string url, string path, string mediaType)
        {
            using var stream = File.OpenWrite(path);
            await GetFileAsync(url, stream, mediaType);
        }

        /// <summary>
        /// GetFile sync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <param name="mediaType"></param>
        public void GetFile(string url, string path, string mediaType)
        {
            GetFileAsync(url, path, mediaType).Sync();
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

        /// <summary>
        /// Sync
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        public void Patch<TReq>(string url, TReq data)
        {
            PatchAsync(url, data).Sync();
        }

        /// <summary>
        /// Put a value
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task PutAsync<TReq>(string url, TReq value)
        {
            await SendAsync(url, HttpMethod.Put, value);
        }

        /// <summary>
        /// Put a value sync
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Put<TReq>(string url, TReq value)
        {
            PutAsync(url, value).Sync();
        }

        /// <summary>
        /// Put a value
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<TResult> PutAsync<TReq, TResult>(string url, TReq value)
        {
            return await SendAsync<TReq, TResult>(url, HttpMethod.Put, value);
        }

        /// <summary>
        /// Put a value sync
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TResult Put<TReq, TResult>(string url, TReq value)
        {
            return PutAsync<TReq, TResult>(url, value).Sync();
        }

        /// <summary>
        /// Send a delete
        /// </summary>
        /// <param name="url">Url</param>
        public async Task DeleteAsync(string url)
        {
            await SendAsync(url, HttpMethod.Delete);
        }

        /// <summary>
        /// Delete sync
        /// </summary>
        /// <param name="url"></param>
        public void Delete(string url)
        {
            DeleteAsync(url).Sync();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {            
            Client?.Dispose();
            // This will prevent derived class with a finalizer to need to re-implement dispose
            GC.SuppressFinalize(this);
        }
    }  
}
