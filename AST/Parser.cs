using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    class Parser
    {
        private List<Token> source;
        private int sourceIndex;
        private Token currentToken;

        private static readonly Dictionary<TokenType, ValueType> tokenToValueType = new Dictionary<TokenType, ValueType>()
        {
            {TokenType.INT, ValueType.INT},
            {TokenType.VOID, ValueType.VOID}
        };

        public Parser(List<Token> source)
        {
            this.source = source;
            this.currentToken = source[0];
            this.sourceIndex = 1;
        }

        ///////////////////////////////////////////////////////////////////////

        private Token NextToken()
        {
            return currentToken = source[sourceIndex++];
        }
        
        private Token Match(TokenType tokenType)
        {
            if (currentToken.type != tokenType)
            {
                throw ParseError(tokenType.ToString());
            }
            return NextToken();
        }

        ///////////////////////////////////////////////////////////////////////

        private SyntaxError ParseError(string expected)
        {
            return new SyntaxError(currentToken.row, currentToken.column, "Expected " + expected + ", got " + currentToken.type.ToString());
        }

        ///////////////////////////////////////////////////////////////////////

        private DeclarationNode ParseVariableDeclaration(Token typeSpecifier, IdentifierToken identifier)
        {
            if (currentToken.type == TokenType.SEMI)
            {
                Match(TokenType.SEMI);
                return new DeclarationNode(tokenToValueType[typeSpecifier.type], identifier.stringValue);
            }
            else if (currentToken.type == TokenType.LBRACK)
            {
                Match(TokenType.LBRACK);
                IntegerToken arraySize = (IntegerToken)Match(TokenType.INTEGER);
                Match(TokenType.RBRACK);

                return new ArrayDeclarationNode(tokenToValueType[typeSpecifier.type], identifier.stringValue, arraySize.intValue);
            }
            else
            {
                throw ParseError("SEMI or LBRACK");
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private ParameterNode ParseParameter()
        {
            Token typeSpecifier = currentToken;
            if (typeSpecifier.type != TokenType.INT &&
                typeSpecifier.type != TokenType.STRING)
            {
                throw ParseError("INT or STRING");
            }
            NextToken();

            IdentifierToken identifier = (IdentifierToken)currentToken;
            Match(TokenType.ID);

            bool isArray = currentToken.type == TokenType.LBRACK;
            if (isArray)
            {
                Match(TokenType.LBRACK);
                Match(TokenType.RBRACK);
            }

            return new ParameterNode(tokenToValueType[typeSpecifier.type], identifier.stringValue, isArray);
        }

        ///////////////////////////////////////////////////////////////////////

        private List<ParameterNode> ParseParameterList()
        {
            if (currentToken.type == TokenType.VOID)
            {
                // void parameter list
                Match(TokenType.VOID);
                Match(TokenType.RPAREN);
                return null;
            }
            else
            {
                List<ParameterNode> parameters = new List<ParameterNode>();
                while (true)
                {
                    parameters.Add(ParseParameter());
                    if (currentToken.type == TokenType.COMMA)
                    {
                        Match(TokenType.COMMA);
                    }
                    else
                    {
                        break;
                    }
                }
                Match(TokenType.RPAREN);
                return parameters;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private CompoundStatementNode ParseCompoundStatement()
        {
            return null;
        }

        ///////////////////////////////////////////////////////////////////////

        private FunctionDeclarationNode ParseFunctionDeclaration(Token typeSpecifier, IdentifierToken identifier)
        {
            Match(TokenType.LPAREN);
            List<ParameterNode> parameters = ParseParameterList();
            Match(TokenType.RPAREN);
            CompoundStatementNode body = ParseCompoundStatement();
            return new FunctionDeclarationNode(tokenToValueType[typeSpecifier.type], identifier.stringValue, parameters, body);
        }

        ///////////////////////////////////////////////////////////////////////

        public ProgramNode Parse()
        {
            ProgramNode program = new ProgramNode();
            while (sourceIndex < source.Count)
            {
                if (currentToken.type != TokenType.INT && currentToken.type != TokenType.VOID)
                {
                    throw ParseError("INT or VOID");
                }
                Token typeSpecifier = currentToken;
                Match(typeSpecifier.type);

                Match(TokenType.ID);
                IdentifierToken identifier = (IdentifierToken)currentToken;

                if (currentToken.type == TokenType.SEMI || currentToken.type == TokenType.LBRACK)
                {
                    program.children.Add(ParseVariableDeclaration(typeSpecifier, identifier));
                }
                else
                {
                    program.children.Add(ParseFunctionDeclaration(typeSpecifier, identifier));
                }
            }

            return null;
        }
    }
}
