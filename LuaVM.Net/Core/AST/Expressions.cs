using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaVM.Net.Core
{
    // 基类
    public class Expression
    {

    }

    // nil
    public class NilExpression : Expression
    {
        public int line { get; private set; } = 0;
    }

    // true
    public class TrueExpression : Expression
    {
        public int line { get; private set; } = 0;
    }

    // false
    public class FalseExpression : Expression
    {
        public int line { get; private set; } = 0;
    }

    // vararg
    public class VarargExpression : Expression
    {
        public int line { get; private set; } = 0;
    }

    // int
    public class IntegerExpression : Expression
    {
        public int line { get; private set; } = 0;
        public long value { get; private set; } = 0;

        public IntegerExpression(int line, int value)
        {
            this.line = line;
            this.value = value;
        }
    }

    // float
    public class FlaotExpression : Expression
    {
        public int line { get; private set; } = 0;
        public double value { get; private set; } = 0.0;
    }

    // string
    public class StringExpression : Expression
    {
        public int line { get; private set; } = 0;
        public string value { get; private set; } = string.Empty;

        public StringExpression(int line, string value)
        {
            this.line = line;
            this.value = value;
        }
    }

    // name(identifier)
    public class NameExpression : Expression
    {
        public int line { get; private set; } = 0;
        public string value { get; private set; } = string.Empty;

        public NameExpression(int line, string value)
        {
            this.line = line;
            this.value = value;
        }
    }

    // 一元运算符
    public class UniOpExpression : Expression
    {
        public int line { get; private set; } = 0;
        public int op { get; private set; } = 0;
        public Expression expression { get; private set; } = null;
    }

    // 二元运算符
    public class BinOpExpression : Expression
    {
        public int line { get; private set; } = 0;
        public int op { get; private set; } = 0;
        public Expression a { get; private set; } = null;
        public Expression b { get; private set; } = null;
    }

    // 拼接运算符
    public class ConcatExpression : Expression
    {
        public int line { get; private set; } = 0;
        public List<Expression> expressions { get; private set; } = new List<Expression>();
    }

    // 构造表
    public class TableConstructionExpression : Expression
    {
        public int line { get; private set; } = 0;
        public int lastline { get; private set; } = 0;
        public Expression key { get; private set; } = null;
        public Expression value { get; private set; } = null;
    }

    // 函数定义
    public class FunctionDefineExpression : Expression
    {
        public int line { get; private set; } = 0;
        public int lastline { get; private set; } = 0;
        public List<Expression> args { get; private set; } = new List<Expression>();
        public bool isVararg { get; private set; } = false;
        public Block block { get; private set; } = null;
    }

    // 圆括号
    public class ParensExpression : Expression
    {
        public Expression expression { get; private set; } = null;

        public ParensExpression(Expression expression)
        {
            this.expression = expression;
        }
    }

    // 访问表
    public class TableAccessExpression : Expression
    {
        public int lastline { get; private set; } = 0;
        public Expression prefix { get; private set; } = null;
        public Expression key { get; private set; } = null;

        public TableAccessExpression(int lastline, Expression prefix, Expression key)
        {
            this.lastline = lastline;
            this.prefix = prefix;
            this.key = key;
        }
    }

    // 函数调用
    public class FunctionCallExpression : Expression
    {
        public int line { get; private set; } = 0;
        public int lastline { get; private set; } = 0;
        public Expression prefix { get; private set; } = null;
        public StringExpression name { get; private set; } = null;
        public List<Expression> args { get; private set; } = null;

        public FunctionCallExpression(int line, int lastline, Expression prefix, StringExpression name, List<Expression> args)
        {
            this.line = line;
            this.lastline = lastline;
            this.prefix = prefix;
            this.name = name;
            this.args = args;
        }
    }
}
