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
            Assert.AreEqual(TokenType.INTLITERAL, token.type, "wrong token type");
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
            Lexer lexer = new Lexer(StreamFromString("!= == <= >= ++ --"));

            Token token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.NEQ, token.type, "!= failed");

            token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.EQ, token.type, "== failed");

            token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.LTE, token.type, "<= failed");

            token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.GTE, token.type, ">= failed");

            token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.INCREMENT, token.type, "++ failed");

            token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.DECREMENT, token.type, "-- failed");
        }

        [TestMethod]
        public void ReadStringToken()
        {
            Lexer lexer = new Lexer(StreamFromString("\"a string\""));
            Token token = lexer.GetNextToken();
            Assert.AreEqual(TokenType.STRINGLITERAL, token.type, "wrong token type");
            Assert.AreEqual("a string", ((StringToken)token).stringValue, "wrong token value");
        }

        [TestMethod]
        public void TestLex()
        {
            Lexer lexer = new Lexer(StreamFromString("int"));
            lexer.lex();
        }
    }
}
