﻿using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface IOrganisationService
{
    public Task<PaginationResult<Organisation>> GetAllAsync(PaginationRequest request);
    public Organisation GetByLocalAuthorityCode(int? localAuthorityCode);
}
