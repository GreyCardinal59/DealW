using AutoMapper;
using DealW.Domain.Models;
using DealW.Persistence.Entities;

namespace DealW.Persistence.Mappings;

public class DataBaseMappings : Profile
{
    public DataBaseMappings()
    {
        //TODO add another mappings
        CreateMap<UserEntity, User>();
    }
}