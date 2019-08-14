using AuctionDemo.DAL.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuctionDemo.ViewModels.Mappers
{
    public class AutoMapperConfig : Profile
    {
        public static MapperConfiguration Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LotViewModel, Lot>().ReverseMap()
                .ForMember(dest => dest.LotId, opt => opt.MapFrom(x => x.LotId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(x => x.Description))
                .ForMember(dest => dest.InitialPrice, opt => opt.MapFrom(x => x.InitialPrice))
                .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(x => x.FinalPrice))
                .ForMember(dest => dest.TimeOfLot, opt => opt.MapFrom(x => x.TimeOfLot));

                cfg.CreateMap<BidViewModel, Bid>().ReverseMap()
                .ForMember(dest => dest.BidId, opt => opt.MapFrom(x => x.BidId))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(x => x.Comments))
                .ForMember(dest => dest.BidPrice, opt => opt.MapFrom(x => x.BidPrice))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(x => x.Date));

                cfg.CreateMap<UserViewModel, User>().ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(x => x.Balance))
                .ForMember(dest => dest.FrozenBalance, opt => opt.MapFrom(x => x.FrozenBalance));
            });

            return config;

        }
    }
}