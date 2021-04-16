# Inversed index
[![Build status](https://ci.appveyor.com/api/projects/status/70nsp5hoy5oscwbp/branch/main?svg=true)](https://ci.appveyor.com/project/yuniyakim/inverseindex/branch/main)
[![Build Status](https://travis-ci.org/AlinaRodina99/InverseIndex.svg?branch=main)](https://travis-ci.org/AlinaRodina99/InverseIndex)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/b2d19a1377c84f24a0868565a0ed4207)](https://www.codacy.com/gh/AlinaRodina99/InverseIndex/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=AlinaRodina99/InverseIndex&amp;utm_campaign=Badge_Grade)

Inversed index is a program for information retrieval from the corpus according to a given boolean query. :mag:

### Prerequisites

*   .NET Core 3.1 (you can [download](https://dotnet.microsoft.com/download/dotnet/3.1) it)

### Installing

*   Download this repository
```cmd
git clone https://github.com/AlinaRodina99/InverseIndex
```

## Getting started

To run the program, do the following:

1.  Go to *InverseIndex* folder
```
cd InverseIndex
```

2.  Launch run file
*   If you're using *Windows*, launch **run.bat**
```
run
```
*   If you're using *Linux*, launch **run.sh**
```
chmod +x run.sh
./run.sh
```

3.  Enjoy! :sunglasses:

### Query
Queries support three boolean operations: AND, OR and NOT. You can also use parenthesis.

Here are some queries examples:

*   Example 1

```
word
```

*   Example 2

```
word1 AND (word2 OR NOT word3)
```

*   Example 3

```
NOT ((word1 OR word 2) AND NOT (word3 OR word4))
```

## Technologies

A list of libraries used within the project:

*   [McBits.Tokenization](https://www.nuget.org/packages/McBits.Tokenization): version 2.0.0 
*   [Porter2StemmerStandard](https://www.nuget.org/packages/Porter2StemmerStandard): version 1.1.0
*   [PriorityQueues_boraaros](https://www.nuget.org/packages/PriorityQueues_boraaros): version 2.0.1

## Authors

*   **Alina Rodina** – [GitHub](https://github.com/AlinaRodina99) :cat:
*   **Yuniya Kim** – [GitHub](https://github.com/YuniyaKim) :dog:

Special thanks to **Kirill Smirnov** for great contribution to the development. :computer:
