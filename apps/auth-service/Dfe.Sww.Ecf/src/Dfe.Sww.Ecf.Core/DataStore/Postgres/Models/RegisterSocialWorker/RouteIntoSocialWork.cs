using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.RegisterSocialWorker;

public enum RouteIntoSocialWork
{
    [Display(Name = "Other")] Other,

    [Display(Name = "Step Up to Social Work")] StepUpToSocialWork,

    [Display(Name = "Approach Social Work")] ApproachSocialWork,

    [Display(Name = "Degree apprenticeship in social work")] DegreeApprenticeship,

    [Display(Name = "Undergraduate degree in social work")] UndergraduateApprenticeship,

    [Display(Name = "Postgraduate degree in social work")] PostgraduateApprenticeship,
}
