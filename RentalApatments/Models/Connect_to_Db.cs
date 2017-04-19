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
        public ApplicationContext() : base("DefaultConnection") { }

        //database entities here
        DbSet<TypeDeal> TypeDeals { get; set; }
        DbSet<TypeRent> TypeRents { get; set; }
        DbSet<TypeForWhatRealty> TypeForWhatRealtys { get; set; }
        DbSet<CurrectRealty> CurrectRealties { get; set; }
        DbSet<TypeDescriptionRealty> TypeDescriptionRealties { get; set; }
        DbSet<TypeAnswer> TypeAnswers { get; set; }
       
    }
}