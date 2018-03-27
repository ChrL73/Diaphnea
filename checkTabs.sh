#!/bin/bash -v
grep -lr --include='*.cs' -P "\t" .
grep -lr --include='*.cpp' -P "\t" .
grep -lr --include='*.h' -P "\t" .
grep -lr --include='*.xml' -P "\t" .
grep -lr --include='*.xsd' -P "\t" .