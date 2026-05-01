using System.ComponentModel.DataAnnotations;

namespace CommandService.Contracts;

public record class CommandCreateDto([Required] string HowTo, [Required] string CommandLine);
