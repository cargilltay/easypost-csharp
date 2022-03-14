using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyPost.Utilities;
using RestSharp;
using RestSharp.Serializers;

namespace EasyPost
{
    public class Request
    {
        private readonly RestRequest _restRequest;
        public string? RootElement { get; set; }

        public Request(string iResource, Method method = Method.Get)
        {
            _restRequest = new RestRequest(iResource, method);
            _restRequest.AddHeader("Accept", "application/json");
        }

        /// <summary>
        ///     Add a body to the request.
        /// </summary>
        /// <param name="parameters">Dictionary of key-value pairs for creating request body.</param>
        public void AddBody(Dictionary<string, object> parameters)
        {
            string? body = JsonSerialization.ConvertObjectToJson(parameters);
            if (body != null)
            {
                _restRequest.AddStringBody(body, ContentType.Json);
            }
        }

        /// <summary>
        ///     Add a parameter to the request.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="value">Value of parameter.</param>
        /// <param name="type">Type of parameter.</param>
        public void AddParameter(string name, string? value, ParameterType type)
        {
            if (value == null)
            {
                return;
            }

            _restRequest.AddParameter(name, value, type);
        }

        /// <summary>
        ///     Add a query string parameter to the request.
        /// </summary>
        /// <param name="parameters">Dictionary of key-value pairs for creating URL query parameters.</param>
        public void AddQueryString(IDictionary<string, object> parameters)
        {
            foreach (KeyValuePair<string, object> pair in parameters)
            {
                AddParameter(pair.Key, Convert.ToString(pair.Value), ParameterType.QueryString);
            }
        }

        /// <summary>
        ///     Add a URL segment to the request.
        /// </summary>
        /// <param name="name">Name of segment.</param>
        /// <param name="value">Value of segment.</param>
        public void AddUrlSegment(string name, string value) => _restRequest.AddUrlSegment(name, value);

        /// <summary>
        ///     Execute the request.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize response data into.</typeparam>
        /// <returns>An instance of a T type object.</returns>
        public async Task<T> Execute<T>() where T : new()
        {
            Client client = ClientManager.Build();
            return await client.Execute<T>(this, RootElement);
        }

        /// <summary>
        ///     Execute the request.
        /// </summary>
        /// <returns>Whether the request was successful or not.</returns>
        public async Task<bool> Execute()
        {
            Client client = ClientManager.Build();
            return await client.Execute(this);
        }

        public static explicit operator RestRequest(Request request) => request._restRequest;
    }
}
