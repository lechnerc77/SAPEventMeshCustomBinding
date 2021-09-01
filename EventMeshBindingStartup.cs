using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using EventMeshCustomBinding;

[assembly: WebJobsStartup(typeof(EventMeshBindingStartup))]

namespace EventMeshCustomBinding
{
    public class EventMeshBindingStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {

            builder.AddExtension<EventMeshBinding>();

        }

    }
}