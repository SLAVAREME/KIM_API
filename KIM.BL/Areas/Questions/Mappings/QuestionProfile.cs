using AutoMapper;
using KIM.BL.Areas.Questions.Models;
using KIM.DAL.Entities;

namespace KIM.BL.Areas.Questions.Mappings;

public class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<Question, QuestionDto>();
        CreateMap<Question, QuestionLookupDto>();
    }
}