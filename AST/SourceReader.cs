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
        private string path;
        List<Token> source;

        public SourceReader(string path)
        {
            this.path = path;
        }

        public List<Token> lex()
        {
            FileStream stream = File.OpenRead(path);
            Lexer lexer = new Lexer(stream);
            source = lexer.lex();
            stream.Close();
            return source;
        }

        public ProgramNode parse()
        {
            return new Parser(source).Parse();
        }
    }
}
