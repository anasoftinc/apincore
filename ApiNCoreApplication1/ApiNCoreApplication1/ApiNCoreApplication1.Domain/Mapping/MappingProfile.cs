using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ApiNCoreApplication1.Entity;

namespace ApiNCoreApplication1.Domain.Mapping
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Create automap mapping profiles
        /// </summary>
        public MappingProfile()
        {
            CreateMap<AccountViewModel, Account>();
            CreateMap<Account, AccountViewModel>();
            CreateMap<UserViewModel, User>()
                .ForMember(dest => dest.DecryptedPassword, opts => opts.MapFrom(src => src.Password))
                .ForMember(dest => dest.Roles, opts => opts.MapFrom(src => string.Join(";", src.Roles)));
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.Password, opts => opts.MapFrom(src => src.DecryptedPassword))
                .ForMember(dest => dest.Roles, opts => opts.MapFrom(src => src.Roles.Split(";", StringSplitOptions.RemoveEmptyEntries)));


            //CreateMap<AnswerSetViewModel, AnswerSet>()
            //    .ForMember(dest => dest.Date, opts => opts.MapFrom(src => src.DateAnswersSubmitted))
            //    .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.AnswerSetId))
            //    .ForMember(dest => dest.Answers, opts => opts.MapFrom(src => src.AnswerSetAnswers))
            //    .ForMember(dest => dest.IsActive, opts => opts.Ignore());
            //CreateMap<AnswerSet, AnswerSetViewModel>()
            //    .ForMember(dest => dest.DateAnswersSubmitted, opts => opts.MapFrom(src => src.Date))
            //    .ForMember(dest => dest.AnswerSetId, opts => opts.MapFrom(src => src.Id))
            //    .ForMember(dest => dest.AnswerSetAnswers, opts => opts.MapFrom(src => src.Answers));

            CreateMissingTypeMaps = true;
        }

    }





}
