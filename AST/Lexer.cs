﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    [Serializable]
    class SyntaxError : Exception
    {
        public SyntaxError(int row, int column, string message)
            : base("Syntax error on row " + row + ", column " + column + ": " + message)
        { }
    }

    public class Lexer
    {
        private Stream stream;
        private List<Token> source;
        private char nextChar = (char)0;
        private bool endOfStream = false;
        private int row = 1;
        private int column = 1;

        private const string alphabet = "_+-*/<>=!;,()[]{}\"";
        private const int ALPHABET_SPACE    = 0,
                          ALPHABET_ALPHA    = 1,
                          ALPHABET_DIGIT    = 2,
                          ALPHABET_SPECIAL  = 3,
                          ALPHABET_OTHER    = 21;

        private enum State
        {
            START = 0, ID_OR_KEYWORD, NUMBER, LT, GT, LTE, GTE, ASSIGN, EQ,
            PLUS, INCR, MINUS, DECR, TIMES, DIVIDE, NOT, NEQ, SEMI, COMMA,
            LPAREN, RPAREN, LBRACK, RBRACK, LBRACE, RBRACE,
            COMMENT, ENDCOMMENT,
            STRING,
            ERROR
        };
        private State state = State.START;
        private static readonly State[] greedyStates =
            { State.GTE, State.LTE, State.EQ, State.INCR, State.DECR,
              State.NEQ, State.COMMENT, State.ENDCOMMENT };

        private static readonly Dictionary<State, TokenType> stateToToken = new Dictionary<State, TokenType>()
            {
                {State.LT, TokenType.LT},
                {State.GT, TokenType.GT},
                {State.LTE, TokenType.LTE},
                {State.GTE, TokenType.GTE},
                {State.ASSIGN, TokenType.ASSIGN},
                {State.EQ, TokenType.EQ},
                {State.PLUS, TokenType.PLUS},
                {State.INCR, TokenType.INCREMENT},
                {State.MINUS, TokenType.MINUS},
                {State.DECR, TokenType.DECREMENT},
                {State.TIMES, TokenType.TIMES},
                {State.DIVIDE, TokenType.DIVIDE},
                {State.NOT, TokenType.NOT},
                {State.NEQ, TokenType.NEQ},
                {State.SEMI, TokenType.SEMI},
                {State.COMMA, TokenType.COMMA},
                {State.LPAREN, TokenType.LPAREN},
                {State.RPAREN, TokenType.RPAREN},
                {State.LBRACK, TokenType.LBRACK},
                {State.RBRACK, TokenType.RBRACK},
                {State.LBRACE, TokenType.LBRACE},
                {State.RBRACE, TokenType.RBRACE}
            };

        private static readonly Dictionary<string, TokenType> stringToKeyword = new Dictionary<string, TokenType>()
            {
                {"int", TokenType.INT},
                {"void", TokenType.VOID},
                {"string", TokenType.STRING},
                {"if", TokenType.IF},
                {"else", TokenType.ELSE},
                {"for", TokenType.FOR},
                {"while", TokenType.WHILE},
                {"return", TokenType.RETURN}
            };

        private static readonly int[,] transitionTable = new int[28, 22]
        {
             /* SP  AL  DI  _   +   -   *   /   <   >   =   !   ;   ,   (   )   [   ]   {   }   "   OTHER */
/*0  START */  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*1  ID|KW */  {0,  1,  1,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*2  NUMBER*/  {0,  26, 2,  26, 9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*3  LT*/      {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  5,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*4  GT*/      {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  6,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*5  LTE*/     {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*6  GTE*/     {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*7  ASSIGN*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  8,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*8  EQ*/      {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*9  PLUS*/    {0,  1,  2,  1,  10, 11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*10 INCR*/    {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*11 MINUS*/   {0,  1,  2,  1,  9,  12, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*12 DECR*/    {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*13 TIMES*/   {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*14 DIVIDE*/  {0,  1,  2,  1,  9,  11, 25, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*15 NOT*/     {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  16, 15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*16 NEQ*/     {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*17 SEMI*/    {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*18 COMMA*/   {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*19 LPAREN*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*20 RPAREN*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*21 LBRACK*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*22 RBRACK*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*23 LBRACE*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*24 RBRACE*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28},
/*25 COM*/     {25, 25, 25, 25, 25, 25, 26, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25},
/*26 ENDCOM*/  {25, 25, 25, 25, 25, 25, 25, 0,  25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25},
/*27 STRING*/  {27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 0,  27}
        };

        public Lexer(Stream stream)
        {
            this.stream = stream;
            GetNextChar();
        }

        private void GetNextChar()
        {
            if (endOfStream)
            {
                throw new EndOfStreamException();
            }

            // get next character, update row and column
            nextChar = (char)stream.ReadByte();

            if (nextChar == 0xffff)
            {
                this.endOfStream = true;
                state = State.START;
                return;
            }
            else if (nextChar == '\n')
            {
                row++;
                column = 1;
            }
            else
            {
                column++;
            }

            // update DFA state
            int transition;

            if (Char.IsWhiteSpace(nextChar))
            {
                transition = ALPHABET_SPACE;
            }
            else if (Char.IsLetter(nextChar))
            {
                transition = ALPHABET_ALPHA;
            }
            else if (Char.IsDigit(nextChar))
            {
                transition = ALPHABET_DIGIT;
            }
            else if (alphabet.Contains(nextChar))
            {
                int index = alphabet.IndexOf(nextChar);
                transition = index + ALPHABET_SPECIAL;
            }
            else
            {
                transition = ALPHABET_OTHER;
            }

            state = (State)transitionTable[(int)state, transition];
            if (state == State.ERROR)
            {
                throw new SyntaxError(row, column, "Unexpected token " + nextChar);
            }
        }

        public Token GetNextToken()
        {
            // Skip whitespace.
            while (state == State.START)
            {
                if (endOfStream)
                {
                    return null;
                }
                GetNextChar();
            }

            State initialState = state;
            string tokenText = "";

            while (true)
            {
                // Keep reading characters until we change to a non-greedy
                // state. In other words, make sure we get all of a multi-
                // character operator.
                if (state != initialState)
                {
                    if (!greedyStates.Contains(state))
                    {
                        break;
                    }
                    else
                    {
                        initialState = state;
                    }
                }

                tokenText += nextChar;
                GetNextChar();
            }

            // return appropriate token
            switch (initialState)
            {
                case State.ID_OR_KEYWORD:
                    if (stringToKeyword.ContainsKey(tokenText))
                    {
                        return new Token(row, column, stringToKeyword[tokenText]);
                    }
                    else
                    {
                        return new IdentifierToken(row, column, tokenText);
                    }

                case State.ENDCOMMENT:
                    return new CommentToken(row, column, tokenText.Substring(2, tokenText.Length - 4));

                case State.STRING:
                    return new StringToken(row, column, tokenText.Substring(1, tokenText.Length - 1));

                case State.NUMBER:
                    return new IntegerToken(row, column, Convert.ToInt32(tokenText));

                default:
                    return new Token(row, column, stateToToken[initialState]);
            }
        }

        public List<Token> lex()
        {
            source = new List<Token>();
            while (!endOfStream)
            {
                Token token = GetNextToken();
                if (token != null)
                {
                    Console.WriteLine(token.ToString());
                    source.Add(token);
                }
            }
            return source;
        }
    }
}
