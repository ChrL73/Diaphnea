#!/bin/bash -v
set -e
rm -f obj/Debug/Data/generated_code/_Gabon/*
cd Data/generated_code/_Gabon
for filename in *.cpp; do
    echo $filename
    g++ -Wall -fexceptions -std=c++11 -g -c $filename -o ../../../obj/Debug/Data/generated_code/_Gabon/${filename%%.cpp}.o
done
cd ../../../obj/Debug/Data/generated_code/_Gabon
ar rc _Gabon.a *
ranlib _Gabon.a