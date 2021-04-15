#!/bin/sh

dotnet build InverseIndex/InverseIndex.sln
dotnet test InverseIndex\InverseIndex.sln
dotnet InverseIndex/InverseIndex/bin/Debug/netcoreapp3.1/InverseIndex.dll