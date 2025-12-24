namespace DotnetWorker.Application.WebsiteCheckers.Dtos;

public sealed record class WebsiteCheckerDto(
    Guid Id,
    string Url,
    int CheckIntervalInMinutes,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? LastCheckedAtUtc);
