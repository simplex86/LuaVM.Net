using System;
using System.Collections.Generic;
using System.Text;

namespace LuaVM.Net.Core
{
    // Token类型
    public static class TokenType
    {
        public const int EOF            = 1;    // EOF  end-of-file
        public const int VARARG         = 2;    // ...

        public const int SEP_SEMI       = 3;    // ;
        public const int SEP_COMMA      = 4;    // ,
        public const int SEP_DOT        = 5;    // .
        public const int SEP_COLON      = 6;    // :
        public const int SEP_LABEL      = 7;    // ::
        public const int SEP_LPAREN     = 8;    // (
        public const int SEP_RPAREN     = 9;    // )
        public const int SEP_LBRACK     = 10;    // [
        public const int SEP_RBRACK     = 11;   // ]
        public const int SEP_LCURLY     = 12;   // {
        public const int SEP_RCURLY     = 13;   // }

        public const int OP_ASSIGN      = 14;   // =
        public const int OP_MINUS       = 15;   // -（负号或者减号）
        public const int OP_WAVE        = 16;   // ~
        public const int OP_ADD         = 17;   // +
        public const int OP_MUL         = 18;   // *
        public const int OP_DIV         = 19;   // /
        public const int OP_IDIV        = 20;   // //
        public const int OP_POW         = 21;   // ^
        public const int OP_MOD         = 22;   // %
        public const int OP_BIT_AND     = 23;   // &
        public const int OP_BIT_OR      = 24;   // |
        public const int OP_BIT_SHR     = 25;   // >>
        public const int OP_BIT_SHL     = 26;   // <<
        public const int OP_CONCAT      = 27;   // ..
        public const int OP_LT          = 28;   // <
        public const int OP_LE          = 29;   // <=
        public const int OP_GT          = 30;   // >
        public const int OP_GE          = 31;   // >=
        public const int OP_EQ          = 32;   // ==
        public const int OP_NE          = 33;   // ~=
        public const int OP_LEN         = 34;   // #
        public const int OP_AND         = 35;   // and
        public const int OP_OR          = 36;   // or
        public const int OP_NOT         = 37;   // not

        public const int KW_BREAK       = 38;   // break
        public const int KW_DO          = 39;   // do
        public const int KW_ELSE        = 40;   // else
        public const int KW_ELSEIF      = 41;   // elseif
        public const int KW_END         = 42;   // end
        public const int KW_FALSE       = 43;   // false
        public const int KW_FOR         = 44;   // for
        public const int KW_FUNCTION    = 45;   // function
        public const int KW_GOTO        = 46;   // goto
        public const int KW_IF          = 47;   // if
        public const int KW_IN          = 48;   // in
        public const int KW_LOCAL       = 49;   // local
        public const int KW_NIL         = 50;   // nil
        public const int KW_REPEAT      = 51;   // repeat
        public const int KW_RETURN      = 52;   // return
        public const int KW_THEN        = 53;   // then
        public const int KW_TRUE        = 54;   // true
        public const int KW_UNTIL       = 55;   // until
        public const int KW_WHILE       = 56;   // while

        public const int IDENTIFIER     = 57;   // identifier
        public const int NUMBER         = 58;   // number literal
        public const int STRING         = 59;   // string literal

        public const int OP_UNM         = 15;   // unary minus
        public const int OP_SUB         = 15;   // 
        public const int OP_BIT_NOT     = 16;   // 
        public const int OP_BIT_XOR     = 16;   // 
    }

    // 关键字
    public static class Keywords
    {
        private static readonly Dictionary<string, int> keywords = new Dictionary<string, int>() {
            { "and",        TokenType.OP_AND        },
            { "break",      TokenType.KW_BREAK      },
            { "do",         TokenType.KW_DO         },
            { "else",       TokenType.KW_ELSE       },
            { "elseif",     TokenType.KW_ELSEIF     },
            { "end",        TokenType.KW_END        },
            { "false",      TokenType.KW_FALSE      },
            { "for",        TokenType.KW_FOR        },
            { "function",   TokenType.KW_FUNCTION   },
            { "goto",       TokenType.KW_GOTO       },
            { "if",         TokenType.KW_IF         },
            { "in",         TokenType.KW_IN         },
            { "local",      TokenType.KW_LOCAL      },
            { "nil",        TokenType.KW_NIL        },
            { "not",        TokenType.OP_NOT        },
            { "or",         TokenType.OP_OR         },
            { "repeat",     TokenType.KW_REPEAT     },
            { "return",     TokenType.KW_RETURN     },
            { "then",       TokenType.KW_THEN       },
            { "true",       TokenType.KW_TRUE       },
            { "until",      TokenType.KW_UNTIL      },
            { "while",      TokenType.KW_WHILE      },
        };

        // 是否包含
        public static bool Has(string word)
        {
            return keywords.ContainsKey(word);
        }

        // 获取值
        public static int Get(string word)
        {
            if (Has(word))
            {
                return keywords[word];
            }
            return 0;
        }
    }
}
