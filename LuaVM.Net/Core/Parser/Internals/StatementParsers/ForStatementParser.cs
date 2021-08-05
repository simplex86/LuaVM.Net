using System;

namespace LuaVM.Net.Core
{
    // for
    class ForStatementParser : IBaseStatementParser
    {
        private BlockParser blockParser = new BlockParser();

        public Statement Parse(Lexer lexer)
        {
            var forToken = lexer.NextTokenOfType(TokenType.KW_FOR); // for
            var nameToken = lexer.NextIdentifier();

            if (lexer.LookAhead() == TokenType.OP_ASSIGN)
            {
                return FinishForNum(lexer, forToken.line, nameToken.text);
            }

            return FinishForIn(lexer, nameToken.text);
        }

        // for i=init, limit, step do
        //     ...
        // end
        private Statement FinishForNum(Lexer lexer, int lineOfFor, string varname)
        {
            lexer.NextTokenOfType(TokenType.OP_ASSIGN); // '='
            var initExp = ExpressionUtils.ParseExpression(lexer);
            lexer.NextTokenOfType(TokenType.SEP_COMMA); // ','
            var limitExp = ExpressionUtils.ParseExpression(lexer);

            Expression stepExp = null;
            if (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                stepExp = ExpressionUtils.ParseExpression(lexer);
            }
            else
            {
                stepExp = new IntegerExpression(lexer.line, 1);
            }

            var token = lexer.NextTokenOfType(TokenType.KW_DO); // do
            var block = blockParser.Parse(lexer);
            lexer.NextTokenOfType(TokenType.KW_END);

            return new ForNumStatement(lineOfFor,
                                       token.line,
                                       varname,
                                       initExp,
                                       limitExp,
                                       stepExp,
                                       block);
        }

        // for in do
        //     ...
        // end
        private Statement FinishForIn(Lexer lexer, string name)
        {
            var names = VariateUtils.GetNames(lexer, name);
            lexer.NextTokenOfType(TokenType.KW_IN); // in
            var exps = ExpressionUtils.ParseExpressions(lexer);
            var token = lexer.NextTokenOfType(TokenType.KW_DO); // do
            var block = blockParser.Parse(lexer);
            lexer.NextTokenOfType(TokenType.KW_END); // end

            return new ForInStatement(token.line, names, exps, block);
        }
    }
}
