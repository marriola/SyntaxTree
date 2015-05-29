using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public enum TokenType
    {
        ERROR,
        ID, INTEGER, STRING, COMMENT,
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
        public int row
        {
            get;
            private set;
        }

        public int column
        {
            get;
            private set;
        }

        public TokenType type
        {
            get;
            protected set;
        }

        public Token(int row, int column, TokenType type)
        {
            this.row = row;
            this.column = column;
            this.type = type;
        }

        public override string ToString()
        {
            return type.ToString();
        }
    }

    public class IdentifierToken : Token
    {
        public string stringValue
        {
            get;
            private set;
        }

        public IdentifierToken(int row, int column, string stringValue)
            : base(row, column, TokenType.ID)
        {
            this.stringValue = stringValue;
        }

        public override string ToString()
        {
            return "ID '" + stringValue + "'";
        }
    }

    public class IntegerToken : Token
    {
        public int intValue
        {
            get;
            private set;
        }

        public IntegerToken(int row, int column, int intValue)
            : base(row, column, TokenType.INTEGER)
        {
            this.intValue = intValue;
        }

        public override string ToString()
        {
            return "INT " + intValue;
        }
    }

    public class StringToken : Token
    {
        public string stringValue
        {
            get;
            private set;
        }

        public StringToken(int row, int column, string stringValue)
            : base(row, column, TokenType.STRING)
        {
            this.stringValue = stringValue;
        }

        public override string ToString()
        {
            return "STRING '" + stringValue + "'";
        }
    }
    public class CommentToken : Token
    {
        public string stringValue
        {
            get;
            private set;
        }

        public CommentToken(int row, int column, string stringValue)
            : base(row, column, TokenType.COMMENT)
        {
            this.stringValue = stringValue;
        }

        public override string ToString()
        {
            return "COMMENT '" + stringValue + "'";
        }
    }
}
