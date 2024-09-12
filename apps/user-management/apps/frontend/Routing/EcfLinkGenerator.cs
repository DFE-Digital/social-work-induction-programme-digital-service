namespace Dfe.Sww.Ecf.Frontend.Routing;

public abstract class EcfLinkGenerator
{
    public string SignIn() => GetRequiredPathByPage("/SignIn");

    public string SignOut() => GetRequiredPathByPage("/SignOut");

    public string Home() => GetRequiredPathByPage("/Index");

    public string ManageAccounts() => GetRequiredPathByPage("/ManageAccounts/Index");

    public string AddAccountDetails() => GetRequiredPathByPage("/ManageAccounts/AddAccountDetails");

    public string AddAccountDetailsChange() =>
        GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change");

    public string EditAccountDetails(Guid id) =>
        GetRequiredPathByPage("/ManageAccounts/EditAccountDetails", routeValues: new { id });

    public string EditAccountDetailsChange(Guid id) =>
        GetRequiredPathByPage(
            "/ManageAccounts/EditAccountDetails",
            handler: "Change",
            routeValues: new { id }
        );

    public string ConfirmAccountDetails() =>
        GetRequiredPathByPage("/ManageAccounts/ConfirmAccountDetails");

    public string ConfirmAccountDetailsUpdate(Guid id) =>
        GetRequiredPathByPage(
            "/ManageAccounts/ConfirmAccountDetails",
            handler: "Update",
            routeValues: new { id }
        );

    public string ViewAccountDetails(Guid id) =>
        GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", routeValues: new { id });

    public string SelectAccountType() => GetRequiredPathByPage("/ManageAccounts/SelectAccountType");

    public string EditAccountType(Guid id) =>
        GetRequiredPathByPage(
            "/ManageAccounts/SelectAccountType",
            handler: "Edit",
            routeValues: new { id }
        );

    public string AddSomeoneNew() =>
        GetRequiredPathByPage("/ManageAccounts/SelectAccountType", handler: "New");

    public string SelectUseCase() => GetRequiredPathByPage("/ManageAccounts/SelectUseCase");

    public string EditUseCase(Guid id) =>
        GetRequiredPathByPage(
            "/ManageAccounts/SelectUseCase",
            handler: "Edit",
            routeValues: new { id }
        );

    public string UnlinkAccount(Guid id) =>
        GetRequiredPathByPage("/ManageAccounts/UnlinkAccount", routeValues: new { id });

    public string AddExistingUser() => GetRequiredPathByPage("/ManageAccounts/AddExistingUser");

    protected abstract string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null
    );
}

public class RoutingEcfLinkGenerator(LinkGenerator linkGenerator) : EcfLinkGenerator
{
    protected override string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null
    ) =>
        linkGenerator.GetPathByPage(page, handler, values: routeValues)
        ?? throw new InvalidOperationException("Page was not found.");
}
