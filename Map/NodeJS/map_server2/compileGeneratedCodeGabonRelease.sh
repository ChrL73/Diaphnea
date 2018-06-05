#!/bin/bash -v
set -e
rm -f obj/Release/Data/generated_code/_Gabon/*
cd Data/generated_code/_Gabon
for filename in *.cpp; do
    echo $filename
    g++ -Wall -fexceptions -std=c++11 -c $filename -o ../../../obj/Release/Data/generated_code/_Gabon/${filename%%.cpp}.o
done
cd ../../../obj/Release/Data/generated_code/_Gabon
ar rc _Gabon.a *
ranlib _Gabon.a