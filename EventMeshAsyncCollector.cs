using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EventMeshCustomBinding
{

    /// <summary>
    /// Collector class used to accumulate and then dispatch requests to SAP Event Mesh (Queue-based messaging)
    /// </summary>
    internal class EventMeshAsyncCollector : IAsyncCollector<EventMeshMessage>
    { 
        private string _eventMeshTokenEndpoint; 
        private string _eventMeshClientId;
        private string _eventMeshClientSecret;
        private string _eventMeshGrantType;
        private string _eventMeshRestBaseEndpoint;
        private string _eventMeshQueueName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMeshAsyncCollector"/> class.
        /// </summary>
        /// <param name="config">Configuration Provider for EventMesh</param>
        /// <param name="attr">EventMeshAttribute containing necessary info for calling Event Mesh REST endpoints (token and messaging)</param>
        public EventMeshAsyncCollector(EventMeshBinding config, EventMeshAttribute attr)
        {
            _eventMeshTokenEndpoint = attr.EventMeshTokenEndpoint;
            _eventMeshClientId = attr.EventMeshClientId;
            _eventMeshClientSecret = attr.EventMeshClientSecret;
            _eventMeshGrantType = attr.EventMeshGrantType;
            _eventMeshRestBaseEndpoint = attr.EventMeshRestBaseEndpoint;
            _eventMeshQueueName = attr.EventMeshQueueName;
        }

        /// <summary>
        /// Add a messge to the list of objects needing to be processed
        /// </summary>
        /// <param name="message">Message object to be sent to SAP Event Mesh</param>
        /// <param name="cancellationToken">Used to propagate notifications</param>
        /// <returns>Task whose resolution results being added to the collector</returns>
        public async Task AddAsync(EventMeshMessage message, CancellationToken cancellationToken = default)
        {
            try
            {

                var token = await GetOauthToken();
                
                var restEndPointWithQueue = $"{_eventMeshRestBaseEndpoint}/messagingrest/v1/queues/{HttpUtility.UrlEncode(_eventMeshQueueName)}/messages"; 
                
                var client = new RestClient(restEndPointWithQueue);

                client.Timeout = -1;

                var request = new RestRequest(Method.POST);

                request.AddHeader("x-qos", "0");

                request.AddHeader("Content-Type", "application/json");

                request.AddHeader("Authorization", $"{token.TokenType} {token.AccessToken}");

                var body = JsonConvert.SerializeObject(message, Formatting.Indented);

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                
                IRestResponse response = await client.ExecuteAsync(request);

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Execute methods associated with Event Mesh message
        /// </summary>
        /// <param name="cancellationToken">Used to propagate notifications</param>
        /// <returns>Task representing the message being uploaded</returns>
        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }


        /// <summary>
        /// Helper method to fetch the OAuth token from the corresponding endpoint
        /// </summary>
        /// <returns>Task representing the token for authentication</returns>
        private async Task<Token> GetOauthToken( )
        {

            var tokenUrl = $"{_eventMeshTokenEndpoint}?grant_type={_eventMeshGrantType}&response_type=token";

            var client = new RestClient(tokenUrl);
   
            client.Timeout = -1;
            
            var request = new RestRequest(Method.POST);
            
            var base64EncodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_eventMeshClientId}:{_eventMeshClientSecret}"));

            request.AddHeader("Authorization", $"Basic {base64EncodedCredentials}");
            
            var body = @"";
            
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            
            IRestResponse response = await client.ExecuteAsync(request);
            
            Token bearerToken = JsonConvert.DeserializeObject<Token>(response.Content);

            return bearerToken;
        }

        /// <summary>
        /// Internal class representing an OAUth token object
        /// </summary>
        internal class Token
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; } = default!;

            [JsonProperty("token_type")]
            public string TokenType { get; set; } = default!;

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; } = default!;

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; } = default!;
        }
    }
}
