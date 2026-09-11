### Please add Intel Pin here!

To build the coverage tools used by MinSet a [copy of Intel Pin](https://software.intel.com/content/www/us/en/develop/articles/pin-a-binary-instrumentation-tool-downloads.html) is needed for the architecture your building for. Any compatible Pin kit can be used.

Extract the kit into any direct child folder of `3rdParty/pin/`. These are
historical examples:

- pin-3.19-98425-clang-mac
- pin-3.19-98425-msvc-windows
- pin-3.19-98425-gcc-linux

MSBuild discovers the extracted kit automatically and includes the required Pin
runtime files in the Peach installation. No build argument or environment
variable is required.

CMake is the build entry point for Peach's Pin tools. Intel Pin distributes its
own supported Makefile rules, so CMake delegates the final compile and link
step to those rules. Ensure a `make` implementation is available alongside
CMake.

The kit must contain `source/tools/Config/makefile.config`; that file is used
to identify a valid Pin installation.
