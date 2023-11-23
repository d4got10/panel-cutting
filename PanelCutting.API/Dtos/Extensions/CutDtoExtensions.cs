namespace PanelCutting.API.Dtos.Extensions;

public static class CutDtoExtensions
{
    public static CutDto ToDto(this PanelCut dto) => new()
    {
        Length = dto.Length
    };
}