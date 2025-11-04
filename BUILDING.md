# Building and Running SteelkiltSharp

## Quick Start

The easiest way to run the examples:

```bash
./run.sh
```

## The Build Issue

There's a known issue with .NET where sometimes the `obj` directory caches can cause duplicate assembly attribute errors when using `dotnet run`. The workaround is to clean build artifacts before running.

### Symptoms

If you see errors like:
```
error CS0579: Duplicate 'global::System.Runtime.Versioning.TargetFrameworkAttribute' attribute
```

### Solution

Use one of these methods:

#### Method 1: Use the run.sh Script (Recommended)

```bash
./run.sh
```

This script automatically:
1. Cleans all build artifacts
2. Builds the library
3. Builds the examples
4. Runs the executable

#### Method 2: Manual Build and Run

```bash
# Clean all build artifacts
find . -type d \( -name "obj" -o -name "bin" \) -exec rm -rf {} + 2>/dev/null

# Build the library
dotnet build

# Build the examples
dotnet build Examples/SteelkiltSharp.Examples.csproj

# Run the pre-built executable (NOT dotnet run)
./Examples/bin/Debug/net9.0/SteelkiltSharp.Examples
```

#### Method 3: Just Build and Run Executable

If you've already built successfully once:

```bash
./Examples/bin/Debug/net9.0/SteelkiltSharp.Examples
```

## Why Not Use `dotnet run`?

The `dotnet run --project Examples/SteelkiltSharp.Examples.csproj` command triggers a rebuild which sometimes encounters the duplicate attribute caching issue. Running the pre-built executable avoids this problem entirely.

## Building for Release

```bash
# Clean
find . -type d \( -name "obj" -o -name "bin" \) -exec rm -rf {} + 2>/dev/null

# Build in Release mode
dotnet build -c Release
dotnet build Examples/SteelkiltSharp.Examples.csproj -c Release

# Run
./Examples/bin/Release/net9.0/SteelkiltSharp.Examples
```

## Using as a Library

To use SteelkiltSharp in your own project:

```bash
dotnet add reference /path/to/SteelkiltSharp.csproj
```

Or copy the DLL:
```bash
cp bin/Debug/net9.0/SteelkiltSharp.dll /your/project/libs/
```

## IDE Usage

If using an IDE like Visual Studio or Rider:
1. Open the solution/project
2. Clean the solution (Build → Clean Solution)
3. Rebuild the solution (Build → Rebuild Solution)
4. Run the Examples project

The IDE handles the build artifacts correctly and shouldn't encounter the duplicate attribute issue.
