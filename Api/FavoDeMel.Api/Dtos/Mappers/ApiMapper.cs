using AutoMapper;

namespace FavoDeMel.Api.Dtos.Mappers
{
    public static class ApiMapper
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg => GetConfiguration(cfg));
            Mapper.AssertConfigurationIsValid();
        }

        private static void GetConfiguration(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<ProdutoMapProfile>();
            cfg.AddProfile<UsuarioMapProfile>();
            cfg.AddProfile<ComandaMapProfile>();
        }
    }
}
