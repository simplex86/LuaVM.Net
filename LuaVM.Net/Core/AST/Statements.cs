using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaVM.Net.Core
{
    // 基类
    public class Statement
    {
        protected Statement()
        {

        }
    }

    // 空语句
    public class EmptyStatement : Statement
    {
        public EmptyStatement()
        {

        }
    }

    // break
    public class BreakStatement : Statement
    {
        public int line { get; private set; } = 0; 

        public BreakStatement(int line)
        {
            this.line = line;
        }
    }

    // 标签
    public class LabelStatement : Statement
    {
        public string name { get; private set; } = string.Empty;

        public LabelStatement(string name)
        {
            this.name = name;
        }
    }

    // goto
    public class GotoStatement : Statement
    {
        public string name { get; private set; } = string.Empty;

        public GotoStatement(string name)
        {
            this.name = name;
        }
    }

    // do
    public class DoStatement : Statement
    {
        public Block block { get; private set; } = null;

        public DoStatement(Block block)
        {
            this.block = block;
        }
    }

    // 函数调用
    public class FunctionCallStatement : Statement
    {
        public FunctionCallExpression expression { get; private set; } = null;

        public FunctionCallStatement(Expression expression)
        {
            this.expression = expression as FunctionCallExpression;
        }
    }

    // while
    public class WhileStatement : Statement
    {
        public Expression expression { get; private set; } = null;
        public Block block { get; private set; } = null;

        public WhileStatement(Expression expression, Block block)
        {
            this.expression = expression;
            this.block = block;
        }
    }

    // repeat
    public class RepeatStatement : Statement
    {
        public Expression expression { get; private set; } = null;
        public Block block { get; private set; } = null;

        public RepeatStatement(Expression expression, Block block)
        {
            this.expression = expression;
            this.block = block;
        }
    }

    // if
    public class IfStatement : Statement
    {
        public List<Expression> expressions { get; private set; } = new List<Expression>();
        public List<Block> blocks { get; private set; } = new List<Block>();

        public IfStatement(List<Expression> expressions, List<Block> blocks)
        {
            this.expressions = expressions;
            this.blocks = blocks;
        }
    }

    // for i=n, m do
    public class ForNumStatement : Statement
    {
        public int lineOfFor { get; private set; } = 0;
        public int lineOfDo { get; private set; } = 0;
        public string varname { get; private set; } = string.Empty;
        public Expression initExpression { get; private set; } = null;
        public Expression limitExpression { get; private set; } = null;
        public Expression stepExpression { get; private set; } = null;
        public Block block { get; private set; } = null;

        public ForNumStatement(int lineOfFor, 
                               int lineOfDo, 
                               string varname, 
                               Expression initExp, 
                               Expression limitExp, 
                               Expression stepExp, 
                               Block block)
        {
            this.lineOfFor = lineOfFor;
            this.lineOfDo = lineOfDo;
            this.varname = varname;
            this.initExpression = initExp;
            this.limitExpression = limitExp;
            this.stepExpression = stepExp;
            this.block = block;
        }
    }

    // for k, v in pairs(table) do
    public class ForInStatement : Statement
    {
        public int lineOfDo { get; private set; } = 0;
        public List<string> names { get; private set; } = new List<string>();
        public List<Expression> expressions { get; private set; } = new List<Expression>();
        public Block block { get; private set; } = null;

        public ForInStatement(int lineOfDo, List<string> names, List<Expression> expressions, Block block)
        {
            this.lineOfDo = lineOfDo;
            this.names = names;
            this.expressions = expressions;
            this.block = block;
        }
    }

    // local var
    public class LocalVariateStatement : Statement
    {
        public int lastline { get; set; } = 0;
        public List<string> names { get; } = new List<string>();
        public List<Expression> expressions { get; } = new List<Expression>();

        public LocalVariateStatement(int lastline, List<string> names, List<Expression> expressions)
        {
            this.lastline = lastline;
            this.names = names;
            this.expressions = expressions;
        }
    }

    // assign
    public class AssignStatement : Statement
    {
        public int lastline { get; set; } = 0;
        public List<Expression> vars { get; } = new List<Expression>();
        public List<Expression> expressions { get; } = new List<Expression>();
    }

    // local function
    public class LocalFunctionDefineStatement : Statement
    {
        public string name { get; private set; } = string.Empty;
        public Expression expression { get; private set; } = null;

        public LocalFunctionDefineStatement(string name, Expression exp)
        {
            this.name = name;
            this.expression = exp;
        }
    }
}
