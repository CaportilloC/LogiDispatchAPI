using Domain.Procedures;
using Domain.Procedures.Common;

namespace Domain.Tests.Procedures
{
    [TestFixture]
    public class InstanceByUserAtProcedureTests
    {
        [Test]
        public void InstanceByUserAtProcedureObject_HasValidValues()
        {
            //Arrange
            var id = 1;
            var name = "Test Name";
            var description = "Test Description";
            var registrationDate = DateTime.Now;
            var createAt = DateTime.Now;
            var userAt = "test@navisaf.com";
            var updateUser = "test@navisaf.com";
            var updatedAt = DateTime.Now;

            //Act
            var entity = new InstanceByUserAtProcedure
            {
                Id = 1,
                Name = name,
                Description = description,
                RegistrationDate = registrationDate,
                CreationAt = createAt,
                UserAt = userAt,
                UpdatedAt = updatedAt,
                UpdateUser = updateUser,
            };

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.EqualTo(id));
                Assert.That(entity.Name, Is.EqualTo(name));
                Assert.That(entity.Description, Is.EqualTo(description));
                Assert.That(entity.RegistrationDate, Is.EqualTo(registrationDate));
                Assert.That(entity.CreationAt, Is.EqualTo(createAt));
                Assert.That(entity.UserAt, Is.EqualTo(userAt));
                Assert.That(entity.UpdatedAt, Is.EqualTo(updatedAt));
                Assert.That(entity.UpdateUser, Is.EqualTo(updateUser));
            });
        }

        [Test]
        public void InstanceByUserAtProcedureObject_HasAValidHierarchy()
        {
            //Arrange
            var type = typeof(InstanceByUserAtProcedure);

            //Act
            var baseType = type.BaseType;

            Assert.Multiple(() =>
            {
                //Assert
                Assert.That(baseType, Is.Not.EqualTo(null));
                Assert.That(baseType, Is.EqualTo(typeof(BaseProcedureResponse)));
            });
        }
    }
}
