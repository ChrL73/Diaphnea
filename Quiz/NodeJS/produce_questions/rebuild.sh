#!/bin/bash -v
set -e
mkdir -p obj/Release
mkdir -p bin/Release
flags='-Wall -fexceptions -O2 -std=c++11 -I../../../mongo-cxx-driver/include'
g++ $flags -c AttributeOrderCategory.cpp -o obj/Release/AttributeOrderCategory.o
g++ $flags -c AttributeOrderChoice.cpp -o obj/Release/AttributeOrderChoice.o
g++ $flags -c Category.cpp -o obj/Release/Category.o
g++ $flags -c Choice.cpp -o obj/Release/Choice.o
g++ $flags -c CompleteQuestion.cpp -o obj/Release/CompleteQuestion.o
g++ $flags -c Level.cpp -o obj/Release/Level.o
g++ $flags -c MapIdInfo.cpp -o obj/Release/MapIdInfo.o
g++ $flags -c MapParameters.cpp -o obj/Release/MapParameters.o
g++ $flags -c MapSubParameters.cpp -o obj/Release/MapSubParameters.o
g++ $flags -c MultipleAnswerCategory.cpp -o obj/Release/MultipleAnswerCategory.o
g++ $flags -c MultipleAnswerQuestion.cpp -o obj/Release/MultipleAnswerQuestion.o
g++ $flags -c produce_questions.cpp -o obj/Release/produce_questions.o
g++ $flags -c QuestionProducer.cpp -o obj/Release/QuestionProducer.o
g++ $flags -c QuizData.cpp -o obj/Release/QuizData.o
g++ $flags -c RandomNumberGenerator.cpp -o obj/Release/RandomNumberGenerator.o
g++ $flags -c RelationOrderCategory.cpp -o obj/Release/RelationOrderCategory.o
g++ $flags -c RelationOrderChoice.cpp -o obj/Release/RelationOrderChoice.o
g++ $flags -c RelationOrderQuestion.cpp -o obj/Release/RelationOrderQuestion.o
g++ $flags -c SimpleAnswerCategory.cpp -o obj/Release/SimpleAnswerCategory.o
g++ $flags -c SimpleAnswerQuestion.cpp -o obj/Release/SimpleAnswerQuestion.o
g++ $flags -c TextAndComment.cpp -o obj/Release/TextAndComment.o
g++ -o bin/Release/produce_questions obj/Release/AttributeOrderCategory.o obj/Release/AttributeOrderChoice.o obj/Release/Category.o obj/Release/Choice.o obj/Release/CompleteQuestion.o obj/Release/Level.o obj/Release/MapIdInfo.o obj/Release/MapParameters.o obj/Release/MapSubParameters.o obj/Release/MultipleAnswerCategory.o obj/Release/MultipleAnswerQuestion.o obj/Release/produce_questions.o obj/Release/QuestionProducer.o obj/Release/QuizData.o obj/Release/RandomNumberGenerator.o obj/Release/RelationOrderCategory.o obj/Release/RelationOrderChoice.o obj/Release/RelationOrderQuestion.o obj/Release/SimpleAnswerCategory.o obj/Release/SimpleAnswerQuestion.o obj/Release/TextAndComment.o -s -pthread ../../../mongo-cxx-driver/lib/libmongoclient.a -lboost_system -lboost_regex -lboost_thread -lssl -lcrypto
cp ./bin/Release/produce_questions ./../produce_questions.exe