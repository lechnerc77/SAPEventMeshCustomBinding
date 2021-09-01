using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;

namespace EventMeshCustomBinding
{
    [Extension("EventMeshBinding")]

    /// <summary>
    /// WebJobs SDK Extension for SAP Event Mesh binding
    /// </summary>
    public class EventMeshBinding : IExtensionConfigProvider
    {
        /// <summary>
        /// Initialize the SAP Event Mesh binding extension
        /// </summary>
        /// <param name="context">Context containing info relevant to this extension</param>
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddConverter<string, EventMeshMessage>(input => new EventMeshMessage { message = input });
            context.AddBindingRule<EventMeshAttribute>().BindToCollector(attr => new EventMeshAsyncCollector(this, attr));
        }
    }
}
