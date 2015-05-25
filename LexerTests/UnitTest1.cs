using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AST;

namespace LexerTests
{
    [TestClass]
    public class UnitTest1
    {
        static Stream StreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [TestMethod]
        public void ReadSingleKeyword()
        {
            Lexer lexer = new Lexer(StreamFromString("int"));
            Assert.AreEqual(TokenType.INT, lexer.GetNextToken().type, "wrong token returned");
        }

        [TestMethod]
        public void ReadIntegerToken()
        {
            Lexer lexer = new Lexer(StreamFromString("1337"));
            Token token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.INTEGER, token.type, "wrong token type");
            Assert.AreEqual(1337, ((IntegerToken)token).intValue, "wrong token value");
        }

        [TestMethod]
        public void ReadTwoTokens()
        {
            Lexer lexer = new Lexer(StreamFromString("int num_1"));
            Token token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.INT, token.type, "token 1 wrong type");

            token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.ID, token.type, "token 2 wrong type");
            Assert.AreEqual("num_1", ((IdentifierToken)token).stringValue, "token 2 wrong value");
        }

        [TestMethod]
        public void ReadGreedyToken()
        {
            Lexer lexer = new Lexer(StreamFromString("var != 15"));

            Token token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.ID, token.type, "token 1 wrong type");
            Assert.AreEqual("var", ((IdentifierToken)token).stringValue, "token 1 wrong value");

            token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.NEQ, token.type, "token 2 wrong type");

            token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.INTEGER, token.type, "token 3 wrong type");
            Assert.AreEqual(15, ((IntegerToken)token).intValue, "token 3 wrong type");
        }
    }
}
