#!/bin/bash
# Run SteelkiltSharp Examples
# This script builds and runs the examples without triggering duplicate attribute errors

set -e

echo "Cleaning previous builds..."
find . -type d \( -name "obj" -o -name "bin" \) -exec rm -rf {} + 2>/dev/null || true

echo "Building SteelkiltSharp library..."
dotnet build -v quiet

echo "Building Examples..."
dotnet build Examples/SteelkiltSharp.Examples.csproj -v quiet

echo ""
echo "Running examples..."
echo "===================="
./Examples/bin/Debug/net9.0/SteelkiltSharp.Examples
