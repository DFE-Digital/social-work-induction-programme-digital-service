namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public class AsyeSocialWorkerService : IAsyeSocialWorkerService
{
    public bool Exists(string socialWorkerId)
    {
        // TODO implement this properly
        if (socialWorkerId.Equals("SW8378", StringComparison.InvariantCultureIgnoreCase)
            || socialWorkerId.Equals("SW2793", StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
