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
        List<DeclarationNode> children;
    }

    ///////////////////////////////////////////////////////////////////////////
    // Declaration nodes
    ///////////////////////////////////////////////////////////////////////////

    class DeclarationNode : Node
    {
        ValueType type;
        string name;
    }

    class ParameterNode : DeclarationNode
    {
        bool isArray;
    }
    
    class ArrayDeclarationNode : DeclarationNode
    {
        int size;
    }

    class FunctionDeclarationNode : DeclarationNode
    {
        List<ParameterNode> parameters;
        CompoundStatementNode body;
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

    ///////////////////////////////////////////////////////////////////////////

    class Parser
    {
    }
}
