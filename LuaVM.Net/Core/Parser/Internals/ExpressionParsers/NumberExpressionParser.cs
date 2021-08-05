using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Net.Core
{
    class NumberExpressionParser : IBaseExpressionParser
    {
        public Expression Parse(Lexer lexer)
        {
            var token = lexer.NextToken();

            long I = 0;
            double F = 0.0;

            if (long.TryParse(token.text, out I))
            {
                return new IntegerExpression(token.line, I);
            }
            else if (double.TryParse(token.text, out F))
            {
                return new FloatExpression(token.line, F);
            }
            else
            {
                Error.Commit($"parser error: {token.text} is not a number");
            }

            return null;
        }
    }
}
