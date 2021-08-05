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

        public NilExpression(int line)
        {
            this.line = line;
        }
    }

    // true
    public class TrueExpression : Expression
    {
        public int line { get; private set; } = 0;

        public TrueExpression(int line)
        {
            this.line = line;
        }
    }

    // false
    public class FalseExpression : Expression
    {
        public int line { get; private set; } = 0;

        public FalseExpression(int line)
        {
            this.line = line;
        }
    }

    // vararg
    public class VarargExpression : Expression
    {
        public int line { get; private set; } = 0;

        public VarargExpression(int line)
        {
            this.line = line;
        }
    }

    // int
    public class IntegerExpression : Expression
    {
        public int line { get; private set; } = 0;
        public long value { get; private set; } = 0;

        public IntegerExpression(int line, long value)
        {
            this.line = line;
            this.value = value;
        }
    }

    // float
    public class FloatExpression : Expression
    {
        public int line { get; private set; } = 0;
        public double value { get; private set; } = 0.0;

        public FloatExpression(int line, double value)
        {
            this.line = line;
            this.value = value;
        }
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
    public class UnaryOpExpression : Expression
    {
        public int line { get; private set; } = 0;
        public int op { get; private set; } = 0;
        public Expression expression { get; private set; } = null;

        public UnaryOpExpression(int line, int op, Expression expression)
        {
            this.line = line;
            this.op = op;
            this.expression = expression;
        }
    }

    // 二元运算符
    public class BinOpExpression : Expression
    {
        public int line { get; private set; } = 0;
        public int op { get; private set; } = 0;
        public Expression a { get; private set; } = null;
        public Expression b { get; private set; } = null;

        public BinOpExpression(int line, int op, Expression a, Expression b)
        {
            this.line = line;
            this.op = op;
            this.a = a;
            this.b = b;
        }
    }

    // 拼接运算符
    public class ConcatExpression : Expression
    {
        public int line { get; private set; } = 0;
        public List<Expression> expressions { get; private set; } = null;

        public ConcatExpression(int line, List<Expression> expressions)
        {
            this.line = line;
            expressions = expressions;
        }
    }

    // 构造表
    public class TableConstructionExpression : Expression
    {
        public int line { get; private set; } = 0;
        public int lastline { get; private set; } = 0;
        public List<Expression> keys { get; private set; } = null;
        public List<Expression> values { get; private set; } = null;

        public TableConstructionExpression(int line, int lastline, List<Expression> keys, List<Expression> values)
        {
            this.line = line;
            this.lastline = lastline;
            this.keys = keys;
            this.values = values;
        }
    }

    // 函数定义
    public class FunctionDefineExpression : Expression
    {
        public int line { get; private set; } = 0;
        public int lastline { get; private set; } = 0;
        public List<string> args { get; private set; } = null;
        public bool isVararg { get; private set; } = false;
        public Block block { get; private set; } = null;

        public FunctionDefineExpression(int line, int lastline, List<string> args, bool isVararg, Block block)
        {
            this.line = line;
            this.lastline = lastline;
            this.args = args;
            this.isVararg = isVararg;
            this.block = block;
        }
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
