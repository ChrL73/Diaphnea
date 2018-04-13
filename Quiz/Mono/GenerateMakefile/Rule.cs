using System;
using System.Collections.Generic;

namespace GenerateMakefile
{
    class Rule
    {
        private readonly List<string> _dependencies = new List<string>();
        private readonly List<string> _commands = new List<string>();

        internal string Target { get; set; }
        internal List<string> Dependencies { get { return _dependencies; } }
        internal List<string> Commands { get { return _commands; } }
    }
}
