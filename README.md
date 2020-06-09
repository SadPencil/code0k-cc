# code0k-cc: Zero-Knowledge Compiler for zk-SNARKs

Warning: This project is still at very early stage. The compiler may crash, producing a wrong output, or having potential bugs. Do **NOT** use it until this project goes to alpha phase.

## Get the compiler

### Download

After this project goes to alpha phase, a pre-compiled binary will be provided for common platforms.


And place the executable file to `/usr/local/bin`, naming `code0k-cc`.

### Compile the compiler by hand

#### Prerequisite

- [.NET Core 3.1 or later](https://dotnet.microsoft.com/download/dotnet-core) Installed on a Windows / Linux platform

#### Compile

```bash
dotnet publish -r <RUNTIME_IDENTIFIER> -c Release -p:PublishSingleFile=true
```

where the `<RUNTIME_IDENTIFIER>` might be one of the following.

```
win-x64
win-arm64
linux-x64
linux-arm64
osx-x64
```

Although supported, do not compile it to a 32-bit platform, e.g. `win-x86`. The compiler requires plenty of memory, compared to traditional compilers.



Take an example.


```bash
dotnet publish -r linux-x64 -c Release -p:PublishSingleFile=true
```

And place the executable file to `/usr/local/bin`, naming `code0k-cc`.

## Compile the zero-knowledge code

For convenience, the input file is called `input.c0k.c`. You may get an example file from `Examples` folder.  

### Pre-Process

You may use `gcc` to pre-process the file. This enabling `#include` `#ifdef` and removing the comment of the source code.

```bash
# command on Bash
gcc -E input.c0k.c | grep -v "^#" > input.c0k.i
```

```cmd
REM command on Windows cmd
gcc -E input.c0k.c | findstr -V "^#" > input.c0k.i
```

### Compile

```bash
code0k-cc input.c0k.i
```

And the output file will be `input.c0k.i.arith` and `input.c0k.i.in.txt`.

Edit `input.c0k.i.in.txt`. Provide input & nizk-input values by replacing `<to-be-filled-by-user>`. 

In the future, a helper program will be provided to help user provide input & nizk-input values with more complex types and values.
