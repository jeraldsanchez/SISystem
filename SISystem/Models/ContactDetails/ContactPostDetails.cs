using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class ContactPostDetails
    {
        public CustomerAddress CustomerAddress { get; set; } = new CustomerAddress();
        public CustomerDetails CustomerDetails { get; set; } = new CustomerDetails();
        //public ContactLoyaltyPoints ContactLoyaltyPoints { get; set; } = new ContactLoyaltyPoints();
        //public ContactMembershipTypeTbl ContactMembershipTypeTbl { get; set; } = new ContactMembershipTypeTbl();
        public CustomerOtherDetails CustomerOtherDetails { get; set; } = new CustomerOtherDetails();
        public CustomerContacts CustomerContacts { get; set; } = new CustomerContacts();
        //public ContactTypeTbl ContactTypeTbl { get; set; } = new ContactTypeTbl();
    }
}
