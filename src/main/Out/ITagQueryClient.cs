using System.Threading;
using System.Threading.Tasks;
using ei8.Data.Tag.Common;

namespace ei8.Data.Tag.Client.Out
{
    public interface ITagQueryClient
    {
        Task<ItemData> GetItemById(string avatarId, string id, CancellationToken token = default(CancellationToken)); 
    }
}
