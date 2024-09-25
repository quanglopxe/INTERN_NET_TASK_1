﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.OrderGiftModelViews
{
    public class OrderGiftModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "địa chỉ không được để trống")]
        public required string Address { get; set; }
        [Required(ErrorMessage = "Id user không được để trống")]
        public required Guid UserId { get; set; }
        [Required(ErrorMessage = "Id quà tặng không được để trống")]
        public required string GiftId { get; set; }
        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public required string Status { get; set; }
    }
}