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

            if (_partitions.ContainsKey(serviceUri))
            {
                
                var partitions = _partitions[serviceUri];
                logger.EnumeratedExistingPartitions(serviceUri, partitions);
            }

            try
            {
                var fabricClient = new FabricClient();
                var servicePartitionList = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
                // For each partition, build a service partition client used to resolve the low key served by the partition.
                IList<Int64RangePartitionInformation> partitionKeys = new List<Int64RangePartitionInformation>(servicePartitionList.Count);
                foreach (var partition in servicePartitionList)
                {
                    var partitionInfo = partition.PartitionInformation as Int64RangePartitionInformation;
                    if (partitionInfo == null)
                    {
                        throw new InvalidOperationException(
                            $"The service {serviceUri} should have a uniform Int64 partition. Instead: {partition.PartitionInformation.Kind}");
                    }

                    partitionKeys.Add(partitionInfo);
                }
                _partitions.Add(serviceUri, partitionKeys);

                logger.EnumeratedAndCachedPartitions(serviceUri, partitionKeys);
                return partitionKeys;
            }
            catch (Exception ex)
            {
                logger.FailedToEnumeratePartitions(serviceUri, ex);
                throw ex;
            }
        }
    }
}