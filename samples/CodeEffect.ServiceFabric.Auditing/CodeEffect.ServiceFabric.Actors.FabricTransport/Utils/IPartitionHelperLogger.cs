using System;
using System.Collections.Generic;
using System.Fabric;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Utils
{
    public interface IPartitionHelperLogger
    {
        void EnumeratingPartitions(Uri serviceUri);
        void FailedToEnumeratePartitions(Uri serviceUri, Exception ex);
        void EnumeratedExistingPartitions(Uri serviceUri, IEnumerable<ServicePartitionInformation> partitions);
        void EnumeratedAndCachedPartitions(Uri serviceUri, IEnumerable<ServicePartitionInformation> partitions);
    }
}