using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    class SourceReader
    {
        private FileStream stream;

        public SourceReader(string filename)
        {
            stream = File.OpenRead(filename);
        }

        public List<Token> lex()
        {
            Lexer lexer = new Lexer(stream);
            return lexer.lex();
        }
    }
}
