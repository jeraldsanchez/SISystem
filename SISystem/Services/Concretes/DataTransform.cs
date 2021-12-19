using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class DataTransform : IDataTransform
    {
        public string CalculateLeadTime(DateTime orderDate, DateTime dateCreated)
        {
            TimeSpan time = orderDate.Subtract(dateCreated);
            string dateString = $"{time.Days}";
            return dateString;
        }

        public Guid? NullSubAgent(Guid? id)
        {
            if (id == null) {
                return Guid.Parse("00000000-0000-0000-0000-000000000000");
            }
            else{
                return id;
            }
             
        }
        public tbCustomerOrderVersion GetData(tbCustomerOrderMain or, string revertBy)
        {
            tbCustomerOrderVersion ver = new tbCustomerOrderVersion() { 
                CustomerOrderId = or.Id,
                OrderStatusID = or.OrderStatusId, //orderStatus == 0 ? or.OrderStatusID : orderStatus,
                DateCreated = DateTime.Now,
                TransactionBy = revertBy
            };
            return ver;
        }

        public tbCustomerOrderProcessor GetDataCustomerOrderPageProcessor(CustomerOrderMainRequest or, Guid id, string encodedName, Guid encodedId)
        {
            tbCustomerOrderProcessor order = new tbCustomerOrderProcessor()
            {
                CustomerOrderId = id,
                EncodedByName = encodedName,
                EndcodedById = encodedId,
                RemarksCustomerOrder = or.RemarksCustomerOrder,
                OrderDate = or.OrderDate
            };
            return order;
        }

        public tbCustomerOrderMain GetDataCustomerOrderPage(CustomerOrderMainRequest or)
        {
            tbCustomerOrderMain order = new tbCustomerOrderMain()
            {
                CustomerId = or.CustomerId,
                CustomerName = or.CustomerName,
                ForMonthOf = or.ForMonthOf,
                SalesAgentId = or.SalesAgentId,
                SalesAgentName = or.SalesAgentName,
                SubSalesAgentId = or.SubSalesAgentId,
                SubSalesAgentName = or.SubSalesAgentName,
                OrderStatusId = 1,
                TinNo = or.TinNo,
                OrderAmount = or.OrderAmount,
                TransactionCode = or.TransactionCode,
                DateCreated = DateTime.Now,
                IsCancelled = false,
        };
            return order;
        }

        public string GetTransactionCode()
        {
            DateTime dateTime = DateTime.Now;
            
            string transactionCode = $"TC"+ dateTime.ToString("MMddyyyy") + "-" + dateTime.ToString("hhmmss") + CreatePin(4);
            return transactionCode;
        }

        public string CreatePin(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int randomInt = Between(0, 9);
                builder.Append(randomInt);
            }

            return builder.ToString();
        }

        public int Between(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(randomNumber);

                double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

                // We are using Math.Max, and substracting 0.00000000001, 
                // to ensure "multiplier" will always be between 0.0 and .99999999999
                // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
                double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

                // We need to add one to the range, to allow for the rounding done with Math.Floor
                int range = maximumValue - minimumValue + 1;

                double randomValueInRange = Math.Floor(multiplier * range);

                return (int)(minimumValue + randomValueInRange);
            }
        }
    }
}
