using CoffeeStoreAspCore.ViewModels.Catalog.BillOrders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeStoreAspCore.ViewModels.Catalog.Bills
{
    public class ThongTinDonHang
    {
        public int Id { get; set; }

        public DateTime Days { get; set; }
        public string UserName { set; get; }
        public string Address { set; get; }
        public List<BillDetailViewModel> BillDetails { set; get; }
    }
}
