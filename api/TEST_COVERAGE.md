# 🧪 Testing & Code Coverage Guide

This guide explains how to run tests and track code coverage for the TaskManager API.

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Test dependencies are automatically restored when running tests

## 🚀 Quick Start

### Basic Test Execution
```bash
# Run all tests
cd api
dotnet test

# Run with detailed output
dotnet test --verbosity normal
```

### Code Coverage Analysis

#### Option 1: Use Scripts (Recommended)
```bash
# Linux/macOS
cd api
chmod +x scripts/coverage.sh
./scripts/coverage.sh

# Windows (PowerShell)
cd api
.\scripts\coverage.ps1
```

#### Option 2: Manual Commands
```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --settings ./coverlet.runsettings

# Generate HTML report (after installing ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"coverage/**/coverage.cobertura.xml" -targetdir:"coverage/html"
```

## 📊 Coverage Reports

After running coverage analysis, you'll find:

- **XML Report**: `./coverage/**/coverage.cobertura.xml` - Machine-readable format
- **HTML Report**: `./coverage/html/index.html` - Human-readable browser view

### Understanding Coverage Metrics

- **Line Coverage**: Percentage of executed code lines
- **Branch Coverage**: Percentage of executed conditional branches
- **Method Coverage**: Percentage of called methods

### Coverage Targets

| Component | Target Coverage | Current Status |
|-----------|----------------|----------------|
| **Core Business Logic** | ≥ 90% | 🟡 Needs tests |
| **Services Layer** | ≥ 85% | 🟢 In progress |
| **Controllers** | ≥ 80% | 🔴 Not started |
| **Overall Project** | ≥ 80% | 🔴 Not started |

## 🏗️ Test Structure

```
api/tests/SaasTaskManager.Tests/
├── Services/           # Service layer tests
│   ├── UserServiceTests.cs
│   ├── ListServiceTests.cs
│   └── TaskServiceTests.cs
├── Controllers/        # API controller tests
├── Entities/          # Domain entity tests
├── Common/           # Utility and helper tests
└── Integration/      # Integration tests
```

## 📝 Writing Tests

### Unit Test Example
```csharp
[Fact]
public async Task CreateUserAsync_WithValidData_ShouldReturnSuccess()
{
    // Arrange
    var request = new CreateUserRequest("test@example.com", "Test User", "Password123!");

    // Act
    var result = await _userService.CreateUserAsync(request);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value!.Email.Should().Be("test@example.com");
}
```

### Test Naming Convention
- **Method**: `MethodName_Scenario_ExpectedResult`
- **Example**: `LoginAsync_WithInvalidCredentials_ShouldReturnFailure`

## 🔧 Configuration

### Coverage Settings (`coverlet.runsettings`)
- **Includes**: Core, Infrastructure, and API projects
- **Excludes**: Test projects, Program.cs, Migrations
- **Output**: Cobertura XML format for cross-platform compatibility

### Test Dependencies
- **xUnit**: Testing framework
- **FluentAssertions**: Readable assertions
- **Moq**: Mocking framework
- **EntityFrameworkCore.InMemory**: In-memory database for testing
- **Coverlet**: Code coverage collection
- **ReportGenerator**: HTML report generation

## 🎯 Best Practices

1. **AAA Pattern**: Arrange, Act, Assert
2. **One Assertion Per Test**: Keep tests focused
3. **Descriptive Names**: Tests should be self-documenting
4. **Test Edge Cases**: Happy path + error scenarios
5. **Mock External Dependencies**: Isolate units under test
6. **Clean Test Data**: Use in-memory database or proper cleanup

## 🚀 CI/CD Integration

```yaml
# Example GitHub Actions step
- name: Run Tests with Coverage
  run: |
    cd api
    dotnet test --collect:"XPlat Code Coverage" --settings ./coverlet.runsettings
    
- name: Generate Coverage Report
  run: |
    dotnet tool install -g dotnet-reportgenerator-globaltool
    reportgenerator -reports:"api/coverage/**/coverage.cobertura.xml" -targetdir:"coverage-report"
```

## 📈 Coverage Goals

| Phase | Target | Components |
|-------|--------|------------|
| **Phase 1** | 60%+ | Services + Core entities |
| **Phase 2** | 75%+ | + Controllers |
| **Phase 3** | 85%+ | + Integration tests |
| **Phase 4** | 90%+ | + Edge cases |

## 🛠️ Troubleshooting

### Common Issues

1. **Tests not running**: Check project references and dependencies
2. **Coverage = 0%**: Verify include/exclude patterns in settings
3. **ReportGenerator errors**: Ensure .NET 6+ runtime is available
4. **Permission errors**: Make scripts executable with `chmod +x`

### Debug Commands
```bash
# Verbose test output
dotnet test --verbosity diagnostic

# List discovered tests
dotnet test --list-tests

# Filter specific tests
dotnet test --filter "ClassName.MethodName"
``` 