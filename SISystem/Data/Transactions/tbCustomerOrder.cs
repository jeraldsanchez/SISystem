using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbCustomerOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CustomerOrderID { get; set; }
        public Guid CustomerID { get; set; }
        public Guid SalesAgentID { get; set; }
        public int OrderStatusID { get; set; }
        public string TransactionCode { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
        public DateTime ForMonthOf { get; set; }
        public decimal LeadTime { get; set; }
        public DateTime? MarkDeliveredDate { get; set; }
        public Guid? MarkDeliveredByID { get; set; }//
        public DateTime? DateCancelled { get; set; }
        public string ReasonForCancellation { get; set; }
        public Guid? CancelledByID { get; set; }//
        public int Version { get; set; }
        public bool IsBadDebt { get; set; }
        public bool IsBilled { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public Guid? ModifiedByID { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? SubSalesAgentID { get; set; }
    }
}
