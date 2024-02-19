namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;

public class UpdateBlockNameResponse
{
    public UpdateBlockNameResponse(string oldName, string newName, DateTime updatedDate)
    {
        OldName = oldName;
        NewName = newName;
        UpdatedDate = updatedDate;
    }
    public string OldName { get; private set; }
    public string NewName { get; private set; }
    public DateTime UpdatedDate { get; private set; }
}
