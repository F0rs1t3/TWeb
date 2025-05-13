using AutoMapper;
using BusinessLogic.Models; 

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Car, CarDTO>();
    }
}

