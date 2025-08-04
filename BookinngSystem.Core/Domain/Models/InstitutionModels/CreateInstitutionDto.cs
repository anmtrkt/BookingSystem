using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Models.InstitutionModels
{
    public class CreateInstitutionDto
    {
        [Required(ErrorMessage = "Название обязательно к заполнению.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Длина названия от 3 до 100 символов.")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 10, ErrorMessage = "Приоритет должен быть в диапазоне от 0 до 10.")]
        public byte Priority { get; set; }

        [AllowNull]
        public Guid? ParentId { get; set; }

 
    }
}
