using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

public class Key_onboardingContext : DbContext
{
    // You can add custom code to this file. Changes will not be overwritten.
    // 
    // If you want Entity Framework to drop and regenerate your database
    // automatically whenever you change your model schema, please use data migrations.
    // For more information refer to the documentation:
    // http://msdn.microsoft.com/en-us/data/jj591621.aspx

    public Key_onboardingContext() : base("name=Key_onboardingContext")
    {
    }

    public System.Data.Entity.DbSet<Key_onboarding.Models.Product> Products { get; set; }

    public System.Data.Entity.DbSet<Key_onboarding.Models.Customer> Customers { get; set; }

    public System.Data.Entity.DbSet<Key_onboarding.Models.Store> Stores { get; set; }

    public System.Data.Entity.DbSet<Key_onboarding.Models.ProductSold> ProductSolds { get; set; }
}
