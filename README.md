# CsvSourceGenerator

CsvSourceGenerator is a .NET source generator for generating CSV serialization code for your classes and records. This project leverages C# Source Generators to automatically create boilerplate code, making it easier to handle CSV serialization.

## Table of Contents

- [Getting Started](#getting-started)
- [Installation](#installation)
- [Usage](#usage)
- [Examples](#examples)
- [Contributing](#contributing)
- [License](#license)

## Getting Started

These instructions will help you set up the CsvSourceGenerator in your .NET project.

### Prerequisites

- .NET SDK 5.0 or higher

### Installation

As of now, no NuGet packages are available for CsvSourceGenerator. You need to clone the repository and include the project in your solution.

1. **Clone the repository**:

    ```sh
    git clone https://github.com/Sadra-jl/CsvSourceGenerator.git
    ```

2. **Add the project to your solution**:

   In your solution directory, add the CsvSourceGenerator project:

    ```sh
    dotnet sln add CsvSourceGenerator/CsvSourceGenerator.csproj
    ```

3. **Reference the CsvSourceGenerator project**:

   In your project file (`.csproj`), add a project reference with `OutputItemType="Analyzer"`(otherwise it will not work):

    ```xml
    <ProjectReference Include="..\CsvSourceGenerator\CsvSourceGenerator.csproj" OutputItemType="Analyzer" />
    ```
4. **Build your project**:

   You can build your project to make sure that the source generator is correctly integrated and that the CSV serialization code is generated.

    ```sh
    dotnet build
    ```

If you prefer a NuGet package for easier integration, feel free to [open an issue](https://github.com/Sadra-jl/CsvSourceGenerator/issues) or contact me directly, and I will create one.


### Usage

1. **Apply the `CsvSerializableAttribute`**:
Apply the `CsvSerializableAttribute` to the classes or records you want to be CSV serializable. The source generator will also generate the `ICsvSerializable` interface, which these classes or records will implement. then make the type as partial

   ```csharp
   using PaleLotus.CsvSourceGenerator;

   [CsvSerializable]
   public partial record MyRecord(int Id, string Name);

   [CsvSerializable]
   public partial class MyClass
   {
       public int Id { get; set; }
       public string Name { get; set; }
   }
   ```
2. **Build Your Project**:
   When you build your project, the source generator will automatically implement the `ICsvSerializable` interface and CSV serialization code for the marked classes and records.

### Examples

Here’s an example of a generated CSV serialization class:

```csharp
   namespace MyNamespace
   {
      public partial class MyClass : PaleLotus.CsvSourceGenerator.ICsvSerializable
      {
        public string ToCsv() => $"{Id},{Name}";
        public string GetCsvHeader() => $"Id,Name";
      }
   }
   namespace MyNamespace
   {
       public partial record MyRecord : PaleLotus.CsvSourceGenerator.ICsvSerializable
       {
          public string ToCsv() => $"{Id},{Name}";
          public string GetCsvHeader() => $"Id,Name";
       }
   }
```

### Contributing

We welcome contributions to enhance CsvSourceGenerator! Here's how you can help:

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/AmazingFeature`).
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4. Push to the branch (`git push origin feature/AmazingFeature`).
5. Open a Pull Request.

### License

This project is licensed under the   Apache License Version 2.0. See the [LICENSE](LICENSE) file for more details.

## Acknowledgements

- [Microsoft](https://github.com/dotnet/roslyn) for the Roslyn API.
- The .NET community for their valuable contributions and feedback.
