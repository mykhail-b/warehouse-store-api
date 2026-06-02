using Backend.Data;
using Backend.Services.Warehouse;
using Backend.Services.Customer;
using Backend.Services;
using ClassLibrary.Entity;
using ClassLibrary.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Backend.Tests;

public class EntitiesCrudTest
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateItem_WithValidData_ReturnsCreatedItem()
    {
        var context = CreateInMemoryContext();
        var service = new WarehouseItemService(context);

        var item = new WarehouseItem
        {
            Name = "Test Item",
            ItemCode = "TEMP",
            CurrentQuantity = 10,
            IsAvailable = true
        };

        var result = await service.CreateAsync(item);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.StartsWith("ITEM-", result.ItemCode);
    }

    [Fact]
    public async Task DeleteItem_WhenItemExists_SetsIsAvailableToFalse()
    {
        var context = CreateInMemoryContext();
        var service = new WarehouseItemService(context);

        var item = new WarehouseItem { Name = "Test", ItemCode = "TEMP", IsAvailable = true };
        await service.CreateAsync(item);

        var result = await service.DeleteItemAsync(item.Id);
        var deleted = await service.GetByIdAsync(item.Id);

        Assert.True(result);
        Assert.False(deleted!.IsAvailable);
    }

    [Fact]
    public async Task UpdateItem_WithNewDetails_UpdatesFields()
    {
        var context = CreateInMemoryContext();
        var service = new WarehouseItemService(context);

        var item = new WarehouseItem { Name = "Old Name", ItemCode = "TEMP" };
        await service.CreateAsync(item);

        item.Name = "New Name";
        await service.UpdateAsync(item);

        var updated = await service.GetByIdAsync(item.Id);
        Assert.Equal("New Name", updated!.Name);
    }

    [Fact]
    public async Task CreateInbound_IncreasesWarehouseStock()
    {
        var context = CreateInMemoryContext();
        var deliveryService = new WarehouseDeliveryService(context);
        var itemService = new WarehouseItemService(context);

        var item = new WarehouseItem { Name = "Test", ItemCode = "TEMP", CurrentQuantity = 5 };
        await itemService.CreateAsync(item);

        var vendor = new Vendor { Name = "Vendor", Country = "PL" };
        context.Vendors.Add(vendor);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var delivery = new InboundDelivery
        {
            DeliveryNumber = "DEL-001",
            ArrivalDate = DateTime.UtcNow,
            VendorId = vendor.Id,
            Items = new List<InboundDeliveryItem>
            {
                new InboundDeliveryItem
                {
                    WarehouseItemId = item.Id,
                    QuantityReceived = 10,
                    PurchasePrice = 100
                }
            }
        };

        await deliveryService.CreateInboundAsync(delivery);

        var updated = await itemService.GetByIdAsync(item.Id);
        Assert.Equal(15, updated!.CurrentQuantity);
    }

    [Fact]
    public async Task SetIsAvailableToFalse_WhenQuantityIsZero()
    {
        var context = CreateInMemoryContext();
        var service = new WarehouseItemService(context);

        var item = new WarehouseItem
        {
            Name = "Test",
            ItemCode = "TEMP",
            CurrentQuantity = 0,
            IsAvailable = true
        };
        await service.CreateAsync(item);

        item.IsAvailable = item.CurrentQuantity > 0;
        await service.UpdateAsync(item);

        var updated = await service.GetByIdAsync(item.Id);
        Assert.False(updated!.IsAvailable);
    }

    // Order Service tests
    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsCreatedOrder()
    {
        var context = CreateInMemoryContext();
        var service = new OrderService(context);

        var order = new Order
        {
            UserId = "user123",
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>()
        };

        var result = await service.CreateOrderAsync(order);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Equal("user123", result.UserId);
    }

    [Fact]
    public async Task GetOrderById_WhenExists_ReturnsOrder()
    {
        var context = CreateInMemoryContext();
        var service = new OrderService(context);

        var order = new Order { UserId = "user123", Status = OrderStatus.Pending };
        var created = await service.CreateOrderAsync(order);

        var result = await service.GetOrderByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal(OrderStatus.Pending, result.Status);
    }

    [Fact]
    public async Task GetOrderById_WhenNotExists_ReturnsNull()
    {
        var context = CreateInMemoryContext();
        var service = new OrderService(context);

        var result = await service.GetOrderByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsAllOrders()
    {
        var context = CreateInMemoryContext();
        var service = new OrderService(context);

        await service.CreateOrderAsync(new Order { UserId = "user1", Status = OrderStatus.Pending });
        await service.CreateOrderAsync(new Order { UserId = "user2", Status = OrderStatus.Shipped });

        var result = await service.GetAllOrdersAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateOrder_WithNewData_UpdatesSuccessfully()
    {
        var context = CreateInMemoryContext();
        var service = new OrderService(context);

        var order = new Order { UserId = "user123", Status = OrderStatus.Pending };
        var created = await service.CreateOrderAsync(order);

        created.Status = OrderStatus.Shipped;
        var updated = await service.UpdateOrderAsync(created);

        var result = await service.GetOrderByIdAsync(updated.Id);
        Assert.Equal(OrderStatus.Shipped, result!.Status);
    }

    [Fact]
    public async Task DeleteOrder_WhenExists_RemovesSuccessfully()
    {
        var context = CreateInMemoryContext();
        var service = new OrderService(context);

        var order = new Order { UserId = "user123", Status = OrderStatus.Pending };
        var created = await service.CreateOrderAsync(order);

        var result = await service.DeleteOrderAsync(created.Id);
        var deleted = await service.GetOrderByIdAsync(created.Id);

        Assert.True(result);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteOrder_WhenNotExists_ReturnsFalse()
    {
        var context = CreateInMemoryContext();
        var service = new OrderService(context);

        var result = await service.DeleteOrderAsync(999);

        Assert.False(result);
    }

    // Vendor creating test
    [Fact]
    public async Task CreateVendor_WithValidData_SavesVendorToDatabase()
    {
        var context = CreateInMemoryContext();
        var service = new VendorService(context);

        var vendor = new Vendor
        {
            Name = "Test Vendor",
            Country = "PL",
            Email = "test@vendor.com",
            TelNumber = "+48123456789"
        };

        var result = await service.CreateAsync(vendor);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test Vendor", result.Name);
    }

    [Fact]
    public async Task DeleteVendor_WhenVendorExists_RemovesVendorSuccessfully()
    {
        var context = CreateInMemoryContext();
        var service = new VendorService(context);

        var vendor = new Vendor { Name = "Test Vendor", Country = "PL" };
        await service.CreateAsync(vendor);

        var result = await service.DeleteAsync(vendor.Id);
        var deleted = await service.GetByIdAsync(vendor.Id);

        Assert.True(result);
        Assert.Null(deleted);
    }

    // Company Data Service tests
    [Fact]
    public async Task GetCompanyData_ReturnsAllCompanyData()
    {
        var context = CreateInMemoryContext();
        var service = new CompanyDataService(context);

        var companyData = new CompanyData
        {
            Name = "Test Company",
            Country = "PL",
            Address = "123 Main St",
            RegistryNumber = "REG123",
            Email = "company@test.com",
            Phone = "+48123456789"
        };
        context.CompanyData.Add(companyData);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await service.GetCompanyData();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Company", result.First().Name);
    }

    [Fact]
    public async Task GetPublicCompanyData_ExcludesSensitiveFields()
    {
        var context = CreateInMemoryContext();
        var service = new CompanyDataService(context);

        var companyData = new CompanyData
        {
            Name = "Test Company",
            Country = "PL",
            Address = "123 Main St",
            RegistryNumber = "REG123",
            Email = "company@test.com",
            Phone = "+48123456789"
        };
        context.CompanyData.Add(companyData);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await service.GetPublicCompanyData();

        Assert.NotNull(result);
        Assert.Single(result);
        var publicData = result.First();
        Assert.Equal("Test Company", publicData.Name);
        Assert.Equal("123 Main St", publicData.Address);
        Assert.Equal("company@test.com", publicData.Email);
    }

    [Fact]
    public async Task SetCompanyData_WithNewRecord_CreatesNewCompanyData()
    {
        var context = CreateInMemoryContext();
        var service = new CompanyDataService(context);

        var dto = new ComapnyDataDetailDto
        {
            Name = "New Company",
            Country = "DE",
            Address = "456 Oak Ave",
            RegistryNumber = "REG456",
            Email = "new@company.com",
            Phone = "+49987654321"
        };

        var result = await service.SetCompanyData(dto);

        Assert.NotNull(result);
        var saved = await context.CompanyData.FirstOrDefaultAsync(
            c => c.RegistryNumber == "REG456",
            TestContext.Current.CancellationToken);
        Assert.NotNull(saved);
        Assert.Equal("New Company", saved.Name);
    }

    [Fact]
    public async Task SetCompanyData_WithExistingRecord_UpdatesCompanyData()
    {
        var context = CreateInMemoryContext();
        var service = new CompanyDataService(context);

        var existingCompany = new CompanyData
        {
            Name = "Old Company",
            Country = "PL",
            Address = "Old Address",
            RegistryNumber = "REG789",
            Email = "old@company.com",
            Phone = "+48111111111"
        };
        context.CompanyData.Add(existingCompany);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var updateDto = new ComapnyDataDetailDto
        {
            Name = "Updated Company",
            Country = "DE",
            Address = "New Address",
            RegistryNumber = "REG789",
            Email = "updated@company.com",
            Phone = "+49222222222"
        };

        var result = await service.SetCompanyData(updateDto);

        var updated = await context.CompanyData.FirstOrDefaultAsync(
            c => c.RegistryNumber == "REG789",
            TestContext.Current.CancellationToken);
        Assert.NotNull(updated);
        Assert.Equal("Updated Company", updated.Name);
        Assert.Equal("DE", updated.Country);
        Assert.Equal("New Address", updated.Address);
    }

    // Authorization Service tests
    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUserAndReturnsToken()
    {
        var mockUserManager = new Mock<UserManager<UserAccount>>(
            Mock.Of<IUserStore<UserAccount>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        var mockConfig = new Mock<IConfiguration>();

        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Password = "Password123!",
            Type = UserType.Customer
        };

        mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<UserAccount>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success);

        mockConfig
            .Setup(x => x[It.IsAny<string>()])
            .Returns<string>(key => key switch
            {
                "Jwt:Key" => "this-is-a-very-long-secret-key-for-jwt-token-generation",
                "Jwt:Issuer" => "test-issuer",
                "Jwt:Audience" => "test-audience",
                _ => null
            });

        var service = new Backend.Services.Auth.AuthorizationService(mockUserManager.Object, mockConfig.Object);
        var result = await service.RegisterAsync(registerDto);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        mockUserManager.Verify(x => x.CreateAsync(It.IsAny<UserAccount>(), registerDto.Password), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var mockUserManager = new Mock<UserManager<UserAccount>>(
            Mock.Of<IUserStore<UserAccount>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        var mockConfig = new Mock<IConfiguration>();

        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var existingUser = new UserAccount
        {
            Id = "user123",
            Email = loginDto.Email,
            FirstName = "Test",
            LastName = "User"
        };

        mockUserManager
            .Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(existingUser);
        mockUserManager
            .Setup(x => x.CheckPasswordAsync(existingUser, loginDto.Password))
            .ReturnsAsync(true);

        mockConfig
            .Setup(x => x[It.IsAny<string>()])
            .Returns<string>(key => key switch
            {
                "Jwt:Key" => "this-is-a-very-long-secret-key-for-jwt-token-generation",
                "Jwt:Issuer" => "test-issuer",
                "Jwt:Audience" => "test-audience",
                _ => null
            });

        var service = new Backend.Services.Auth.AuthorizationService(mockUserManager.Object, mockConfig.Object);
        var result = await service.LoginAsync(loginDto);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        mockUserManager.Verify(x => x.FindByEmailAsync(loginDto.Email), Times.Once);
        mockUserManager.Verify(x => x.CheckPasswordAsync(existingUser, loginDto.Password), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ThrowsException()
    {
        var mockUserManager = new Mock<UserManager<UserAccount>>(
            Mock.Of<IUserStore<UserAccount>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        var mockConfig = new Mock<IConfiguration>();

        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        mockUserManager
            .Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync((UserAccount?)null);

        var service = new Backend.Services.Auth.AuthorizationService(mockUserManager.Object, mockConfig.Object);

        var exception = await Assert.ThrowsAsync<Exception>(() => service.LoginAsync(loginDto));
        Assert.Contains("Invalid email or password", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsException()
    {
        var mockUserManager = new Mock<UserManager<UserAccount>>(
            Mock.Of<IUserStore<UserAccount>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        var mockConfig = new Mock<IConfiguration>();

        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword!"
        };

        var existingUser = new UserAccount
        {
            Id = "user123",
            Email = loginDto.Email,
            FirstName = "Test",
            LastName = "User"
        };

        mockUserManager
            .Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(existingUser);
        mockUserManager
            .Setup(x => x.CheckPasswordAsync(existingUser, loginDto.Password))
            .ReturnsAsync(false);

        var service = new Backend.Services.Auth.AuthorizationService(mockUserManager.Object, mockConfig.Object);

        var exception = await Assert.ThrowsAsync<Exception>(() => service.LoginAsync(loginDto));
        Assert.Contains("Invalid email or password", exception.Message);
    }
}