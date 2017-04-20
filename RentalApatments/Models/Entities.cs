using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentalApatments.Models
{
    public class TypeDeal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<TypeRent> TypeRent { get; set; }
    }

    public class TypeRent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<TypeForWhatRealty> TypeForWhatRealty { get; set; }
    }
    public class TypeForWhatRealty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<CurrectRealty> CurrectRealty { get; set; }
    }
    public class CurrectRealty//object
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<TypeDescriptionRealty> Answers { get; set; }
    }

    public class TypeDescriptionRealty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual TypeAnswer Type { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Answer { get; set; }
        public virtual TypeAnswer TypeАddition { get; set; }
        public IEnumerable<string> АdditionAnswer { get; set; }
        public virtual TypeAnswer TypeАddition2 { get; set; }
        public IEnumerable<string> АdditionAnswer2 { get; set; }


    }
    public class TypeAnswer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}