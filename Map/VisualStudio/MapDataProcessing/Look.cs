﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    abstract class Look
    {
        private static int _counter = -1;
        internal static void reset() { _counter = -1; }

        private readonly int _id;
        internal Look() { _id = ++_counter; }
        internal int Id { get { return _id; } }

        abstract internal BsonDocument getBsonDocument();
    }
}