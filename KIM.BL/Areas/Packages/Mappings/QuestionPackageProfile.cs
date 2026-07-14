using AutoMapper;
using KIM.BL.Areas.Packages.Models;
using KIM.DAL.Entities;

namespace KIM.BL.Areas.Packages.Mappings;

public class QuestionPackageProfile : Profile
{
    public QuestionPackageProfile()
    {
        CreateMap<QuestionPackage, QuestionPackageListItemDto>()
            .ForCtorParam(nameof(QuestionPackageListItemDto.QuestionsCount), options => options.MapFrom(src => src.Questions.Count));

        CreateMap<QuestionPackage, QuestionPackageDetailsDto>();
    }
}