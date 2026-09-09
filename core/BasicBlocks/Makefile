PIN_CONFIG := $(firstword $(wildcard ../../3rdParty/pin/*/source/tools/Config/makefile.config))
PIN_ROOT ?= $(abspath $(dir $(PIN_CONFIG))/../../..)

CONFIG_ROOT := $(PIN_ROOT)/source/tools/Config
include $(CONFIG_ROOT)/makefile.config

TOOL_ROOTS := bblocks
bblocks_SOURCES := bblocks.cpp compat.cpp

include $(TOOLS_ROOT)/Config/makefile.default.rules
