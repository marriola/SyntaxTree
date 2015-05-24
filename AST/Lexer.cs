using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    class SyntaxError : Exception
    {
        private int row;
        private int column;
        private string message;

        public SyntaxError(int row, int column, string message)
            : base("Syntax error on row " + row + ", column " + column + ": " + message)
        { }
    }

    public class Lexer
    {
        private Stream stream;
        private List<Token> source;
        private StringBuilder currentToken;
        private char nextChar = (char)0;
        private bool endOfStream = false;
        private int row = 1;
        private int column = 1;

        private const string alphabet = "_+-*/<>=!;,()[]{}";
        private enum State
        {
            START = 0, ID_OR_KEYWORD, NUMBER, GT, LT, GTE, LTE, ASSIGN, EQ,
            PLUS, INCR, MINUS, DECR, TIMES, DIVIDE, NOT, NEQ, SEMI, COMMA,
            LPAREN, RPAREN, LBRACK, RBRACK, LBRACE, RBRACE,
            COMMENT, ENDCOMMENT,
            ERROR
        };
        private State state = State.START;
        private State[] greedyStates =
            { State.GTE, State.LTE, State.EQ, State.INCR, State.DECR,
              State.NEQ, State.COMMENT, State.ENDCOMMENT };

        private Dictionary<State, TokenType> stateToToken = new Dictionary<State, TokenType>()
            {
                {State.GT, TokenType.GT},
                {State.GTE, TokenType.GTE},
                {State.LT, TokenType.LT},
                {State.LTE, TokenType.LTE},
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

        private Dictionary<string, TokenType> stringToKeyword = new Dictionary<string, TokenType>()
            {
                {"int", TokenType.INT},
                {"void", TokenType.VOID},
                {"if", TokenType.IF},
                {"else", TokenType.ELSE},
                {"for", TokenType.FOR},
                {"while", TokenType.WHILE},
                {"return", TokenType.RETURN}
            };

        private static int[,] transitionTable = new int[27, 21]
        {
             /* SP  AL  DI  _   +   -   *   /   <   >   =   !   ;   ,   (   )   [   ]   {   }   OTHER */
/*0  START */  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*1  ID|KW */  {0,  1,  1,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*2  NUMBER*/  {0,  26, 2,  26, 9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*3  GT*/      {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  5,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*4  LT*/      {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  6,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*5  GTE*/     {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*6  LTE*/     {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*7  ASSIGN*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  8,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*8  EQ*/      {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*9  PLUS*/    {0,  1,  2,  1,  10, 11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*10 INCR*/    {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*11 MINUS*/   {0,  1,  2,  1,  9,  12, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*12 DECR*/    {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*13 TIMES*/   {0,  1,  2,  1,  9,  11, 25, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*14 DIVIDE*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*15 NOT*/     {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  16, 15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*16 NEQ*/     {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*17 SEMI*/    {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*18 COMMA*/   {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*19 LPAREN*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*20 RPAREN*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*21 LBRACK*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*22 RBRACK*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*23 LBRACE*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*24 RBRACE*/  {0,  1,  2,  1,  9,  11, 13, 14, 3,  4,  7,  15, 17, 18, 19, 20, 21, 22, 23, 24, 27},
/*25 COM*/     {25, 25, 25, 25, 25, 25, 26, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25},
/*26 ENDCOM*/  {25, 25, 25, 25, 25, 25, 25, 0,  25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25},
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
                endOfStream = true;
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
            if (Char.IsWhiteSpace(nextChar))
            {
                state = (State)transitionTable[(int)state, 0];
            }
            else if (Char.IsLetter(nextChar))
            {
                state = (State)transitionTable[(int)state, 1];
            }
            else if (Char.IsDigit(nextChar))
            {
                state = (State)transitionTable[(int)state, 2];
            }
            else if (alphabet.Contains(nextChar))
            {
                state = (State)transitionTable[(int)state, alphabet.IndexOf(nextChar) + 3];
            }
            else
            {
                state = (State)transitionTable[(int)state, 20];
            }
        }

        public Token GetNextToken()
        {
            // Skip whitespace.
            while (state == State.START)
            {
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
                        return new Token(stringToKeyword[tokenText]);
                    }
                    else
                    {
                        return new IdentifierToken(tokenText);
                    }

                case State.NUMBER:
                    return new IntegerToken(Convert.ToInt32(tokenText));

                default:
                    return new Token(stateToToken[state]);
            }
        }

        public List<Token> lex()
        {
            return source;
        }
    }
}
