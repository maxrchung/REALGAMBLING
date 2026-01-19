#!/bin/bash -eu
set -o pipefail

cd "$(dirname "$0")"

# build_type=Debug

# while [ $# -gt 0 ]; do
#   case $1 in -d | --debug)
#         build_type=Debug;;
#     -r | --release)
#         build_type=Release;;
#   esac
#   shift
# done

echo "Building GambleCore"
dotnet build -c Debug GambleCore

echo "Adding to Unity project"
cp GambleCore/GambleCore/bin/Debug/netstandard2.0/*.dll real-gambling/Assets/GambleCore

