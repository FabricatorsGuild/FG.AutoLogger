using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;
using System.Text;
using System.Threading.Tasks;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Utils
{
    public class PartitionHelper
    {
        private readonly object _lock = new object();

        // ReSharper disable once UnusedMember.Global - Used for logging!
        public static string ToString(IEnumerable<Int64RangePartitionInformation> partitions)
        {
            var partitionsString = new StringBuilder();
            var delimiter = "";
            foreach (var partition in partitions)
            {
                partitionsString.Append(delimiter);
                partitionsString.Append($"{partition.LowKey}-{partition.HighKey}");
                delimiter = ",";
            }
            return partitionsString.ToString();
        }

        private readonly IDictionary<Uri, IEnumerable<Int64RangePartitionInformation>> _partitions;

        public PartitionHelper()
        {
            _partitions = new ConcurrentDictionary<Uri, IEnumerable<Int64RangePartitionInformation>>();
        }

        public async Task<IEnumerable<Int64RangePartitionInformation>> GetInt64Partitions(Uri serviceUri, IPartitionHelperLogger logger)
        {
            logger.EnumeratingPartitions(serviceUri);

            lock (_lock)
            {
                if (_partitions.ContainsKey(serviceUri))
                {
                    var partitions = _partitions[serviceUri];
                    logger.EnumeratedExistingPartitions(serviceUri, partitions);
                }
            }

            try
            {
                var fabricClient = new FabricClient();
                var servicePartitionList = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
                IList<Int64RangePartitionInformation> partitionKeys = new List<Int64RangePartitionInformation>(servicePartitionList.Count);
                foreach (var partition in servicePartitionList)
                {
                    var partitionInfo = partition.PartitionInformation as Int64RangePartitionInformation;
                    if (partitionInfo == null)
                    {
                        throw new InvalidOperationException($"The service {serviceUri} should have a uniform Int64 partition. Instead: {partition.PartitionInformation.Kind}");
                    }
                    partitionKeys.Add(partitionInfo);                    
                }
                lock (_lock)
                {
                    if (!_partitions.ContainsKey(serviceUri))
                    {
                        _partitions.Add(serviceUri, partitionKeys);
                    }
                }

                logger.EnumeratedAndCachedPartitions(serviceUri, partitionKeys);
                return partitionKeys;
            }
            catch (Exception ex)
            {
                logger.FailedToEnumeratePartitions(serviceUri, ex);
                throw new PartitionEnumerationException(serviceUri, ex);
            }
        }
    }
}