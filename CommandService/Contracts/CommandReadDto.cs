namespace CommandService.Contracts;

public record CommandReadDto(int Id, string HowTo, string CommandLine, int PlatformId);
