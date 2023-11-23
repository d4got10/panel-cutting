namespace PanelCutting.API.Dtos.Extensions;

public static class PointDtoExtensions
{
    public static Point ToDomainObject(this PointDto dto) => new(dto.X, dto.Y);
}