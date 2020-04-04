using System.Threading;
using System.Threading.Tasks;
using works.ei8.Data.Tag.Common;

namespace works.ei8.Data.Tag.Client.Out
{
    public interface ITagQueryClient
    {
        Task<ItemData> GetItemById(string avatarId, string id, CancellationToken token = default(CancellationToken)); 
    }
}
