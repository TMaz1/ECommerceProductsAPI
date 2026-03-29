# E-Commerce Products API

This RESTful API is designed to manage users, addresses, products (across multiple types), and subscriptions for e-commerce platforms.

### Technologies
- **ASP.NET 8 Web API**
- **Entity Framework Core** (Code-First)
- **SQL Server** for production database
- **Redis** caching
- **Testing**: xUnit, in-memory SQLite, minimal Moq for peripheral dependencies

### Architecture

Built with **Dependency Injection** and structured around **DTOs**, **custom mappers**, **models**, **services**, **repositories**, and **controllers** to ensure maintainable, testable service logic.

---

### Supported Product Types
- **Simple Products**:  Items sold without requiring attribute selection (e.g., size or colour).
- **Variable**: Items with variations (e.g., color, size, material), each with unique price and stock.
- **Grouped**: Pre-defined sets of two or more individual products sold together (bundles).

> Note: All products are assumed to be physical goods.

---

## Features

### User Management

#### User Service
Handles CRUD operations for users and secure password management.

- `GetAllUsers()`: Retrieve all users.
- `GetUserById(int id)`: Get a user by its ID.
- `UpdateUser(int id, UserRequest userRequest)`: Update basic user details i.e., email, first name, last name, phone number.
- `UpdatePassword(int id, UpdatePasswordRequest passwordRequest)`: Update a user password.
- `DeleteUser(int id)`: Delete a user.

#### User Register Service
Handles registration and login. No real auth flow; used for managing user-product relationships.

- `Register(UserRegister userRequest)`: Adds a new user with email and password validation.
- `Login(UserLogin userLogin)`: Basic email/password check (no sessions or tokens).

#### Password Service
Secure password handling and validation. There are no endpoints directly associated with this service, it is injected in other services.

- `IsStrongPassword(string password)`: Validate password strength against custom regex.
- `CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)`: Hashes password.
- `VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)`: Verify a password against its hash.

#### Address Service
CRUD operations for addresses linked to users.

- `GetAddressesByUserId(int userId)`: Retrieve addresses for a specific user.
- `AddAddress(int userId, AddressRequest addressRequest)`: Add a new address for a user.
- `UpdateAddress(int userId, int addressId, AddressRequest addressRequest)`: Update an existing address.
- `DeleteAddress(int userId, int addressId)`: Remove an address for a user.


#### Subscription Service
Handles subscriptions related to subscription-enabled products.

- `GetSubscriptionsByUserId(int userId)`: Retrieve all subscriptions for a user.
- `AddUserSubscription(AddSubscriptionRequest subscriptionRequest)`: Create a new subscription for a user.
- `UpdateUserSubscription(UpdateSubscriptionRequest updatedRequest)`: Update existing subscription.
- `DeleteUserSubscription(int userId, int subscriptionId)`: Cancel a user subscription.
- `DeleteAllSubscriptionsForUser(int userId)`: Cancel all subscriptions for a user.

--- 

### Product Management

#### Product Service
CRUD operations, filtering, and validation.

- `GetAllProducts()`: Retrieve all products.
- `GetProductById(int id)`: Fetch a product by its ID.
- `GetProductsByFilter(FilterProductsRequest queryFilter)`: Retrieve products based on price range, product variations, product type and product name.
- `AddProduct(ProductRequest productRequest)`: Add a new product.
- `UpdateProduct(int id, ProductRequest updatedProduct)`: Update an existing product's name, descriptions, price, product type.
- `DeleteProduct(int id)`: Remove a product.
- `IsProductExistsById(int id)`: Validate if a product exists.

#### Product Variation Service
Handles creation, updates, and deletion of product variations.

- `CreateProductVariation(int productId, ProductVariation variation, List<ProductAttribute> attributes)`: Add a variation to a product.
- `UpdateProductVariation(int productId, ProductVariation variation, List<ProductAttribute> attributes)`: Update an existing product variation.
- `DeleteVariationByProductAndVariationId(int productId, int variationId)`: Remove a variation from a product.
- `Task<ServiceResponse<string>> CleanUpProductAttributes()`: Removes unused attributes from `ProductAttribute` table i.e., attributes not used by any product variation.

#### Grouped Product Service
Manages grouped products, including adding, updating, and removing products within a bundle.

- `AddProductToGroupedProduct(int groupedProductId, int productId, int quantity)`: Add a product to a grouped product.
- `UpdateProductQuantityInGroupedProduct(int groupedProductId, int productId, int newQuantity)`: Update quantity for the product within grouped product.
- `DeleteProductFromGroupedProduct(int groupedProductId, int productId)`: Remove a product from a grouped product.

#### Redis Cache Service
Provide caching of frequently accessed data, not all endpoints.

- `GetData<T>(string key)`: Retrieve cached data.
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
- A **Product** can have multiple **ProductVariations**, each **ProductVariation** can be associated with multiple **ProductAttributes** through **ProductVariationAttribute**.
- A **Product**, of type `Grouped`, can have multiple **GroupedProductItem** which can consist of products of type `Simple` or `Variable`; allows the creation of product bundles.

---

## Testing

- **Integration-first**: in-memory SQLite validates real service and repository behavior.
- **Tech stack**: xUnit, minimal Moq for peripheral dependencies.
- **Helpers**: builders and factories streamline domain object setup.
- **Current coverage**: `GetAllUsers`, `GetAllProducts`.

---

## Running the Project

#### 1. Run Tests

```bash
dotnet test
```

#### 2. Run the API

```bash
dotnet run --project ECommerceProductsAPI
```

Access Swagger UI: http://localhost:5081/swagger/index.html


---

## Potential Future Updates to Project

#### Minor Updates
- Support for product variations within grouped products.
- Option to set products as published or draft.

#### Major Updates
- Enhanced subscription handling, including recurring charges and improved date management (covering paused/cancelled subscriptions).
- Extended product type support, e.g., virtual or downloadable products.
- Import/export products via CSV.
- Bulk product management features.

> Note: These are potential directions for future development and may evolve based on project priorities.