using Microsoft.Azure.WebJobs.Description;
using System;

namespace EventMeshCustomBinding
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]

    /// <summary>
    /// Class representing the configuration attributes from teh Azure Function to connect to SAP Event Mesh (Queue-based messaging)
    /// </summary>
    public class EventMeshAttribute : Attribute
    {
        [AutoResolve]
        public string EventMeshTokenEndpoint {  get; set; } = default!;

        [AutoResolve]
        public string EventMeshClientId { get; set; } = default!;

        [AutoResolve]
        public string EventMeshClientSecret { get; set; } = default!;

        [AutoResolve]
        public string EventMeshGrantType { get; set; } = default!;

        [AutoResolve]
        public string EventMeshRestBaseEndpoint { get; set; } = default!;

        [AutoResolve]
        public string EventMeshQueueName { get; set; } = default!;
    }
}