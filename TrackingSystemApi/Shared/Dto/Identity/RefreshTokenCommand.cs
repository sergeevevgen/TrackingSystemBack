using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.Shared.Dto.Identity
{
    public class RefreshTokenCommand
    {
        [Required(ErrorMessage = "поле должно быть заполнено")]
        public string TokenHash { get; init; } = null!;
    }
}
