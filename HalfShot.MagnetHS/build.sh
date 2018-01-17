#!/bin/bash
CONFIG=$1
OUT_DIR="$(dirname $0)/build/$CONFIG/"
DOTNET_BIN="$(which dotnet)"
SLN_FILE="$(dirname $0)/HalfShot.MagnetHS.sln"

if [ ! -e "$OUT_DIR" ] ; then
    mkdir "$OUT_DIR"
fi



echo "Outputting to $OUT_DIR"
dotnet build -o $OUT_DIR -c $CONFIG $SLN_FILE
