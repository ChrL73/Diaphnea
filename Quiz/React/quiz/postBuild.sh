#!/bin/bash -v
set -e
mkdir -p ../../NodeJS/views
cp build/index.html ../../NodeJS/views/reactIndex.ejs
sed -i -E "s/http:\/\/.+(\/socket.io\/socket.io.js)/\1/" ../../NodeJS/views/reactIndex.ejs
rm -rf ../../NodeJS/public/static
cp -R build/static/ ../../NodeJS/public/static/
sed -i -E "s/(io.connect\()\"[^\"]+\"(\))/\1\2/" ../../NodeJS/public/static/js/main.*.js