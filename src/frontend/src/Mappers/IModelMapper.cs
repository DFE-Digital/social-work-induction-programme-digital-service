namespace SocialWorkInductionProgramme.Frontend.Mappers;

public interface IModelMapper<TDto, TBo>
{
    public TBo MapToBo(TDto dto);

    public TDto MapFromBo(TBo bo);
}
