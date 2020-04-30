using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Data.Tag.Client.In
{
    public interface ITagClient
    {
        Task ChangeTag(string avatarUrl, string id, string newTag, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken));
    }
}
