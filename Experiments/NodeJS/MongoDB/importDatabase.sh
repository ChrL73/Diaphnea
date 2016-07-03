#!/bin/bash
mongoimport --db testDb --drop --collection test --file ../../../Shared/test.json
