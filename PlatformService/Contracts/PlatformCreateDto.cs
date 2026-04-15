using System.ComponentModel.DataAnnotations;

namespace PlatformService.Contracts;

public record class PlatformCreateDto(
    [Required] string Name,
    [Required] string Publisher,
    [Required] string Cost
);
