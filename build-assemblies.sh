#!/bin/bash -eu
set -o pipefail

cd "$(dirname "$0")"

echo "Building GambleCore"
dotnet build -c Release GambleCore

echo "Adding to Unity project"
cp GambleCore/GambleCore/bin/Release/netstandard2.0/*.dll real-gambling/Assets/GambleCore
