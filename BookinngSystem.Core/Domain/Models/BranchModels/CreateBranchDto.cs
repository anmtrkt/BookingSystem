using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Models.BranchModel
{
    public class CreateBranchDto
    {
        [Required(ErrorMessage = "Идентификатор организации обязателен.")]
        public Guid InstitutionId { get; set; }

        [Required(ErrorMessage = "Название филиала обязательно.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Длина названия от 3 до 100 символов.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Адрес филиала обязателен.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Длина адреса от 5 до 200 символов.")]
        public string Address { get; set; } = string.Empty;

    }
}
