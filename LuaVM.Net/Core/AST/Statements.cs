using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaVM.Net.Core
{
    // 基类
    public class Statement
    {

    }

    // 空语句
    public class EmptyStatement : Statement
    {
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
        public List<Expression> expressions { get; } = new List<Expression>();
        public List<Block> blocks { get; } = new List<Block>();
    }

    // for i=n, m do
    public class ForNumStatement : Statement
    {
        public int lineOfFor { get; set; } = 0;
        public int lineOfDo { get; set; } = 0;
        public string varname { get; set; } = string.Empty;
        public Expression initExpression { get; set; } = null;
        public Expression limitExpression { get; set; } = null;
        public Expression stepExpression { get; set; } = null;
        public Block block { get; set; } = null;
    }

    // for k, v in pairs(table) do
    public class ForInStatement : Statement
    {
        public int lineOfDo { get; set; } = 0;
        public List<string> names { get; } = new List<string>();
        public List<Expression> expressions { get; } = new List<Expression>();
        public Block block { get; set; } = null;
    }

    // local var
    public class LocalVarStatement : Statement
    {
        public int lastline { get; set; } = 0;
        public List<string> names { get; } = new List<string>();
        public List<Expression> expressions { get; } = new List<Expression>();
    }

    // assign
    public class AssignStatement : Statement
    {
        public int lastline { get; set; } = 0;
        public List<Expression> vars { get; } = new List<Expression>();
        public List<Expression> expressions { get; } = new List<Expression>();
    }

    // local function
    public class LocalFunctionStatement : Statement
    {
        public string name { get; set; } = string.Empty;
        public Expression expression { get; set; } = null;
    }
}
