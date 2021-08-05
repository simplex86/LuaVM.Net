using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 赋值和函数调用
    class AssignOrFunctionCallStatementParser : IBaseStatementParser
    {
        PrefixExpressionParser prefixParser = new PrefixExpressionParser();

        public Statement Parse(Lexer lexer)
        {
            var prefix = prefixParser.Parse(lexer);
            if (prefix.GetType() == typeof(FunctionCallExpression))
            {
                var exp = prefix as FunctionCallExpression;
                return new FunctionCallStatement(exp);
            }

            return ParseAssignStatement(lexer, prefix);
        }

        // 
        private Statement ParseAssignStatement(Lexer lexer, Expression exp)
        {
            var vars = FinishVars(lexer, exp);
            lexer.NextTokenOfType(TokenType.OP_ASSIGN);
            var exps = ExpressionUtils.ParseExpressions(lexer);

            return new AssignStatement(lexer.line, vars, exps);
        }

        private List<Expression> FinishVars(Lexer lexer, Expression exp)
        {
            List<Expression> vars = new List<Expression>();

            exp = CheckVar(lexer, exp);
            vars.Add(exp);

            while (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                exp = prefixParser.Parse(lexer);
                exp = CheckVar(lexer, exp);
                vars.Add(exp);
            }

            return vars;
        }

        private Expression CheckVar(Lexer lexer, Expression exp)
        {
            var type = exp.GetType();

            if (type == typeof(NameExpression) ||
                type == typeof(TableAccessExpression))
            {
                return exp;
            }

            lexer.NextTokenOfType(-1);
            Error.Commit("unreachable");

            return null;
        }
    }
}
