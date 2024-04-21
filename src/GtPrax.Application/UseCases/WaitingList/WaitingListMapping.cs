namespace GtPrax.Application.UseCases.WaitingList;

internal static class WaitingListMapping
{
    public static WaitingListDto MapToDto(this Domain.Models.WaitingListItem item) =>
        new(item.Name);
}
