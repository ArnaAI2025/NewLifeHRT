
# xUnit Testing Guide for NewLifeHRT.Application.Services

This guide provides instructions and best practices for writing xUnit tests for services within the `NewLifeHRT.Application.Services` project. It is based on the analysis of the existing testing implementation in `NewLifeHRT.Application.Services.Tests`, with a specific focus on the `UserServiceTests` class.

## 1. Testing Philosophy

The primary goal of our testing strategy is to ensure the correctness and reliability of our application services. We aim for clear, concise, and maintainable tests that are easy to understand and contribute to. We follow the "Arrange-Act-Assert" (AAA) pattern for structuring our tests.

## 2. Testing Stack

Our testing stack consists of the following libraries:

*   **xUnit:** The core testing framework.
*   **Moq:** A powerful and flexible mocking library for creating test doubles.
*   **FluentAssertions:** Provides a rich set of extension methods for asserting conditions in a more readable and fluent way.
*   **AutoFixture:** A library for generating test data, which helps to reduce the amount of boilerplate code in our tests.

These dependencies are managed in the `NewLifeHRT.Tests.Common` project, which is referenced by all test projects.

## 3. Naming Conventions

Test methods should be named using one of the following conventions, which are designed to be highly readable and expressive.

### Convention 1: `<MethodUnderTest>_Should_<ExpectedOutcome>`

This convention is used for tests where the condition is implied or simple.

*   **MethodUnderTest:** The name of the method being tested.
*   **ExpectedOutcome:** The expected result of the method under test.

**Example from `UserServiceTests.cs`:**

```csharp
[Fact]
public async Task CreateAsync_Should_Create_UserSuccessfully()
{
    // Arrange
    var createUserRequestDto = new CreateUserRequestDto
    {
        UserName = "testuser",
        Email = "testuser@example.com",
        Password = "Password123!",
        FirstName = "Test",
        LastName = "User",
        PhoneNumber = "1234567890",
        RoleIds = new List<int> { 1 }
    };

    var userRepositoryMock = new Mock<IUserRepository>();
    userRepositoryMock.Setup(repo => repo.ExistAsync(It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(false);

    var userManagerMock = MockProvider.GetUserManagerMock();
    userManagerMock.Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>()))
        .ReturnsAsync(IdentityResult.Success);

    var passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
    passwordHasherMock.Setup(hasher => hasher.HashPassword(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
        .Returns("hashedpassword");

    var userService = new UserServiceBuilder()
        .SetParameter(userRepositoryMock)
        .SetParameter(userManagerMock)
        .SetParameter(passwordHasherMock)
        .Build();

    // Act
    var result = await userService.CreateAsync(createUserRequestDto, 1);

    // Assert
    result.Should().NotBeNull();
    result.Message.Should().Be("User created successfully");
    userRepositoryMock.Verify(repo => repo.ExistAsync(createUserRequestDto.UserName, createUserRequestDto.Email), Times.Once);
    userManagerMock.Verify(manager => manager.CreateAsync(It.IsAny<ApplicationUser>()), Times.Once);
}
```

### Convention 2: `<MethodUnderTest>_Should_<ExpectedOutcome>_When_<Condition>`

This is a more descriptive convention used when a specific condition is being tested.

*   **MethodUnderTest:** The name of the method being tested.
*   **ExpectedOutcome:** The expected result of the method under test.
*   **Condition:** The specific condition under which the expected outcome occurs.

**Examples from `UserServiceTests.cs`:**

```csharp
[Fact]
public async Task GetByIdAsync_Should_ReturnUser_When_UserExists()
{
    // Arrange
    var user = _userBuilder.WithId(1).Build();

    var userRepositoryMock = new Mock<IUserRepository>();
    userRepositoryMock.Setup(repo => repo.GetWithIncludeAsync(1, It.IsAny<string[]>()))
        .ReturnsAsync(user);

    var userService = new UserServiceBuilder()
       .SetParameter(userRepositoryMock)
       .Build();

    // Act
    var result = await userService.GetByIdAsync(1);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(1);
}

[Fact]
public async Task GetByIdAsync_Should_ReturnNull_When_UserDoesNotExist()
{
    // Arrange
    var userRepositoryMock = new Mock<IUserRepository>();
    userRepositoryMock.Setup(repo => repo.GetWithIncludeAsync(1, It.IsAny<string[]>()))
        .ReturnsAsync((ApplicationUser)null);

    var userService = new UserServiceBuilder()
       .SetParameter(userRepositoryMock)
       .Build();

    // Act
    var result = await userService.GetByIdAsync(1);

    // Assert
    result.Should().BeNull();
}
```

These naming conventions clearly communicate the intent and context of each test, making the test suite easier to read and maintain.

## 4. Test Structure (Arrange-Act-Assert)

All test methods should follow the Arrange-Act-Assert (AAA) pattern:

*   **Arrange:** Set up the test environment. This includes creating mocks, setting up test data, and creating an instance of the service under test.
*   **Act:** Execute the method being tested.
*   **Assert:** Verify the outcome of the test. This includes asserting that the method returned the expected value, that a specific exception was thrown, or that a mock method was called.

**Example from `UserServiceTests.cs`:**

```csharp
[Fact]
public async Task GetByIdAsync_Should_ReturnUser_When_UserExists()
{
    // Arrange
    var user = _userBuilder.WithId(1).Build();

    var userRepositoryMock = new Mock<IUserRepository>();
    userRepositoryMock.Setup(repo => repo.GetWithIncludeAsync(1, It.IsAny<string[]>()))
        .ReturnsAsync(user);

    var userService = new UserServiceBuilder()
       .SetParameter(userRepositoryMock)
       .Build();

    // Act
    var result = await userService.GetByIdAsync(1);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(1);
}
```

## 5. Mocking Dependencies

We use Moq to create mocks of dependencies. Mocks should be used for all external dependencies, such as repositories, other services, and external APIs.

**Example from `UserServiceTests.cs`:**

```csharp
// Create a mock of the IUserRepository
var userRepositoryMock = new Mock<IUserRepository>();

// Set up the mock to return a specific user when GetWithIncludeAsync is called
userRepositoryMock.Setup(repo => repo.GetWithIncludeAsync(1, It.IsAny<string[]>()))
    .ReturnsAsync(user);

// Create an instance of the UserService with the mocked repository
var userService = new UserServiceBuilder()
   .SetParameter(userRepositoryMock)
   .Build();
```

## 6. Assertions

We use FluentAssertions for writing assertions. FluentAssertions provides a more readable and expressive way to write assertions compared to the standard xUnit `Assert` class.

**Example from `UserServiceTests.cs`:**

```csharp
// Assert that the result is not null
result.Should().NotBeNull();

// Assert that the user's ID is correct
result.Id.Should().Be(1);

// Assert that a specific exception was thrown
await Assert.ThrowsAsync<Exception>(() => userService.PermanentDeleteAsync(1, 1));
```

## 7. Using AutoFixture for Test Data

We use AutoFixture, often abstracted behind custom builder classes (e.g., `UserBuilder`), to generate test data. This approach helps to reduce the amount of boilerplate code in our tests and makes them more robust by using randomly generated data.

**Example from `UserServiceTests.cs` (using `UserBuilder`):**

```csharp
private readonly UserBuilder _userBuilder;
public UserServiceTests(DatabaseFixture databaseFixture) : base(databaseFixture)
{
    _userBuilder = new UserBuilder(_fixture);
}

// ...

// Usage within a test
var user = _userBuilder.WithId(1).Build();
```

## 8. Testing for Exceptions

When testing for exceptions, use `Assert.ThrowsAsync<T>` for asynchronous methods and `Assert.Throws<T>` for synchronous methods.

**Example from `UserServiceTests.cs`:**

```csharp
[Fact]
public async Task PermanentDeleteAsync_Should_ThrowException_When_UserNotFound()
{
    // Arrange
    var userRepositoryMock = new Mock<IUserRepository>();
    userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
        .ReturnsAsync((ApplicationUser)null);

    var userService = new UserServiceBuilder()
       .SetParameter(userRepositoryMock)
       .Build();

    // Act & Assert
    await Assert.ThrowsAsync<Exception>(() => userService.PermanentDeleteAsync(1, 1));
}
```

## 9. Getting Started with a New Test Class

Follow these steps to create a new test class for a service in `NewLifeHRT.Application.Services`. We'll use a hypothetical `ProductService` as an example.

1.  **Create the Test File:**
    Create a new C# class file in the `NewLifeHRT.Application.Services.Tests` project. The folder structure should mirror the service's location.
    *   **Service:** `NewLifeHRT.Application.Services/Services/ProductService.cs`
    *   **Test:** `NewLifeHRT.Application.Services.Tests/Services/ProductServiceTests.cs`

2.  **Create an Entity Builder:**
    If one doesn't already exist, create a builder for the primary entity the service manages (e.g., `Product`). This builder, similar to `UserBuilder`, is responsible for creating valid entity instances for your tests. It should reside in the `NewLifeHRT.Tests.Common/Builders/Data` directory.

    ```csharp
    // In NewLifeHRT.Tests.Common/Builders/Data/ProductBuilder.cs
    public class ProductBuilder : BaseEntityBuilder
    {
        private Product _product;
        public ProductBuilder(Fixture fixture) : base(fixture) { }

        public ProductBuilder WithId(int id)
        {
            _product.id = id;
            return this;
        }

        // Add other `With...` methods for customization
    }
    ```

3.  **Create a Service-Specific Builder:**
    Inside your new test file (`ProductServiceTests.cs`), create a builder for the service under test. This builder will inherit from `ServiceBuilder<T>`. Its only role is to call the service's constructor, passing in the required mocks retrieved from the base `ServiceBuilder<T>`.

    ```csharp
    // In ProductServiceTests.cs
    public class ProductServiceBuilder : ServiceBuilder<ProductService>
    {
        public override ProductService Build()
        {
            return new ProductService(
                ProductRepositoryMock.Object,
                CategoryRepositoryMock.Object // Assumes ICategoryRepository is another dependency
            );
        }
    }
    ```

4.  **IMPORTANT: Extending the Base ServiceBuilder for New Mocks**
    If your service requires a new dependency that hasn't been used in other tests before (e.g., `ICategoryRepository`), you **must** add it to the `ServiceBuilder<T>` base class located in the `NewLifeHRT.Tests.Common` project.

    This is the most critical step for extending the test framework. All mock management happens in the base class.

    **Example: Adding `ICategoryRepository` to `ServiceBuilder<T>`**

    ```csharp
    // In .../NewLifeHRT.Tests.Common/Builders/ServiceBuilder.cs
    public abstract class ServiceBuilder<T>
    {
        // ... existing properties ...
        public Mock<IUserRepository> UserRepositoryMock { get; private set; } = new() 

        // 1. Add a new property for the new mock
        public Mock<ICategoryRepository> CategoryRepositoryMock { get; private set; } = new()


        public T BuildService()
        {
            // ...
        }

        // ... existing SetParameter methods ...
        public ServiceBuilder<T> SetParameter(Mock<IUserRepository> userRepositoryMock)
        {
            UserRepositoryMock = userRepositoryMock;
            return this;
        }

        // 2. Add a new SetParameter method for the new mock
        public ServiceBuilder<T> SetParameter(Mock<ICategoryRepository> categoryRepositoryMock)
        {
            CategoryRepositoryMock = categoryRepositoryMock;
            return this;
        }

        // ... other methods ...
    }
    ```

5.  **Set Up the Test Class:**
    The test class setup remains the same. It inherits from `BaseServiceTest` and initializes the entity builder.

    ```csharp
    // In ProductServiceTests.cs
    public class ProductServiceTests : BaseServiceTest, IClassFixture<DatabaseFixture>
    {
        private readonly ProductBuilder _productBuilder;

        public ProductServiceTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            _productBuilder = new ProductBuilder(_fixture);
        }

        // ... your test methods will go here
    }
    ```

6.  **Write Your Test Methods:**
    When writing tests, use the `SetParameter` method from the service builder to provide a specific, configured mock for the test you are writing.

    ```csharp
    [Fact]
    public async Task GetByIdAsync_Should_ReturnProduct_When_ProductExists()
    {
        // Arrange
        var product = _productBuilder.WithId(1).Build();

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

        var productService = new ProductServiceBuilder()
           .SetParameter(productRepositoryMock) // This method is on the base builder
           .Build();

        // Act
        var result = await productService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }
    ```

By following these steps, you create a clean, maintainable, and consistent test setup that aligns with the project's established patterns.
