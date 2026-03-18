using System.ComponentModel.DataAnnotations;

namespace ProductCatalogueManagement.Models
{
    public  class ProductData
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Name must be at least 5 characters long.")]
        public string Name { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Description must be at least 7 characters long.")]
        public string Description { get; set; }
    }
}