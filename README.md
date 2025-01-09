# E-Commerce Products API

This RESTful API is designed to manage users, addresses, products (across multiple types), and subscriptions for e-commerce platforms.

### Technologies Used: 
- **ASP.NET 8 Web API**
- **Entity Framework Core** (**Code-First Migrations**)
- **SQL Server** with **Microsoft SQL Server Management Studio**
- **Redis Caching**. 

The architecture includes **Dependency Injection**, **DTOs**, **mappers**, and **controllers**.


### Supported Product Types:
- **Simple Products**:  Items sold without requiring attribute selection (e.g., size or colour).
- **Variable**: Items with variations (e.g., color, size, material), each with unique price and stock.
- **Grouped**: Pre-defined sets of two or more individual products sold together (bundles).

*Note: This system currently assumes all products are physical goods.*

---

## Features

### User Management

#### User Service:
Handles CRUD operations for users and secure password management.

- `GetAllUsers()`: Retrieve all users.
- `GetUserById(int id)`: Get a user by its ID.
- `UpdateUser(int id, UserRequest userRequest)`: Update basic user details i,e., email, first name, last name, phone number.
- `UpdatePassword(int id, UpdatePasswordRequest passwordRequest)`: Update a user password.
- `DeleteUser(int id)`: Delete a user.

#### User Register Service:
Handles user registration and login functionality.

- `Register(UserRegister userRequest)`: : Register a new user, with unique email.
- `Login(UserLogin userLogin)`: Basic authentication for a user.

#### Password Service:
Secure password handling and validation. There are no endpoints directly associated with this service, it is injected in other services.

- `IsStrongPassword(string password)`: Validate password strength against custom regex.
- `CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)`: Hashes password.
- `VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)`: Verify a password against its hash.

#### Address Service:
CRUD operations for addresses linked to users.

- `GetAddressesByUserId(int userId)`: Retrieve addresses for a specific user.
- `AddAddress(int userId, AddressRequest addressRequest)`: Add a new address for a user.
- `UpdateAddress(int userId, int addressId, AddressRequest addressRequest)`: Update an existing address.
- `DeleteAddress(int userId, int addressId)`: Remove an address for a user.


#### Subscription Service:
Handles subscriptions related to subscription-enabled products.

- `GetSubscriptionsByUserId(int userId)`: Retrieve all subscriptions for a user.
- `AddUserSubscription(SubscriptionRequest subscriptionRequest, int userId)`: Create a new subscription for a user.
- `UpdateUserSubscription(int subscriptionId, SubscriptionRequest updatedRequest)`: Update existing subscription.
- `DeleteUserSubscription(int userId, int subscriptionId)`: Cancel a user subscription.
- `DeleteAllSubscriptionsForUser(int userId)`: Cancel all subscriptions for a user.

--- 

### Product Management

#### Product Service:
CRUD operations, filtering, and validation.

- `GetAllProducts()`: Retrieve all products.
- `GetProductById(int id)`: Fetch a product by its ID.
- `GetProductsByFilter(FilterProductsRequest queryFilter)`: Retrieve products based on price range, product variations, product type and product name.
- `AddProduct(ProductRequest productRequest)`: Add a new product.
- `UpdateProduct(int id, ProductRequest updatedProduct)`: Update an existing product's name, descriptions, price, product type.
- `DeleteProduct(int id)`: Remove a product.
- `IsProductExistsById(int id)`: Validate if a product exists.

#### Product Variation Service:
Handles creation, updates, and deletion of product variations.

- `CreateProductVariation(int productId, ProductVariation variation, List<ProductAttribute> attributes)`: Add a variation to a product.
- `UpdateProductVariation(int productId, ProductVariation variation, List<ProductAttribute> attributes)`: Update an existing product variation.
- `DeleteVariationByProductAndVariationId(int productId, int variationId)`: Remove a variation from a product.
- `Task<ServiceResponse<string>> CleanUpProductAttributes()`: Removes unused attributes from `ProductAttribute` table i.e., attributes not used by any product variation.

#### Grouped Product Service:
Manages grouped products by adding and deletion of products in a bundle.

- `AddProductToGroupedProduct(int groupedProductId, int productId, int quantity)`: Add a product to a grouped product.
- `UpdateProductQuantityInGroupedProduct(int groupedProductId, int productId, int newQuantity)`: Update quantity for the product within grouped product.
- `DeleteProductFromGroupedProduct(int groupedProductId, int productId)`: Remove a product from a grouped product.

#### Redis Cache Service:
Provide caching of frequently accessed data.

- `GetData<T>(string key)`: : Retrieve cached data.
- `SetData<T>(string key, T data)`: Store data in cache.
- `RemoveData(string key)`: Clear specific cache entries by key.

---

### Database Schema

Below is an overview of the key entities and their relationships:

#### Entities
- **User**: Represents a customer.
- **Address**: Stores address details for users (supports multiple addresses per user).
- **UserSubscription**: Links users to their active subscriptions.
- **Subscription**: Represents a recurring product subscription as long as the Product offers subscriptions.
- **Product**: A single item available for sale.
- **ProductVariationAttribute**: Associates specific attributes with product variations.
- **ProductVariation**: Defines variations of a product. Each variation links to a parent product via ProductId and includes specific attributes like SKU, Price, and Quantity.
- **ProductAttribute**: Represents characteristics that can be assigned to product variations, such as colour, size, or material. Attributes have properties like Id, Type, and Value.
- **GroupedProductItem**: Manages the relationship between a grouped product and its products that exist within it. This entity includes GroupedProductId, ProductId, and Quantity, facilitating the creation of product bundles.

#### Relationships
- A **User** can have multiple **Addresses**. 
- Each **User** can subscribe to multiple **Subscriptions** through **UserSubscription**. 
- A **Product** can have multiple **ProductVariations**, each **ProductVariation can be associated with multiple **ProductAttributes** through **ProductVariationAttribute**.
- A **Product**, of type `Grouped`, can have multiple **GroupedProductItem** which can consist of products of type `Simple` or `Variable`; allows the creation of product bundles.

--- 

## Run project
```
dotnet run
```

**Test API URL:** http://localhost:5081/swagger/index.html


## Potential Future Updates to Project

#### Minor Updates:
- Offer product variation to grouped product.
- Subscriptions of type "cancelled" should have a "NextBillingDate" set to NULL.
- Subscriptions of type "paused" should have a different "NextBillingDate" from what it is currently (setting it to "PreviousBillingDate").
- Task scheduler for billing date for subscriptions; set "NextBillingDate" when date surpasses Date.UtcNow.
- Email if subscription is no longer available e.g., if product got deleted.
- Delete unused attributes in delete product by ID and delete variation endpoints.
- Offer published/draft options to products.

#### Major Updates:
1. **Authentication & Security**: Implement JWT/oAuth, role-based authorization, and 2FA.
2. **Expanded Database Schema**: Add shopping cart functionality and simulate billing, shipping, stock management processes.
3. **Enhanced Subscription**: Handle recurring charges on subscription products.
4. **Extend Product Type Support**: E.g., virtual/downloadable products.
5. **Import/export products from CSV**