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
        public int line { get; set; } = 0;
    }

    // true
    public class TrueExpression : Expression
    {
        public int line { get; set; } = 0;
    }

    // false
    public class FalseExpression : Expression
    {
        public int line { get; set; } = 0;
    }

    // vararg
    public class VarargExpression : Expression
    {
        public int line { get; set; } = 0;
    }

    // int
    public class IntegerExpression : Expression
    {
        public int line { get; set; } = 0;
        public long value { get; set; } = 0;

        public IntegerExpression(int line, int value)
        {
            this.line = line;
            this.value = value;
        }
    }

    // float
    public class FlaotExpression : Expression
    {
        public int line { get; set; } = 0;
        public double value { get; set; } = 0.0;
    }

    // string
    public class StringExpression : Expression
    {
        public int line { get; set; } = 0;
        public string value { get; set; } = string.Empty;
    }

    // name(identifier)
    public class NameExpression : Expression
    {
        public int line { get; set; } = 0;
        public string value { get; set; } = string.Empty;
    }

    // 一元运算符
    public class UniOpExpression : Expression
    {
        public int line { get; set; } = 0;
        public int op { get; set; } = 0;
        public Expression expression { get; set; } = null;
    }

    // 二元运算符
    public class BinOpExpression : Expression
    {
        public int line { get; set; } = 0;
        public int op { get; set; } = 0;
        public Expression a { get; set; } = null;
        public Expression b { get; set; } = null;
    }

    // 拼接运算符
    public class ConcatExpression : Expression
    {
        public int line { get; set; } = 0;
        public List<Expression> expressions { get; } = new List<Expression>();
    }

    // 构造表
    public class TableConstructionExpression : Expression
    {
        public int line { get; set; } = 0;
        public int lastline { get; set; } = 0;
        public Expression key { get; set; } = null;
        public Expression value { get; set; } = null;
    }

    // 函数定义
    public class FunctionDefineExpression : Expression
    {
        public int line { get; set; } = 0;
        public int lastline { get; set; } = 0;
        public List<Expression> args { get; } = new List<Expression>();
        public bool isVararg { get; set; } = false;
        public Block block { get; set; } = null;
    }

    // 圆括号
    public class ParensExpression : Expression
    {
        public Expression expression { get; set; } = null;
    }

    // 访问表
    public class TableAccessExpression : Expression
    {
        public int lastline { get; set; } = 0;
        public Expression prefix { get; set; } = null;
        public Expression key { get; set; } = null;
    }

    // 函数调用
    public class FunctionCallExpression : Expression
    {
        public int line { get; set; } = 0;
        public int lastline { get; set; } = 0;
        public Expression prefix { get; set; } = null;
        public StringExpression name { get; set; } = null;
        public List<Expression> args { get; } = new List<Expression>();
    }
}
