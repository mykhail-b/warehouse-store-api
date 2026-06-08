using Backend.Data;
using Backend.Services.Warehouse;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests;

public class CrudProductTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductService _service;

    public CrudProductTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new ProductService(_context);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_WithValidItem_ReturnsItemWithId()
    {
        var item = MakeItem("Laptop");

        var result = await _service.CreateAsync(item);

        Assert.True(result.Id > 0);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task CreateAsync_GeneratesItemCode()
    {
        var item = MakeItem("Laptop");

        var result = await _service.CreateAsync(item);

        Assert.Contains("ITEM-", result.ItemCode);
        Assert.Contains(result.Id.ToString(), result.ItemCode);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAt()
    {
        var item = MakeItem("Laptop");

        var result = await _service.CreateAsync(item);

        Assert.True(result.CreatedAt > DateTime.MinValue);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_ExistingItem_ReturnsItem()
    {
        var item = MakeItem("Laptop");
        await _service.CreateAsync(item);

        var result = await _service.GetByIdAsync(item.Id);

        Assert.NotNull(result);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingItem_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        await _service.CreateAsync(MakeItem("Laptop"));
        await _service.CreateAsync(MakeItem("Mouse"));
        await _service.CreateAsync(MakeItem("Keyboard"));

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_WithValidItem_UpdatesFields()
    {
        var item = MakeItem("Laptop");
        await _service.CreateAsync(item);

        item.Name = "Gaming Laptop";
        item.Cost = 1999.99m;

        var result = await _service.UpdateAsync(item);

        Assert.Equal("Gaming Laptop", result.Name);
        Assert.Equal(1999.99m, result.Cost);
    }

    [Fact]
    public async Task UpdateAsync_SetsChangedAt()
    {
        var item = MakeItem("Laptop");
        await _service.CreateAsync(item);

        var result = await _service.UpdateAsync(item);

        Assert.True(result.ChangedAt > DateTime.MinValue);
    }

    // --- DeleteItemAsync (Soft Delete) ---

    [Fact]
    public async Task DeleteItemAsync_ExistingItem_ReturnsTrue()
    {
        var item = MakeItem("Laptop");
        await _service.CreateAsync(item);

        var result = await _service.DeleteItemAsync(item.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteItemAsync_SetsIsAvailableToFalse()
    {
        var item = MakeItem("Laptop");
        await _service.CreateAsync(item);

        await _service.DeleteItemAsync(item.Id);

        var deleted = await _service.GetByIdAsync(item.Id);
        Assert.False(deleted!.IsAvailable);
    }

    [Fact]
    public async Task DeleteItemAsync_NonExistingItem_ReturnsFalse()
    {
        var result = await _service.DeleteItemAsync(999);

        Assert.False(result);
    }

    // --- GetSummaryListAsync ---

    [Fact]
    public async Task GetSummaryListAsync_ReturnsCorrectPage()
    {
        for (int i = 1; i <= 10; i++)
            await _service.CreateAsync(MakeItem($"Item {i}"));

        var result = await _service.GetSummaryListAsync(page: 1, pageSize: 5);

        Assert.Equal(5, result.Count());
    }

    [Fact]
    public async Task GetSummaryListAsync_SecondPage_ReturnsNextItems()
    {
        for (int i = 1; i <= 10; i++)
            await _service.CreateAsync(MakeItem($"Item {i}"));

        var page1 = (await _service.GetSummaryListAsync(1, 5)).Select(i => i.Id).ToList();
        var page2 = (await _service.GetSummaryListAsync(2, 5)).Select(i => i.Id).ToList();

        Assert.Empty(page1.Intersect(page2));
    }

    // --- GetDetailByIdAsync ---

    [Fact]
    public async Task GetDetailByIdAsync_ExistingItem_ReturnsFullDetails()
    {
        var item = MakeItem("Laptop");
        item.Description = "Gaming laptop";
        item.Cost = 1299.99m;
        await _service.CreateAsync(item);

        var result = await _service.GetDetailByIdAsync(item.Id);

        Assert.NotNull(result);
        Assert.Equal("Laptop", result.Name);
        Assert.Equal("Gaming laptop", result.Description);
        Assert.Equal(1299.99m, result.Cost);
    }

    [Fact]
    public async Task GetDetailByIdAsync_NonExistingItem_ReturnsNull()
    {
        var result = await _service.GetDetailByIdAsync(999);

        Assert.Null(result);
    }

    // --- Helpers ---

    private static Product MakeItem(string name) => new()
    {
        Name = name,
        ItemCode = "TEMP",
        CurrentQuantity = 10,
        MinQuantity = 1,
        MaxQuantity = 100,
        IsAvailable = true,
        Cost = 999.99m,
        Currency = CurrencyType.EUR
    };

    public void Dispose() => _context.Dispose();
}