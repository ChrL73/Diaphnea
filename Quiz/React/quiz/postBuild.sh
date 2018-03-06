#!/bin/bash
cp build/index.html test.html
sed -i -E "s/http:\/\/.+(\/socket.io\/socket.io.js)/\1/" test.html
