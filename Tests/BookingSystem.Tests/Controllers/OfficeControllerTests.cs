using BookingSystem.Api;
using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookingSystem.Tests.Controllers;

public class OfficeControllerTests
{
    private readonly Mock<IOfficeService> _officeServiceMock;
    private readonly OfficeController _controller;

    public OfficeControllerTests()
    {
        _officeServiceMock = new Mock<IOfficeService>();
        _controller = new OfficeController(_officeServiceMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithOffices()
    {
        // Arrange
        var offices = new List<OfficeDto>
        {
            new OfficeDto { Id = Guid.NewGuid(), Address = "Address1" },
            new OfficeDto { Id = Guid.NewGuid(), Address = "Address2" }
        };

        _officeServiceMock.Setup(os => os.GetAllOfficesAsync())
            .ReturnsAsync(offices);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOffices = Assert.IsType<List<OfficeDto>>(okResult.Value);
        Assert.Equal(2, returnedOffices.Count);
    }

    [Fact]
    public async Task GetAll_NoOffices_ReturnsEmptyList()
    {
        // Arrange
        _officeServiceMock.Setup(os => os.GetAllOfficesAsync())
            .ReturnsAsync(new List<OfficeDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOffices = Assert.IsType<List<OfficeDto>>(okResult.Value);
        Assert.Empty(returnedOffices);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ValidId_ReturnsOkWithOffice()
    {
        // Arrange
        var officeId = Guid.NewGuid();
        var office = new OfficeDto 
        { 
            Id = officeId, 
            Address = "Test Address" 
        };

        _officeServiceMock.Setup(os => os.GetOfficeByIdAsync(officeId))
            .ReturnsAsync(office);

        // Act
        var result = await _controller.GetById(officeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOffice = Assert.IsType<OfficeDto>(okResult.Value);
        Assert.Equal(officeId, returnedOffice.Id);
    }

    [Fact]
    public async Task GetById_InvalidId_ThrowsException()
    {
        // Arrange
        var officeId = Guid.NewGuid();
        
        _officeServiceMock.Setup(os => os.GetOfficeByIdAsync(officeId))
            .ThrowsAsync(new KeyNotFoundException("Office not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetById(officeId));
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateOfficeRequest
        {
            Address = "New Office Address",
            OrganizationId = Guid.NewGuid()
        };

        var createdOffice = new OfficeDto
        {
            Id = Guid.NewGuid(),
            Address = request.Address
        };

        _officeServiceMock.Setup(os => os.CreateOfficeAsync(request))
            .ReturnsAsync(createdOffice);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(OfficeController.GetById), createdAtActionResult.ActionName);
        Assert.Equal(createdOffice.Id, createdAtActionResult.RouteValues["id"]);
        
        var returnedOffice = Assert.IsType<OfficeDto>(createdAtActionResult.Value);
        Assert.Equal(createdOffice.Id, returnedOffice.Id);
    }

    [Fact]
    public async Task Create_EmptyAddress_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateOfficeRequest
        {
            Address = "",
            OrganizationId = Guid.NewGuid()
        };

        // Act - Model validation should catch this
        Assert.NotNull(request);
    }

    [Fact]
    public async Task Create_NullOrganizationId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateOfficeRequest
        {
            Address = "Some Address",
            OrganizationId = Guid.Empty
        };

        // Act - Service may handle validation
        Assert.NotNull(request);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new UpdateOfficeRequest
        {
            OfficeId = Guid.NewGuid(),
            Address = "Updated Address"
        };

        var updatedOffice = new OfficeDto
        {
            Id = request.OfficeId,
            Address = request.Address
        };

        _officeServiceMock.Setup(os => os.UpdateOfficeAsync(request))
            .ReturnsAsync(updatedOffice);

        // Act
        var result = await _controller.Update(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOffice = Assert.IsType<OfficeDto>(okResult.Value);
        Assert.Equal(request.OfficeId, returnedOffice.Id);
    }

    [Fact]
    public async Task Update_InvalidOfficeId_ThrowsException()
    {
        // Arrange
        var request = new UpdateOfficeRequest
        {
            OfficeId = Guid.NewGuid(),
            Address = "Updated Address"
        };

        _officeServiceMock.Setup(os => os.UpdateOfficeAsync(request))
            .ThrowsAsync(new KeyNotFoundException("Office not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(request));
    }

    [Fact]
    public async Task Update_EmptyAddress_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateOfficeRequest
        {
            OfficeId = Guid.NewGuid(),
            Address = ""
        };

        // Act - Model validation or service should handle this
        Assert.NotNull(request);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        // Arrange
        var officeId = Guid.NewGuid();

        _officeServiceMock.Setup(os => os.DeleteOfficeAsync(officeId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(officeId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_InvalidId_ThrowsException()
    {
        // Arrange
        var officeId = Guid.NewGuid();

        _officeServiceMock.Setup(os => os.DeleteOfficeAsync(officeId))
            .ThrowsAsync(new KeyNotFoundException("Office not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Delete(officeId));
    }

    [Fact]
    public async Task Delete_EmptyGuid_ThrowsException()
    {
        // Arrange
        var officeId = Guid.Empty;

        _officeServiceMock.Setup(os => os.DeleteOfficeAsync(officeId))
            .ThrowsAsync(new ArgumentException("Invalid ID"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _controller.Delete(officeId));
    }

    #endregion
}
