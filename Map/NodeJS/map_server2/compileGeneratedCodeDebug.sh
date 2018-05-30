#!/bin/bash -v
set -e
rm obj/Debug/Data/generated_code/_France/*
cd Data/generated_code/_France
for filename in *.cpp; do
    echo $filename
    g++ -Wall -fexceptions -std=c++11 -g -c $filename -o ../../../obj/Debug/Data/generated_code/_France/${filename%%.cpp}.o
done
cd ../../../obj/Debug/Data/generated_code/_France
ar rc _France.a *
ranlib _France.a