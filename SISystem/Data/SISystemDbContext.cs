using Microsoft.EntityFrameworkCore;
using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class SISystemDbContext : DbContext
    {
        public SISystemDbContext(DbContextOptions<SISystemDbContext> options)
           : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<tbIssuingCompanyDocuments>().ToTable("tbIssuingCompanyDocuments", schema: "dbo");
            builder.Entity<tbIssuingCompany>().ToTable("tbIssuingCompany", schema: "dbo");
            builder.Entity<tbCompanyType>().ToTable("tbCompanyType", schema: "dbo");

            builder.Entity<tbSalesSubAgent>().ToTable("tbSalesSubAgent", schema: "dbo");
            builder.Entity<tbSalesAgentContact>().ToTable("tbSalesAgentContact", schema: "dbo");
            builder.Entity<tbSalesAgent>().ToTable("tbSalesAgent", schema: "dbo");

            builder.Entity<tbCustomer>().ToTable("tbCustomer", schema: "dbo");
            builder.Entity<tbCustomerSalesAgent>().ToTable("tbCustomerSalesAgent", schema: "dbo");

            builder.Entity<tbPaymentForm>().ToTable("tbPaymentForm", schema: "dbo");
            builder.Entity<tbPaymentParentCustomer>().ToTable("tbPaymentParentCustomer", schema: "dbo");
            builder.Entity<tbPaymentParentDetails>().ToTable("tbPaymentParentDetails", schema: "dbo");
            builder.Entity<tbBilling>().ToTable("tbBilling", schema: "dbo");
            builder.Entity<tbBillingDetails>().ToTable("tbBillingDetails", schema: "dbo");
            builder.Entity<tbCustomerOrder>().ToTable("tbCustomerOrder", schema: "dbo");
            builder.Entity<tbCustomerOrderDetails>().ToTable("tbCustomerOrderDetails", schema: "dbo");
            builder.Entity<tbOrderStatus>().ToTable("tbOrderStatus", schema: "dbo");
            builder.Entity<tbPaymentDetails>().ToTable("tbPaymentDetails", schema: "dbo");
            builder.Entity<tbUserDetails>().ToTable("tbUserDetails", schema: "dbo");
            builder.Entity<tbUserRole>().ToTable("tbUserRole", schema: "dbo");
            builder.Entity<tbUser>().ToTable("tbUser", schema: "dbo");
            builder.Entity<tbCustomerOrderVersion>().ToTable("tbCustomerOrderVersion", schema: "dbo");
            builder.Entity<tbCustomerOrderProcessor>().ToTable("tbCustomerOrderProcessor", schema: "dbo");
            builder.Entity<tbCustomerOrderMain>().ToTable("tbCustomerOrderMain", schema: "dbo");

            #region User Accounts and Registration
            builder.Entity<tbUser>().ToTable("tbUser", schema: "dbo");
            builder.Entity<tbLogs>().ToTable("tbLogs", schema: "dbo");
            builder.Entity<OAuthTbl>().ToTable("OAuthTbl", schema: "dbo");
            builder.Entity<PayMethodTbl>().ToTable("PayMethodTbl", schema: "dbo");
            builder.Entity<PayModeTbl>().ToTable("PayModeTbl", schema: "dbo");
            builder.Entity<SubscriptionTbl>().ToTable("SubscriptionTbl", schema: "dbo");
            builder.Entity<SubscriptionUserTbl>().ToTable("SubscriptionUserTbl", schema: "dbo");
            builder.Entity<tbUserDetails>().ToTable("tbUserDetails", schema: "dbo");
            builder.Entity<UserForgotPassword>().ToTable("UserForgotPassword", schema: "dbo");

            builder.Entity<tbEmployeeLogs>().ToTable("tbEmployeeLogs", schema: "dbo");

            builder.Entity<tbSOA>().ToTable("tbSOA", schema: "dbo");

            #endregion

            #region Roles
            builder.Entity<tbRoles>().ToTable("tbRoles", schema: "dbo");
            builder.Entity<tbRolesColumnExcept>().ToTable("tbRolesColumnExcept", schema: "dbo");
            builder.Entity<tbRolesDetails>().ToTable("tbRolesDetails", schema: "dbo");
            builder.Entity<tbRolesFunctions>().ToTable("tbRolesFunctions", schema: "dbo");
            #endregion
        }

        #region Roles
        public DbSet<tbRoles> tbRoles { get; set; }
        public DbSet<tbRolesColumnExcept> tbRolesColumnExcept { get; set; }
        public DbSet<tbRolesDetails> tbRolesDetails { get; set; }
        public DbSet<tbRolesFunctions> tbRolesFunctions { get; set; }
        #endregion

        #region User Accounts and Registration
        public DbSet<tbUser> tbUser { get; set; }
        public DbSet<tbLogs> tbLogs { get; set; }
        public DbSet<tbEmployeeLogs> tbEmployeeLogs { get; set; }
        public DbSet<OAuthTbl> OAuthTbl { get; set; }
        public DbSet<PayMethodTbl> PayMethodTbl { get; set; }
        public DbSet<PayModeTbl> PayModeTbl { get; set; }
        public DbSet<SubscriptionTbl> SubscriptionTbl { get; set; }
        public DbSet<SubscriptionUserTbl> SubscriptionUserTbl { get; set; }
        public DbSet<tbUserDetails> UserDetailsTbl { get; set; }
        public DbSet<UserForgotPassword> UserForgotPassword { get; set; }
        #endregion

        #region Issuing Company
        public DbSet<tbIssuingCompanyDocuments> tbIssuingCompanyDocuments { get; set; }
        public DbSet<tbIssuingCompany> tbIssuingCompany { get; set; }
        public DbSet<tbCompanyType> tbCompanyType { get; set; }
        public DbSet<tbSOA> tbSOA { get; set; }
        #endregion

        #region Agents
        public DbSet<tbSalesSubAgent> tbSalesSubAgent { get; set; }
        public DbSet<tbSalesAgentContact> tbSalesAgentContact { get; set; }
        public DbSet<tbSalesAgent> tbSalesAgent { get; set; }
        #endregion

        #region Customer
        public DbSet<tbCustomer> tbCustomer { get; set; }
        public DbSet<tbCustomerSalesAgent> tbCustomerSalesAgent { get; set; }
        #endregion

        #region Transactions
        public DbSet<tbPaymentForm> tbPaymentForm { get; set; }
        public DbSet<tbPaymentParentCustomer> tbPaymentParentCustomer { get; set; }
        public DbSet<tbPaymentParentDetails> tbPaymentParentDetails { get; set; }
        public DbSet<tbBilling> tbBilling { get; set; }
        public DbSet<tbBillingDetails> tbBillingDetails { get; set; }
        public DbSet<tbCustomerOrder> tbCustomerOrder { get; set; }
        public DbSet<tbCustomerOrderDetails> tbCustomerOrderDetails { get; set; }
        public DbSet<tbOrderStatus> tbOrderStatus { get; set; }
        public DbSet<tbPaymentDetails> tbPaymentDetails { get; set; }
        public DbSet<tbUserDetails> tbUserDetails { get; set; }
        public DbSet<tbUserRole> tbUserRole { get; set; }
        public DbSet<tbCustomerOrderVersion> tbCustomerOrderVersion { get; set; }
        public DbSet<tbCustomerOrderProcessor> tbCustomerOrderProcessor { get; set; }
        public DbSet<tbCustomerOrderMain> tbCustomerOrderMain { get; set; }
        #endregion

        #region Reports
        public DbSet<ReportSOAResponse> ReportSOAResponse { get; set; }
        #endregion

        #region Billing
        public DbSet<BillingStatementResponse> BillingStatementResponse { get; set; }
        #endregion
    }
}
