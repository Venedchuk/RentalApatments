using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentalApatments.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection") {

            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationContext>());
            //Database.SetInitializer<ApplicationDbContext>(new AppDbInitializer());
        }
        //database entities here
        public DbSet<TypeDeal> TypeDeals { get; set; }
        public DbSet<TypeRent> TypeRents { get; set; }
        public DbSet<TypeForWhatRealty> TypeForWhatRealtys { get; set; }
        public DbSet<CurrectRealty> CurrectRealties { get; set; }
        public DbSet<TypeDescriptionRealty> TypeDescriptionRealties { get; set; }
        public DbSet<TypeAnswer> TypeAnswers { get; set; }
    }
}