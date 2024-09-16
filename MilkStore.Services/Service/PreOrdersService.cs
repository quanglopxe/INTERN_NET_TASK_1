using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.ModelViews.PreOrdersModelView;
using MilkStore.Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Services.Service
{
    public class PreOrdersService : IPreOrdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext _context;
        public PreOrdersService(DatabaseContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<PreOrders> CreatePreOrders(PreOrdersModelView preOrdersModel)
        {
            var newPreOrder = new PreOrders
            {
                ProductID = preOrdersModel.ProductID,
                UserID = preOrdersModel.UserID,
                PreoderDate = preOrdersModel.PreoderDate,
                Status = preOrdersModel.Status,
                Quantity = preOrdersModel.Quantity,
            };
            await _unitOfWork.GetRepository<PreOrders>().InsertAsync(newPreOrder);
            await _unitOfWork.SaveAsync();
            return newPreOrder;
        }

        public async Task<PreOrders> DeletePreOrders(string id)
        {
            var preord = await _unitOfWork.GetRepository<PreOrders>().GetByIdAsync(id);

            if (preord == null)
            {
                throw new Exception("Pre-order không tồn tại.");
            }

            await _unitOfWork.GetRepository<PreOrders>().DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return preord;
        }

        public async Task<IEnumerable<PreOrders>> GetPreOrders(string? id)
        {
            if (id == null)
            {
                return await _unitOfWork.GetRepository<PreOrders>().GetAllAsync();
            }
            else
            {
                var preord = await _unitOfWork.GetRepository<PreOrders>().GetByIdAsync(id);
                return preord != null ? new List<PreOrders> { preord } : new List<PreOrders>();
            }

        }

        public async Task<PreOrders> UpdatePreOrders(string id, PreOrdersModelView preOrdersModel)
        {

            var preord = await _unitOfWork.GetRepository<PreOrders>().GetByIdAsync(id);

            if (preord == null)
            {
                throw new Exception("Pre-order không tồn tại.");
            }

            preord.ProductID = preOrdersModel.ProductID;
            preord.UserID = preOrdersModel.UserID;
            preord.PreoderDate = preOrdersModel.PreoderDate;
            preord.Status = preOrdersModel.Status;
            preord.Quantity = preOrdersModel.Quantity;
            preord.LastUpdatedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<PreOrders>().UpdateAsync(preord);
            await _unitOfWork.SaveAsync();
            return preord;
        }
        public async Task<IList<PreOrdersModelView>> Pagination(int pageSize, int pageNumber)
        {
            var skip = (pageNumber - 1) * pageSize;
            return await _context.PreOrders.Skip(skip).Take(pageSize).Select(p => new PreOrdersModelView
            {
                ProductID = p.ProductID,
                UserID = p.UserID,
                PreoderDate = p.PreoderDate,
                Status = p.Status,
                Quantity = p.Quantity,
            }).ToArrayAsync();
        }
    }
}
