using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core;
using MilkStore.ModelViews.GiftModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IGiftService
    {
        Task<IEnumerable<GiftModel>> GetGift(string? id);
        Task CreateGift(GiftModel GiftModel);
        Task UpdateGift(string id, GiftModel GiftModel);
        Task DeleteGift(string id);
        Task<BasePaginatedList<Gift>> PagingGift(int page, int pageSize);
    }
}
