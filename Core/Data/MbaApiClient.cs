using Core.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public class MbaApiClient
    {
        private readonly Uri apiServerUri;
        private LoginModel credentials;
        private string userId;
        private string token;
        private DateTime tokenGenerationDate;

        public MbaApiClient(Uri apiServerUri)
        {
            this.apiServerUri = apiServerUri;
        }

        /// <summary>
        /// Get a JWT token from the API.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<bool> Login(LoginModel login)
        {
            var uri = "api/token";
            DateTime postRequestDate = DateTime.Now;
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Post, uri, JsonConvert.SerializeObject(login));
            if (!isSuccess)
            {
                return false;
            }
            TokenModel tokenAnswer = JsonConvert.DeserializeObject<TokenModel>(content);
            this.userId = tokenAnswer.UserId;
            this.token = tokenAnswer.Token;
            this.tokenGenerationDate = postRequestDate;
            this.credentials = login;
            return true;
        }

        /// <summary>
        /// Get the user information.
        /// </summary>
        /// <param name="includeEntries"></param>
        /// <returns></returns>
        public async Task<(UserModel, string)> GetUser(bool includeEntries = false)
        {
            await LoginIfTokenExpiredAsync();

            var uri = $"api/Users/{userId}?includeEntries={includeEntries}";
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Get, uri, "");
            if (!isSuccess)
            {
                return (null, content);
            }
            return (JsonConvert.DeserializeObject<UserModel>(content), "");
        }


        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<(UserModel, string)> CreateUser(LoginModel login)
        {
            string uri = "/api/Users/";
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Post, uri, JsonConvert.SerializeObject(login));
            if (!isSuccess)
            {
                return (null, content);
            }
            return (JsonConvert.DeserializeObject<UserModel>(content), "");
        }

        /// <summary>
        /// Update the user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(UserModel, string)> UpdateUser(UserModel user)
        {
            var uri = $"/api/users/{userId}/";
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Put, uri, JsonConvert.SerializeObject(user));
            if (!isSuccess)
            {
                return (null, content);
            }
            return (JsonConvert.DeserializeObject<UserModel>(content), "");
        }

        /// <summary>
        /// Delete a user and all its entries.
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> DeleteUser()
        {
            var uri = $"/api/users/{userId}";
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Delete, uri, "");
            if (!isSuccess)
            {
                return (false, content);
            }
            return (true, "");
        }

        /// <summary>
        /// Get all the logged in user entries.
        /// </summary>
        /// <returns></returns>
        public async Task<(EntryModel[] entry, string error)> GetEntries()
        {
            var uri = $"/api/users/{userId}/entries/";
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Get, uri, "");
            if (!isSuccess)
            {
                return (null, content);
            }
            return (JsonConvert.DeserializeObject<EntryModel[]>(content), "");
        }

        /// <summary>
        /// Get a specific logged in user entry.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(EntryModel entry, string error)> GetEntry(int id)
        {
            var uri = $"/api/users/{userId}/entries/{id}";
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Get, uri, "");
            if (!isSuccess)
            {
                return (null, content);
            }
            return (JsonConvert.DeserializeObject<EntryModel>(content), "");
        }

        /// <summary>
        /// Create a new entry for the logged in user.
        /// </summary>
        /// <param name="entryModel"></param>
        /// <returns></returns>
        public async Task<(EntryModel, string)> CreateEntry(EntryModel entryModel)
        {
            var uri = $"/api/users/{userId}/entries/";
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Post, uri, JsonConvert.SerializeObject(entryModel));
            if (!isSuccess)
            {
                return (null, content);
            }
            return (JsonConvert.DeserializeObject<EntryModel>(content), "");
        }

        /// <summary>
        /// Update an entry of the logged in user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entryModel"></param>
        /// <returns></returns>
        public async Task<(EntryModel, string)> UpdateEntry(int id, EntryModel entryModel)
        {
            var uri = $"/api/users/{userId}/entries/{id}";
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Put, uri, JsonConvert.SerializeObject(entryModel));
            if (!isSuccess)
            {
                return (null, content);
            }
            return (JsonConvert.DeserializeObject<EntryModel>(content), "");
        }

        /// <summary>
        /// Delete an entry of the logged in user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(bool, string)> DeleteEntry(int id)
        {
            var uri = $"/api/users/{userId}/entries/{id}";
            (string content, bool isSuccess) = await ExecuteRequest(RequestType.Delete, uri,"");
            if (!isSuccess)
            {
                return (false, content);
            }
            return (true, "");
        }


        /// <summary>
        /// Check if the token is expired ; claim a new one if it's the case.
        /// </summary>
        /// <returns></returns>
        private async Task LoginIfTokenExpiredAsync()
        {
            if ((DateTime.Now - tokenGenerationDate).TotalMinutes > 30)
            {
                bool isLogged = await Login(this.credentials);
                if (!isLogged)
                {
                    throw new Exception("The user session has expired.");
                }
            }
        }

        /// <summary>
        /// Execute an HTTP Request to the api.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="uri">endpoint to call</param>
        /// <param name="json">content to send (only used for the types POST and PUT)</param>
        /// <returns></returns>
        public async Task<(string content, bool isSuccess)> ExecuteRequest(RequestType type, string uri, string json)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = apiServerUri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                HttpResponseMessage response;
                switch (type)
                {
                    case RequestType.Get:
                        response = await client.GetAsync(uri);
                        break;
                    case RequestType.Post:
                        response = await client.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
                        break;
                    case RequestType.Put:
                        response = await client.PutAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
                        break;
                    case RequestType.Delete:
                        response = await client.DeleteAsync(uri);
                        break;
                    default:
                        throw new NotSupportedException();
                }

                return (await response.Content.ReadAsStringAsync(), response.IsSuccessStatusCode);
            }
        }

        /// <summary>
        /// Define the type of HTTP Request.
        /// </summary>
        public enum RequestType
        {
            Get, Post, Put, Delete
        }
    }
}
