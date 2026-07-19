# Kaleido Framework Prototype v7

This version rebuilds the framework test foundation using **xUnit**.

## Why xUnit?

I picked xUnit for this prototype because it is common in modern .NET/.NET Core projects, integrates cleanly with `dotnet test`, supports simple `[Fact]`/`[Theory]` patterns, and keeps the test classes lightweight.

NUnit would also be fine. The important rules are the ones you called out:

- one test project per production project
- one focused test class per production class / contract area
- no catch-all test classes
- avoid testing multiple unrelated production classes in one test class
- fake dependencies when possible
- framework tests first, API tests only at the boundary

## Projects

```text
src/Kaleido.Abstractions
src/Kaleido.Core
src/Kaleido.Queryable

tests/Kaleido.Abstractions.Tests
tests/Kaleido.Core.Tests
tests/Kaleido.Queryable.Tests
```

## Run

```bash
dotnet test KaleidoFrameworkPrototype_v7.sln
```

Or individually:

```bash
dotnet test tests/Kaleido.Abstractions.Tests/Kaleido.Abstractions.Tests.csproj
dotnet test tests/Kaleido.Core.Tests/Kaleido.Core.Tests.csproj
dotnet test tests/Kaleido.Queryable.Tests/Kaleido.Queryable.Tests.csproj
```

## Current test classes

### Abstractions

- `AttributeContractTests`
- `DescriptorContractTests`
- `EnumDescriptionTests`
- `QueryContractTests`

### Core

- `KaleidoMetadataBuilderTests`
- `KaleidoDescriptorFactoryTests`
- `KaleidoQueryValidatorTests`
- `KaleidoQueryCompilerTests`
- `KaleidoRegistryTests`
- `KaleidoCatalogTests`
- `KaleidoMetadataCatalogTests`
- `CursorCodecTests`

### Queryable

- `QueryableCompiledQueryApplierTests`
- `QueryableKaleidoExecutorTests`
- `QueryableKaleidoQueryEngineTests`
- `KaleidoQueryableServiceCollectionExtensionsTests`

## Note

This is still prototype/guidance code. The goal is to verify test structure, test expectations, and framework coverage direction — not to pretend this is production-ready.
