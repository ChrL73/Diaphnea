#!/bin/bash -v
set -e
cd Data/generated_code/_France
for filename in *.cpp; do
    echo $filename
    g++ -Wall -fexceptions -std=c++11 -c $filename -o ../../../obj/Release/Data/generated_code/_France/${filename%%.cpp}.o
done
cd ../../../obj/Release/Data/generated_code/_France
ar ra rc _France.a *
ranlib _France.a