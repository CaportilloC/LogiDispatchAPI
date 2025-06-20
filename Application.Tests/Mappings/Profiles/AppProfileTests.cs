using Application.Mappings.Profiles;
using AutoMapper;

namespace Application.Tests.Mappings.Profiles
{
    [TestFixture]
    public class AppProfileTests
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ApplicationProfile>();
            });
            _mapper = config.CreateMapper();
        }

        [Test]
        public void Should_Map_CargaGenerada_To_CargaGeneradaDto()
        {
        }

    }
}
