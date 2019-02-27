using System;
using System.Linq;
using AutoMapper;
using DatingApp.Api.DTOS;
using DatingApp.Api.Models;

namespace DatingApp.Api.Helpers {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles() {
            CreateMap<User, UserForListDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                });

            CreateMap<User, UserForDetailsDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                });

            CreateMap<UserForUpdateDTO, User>();
            CreateMap<PhotoForCreationDTO, Photo>(); 
            CreateMap<Photo, PhotoForReturnDTO>(); 
        }
    }
}