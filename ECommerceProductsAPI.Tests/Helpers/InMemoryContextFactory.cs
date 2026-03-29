using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ECommerceProductsAPI.Data;

namespace ECommerceProductsAPI.Tests.Helpers;


/// <summary>
/// Provides an in-memory <see cref="ProductsDataContext"/> for integration tests.
/// Disposes the underlying SQLite connection automatically.
/// </summary>
public sealed class InMemoryContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;
    public ProductsDataContext Context { get; }

    public InMemoryContextFactory()
    {
        // Keep the SQLite connection open for the lifetime of the context
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ProductsDataContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new ProductsDataContext(options);

        // Ensure database schema is created
        Context.Database.EnsureCreated();
    }

    /// <summary>
    /// Deletes and recreates the database, useful for starting tests with a clean state.
    /// </summary>
    public void ResetDatabase()
    {
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}