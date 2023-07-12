using AutoMapper;
using Mango.ProductApi.Models;
using Mango.ProductApi.Models.Dto;

namespace Mango.ProductApi
{
    public class MappingConfig
    { 
        //add comments here
        public static MapperConfiguration RegisterMaps() 
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
