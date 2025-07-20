using AutoMapper;
using Cronometro.API_.Models;
using Cronometro.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Cronometro.API_.Extensions
{
    public class MappingProfileExtensions : Profile
    {
        public MappingProfileExtensions()
        {
            CreateMap<tbProyectosTiempos, ProyectosTiemposViewModel>().ReverseMap();
 
        }
    }
}
