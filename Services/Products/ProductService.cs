using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Products.Requests;
using ECommerceProductsAPI.Dtos.Products.Responses;
using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Models.Enums;
using ECommerceProductsAPI.Repositories.Products;
using ECommerceProductsAPI.Utils.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Services.Products;
public class ProductService(ProductsDataContext context, IProductRepository productRepository) : IProductService
{
    private readonly ProductsDataContext _context = context;
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ServiceResponse<List<ProductResponse>>> GetAllProducts()
    {
        var response = new ServiceResponse<List<ProductResponse>>();
        try
        {
            var products = await _context.Products
                .AsNoTracking()
                .Include(p => p.Variations)
                .ThenInclude(v => v.VariationAttributes)
                    .ThenInclude(va => va.Attribute)
                .Include(p => p.GroupedProductItems)
                    .ThenInclude(gpi => gpi.ProductItem)
                    .Select(p => MapToProductResponse(p))
                .AsQueryable()
                .ToListAsync();

            response.Data = products;
            response.Message = $"Successfully retrieved {products.Count} products.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving products: {ex.Message}";
        }

        return response;
    }


    public async Task<ServiceResponse<PaginatedResponse<ProductResponse>>> GetProductsWithPagination(int pageNumber, int totalItemsPerPage)
    {
        var response = new ServiceResponse<PaginatedResponse<ProductResponse>>();
        try
        {
            if (totalItemsPerPage <= 0 || pageNumber <= 0)
            {
                throw new Exception("Invalid number. Please enter a number greater than 0.");
            }

            // calculate how many pages via product total and size of page
            int totalNumberOfProducts = await _context.Products.CountAsync();
            int totalNumberOfPages = (int)Math.Ceiling((double)totalNumberOfProducts / totalItemsPerPage);

            if (pageNumber > totalNumberOfPages)
            {
                throw new Exception($"Invalid page number. Page {pageNumber} does not exist. There are only {totalNumberOfPages} pages.");
            }

            var products = await _context.Products
                .Include(p => p.Variations)
                    .ThenInclude(v => v.VariationAttributes)
                        .ThenInclude(va => va.Attribute)
                .Include(p => p.GroupedProductItems)
                    .ThenInclude(gpi => gpi.ProductItem)
                .AsNoTracking()
                .Skip((pageNumber - 1) * totalItemsPerPage)
                .Take(totalItemsPerPage)
                .Select(p => MapToProductResponse(p))
                .ToListAsync();


            response.Data = new PaginatedResponse<ProductResponse>
            {
                Items = products,
                CurrentPageNumber = pageNumber,
                TotalPages = totalNumberOfPages,
                ItemsPerPage = totalItemsPerPage,
                TotalItems = totalNumberOfProducts,
            };

            response.Message = $"Successfully retrieved {products.Count} products.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving products: {ex.Message}";
        }

        return response;
    }


    public async Task<ServiceResponse<ProductResponse>> GetProductById(int id)
    {
        var response = new ServiceResponse<ProductResponse>();

        try
        {
            var product = await _productRepository.GetProductDetailsById(id) ?? throw new Exception($"Product with ID '{id}' not found.");

            response.Data = MapToProductResponse(product);
            response.Message = $"Successfully retrieved product with ID '{product.Id}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving product: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<List<ProductResponse>>> GetProductsByFilter(FilterProductsRequest queryFilter)
    {
        var response = new ServiceResponse<List<ProductResponse>>();

        try
        {
            var products = await BuildProductQuery(queryFilter)
                .Select(p => MapToProductResponse(p))
                .ToListAsync();

            response.Data = products;
            response.Message = $"Successfully retrieved {products.Count} products.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving product: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<PaginatedResponse<ProductResponse>>> GetProductsByFilterWithPagination(int pageNumber, int totalItemsPerPage, FilterProductsRequest queryFilter)
    {
        var response = new ServiceResponse<PaginatedResponse<ProductResponse>>();

        try
        {
            if (totalItemsPerPage <= 0 || pageNumber <= 0)
            {
                throw new Exception("Invalid number. Please enter a number greater than 0.");
            }

            int totalNumberOfProducts = await BuildProductQuery(queryFilter).CountAsync();
            int totalNumberOfPages = (int)Math.Ceiling((double)totalNumberOfProducts / totalItemsPerPage);

            if (totalNumberOfProducts == 0 || totalNumberOfPages == 0)
            {
                throw new Exception("No results found.");
            }

            if (pageNumber > totalNumberOfPages)
            {
                throw new Exception($"Invalid page number, There are only {totalNumberOfPages} page(s).");
            }

            var products = await BuildProductQuery(queryFilter)
                .Skip((pageNumber - 1) * totalItemsPerPage)
                .Take(totalItemsPerPage)
                .Select(p => MapToProductResponse(p))
                .ToListAsync();


            response.Data = new PaginatedResponse<ProductResponse>
            {
                Items = products,
                CurrentPageNumber = pageNumber,
                TotalPages = totalNumberOfPages,
                ItemsPerPage = totalItemsPerPage,
                TotalItems = totalNumberOfProducts,
            };

            response.Message = $"Successfully retrieved {totalNumberOfProducts} products.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving product(s): {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<ProductResponse>> AddProduct(AddProductRequest productRequest)
    {
        var response = new ServiceResponse<ProductResponse>();

        try
        {
            ValidateProductType(productRequest.ProductType);

            var product = new Product
            {
                Name = productRequest.Name,
                ShortDescription = productRequest.ShortDescription,
                Description = productRequest.Description,
                Price = productRequest.Price,
                ProductType = productRequest.ProductType,
                IsSubscription = productRequest.IsSubscription
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var addedProduct = await _productRepository.GetProductDetailsById(product.Id) ?? throw new Exception("Failed to retrieve the newly added product.");

            response.Data = MapToProductResponse(addedProduct);
            response.Message = $"Successfully added product with ID '{product.Id}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error adding product: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<ProductResponse>> UpdateProduct(int id, UpdateProductRequest productRequest)
    {
        var response = new ServiceResponse<ProductResponse>();

        try
        {
            ValidateProductType(productRequest.ProductType);

            var product = await _productRepository.GetProductDetailsById(id, asNoTracking: false) ?? throw new Exception($"Product with ID '{id}' not found.");

            if (productRequest.Name != product.Name && !string.IsNullOrWhiteSpace(productRequest.Name))
            {
                product.Name = productRequest.Name;
            }

            if (productRequest.ShortDescription != product.ShortDescription && !string.IsNullOrWhiteSpace(productRequest.ShortDescription))
            {
                product.ShortDescription = productRequest.ShortDescription;
            }

            if (productRequest.Description != product.Description && !string.IsNullOrWhiteSpace(productRequest.Description))
            {
                product.Description = productRequest.Description;
            }

            if (productRequest.Price != product.Price && productRequest.Price > 0)
            {
                product.Price = productRequest.Price;
            }

            if (productRequest.ProductType != product.ProductType && productRequest.ProductType.HasValue)
            {
                product.ProductType = productRequest.ProductType;
            }

            if (productRequest.IsSubscription != product.IsSubscription)
            {
                product.IsSubscription = productRequest.IsSubscription;
            }

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            response.Data = MapToProductResponse(product);
            response.Message = $"Successfully updated product with ID '{id}'.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating product: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<string>> DeleteProduct(int id)
    {
        var response = new ServiceResponse<string>();

        try
        {
            var product = await _productRepository.GetProductDetailsById(id, asNoTracking: false) ?? throw new Exception($"Product with ID '{id}' not found.");

            await RemoveItemsFromGroupedProduct(product, id);
            await RemoveProductItemReferences(id);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            response.Data = string.Empty;
            response.Message = $"Successfully deleted product with ID '{id}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting product: {ex.Message}";
        }
        return response;
    }

    private async Task RemoveItemsFromGroupedProduct(Product? product, int productId)
    {
        // check product has any GroupedProductItems (is product typed 'Grouped')
        // find and remove all GroupedProductItem entries associated with this grouped product
        if (product == null || product.ProductType != ProductType.Grouped)
        {
            return;
        }

        var groupedProductItems = await _context.GroupedProductItems.Where(gpi => gpi.GroupedProductId == productId).ToListAsync();

        if (groupedProductItems.Count > 0)
        {
            _context.GroupedProductItems.RemoveRange(groupedProductItems);
            await _context.SaveChangesAsync();
        }
    }

    private async Task RemoveProductItemReferences(int productId)
    {
        // check if this product is referenced as a ProductItem in other GroupedProductItems
        // and remove all GroupedProductItem entries whereever this product is a ProductItem
        var productItemEntries = await _context.GroupedProductItems.Where(gpi => gpi.ProductItemId == productId).ToListAsync();

        if (productItemEntries.Count > 0)
        {
            _context.GroupedProductItems.RemoveRange(productItemEntries);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ServiceResponse<bool>> IsProductExistsById(int id)
    {
        var response = new ServiceResponse<bool>();

        try
        {
            bool isExists = await _context.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == id);

            string responseMsg = isExists ? $"Product with ID '{id}' does exist." : $"Product with ID '{id}' does not exist.";

            response.Data = isExists;
            response.Message = responseMsg;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    private static void ValidateProductType(ProductType? productType)
    {
        if (productType.HasValue && !Enum.IsDefined(typeof(ProductType), productType))
        {
            throw new ArgumentException($"Invalid ProductType: {productType}");
        }
    }

    private IQueryable<Product> BuildProductQuery(FilterProductsRequest filter)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrEmpty(filter.ProductType) && Enum.TryParse<ProductType>(filter.ProductType, out var productType))
        {
            query = query.Where(p => p.ProductType == productType);
        }

        if (!string.IsNullOrEmpty(filter.AttributeType))
        {
            query = query.Where(p => p.Variations.Any(v => v.VariationAttributes.Any(va => va.Attribute.Type.Contains(filter.AttributeType))));
        }

        if (!string.IsNullOrEmpty(filter.AttributeValue))
        {
            query = query.Where(p => p.Variations.Any(v => v.VariationAttributes.Any(va => va.Attribute.Value.Contains(filter.AttributeValue))));
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        }

        if (filter.IsSubscription.HasValue)
        {
            query = query.Where(s => s.IsSubscription == filter.IsSubscription);
        }

        if (!string.IsNullOrEmpty(filter.ProductName))
        {
            // case insensitive collation
            // not using .ToLower() as this transforms every (entire table) row's product name into lower case
            // this prevents the database to use the ProductName column's indexes that were created from the original values (before lowercase transformation)
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{filter.ProductName}%"));
        }

        query = query.Include(p => p.Variations)
                    .ThenInclude(v => v.VariationAttributes)
                        .ThenInclude(va => va.Attribute)
                .Include(p => p.GroupedProductItems)
                    .ThenInclude(gpi => gpi.ProductItem);

        return query;
    }

    private static ProductResponse MapToProductResponse(Product product) => ProductMapper.MapToProductResponse(product);
}