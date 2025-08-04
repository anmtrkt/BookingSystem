using BookingSystem.Core.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Entities.Institutions
{
    public class CreateRoomDto
    {
        [Required(ErrorMessage = "Номер комнаты обязателен.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Длина номера от 1 до 20 символов.")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "Идентификатор здания обязателен.")]
        public Guid BuildingId { get; set; }

        [Required(ErrorMessage = "Оборудование обязательно.")]
        public required Equipment Equipment { get; set; }

        [Range(1, 1000, ErrorMessage = "Количество мест должно быть от 1 до 1000.")]
        public uint CountOfPlaces { get; set; }
    }
    }
