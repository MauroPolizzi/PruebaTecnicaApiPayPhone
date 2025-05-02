using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using PP_Dominio.Entidades;
using PP_Infraestructura;
using PP_Servicios;

namespace PP_Tests
{
    [TestClass]
    public class MovimientoServicioTests
    {
        [TestMethod]
        public async Task Create_ShouldCallCreateWithNewMovimiento()
        {
            // Arrange
            var movimientoInput = new Movimiento
            {
                WalletId = 1,
                Amount = 100,
                Type = TypeOperation.Credito
            };

            var mockRepoMovimiento = new Mock<IRepositorio<Movimiento>>();
            var mockRepoBilletera = new Mock<IRepositorio<Billetera>>();

            // Simulamos el DbSet<Billetera> como una lista con IQueryable y lo conectamos manualmente
            var billeteras = new List<Billetera>
            {
                new Billetera { Id = 1, Name = "Wallet 1", Balance = 500 }
            }.AsQueryable();

            var mockDbSetBilletera = new Mock<DbSet<Billetera>>();
            mockDbSetBilletera.As<IQueryable<Billetera>>().Setup(m => m.Provider).Returns(billeteras.Provider);
            mockDbSetBilletera.As<IQueryable<Billetera>>().Setup(m => m.Expression).Returns(billeteras.Expression);
            mockDbSetBilletera.As<IQueryable<Billetera>>().Setup(m => m.ElementType).Returns(billeteras.ElementType);
            mockDbSetBilletera.As<IQueryable<Billetera>>().Setup(m => m.GetEnumerator()).Returns(billeteras.GetEnumerator());

            var mockDataContext = new Mock<DataContext>();
            mockDataContext.Setup(dc => dc.Billetera).Returns(mockDbSetBilletera.Object);

            var servicio = new MovimientoServicio(mockRepoMovimiento.Object, mockRepoBilletera.Object, mockDataContext.Object);

            // Act
            await servicio.Create(movimientoInput);

            // Assert
            mockRepoMovimiento.Verify(repo => repo.Create(It.Is<Movimiento>(m =>
                m.WalletId == movimientoInput.WalletId &&
                m.Amount == movimientoInput.Amount &&
                m.Type == movimientoInput.Type &&
                m.EstaBorrado == false &&
                m.CreatedAt != default
            )), Times.Once);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnMappedMovimientos()
        {
            // Arrange
            var mockMovimientos = new List<Movimiento>
            {
                new Movimiento
                {
                    Id = 1,
                    WalletId = 10,
                    Amount = 200,
                    Type = TypeOperation.Credito,
                    EstaBorrado = false,
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new Movimiento
                {
                    Id = 2,
                    WalletId = 20,
                    Amount = 300,
                    Type = TypeOperation.Debito,
                    EstaBorrado = true,
                    CreatedAt = DateTime.Now.AddDays(-2)
                }
            };

            var mockRepoMovimiento = new Mock<IRepositorio<Movimiento>>();
            var mockRepoBilletera = new Mock<IRepositorio<Billetera>>();

            // Setup de GetAll en repositorioMovimiento
            mockRepoMovimiento.Setup(repo => repo.GetAll(null, null, true))
                              .ReturnsAsync(mockMovimientos);

            // Simulación del DbSet<Billetera> necesario por el constructor
            var billeteras = new List<Billetera>().AsQueryable();
            var mockDbSet = new Mock<DbSet<Billetera>>();
            mockDbSet.As<IQueryable<Billetera>>().Setup(m => m.Provider).Returns(billeteras.Provider);
            mockDbSet.As<IQueryable<Billetera>>().Setup(m => m.Expression).Returns(billeteras.Expression);
            mockDbSet.As<IQueryable<Billetera>>().Setup(m => m.ElementType).Returns(billeteras.ElementType);
            mockDbSet.As<IQueryable<Billetera>>().Setup(m => m.GetEnumerator()).Returns(billeteras.GetEnumerator());

            var mockDataContext = new Mock<DataContext>();
            mockDataContext.Setup(dc => dc.Billetera).Returns(mockDbSet.Object);

            var servicio = new MovimientoServicio(mockRepoMovimiento.Object, mockRepoBilletera.Object, mockDataContext.Object);

            // Act
            var result = await servicio.GetAll();

            // Assert
            var resultList = result.ToList();
            Assert.AreEqual(2, resultList.Count);

            Assert.AreEqual(1, resultList[0].Id);
            Assert.AreEqual(10, resultList[0].WalletId);
            Assert.AreEqual(200, resultList[0].Amount);
            Assert.AreEqual(TypeOperation.Credito, resultList[0].Type);
            Assert.IsFalse(resultList[0].EstaBorrado);

            Assert.AreEqual(2, resultList[1].Id);
            Assert.AreEqual(20, resultList[1].WalletId);
            Assert.AreEqual(300, resultList[1].Amount);
            Assert.AreEqual(TypeOperation.Debito, resultList[1].Type);
            Assert.IsTrue(resultList[1].EstaBorrado);

            mockRepoMovimiento.Verify(repo => repo.GetAll(null, null, true), Times.Once);
        }
    }
}
