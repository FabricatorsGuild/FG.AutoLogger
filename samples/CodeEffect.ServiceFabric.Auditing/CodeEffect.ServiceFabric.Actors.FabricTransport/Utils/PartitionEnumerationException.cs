using System;
using System.Runtime.Serialization;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Utils
{
    [Serializable]
    public class PartitionEnumerationException : Exception
    {
        public PartitionEnumerationException()
        {
        }

        public PartitionEnumerationException(Uri serviceUri) : base(serviceUri?.ToString())
        {
        }

        public PartitionEnumerationException(Uri serviceUri, Exception inner) : base(serviceUri?.ToString(), inner)
        {
        }

        protected PartitionEnumerationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}