using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using PP_Dominio.Entidades;
using PP_Infraestructura;
using PP_Servicios;

namespace PP_Tests
{
    [TestClass]
    public class BilleteraServicioTests
    {
        [TestMethod]
        public async Task Create_ShouldCallRepositorioCreate_WithCorrectBilletera()
        {
            // Arrange
            var mockRepositorio = new Mock<IRepositorio<Billetera>>();
            var mockDataContext = new Mock<DataContext>();

            var service = new BilleteraServicio(mockRepositorio.Object, mockDataContext.Object);

            var input = new Billetera
            {
                DocumentId = "123456",
                Name = "Test Wallet",
                Balance = 1000
            };

            // Act
            await service.Create(input);

            // Assert
            mockRepositorio.Verify(r => r.Create(It.Is<Billetera>(b =>
                b.DocumentId == input.DocumentId &&
                b.Name == input.Name &&
                b.Balance == input.Balance &&
                b.EstaBorrado == false &&
                b.CreatedAt != default &&
                b.UpdatedAt != default
            )), Times.Once);
        }

        [TestMethod]
        public async Task Update_ShouldModifyFields_AndCallRepositorioUpdate()
        {
            // Arrange
            var billeteraExistente = new Billetera
            {
                Id = 1,
                DocumentId = "123456",
                Name = "Wallet",
                Balance = 500,
                UpdatedAt = DateTime.MinValue
            };

            var billeteras = new List<Billetera> { billeteraExistente }.AsQueryable();

            var mockSet = new Mock<DbSet<Billetera>>();
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.Provider).Returns(billeteras.Provider);
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.Expression).Returns(billeteras.Expression);
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.ElementType).Returns(billeteras.ElementType);
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.GetEnumerator()).Returns(billeteras.GetEnumerator());

            var mockDataContext = new Mock<DataContext>();
            mockDataContext.Setup(dc => dc.Billetera).Returns(mockSet.Object);

            var mockRepositorio = new Mock<IRepositorio<Billetera>>();

            var servicio = new BilleteraServicio(mockRepositorio.Object, mockDataContext.Object);

            var input = new Billetera
            {
                Balance = 999
            };

            // Act
            await servicio.Update(1, input);

            // Assert
            mockRepositorio.Verify(r => r.Update(It.Is<Billetera>(b =>
                b.Id == 1 &&
                b.Balance == 999 &&
                b.UpdatedAt != DateTime.MinValue
            )), Times.Once);
        }

        [TestMethod]
        public async Task UpdateWalletForMovimiento_ShouldAddAmount_WhenTypeIsCredito()
        {
            // Arrange
            var billetera = new Billetera
            {
                Id = 1,
                Balance = 100
            };

            var movimiento = new Movimiento
            {
                WalletId = 1,
                Amount = 50,
                Type = TypeOperation.Credito
            };

            var billeteras = new List<Billetera> { billetera }.AsQueryable();

            var mockSet = new Mock<DbSet<Billetera>>();
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.Provider).Returns(billeteras.Provider);
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.Expression).Returns(billeteras.Expression);
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.ElementType).Returns(billeteras.ElementType);
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.GetEnumerator()).Returns(billeteras.GetEnumerator());

            var mockDataContext = new Mock<DataContext>();
            mockDataContext.Setup(dc => dc.Billetera).Returns(mockSet.Object);

            var mockRepositorio = new Mock<IRepositorio<Billetera>>();

            var servicio = new BilleteraServicio(mockRepositorio.Object, mockDataContext.Object);

            // Act
            await servicio.UpdateWalletForMovimiento(movimiento);

            // Assert
            mockRepositorio.Verify(r => r.Update(It.Is<Billetera>(b =>
                b.Id == 1 &&
                b.Balance == 150 &&
                b.UpdatedAt != default
            )), Times.Once);
        }

        [TestMethod]
        public async Task Delete_ShouldCallRepositorioDelete_WithCorrectBilletera()
        {
            // Arrange
            var billeteraExistente = new Billetera
            {
                Id = 1,
                DocumentId = "123456",
                Name = "Wallet",
                Balance = 500
            };

            var billeteras = new List<Billetera> { billeteraExistente }.AsQueryable();

            var mockSet = new Mock<DbSet<Billetera>>();
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.Provider).Returns(billeteras.Provider);
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.Expression).Returns(billeteras.Expression);
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.ElementType).Returns(billeteras.ElementType);
            mockSet.As<IQueryable<Billetera>>().Setup(m => m.GetEnumerator()).Returns(billeteras.GetEnumerator());

            var mockDataContext = new Mock<DataContext>();
            mockDataContext.Setup(dc => dc.Billetera).Returns(mockSet.Object);

            var mockRepositorio = new Mock<IRepositorio<Billetera>>();

            var servicio = new BilleteraServicio(mockRepositorio.Object, mockDataContext.Object);

            // Act
            await servicio.Delete(1);

            // Assert
            mockRepositorio.Verify(r => r.Delete(It.Is<Billetera>(b => b.Id == 1)), Times.Once);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnProjectedBilleteras()
        {
            // Arrange
            var billeterasRepo = new List<Billetera>
            {
                new Billetera
                {
                    Id = 1,
                    EstaBorrado = false,
                    DocumentId = "123",
                    Name = "Wallet 1",
                    Balance = 100,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow
                },
                new Billetera
                {
                    Id = 2,
                    EstaBorrado = true,
                    DocumentId = "456",
                    Name = "Wallet 2",
                    Balance = 200,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            var mockRepositorio = new Mock<IRepositorio<Billetera>>();
            mockRepositorio
                .Setup(r => r.GetAll(
                    It.IsAny<Func<IQueryable<Billetera>, IOrderedQueryable<Billetera>>>(),
                    It.IsAny<Func<IQueryable<Billetera>, IIncludableQueryable<Billetera, object>>>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(billeterasRepo);

            var mockDataContext = new Mock<DataContext>();

            var servicio = new BilleteraServicio(mockRepositorio.Object, mockDataContext.Object);

            // Act
            var resultado = await servicio.GetAll();

            // Assert
            Assert.IsNotNull(resultado);
            var lista = resultado.ToList();
            Assert.AreEqual(2, lista.Count);

            Assert.AreEqual("123", lista[0].DocumentId);
            Assert.AreEqual("Wallet 1", lista[0].Name);
            Assert.AreEqual(100, lista[0].Balance);
            Assert.AreEqual(false, lista[0].EstaBorrado);

            Assert.AreEqual("456", lista[1].DocumentId);
            Assert.AreEqual("Wallet 2", lista[1].Name);
            Assert.AreEqual(200, lista[1].Balance);
            Assert.AreEqual(true, lista[1].EstaBorrado);

            // Verifica que se llamó al repositorio
            mockRepositorio.Verify(r => r.GetAll(It.IsAny<Func<IQueryable<Billetera>, IOrderedQueryable<Billetera>>>(),
                                                 It.IsAny<Func<IQueryable<Billetera>, IIncludableQueryable<Billetera, object>>>(),
                                                 It.IsAny<bool>()),
                                   Times.Once);
        }

    }
}
