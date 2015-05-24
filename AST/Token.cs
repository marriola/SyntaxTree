﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public enum TokenType
    {
        ERROR,
        ID, INTEGER, COMMENT,
        INT, VOID,
        IF, ELSE, FOR, WHILE, RETURN,
        PLUS, MINUS, TIMES, DIVIDE,
        LT, LTE, EQ, NEQ, GTE, GT,
        INCREMENT, DECREMENT,
        ASSIGN,
        SEMI, COMMA,
        LPAREN, RPAREN, LBRACK, RBRACK, LBRACE, RBRACE,
        NOT
    }

    public class Token
    {
        public TokenType type
        {
            get;
            protected set;
        }

        public Token(TokenType type)
        {
            this.type = type;
        }
    }

    public class IdentifierToken : Token
    {
        public string stringValue
        {
            get;
            private set;
        }

        public IdentifierToken(string stringValue) : base(TokenType.ID)
        {
            this.stringValue = stringValue;
        }
    }

    public class IntegerToken : Token
    {
        public int intValue
        {
            get;
            private set;
        }

        public IntegerToken(int intValue) : base(TokenType.INTEGER)
        {
            this.intValue = intValue;
        }
    }
}