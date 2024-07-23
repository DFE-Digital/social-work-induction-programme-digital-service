namespace Dfe.Sww.Ecf.Core.Services.PersonMatching;

public record MatchResult(Guid PersonId, string? Trn, IReadOnlyCollection<KeyValuePair<OneLoginUserMatchedAttribute, string>> MatchedAttributes);
