using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Models.BuildingModels
{
    public class CreateBuildingDto
    {
        [Required(ErrorMessage = "Адрес обязателен.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Длина адреса от 5 до 200 символов.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "BranchId обязателен.")]
        public Guid BranchId { get; set; }

    }
}
