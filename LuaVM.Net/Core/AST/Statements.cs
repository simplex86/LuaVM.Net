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
        public int line { get; set; } = 0; 
    }

    // 标签
    public class LabelStatement : Statement
    {
        public string name { get; set; } = string.Empty;
    }

    // goto
    public class GotoStatement : Statement
    {
        public string name { get; set; } = string.Empty;
    }

    // do
    public class DoStatement : Statement
    {
        public Block block { get; set; } = null;
    }

    // 函数调用
    public class FunctionCallStatement : Statement
    {
        
    }

    // while
    public class WhileStatement : Statement
    {
        public Expression expression { get; set; } = null;
        public Block block { get; set; } = null;
    }

    // repeat
    public class RepeatStatement : Statement
    {
        public Expression expression { get; set; } = null;
        public Block block { get; set; } = null;
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
