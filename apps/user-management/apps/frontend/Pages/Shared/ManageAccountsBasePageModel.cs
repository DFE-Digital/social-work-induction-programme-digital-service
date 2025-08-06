using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.Shared;

public class ManageAccountsBasePageModel : BasePageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? OrganisationId { get; set; }
}
