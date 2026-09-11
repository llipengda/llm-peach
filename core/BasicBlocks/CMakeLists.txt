cmake_minimum_required(VERSION 3.20)

project(PeachBasicBlocks LANGUAGES NONE)

set(PIN_ROOT "" CACHE PATH "Root of a compatible Intel Pin kit")
set(PIN_TARGET "intel64" CACHE STRING "Intel Pin target architecture (ia32 or intel64)")

if(NOT PIN_TARGET STREQUAL "ia32" AND NOT PIN_TARGET STREQUAL "intel64")
  message(FATAL_ERROR "PIN_TARGET must be ia32 or intel64; got '${PIN_TARGET}'.")
endif()
if(NOT EXISTS "${PIN_ROOT}/source/tools/Config/makefile.config")
  message(FATAL_ERROR "PIN_ROOT is not a valid Intel Pin kit: '${PIN_ROOT}'.")
endif()

find_program(PEACH_PIN_MAKE NAMES make gmake REQUIRED)

if(WIN32)
  set(PIN_TOOL_EXTENSION dll)
elseif(APPLE)
  set(PIN_TOOL_EXTENSION dylib)
else()
  set(PIN_TOOL_EXTENSION so)
endif()

set(PIN_TOOL "${CMAKE_BINARY_DIR}/bblocks.${PIN_TOOL_EXTENSION}")
add_custom_command(
  OUTPUT "${PIN_TOOL}"
  COMMAND "${PEACH_PIN_MAKE}" -C "${CMAKE_CURRENT_SOURCE_DIR}"
          "PIN_ROOT=${PIN_ROOT}" "TARGET=${PIN_TARGET}"
  COMMAND "${CMAKE_COMMAND}" -E copy_if_different
          "${CMAKE_CURRENT_SOURCE_DIR}/obj-${PIN_TARGET}/bblocks.${PIN_TOOL_EXTENSION}"
          "${PIN_TOOL}"
  DEPENDS Makefile bblocks.cpp compat.cpp compat.h uthash.h
  COMMENT "Building Intel Pin BasicBlocks tool")
add_custom_target(bblocks ALL DEPENDS "${PIN_TOOL}")
