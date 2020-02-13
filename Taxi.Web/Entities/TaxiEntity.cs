using System.ComponentModel.DataAnnotations;

namespace Taxi.Web.Entities
{
    public class TaxiEntity
    {
        public int Id { get; set; }

        [StringLength(6, MinimumLength = 6, ErrorMessage = "The {0} field must have {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        //Esta expresión valida que primero sean 3 letras y luego 3 numeros
        [RegularExpression(@"^([A-Za-z]{3}\d{3})$", ErrorMessage = "The field {0} must have three characters and three numbers.")]
        public string Plaque { get; set; }

    }
}
