#!/bin/sh

proto=$(echo "$PROTOCOL" | tr '[:upper:]' '[:lower:]')
cd "/peach/templates/$proto" || exit 1
exec "./run.sh"