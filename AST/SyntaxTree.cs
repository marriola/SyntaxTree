using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    enum ValueType
    {
        INT, VOID
    }

    enum AdditiveOperator
    {
        PLUS, MINUS
    }

    enum MultiplicativeOperator
    {
        TIMES, DIVIDE
    }

    enum RelationalOperator
    {
        LTE, LT, EQ, NEQ, GT, GTE
    }

    enum UnaryOperator
    {
        INCREMENT, DECREMENT
    }

    class Node
    {
    }

    class ProgramNode : Node
    {
        public List<DeclarationNode> children;

        public ProgramNode()
        {
            children = new List<DeclarationNode>();
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    // Declaration nodes
    ///////////////////////////////////////////////////////////////////////////

    class DeclarationNode : Node
    {
        ValueType type;
        string identifier;

        public DeclarationNode(ValueType type, string identifier)
        {
            this.type = type;
            this.identifier = identifier;
        }
    }

    class ParameterNode : DeclarationNode
    {
        bool isArray;

        public ParameterNode(ValueType type, string identifier, bool isArray)
            : base(type, identifier)
        {
            this.isArray = isArray;
        }
    }

    class ArrayDeclarationNode : DeclarationNode
    {
        int size;

        public ArrayDeclarationNode(ValueType type, string identifier, int size)
            : base(type, identifier)
        {
            this.size = size;
        }
    }

    class FunctionDeclarationNode : DeclarationNode
    {
        List<ParameterNode> parameters;
        CompoundStatementNode body;

        public FunctionDeclarationNode(ValueType type, string identifier, List<ParameterNode> parameters, CompoundStatementNode body)
            : base(type, identifier)
        {
            this.parameters = parameters;
            this.body = body;
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    // Statement nodes
    ///////////////////////////////////////////////////////////////////////////

    class StatementNode : Node
    {
    }

    class CompoundStatementNode : StatementNode
    {
        List<DeclarationNode> localDeclarations;
        List<StatementNode> statements;
    }

    class IfStatementNode : StatementNode
    {
        ExpressionNode condition;
        StatementNode thenStatement;
        StatementNode elseStatement;
    }

    class ForStatementNode : StatementNode
    {
        ExpressionNode initializer;
        ExpressionNode test;
        ExpressionNode update;
        StatementNode body;
    }

    class WhileStatementNode : StatementNode
    {
        ExpressionNode test;
        StatementNode body;
    }

    ///////////////////////////////////////////////////////////////////////////
    // Expression nodes
    ///////////////////////////////////////////////////////////////////////////

    class ExpressionNode : Node
    {
        ValueType type;
        bool isArray;
    }

    class IntegerLiteralExpressionNode : ExpressionNode
    {
        int value;
    }

    class VariableExpressionNode : ExpressionNode
    {
        string name;
    }

    class SubscriptExpressionNode : VariableExpressionNode
    {
        int index;
    }

    class CallExpressionNode : ExpressionNode
    {
        string identifier;
        List<ExpressionNode> arguments;
    }

    class BinaryExpressionNode : ExpressionNode
    {
        ExpressionNode left;
        ExpressionNode right;
    }

    class AdditiveExpressionNode : BinaryExpressionNode
    {
        AdditiveOperator addop;
    }

    class MultiplicativeExpressionNode : BinaryExpressionNode
    {
        MultiplicativeOperator multop;
    }

    class RelationalExpressionNode : BinaryExpressionNode
    {
        RelationalOperator relop;
    }


    class UnaryExpressionNode : ExpressionNode
    {
        VariableExpressionNode variable;
        UnaryOperator unop;
    }
}