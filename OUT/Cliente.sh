#!/bin/sh
printf '\033c\033]0;%s\a' Cliente
base_path="$(dirname "$(realpath "$0")")"
"$base_path/Cliente.x86_64" "$@"
