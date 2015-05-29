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
        public ValueType type
        {
            get;
            private set;
        }

        public string identifier
        {
            get;
            private set;
        }

        public DeclarationNode(ValueType type, string identifier)
        {
            this.type = type;
            this.identifier = identifier;
        }
    }

    class ParameterNode : DeclarationNode
    {
        public bool isArray
        {
            get;
            private set;
        }

        public ParameterNode(ValueType type, string identifier, bool isArray)
            : base(type, identifier)
        {
            this.isArray = isArray;
        }
    }

    class ArrayDeclarationNode : DeclarationNode
    {
        public int size
        {
            get;
            private set;
        }

        public ArrayDeclarationNode(ValueType type, string identifier, int size)
            : base(type, identifier)
        {
            this.size = size;
        }
    }

    class FunctionDeclarationNode : DeclarationNode
    {
        public List<ParameterNode> parameters
        {
            get;
            private set;
        }

        public CompoundStatementNode body
        {
            get;
            private set;
        }

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
        public List<DeclarationNode> localDeclarations
        {
            get;
            private set;
        }

        public List<StatementNode> statementList
        {
            get;
            private set;
        }

        public CompoundStatementNode(List<DeclarationNode> localDeclarations, List<StatementNode> statementList)
        {
            this.localDeclarations = localDeclarations;
            this.statementList = statementList;
        }
    }

    class IfStatementNode : StatementNode
    {
        public ExpressionNode condition
        {
            get;
            private set;
        }

        public StatementNode thenStatement
        {
            get;
            private set;
        }

        public StatementNode elseStatement
        {
            get;
            private set;
        }
    }

    class ForStatementNode : StatementNode
    {
        public ExpressionNode initializer
        {
            get;
            private set;
        }

        public ExpressionNode test
        {
            get;
            private set;
        }

        public ExpressionNode update
        {
            get;
            private set;
        }

        public StatementNode body
        {
            get;
            private set;
        }
    }

    class WhileStatementNode : StatementNode
    {
        public ExpressionNode test
        {
            get;
            private set;
        }

        public StatementNode body
        {
            get;
            private set;
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    // Expression nodes
    ///////////////////////////////////////////////////////////////////////////

    class ExpressionNode : Node
    {
        public ValueType type
        {
            get;
            private set;
        }

        public bool isArray
        {
            get;
            private set;
        }
    }

    class IntegerLiteralExpressionNode : ExpressionNode
    {
        public int value
        {
            get;
            private set;
        }
    }

    class VariableExpressionNode : ExpressionNode
    {
        public string name
        {
            get;
            private set;
        }
    }

    class SubscriptExpressionNode : VariableExpressionNode
    {
        public int index
        {
            get;
            private set;
        }
    }

    class CallExpressionNode : ExpressionNode
    {
        public string identifier
        {
            get;
            private set;
        }

        public List<ExpressionNode> arguments
        {
            get;
            private set;
        }
    }

    class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode left
        {
            get;
            private set;
        }

        public ExpressionNode right
        {
            get;
            private set;
        }
    }

    class AdditiveExpressionNode : BinaryExpressionNode
    {
        public AdditiveOperator addop
        {
            get;
            private set;
        }
    }

    class MultiplicativeExpressionNode : BinaryExpressionNode
    {
        public MultiplicativeOperator multop
        {
            get;
            private set;
        }
    }

    class RelationalExpressionNode : BinaryExpressionNode
    {
        public RelationalOperator relop
        {
            get;
            private set;
        }
    }


    class UnaryExpressionNode : ExpressionNode
    {
        public VariableExpressionNode variable
        {
            get;
            private set;
        }
        
        public UnaryOperator unop
        {
            get;
            private set;
        }
    }
}