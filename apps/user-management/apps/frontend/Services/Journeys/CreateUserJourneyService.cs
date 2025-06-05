using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class CreateUserJourneyService(
    IHttpContextAccessor httpContextAccessor,
    INotificationServiceClient notificationServiceClient,
    IOptions<EmailTemplateOptions> emailTemplateOptions,
    IAuthServiceClient authServiceClient,
    IUserService userService,
    IMoodleServiceClient moodleServiceClient,
    EcfLinkGenerator linkGenerator
) : ICreateUserJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private const string CreateUserSessionKey = "_createUser";

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private CreateUserJourneyModel GetCreateUserJourneyModel()
    {
        Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );
        return createUserJourneyModel ?? new CreateUserJourneyModel();
    }

    public IList<UserType>? GetUserTypes()
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        return createUserJourneyModel.UserTypes;
    }

    public UserDetails? GetUserDetails()
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        return createUserJourneyModel.UserDetails;
    }

    public bool? GetIsStaff()
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        return createUserJourneyModel.IsStaff;
    }

    public void SetUserDetails(UserDetails userDetails)
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        createUserJourneyModel.UserDetails = userDetails;
        SetCreateUserJourneyModel(createUserJourneyModel);
    }

    public void SetUserTypes(IList<UserType> userTypes)
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        createUserJourneyModel.UserTypes = userTypes.ToImmutableList();
        SetCreateUserJourneyModel(createUserJourneyModel);
    }

    public void SetIsStaff(bool? isStaff)
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        createUserJourneyModel.IsStaff = isStaff;
        SetCreateUserJourneyModel(createUserJourneyModel);
    }

    public void SetExternalUserId(int? externalUserId)
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        createUserJourneyModel.ExternalUserId = externalUserId;
        SetCreateUserJourneyModel(createUserJourneyModel);
    }

    public bool? GetIsRegisteredWithSocialWorkEngland()
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        return createUserJourneyModel.IsRegisteredWithSocialWorkEngland;
    }

    public void SetIsRegisteredWithSocialWorkEngland(bool? isRegisteredWithSocialWorkEngland)
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        createUserJourneyModel.IsRegisteredWithSocialWorkEngland = isRegisteredWithSocialWorkEngland;
        SetCreateUserJourneyModel(createUserJourneyModel);
    }

    public bool? GetIsStatutoryWorker()
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        return createUserJourneyModel.IsStatutoryWorker;
    }

    public void SetIsStatutoryWorker(bool? isStatutoryWorker)
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        createUserJourneyModel.IsStatutoryWorker = isStatutoryWorker;
        SetCreateUserJourneyModel(createUserJourneyModel);
    }

    public bool? GetIsAgencyWorker()
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        return createUserJourneyModel.IsAgencyWorker;
    }

    public void SetIsAgencyWorker(bool? isAgencyWorker)
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        createUserJourneyModel.IsAgencyWorker = isAgencyWorker;
        SetCreateUserJourneyModel(createUserJourneyModel);
    }

    public bool? GetIsQualifiedWithin3Years()
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        return createUserJourneyModel.IsQualifiedWithin3Years;
    }

    public void SetIsQualifiedWithin3Years(bool? isQualifiedWithin3Years)
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        createUserJourneyModel.IsQualifiedWithin3Years = isQualifiedWithin3Years;
        SetCreateUserJourneyModel(createUserJourneyModel);
    }

    public void ResetCreateUserJourneyModel()
    {
        Session.Remove(CreateUserSessionKey);
    }

    private void SetCreateUserJourneyModel(CreateUserJourneyModel createUserJourneyModel)
    {
        Session.Set(CreateUserSessionKey, createUserJourneyModel);
    }

    public async Task<User> CompleteJourneyAsync()
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();

        var user = createUserJourneyModel.ToUser();

        user = await userService.CreateAsync(user);

        await SendInvitationEmailAsync(user);

        ResetCreateUserJourneyModel();

        return user;
    }

    public void SetSocialWorkerDetails(SocialWorker socialWorkerDetails)
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        createUserJourneyModel.SocialWorkerDetails = socialWorkerDetails;
        SetCreateUserJourneyModel(createUserJourneyModel);
    }

    public SocialWorker? GetSocialWorkerDetails()
    {
        var createUserJourneyModel = GetCreateUserJourneyModel();
        return createUserJourneyModel.SocialWorkerDetails;
    }

    private async Task SendInvitationEmailAsync(User user)
    {
        var userTypes = GetUserTypes();

        if (
            userTypes is null
            || string.IsNullOrWhiteSpace(user.Email)
            || _httpContextAccessor.HttpContext is null
        )
        {
            return;
        }

        var linkingToken = await authServiceClient.Users.GetLinkingTokenByAccountIdAsync(
            user.Id
        );

        var invitationLink = linkGenerator.SignInWithLinkingToken(
            _httpContextAccessor.HttpContext,
            linkingToken
        );

        // Get the highest ranking role - the lowest (int)enum
        var invitationEmailType = userTypes.Min();

        var templateId = emailTemplateOptions
            .Value
            .Roles[invitationEmailType.ToString()]
            .Invitation;
        var notificationRequest = new NotificationRequest
        {
            EmailAddress = user.Email,
            TemplateId = templateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", user.FullName },
                { "organisation", "TEST ORGANISATION" }, // TODO Retrieve this value when we can
                { "invitation_link", invitationLink }
            }
        };

        await notificationServiceClient.Notification.SendEmailAsync(notificationRequest);
    }
}
