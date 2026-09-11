cmake_minimum_required(VERSION 3.20)

project(PeachTestFixtures LANGUAGES C CXX)

# Keep fixture binaries in a directory supplied by MSBuild.  This lets every
# target framework copy the matching native fixtures to its test output.
set(PEACH_FIXTURES_OUTPUT_DIR "${CMAKE_BINARY_DIR}/artifacts" CACHE PATH
    "Directory for Peach native test fixtures")

function(add_peach_fixture target source)
  add_executable(${target} "${source}")
  set_target_properties(${target} PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${PEACH_FIXTURES_OUTPUT_DIR}")

  # Multi-config generators (Visual Studio, Xcode) otherwise add a Debug or
  # Release subdirectory while single-config generators do not.
  foreach(configuration Debug Release RelWithDebInfo MinSizeRel)
    string(TOUPPER "${configuration}" configuration_upper)
    set_target_properties(${target} PROPERTIES
      "RUNTIME_OUTPUT_DIRECTORY_${configuration_upper}" "${PEACH_FIXTURES_OUTPUT_DIR}")
  endforeach()

  if(CMAKE_C_COMPILER_ID MATCHES "GNU|Clang|AppleClang" OR
     CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang|AppleClang")
    target_compile_options(${target} PRIVATE -g -O0)
  endif()
endfunction()

add_peach_fixture(CrashTest CrashTest/main.c)
add_peach_fixture(CrashableServer CrashableServer/CrashableServer.cpp)
add_peach_fixture(CrashingFileConsumer CrashingFileConsumer/CrashingFileConsumer.cpp)

# These programs deliberately exercise stack-corruption paths in debugger
# tests.  Retain the previous GCC/Clang behaviour without applying unsupported
# flags to MSVC.
foreach(target CrashableServer CrashingFileConsumer)
  if(CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang|AppleClang")
    target_compile_options(${target} PRIVATE -fno-stack-protector -fno-omit-frame-pointer)
  endif()
endforeach()
