using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

public class Keys_Onboarding_KoContext : DbContext
{
    // You can add custom code to this file. Changes will not be overwritten.
    // 
    // If you want Entity Framework to drop and regenerate your database
    // automatically whenever you change your model schema, please use data migrations.
    // For more information refer to the documentation:
    // http://msdn.microsoft.com/en-us/data/jj591621.aspx

    public Keys_Onboarding_KoContext() : base("name=Keys_Onboarding_KoContext")
    {
    }

    public System.Data.Entity.DbSet<Keys_Onboarding_Ko.Models.Customer> Customers { get; set; }

    public System.Data.Entity.DbSet<Keys_Onboarding_Ko.Models.Product> Products { get; set; }

    public System.Data.Entity.DbSet<Keys_Onboarding_Ko.Models.Store> Stores { get; set; }

    public System.Data.Entity.DbSet<Keys_Onboarding_Ko.Models.ProductSold> ProductSolds { get; set; }
}
