using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Products.Requests;
using ECommerceProductsAPI.Dtos.Products.Responses;
using ECommerceProductsAPI.Dtos.Products.Requests.Variations;
using ECommerceProductsAPI.Services.Caching;
using ECommerceProductsAPI.Services.Products;
using ECommerceProductsAPI.Services.Products.GroupedProducts;
using ECommerceProductsAPI.Services.Products.VariableProducts;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerceProductsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService, IProductVariationService productVariationService, IGroupedProductService groupedProductService, IRedisCacheService cache) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly IProductVariationService _productVariationService = productVariationService;
    private readonly IGroupedProductService _groupedProductService = groupedProductService;
    private readonly IRedisCacheService _cache = cache;
    const string allProductsCacheKey = "products";
    const string productKeyPrefix = "product_";
    const string productPageKeyPrefix = "products_page_";
    private const int maxCachedPages = 3;

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<ProductResponse>>>> GetAllProducts()
    {
        var response = _cache.GetData<ServiceResponse<List<ProductResponse>>>(allProductsCacheKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _productService.GetAllProducts();

        if (response.Success)
        {
            _cache.SetData(allProductsCacheKey, response);
        }

        return Ok(response);
    }

    [HttpGet("page={pageNumber}")]
    public async Task<ActionResult<ServiceResponse<PaginatedResponse<ProductResponse>>>> GetAllProductsWithPagination(int pageNumber, int totalItemsPerPage)
    {
        string cachingKey = $"{productPageKeyPrefix}{pageNumber}";

        var response = _cache.GetData<ServiceResponse<PaginatedResponse<ProductResponse>>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _productService.GetProductsWithPagination(pageNumber, totalItemsPerPage);

        if (response.Success)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<ProductResponse>>> GetProductById(int id)
    {
        string cachingKey = $"{productKeyPrefix}{id}";
        var response = _cache.GetData<ServiceResponse<ProductResponse>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _productService.GetProductById(id);

        if (response.Success && response.Data != null)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }

    [HttpPost("search")]
    public async Task<ActionResult<ServiceResponse<List<ProductResponse>>>> GetProductsByFilter(FilterProductsRequest queryFilter)
    {
        var cachingKey = JsonSerializer.Serialize(queryFilter);
        var response = _cache.GetData<ServiceResponse<List<ProductResponse>>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _productService.GetProductsByFilter(queryFilter);

        if (response.Success)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }

    [HttpPost("search/page={pageNumber}")]
    public async Task<ActionResult<ServiceResponse<PaginatedResponse<ProductResponse>>>> GetProductsByFilterWithPagination(int pageNumber, int totalItemsPerPage, FilterProductsRequest queryFilter)
    {
        string serialisedRequest = JsonSerializer.Serialize(queryFilter);
        string cachingKey = $"users_search_{pageNumber}_{serialisedRequest}";

        var response = _cache.GetData<ServiceResponse<PaginatedResponse<ProductResponse>>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _productService.GetProductsByFilterWithPagination(pageNumber, totalItemsPerPage, queryFilter);

        if (response.Success)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }

    [HttpGet("exists/{id}")]
    public async Task<ActionResult<ServiceResponse<bool>>> IsProductExistsById(int id)
    {
        var cachingKey = $"{productKeyPrefix}{id}_exists";
        var response = _cache.GetData<ServiceResponse<bool>>(cachingKey);
        
        if (response is not null)
        {
            return Ok(response);
        }

        response = await _productService.IsProductExistsById(id);

        if (response.Success)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<ProductResponse>>> AddProduct([FromBody] AddProductRequest productRequest)
    {
        var response = await _productService.AddProduct(productRequest);

        // clear and set cache
        if (response.Success && response.Data != null)
        {
            string cachingKey = $"{productKeyPrefix}{response.Data.Id}";
            _cache.SetData(cachingKey, response);

            InvalidateProductCache(response.Data.Id);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPut]
    public async Task<ActionResult<ServiceResponse<ProductResponse>>> UpdateProduct(int id, [FromBody] UpdateProductRequest productRequest)
    {
        var response = await _productService.UpdateProduct(id, productRequest);

        if (response.Success && response.Data != null)
        {
            _cache.SetData($"{productKeyPrefix}{response.Data.Id}", response);

            InvalidateProductCache(response.Data.Id);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<string>>> DeleteProduct(int id)
    {
        var response = await _productService.DeleteProduct(id);

        // clear necessary caches
        if (response.Success && response.Data != null)
        {
            InvalidateProductCache(id);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }



    [HttpPost("variation")]
    public async Task<ActionResult<ServiceResponse<ProductResponse>>> AddProductVariation([FromBody] AddVariationRequest variationRequest)
    {
        var response = await _productVariationService.AddProductVariation(variationRequest);

        if (response.Success && response.Data != null)
        {
            _cache.SetData($"{productKeyPrefix}{variationRequest.ProductId}", response);

            InvalidateProductCache(variationRequest.ProductId);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPut("variation")]
    public async Task<ActionResult<ServiceResponse<ProductResponse>>> UpdateProductVariation([FromBody] UpdateVariationRequest variationRequest)
    {
        var response = await _productVariationService.UpdateProductVariation(variationRequest);

        if (response.Success && response.Data != null)
        {
            _cache.SetData($"{productKeyPrefix}{variationRequest.ProductId}", response);

            InvalidateProductCache(variationRequest.ProductId);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("variation")]
    public async Task<ActionResult<ServiceResponse<ProductResponse>>> DeleteVariationByProductAndVariationId(int productId, int variationId)
    {
        var response = await _productVariationService.DeleteVariationByProductAndVariationId(productId, variationId);

        // clear necessary caches
        if (response.Success)
        {
            InvalidateProductCache(productId);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("unused-attributes")]
    public async Task<ActionResult<ServiceResponse<string>>> CleanUpProductAttributes()
    {
        var response = await _productVariationService.CleanUpProductAttributes();

        return response.Success ? Ok(response) : NotFound(response);
    }



    [HttpPost("grouped")]
    public async Task<ActionResult<ServiceResponse<ProductResponse>>> AddProductToGroupedProduct(GroupedProductRequest request)
    {
        var response = await _groupedProductService.AddProductToGroupedProduct(request);

        if (response.Success && response.Data != null)
        {
            _cache.SetData($"{productKeyPrefix}{request.GroupedProductId}", response);

            InvalidateProductCache(request.GroupedProductId);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPut("grouped")]
    public async Task<ActionResult<ServiceResponse<ProductResponse>>> UpdateProductQuantityInGroupedProduct(GroupedProductRequest request)
    {
        var response = await _groupedProductService.UpdateProductQuantityInGroupedProduct(request);

        if (response.Success && response.Data != null)
        {
            _cache.SetData($"{productKeyPrefix}{request.GroupedProductId}", response);

            InvalidateProductCache(request.GroupedProductId);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("grouped")]
    public async Task<ActionResult<ServiceResponse<ProductResponse>>> DeleteProductFromGroupedProduct(int groupedProductId, int productId)
    {
        var response = await _groupedProductService.DeleteProductFromGroupedProduct(groupedProductId, productId);

        if (response.Success && response.Data != null)
        {
            _cache.SetData($"{productKeyPrefix}{groupedProductId}", response);

            InvalidateProductCache(groupedProductId);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }


    private void InvalidateProductCache(int productId)
    {
        _cache.RemoveData($"{productKeyPrefix}{productId}");
        _cache.RemoveData(allProductsCacheKey);

        for (int i = 1; i <= maxCachedPages; i++)
        {
            string cachingKey = $"{productPageKeyPrefix}{i}";
            _cache.RemoveData(cachingKey);
        }
    }
}