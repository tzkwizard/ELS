using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Partitioning;

namespace LMS.Common.Service.Interface
{
    public interface IResolverService
    {
        RangePartitionResolver<long> GetResolver();
        Task<bool> UpdateResolver(DocumentCollection newDc);
        Task<bool> InitResolver();    
    }
}