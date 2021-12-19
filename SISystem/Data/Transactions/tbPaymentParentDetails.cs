using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbPaymentParentDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid SalesAgentId { get; set; }
        public string AgentName { get; set; }
        public decimal TotalPayable { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid? ModifiedByID { get; set; }
        public DateTime? DateModified { get; set; }
        public string Remarks { get; set; }
    }
}
