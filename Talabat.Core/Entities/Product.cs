using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        //[ForeignKey("ProductBrand")]
        public int ProductBrandId { get; set; }         // Foreign Key : Not Allow NULL

        //[ForeignKey("ProductType")]
        public int ProductTypeId { get; set; }          // Foreign Key : Not Allow NULL

        public ProductBrand ProductBrand { get; set; }  // Navigational Properity [ONE]

        public ProductType ProductType { get; set; }    // Navigational Properity [ONE]

    }
}
