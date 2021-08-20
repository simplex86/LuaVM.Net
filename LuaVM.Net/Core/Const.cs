using System;
using System.Collections.Generic;
using System.Text;

namespace LuaVM.Net.Core
{
    // Token类型
    internal static class TokenType
    {
        internal const int EOF            = 1;    // EOF  end-of-file
        internal const int VARARG         = 2;    // ...

        internal const int SEP_SEMI       = 3;    // ;
        internal const int SEP_COMMA      = 4;    // ,
        internal const int SEP_DOT        = 5;    // .
        internal const int SEP_COLON      = 6;    // :
        internal const int SEP_LABEL      = 7;    // ::
        internal const int SEP_LPAREN     = 8;    // (
        internal const int SEP_RPAREN     = 9;    // )
        internal const int SEP_LBRACK     = 10;    // [
        internal const int SEP_RBRACK     = 11;   // ]
        internal const int SEP_LCURLY     = 12;   // {
        internal const int SEP_RCURLY     = 13;   // }

        internal const int OP_ASSIGN      = 14;   // =
        internal const int OP_MINUS       = 15;   // -（负号或者减号）
        internal const int OP_WAVE        = 16;   // ~
        internal const int OP_ADD         = 17;   // +
        internal const int OP_MUL         = 18;   // *
        internal const int OP_DIV         = 19;   // /
        internal const int OP_IDIV        = 20;   // //
        internal const int OP_POW         = 21;   // ^
        internal const int OP_MOD         = 22;   // %
        internal const int OP_BIT_AND     = 23;   // &
        internal const int OP_BIT_OR      = 24;   // |
        internal const int OP_BIT_SHR     = 25;   // >>
        internal const int OP_BIT_SHL     = 26;   // <<
        internal const int OP_CONCAT      = 27;   // ..
        internal const int OP_LT          = 28;   // <
        internal const int OP_LE          = 29;   // <=
        internal const int OP_GT          = 30;   // >
        internal const int OP_GE          = 31;   // >=
        internal const int OP_EQ          = 32;   // ==
        internal const int OP_NE          = 33;   // ~=
        internal const int OP_LEN         = 34;   // #
        internal const int OP_AND         = 35;   // and
        internal const int OP_OR          = 36;   // or
        internal const int OP_NOT         = 37;   // not

        internal const int KW_BREAK       = 38;   // break
        internal const int KW_DO          = 39;   // do
        internal const int KW_ELSE        = 40;   // else
        internal const int KW_ELSEIF      = 41;   // elseif
        internal const int KW_END         = 42;   // end
        internal const int KW_FALSE       = 43;   // false
        internal const int KW_FOR         = 44;   // for
        internal const int KW_FUNCTION    = 45;   // function
        internal const int KW_GOTO        = 46;   // goto
        internal const int KW_IF          = 47;   // if
        internal const int KW_IN          = 48;   // in
        internal const int KW_LOCAL       = 49;   // local
        internal const int KW_NIL         = 50;   // nil
        internal const int KW_REPEAT      = 51;   // repeat
        internal const int KW_RETURN      = 52;   // return
        internal const int KW_THEN        = 53;   // then
        internal const int KW_TRUE        = 54;   // true
        internal const int KW_UNTIL       = 55;   // until
        internal const int KW_WHILE       = 56;   // while

        internal const int IDENTIFIER     = 57;   // identifier
        internal const int NUMBER         = 58;   // number literal
        internal const int STRING         = 59;   // string literal

        internal const int OP_UNM         = 15;   // unary minus
        internal const int OP_SUB         = 15;   // 
        internal const int OP_BIT_NOT     = 16;   // 
        internal const int OP_BIT_XOR     = 16;   // 
    }

    // 关键字
    internal static class Keywords
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
        internal static bool Has(string word)
        {
            return keywords.ContainsKey(word);
        }

        // 获取值
        internal static int Get(string word)
        {
            if (Has(word))
            {
                return keywords[word];
            }
            return 0;
        }
    }

    // 操作码
    internal static class OperationCodes
    {
        internal const byte OP_MOVE     = 0;
        internal const byte OP_LOADK    = 1;
        internal const byte OP_LOADKX   = 2;
        internal const byte OP_LOADBOOL = 3;
        internal const byte OP_LOADNIL  = 4;
        internal const byte OP_GETUPVAL = 5;
        internal const byte OP_GETTABUP = 6;
        internal const byte OP_GETTABLE = 7;
        internal const byte OP_SETTABUP = 8;
        internal const byte OP_SETUPVAL = 9;
        internal const byte OP_SETTABLE = 10;
        internal const byte OP_NEWTABLE = 11;
        internal const byte OP_SELF     = 12;
        internal const byte OP_ADD      = 13;
        internal const byte OP_SUB      = 14;
        internal const byte OP_MUL      = 15;
        internal const byte OP_MOD      = 16;
        internal const byte OP_POW      = 17;
        internal const byte OP_DIV      = 18;
        internal const byte OP_IDIV     = 19;
        internal const byte OP_BAND     = 20;
        internal const byte OP_BOR      = 21;
        internal const byte OP_BXOR     = 22;
        internal const byte OP_SHL      = 23;
        internal const byte OP_SHR      = 24;
        internal const byte OP_UNM      = 25;
        internal const byte OP_BNOT     = 26;
        internal const byte OP_NOT      = 27;
        internal const byte OP_LEN      = 28;
        internal const byte OP_CONCAT   = 29;
        internal const byte OP_JMP      = 30;
        internal const byte OP_EQ       = 31;
        internal const byte OP_LT       = 32;
        internal const byte OP_LE       = 33;
        internal const byte OP_TEST     = 34;
        internal const byte OP_TESTSET  = 35;
        internal const byte OP_CALL     = 36;
        internal const byte OP_TAILCALL = 37;
        internal const byte OP_RETURN   = 38;
        internal const byte OP_FORLOOP  = 39;
        internal const byte OP_FORPREP  = 40;
        internal const byte OP_TFORCALL = 41;
        internal const byte OP_TFORLOOP = 42;
        internal const byte OP_SETLIST  = 43;
        internal const byte OP_CLOSURE  = 44;
        internal const byte OP_VARARG   = 45;
        internal const byte OP_EXTRAARG = 46;

        private static readonly OperationCode[] codes = new OperationCode[] {
            /*                T  A         B                C                 mode             name       invoke    */
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.N, InstructionMode.IABC,  "MOVE    ", VM.Move),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.N, InstructionMode.IABx,  "LOADK   ", VM.LoadK),
            new OperationCode(0, 1, OperationArgs.N, OperationArgs.N, InstructionMode.IABx,  "LOADKX  ", VM.LoadKx),
            new OperationCode(0, 1, OperationArgs.U, OperationArgs.U, InstructionMode.IABC,  "LOADBOOL", VM.LoadBoolean),
            new OperationCode(0, 1, OperationArgs.U, OperationArgs.N, InstructionMode.IABC,  "LOADNIL ", VM.LoadNil),
            new OperationCode(0, 1, OperationArgs.U, OperationArgs.N, InstructionMode.IABC,  "GETUPVAL", VM.Func),
            new OperationCode(0, 1, OperationArgs.U, OperationArgs.K, InstructionMode.IABC,  "GETTABUP", VM.Func),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.K, InstructionMode.IABC,  "GETTABLE", VM.GetTable),
            new OperationCode(0, 0, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "SETTABUP", VM.Func),
            new OperationCode(0, 0, OperationArgs.U, OperationArgs.N, InstructionMode.IABC,  "SETUPVAL", VM.Func),
            new OperationCode(0, 0, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "SETTABLE", VM.SetTable),
            new OperationCode(0, 1, OperationArgs.U, OperationArgs.U, InstructionMode.IABC,  "NEWTABLE", VM.NewTable),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.K, InstructionMode.IABC,  "SELF    ", VM.Func),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "ADD     ", VM.Add),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "SUB     ", VM.Sub),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "MUL     ", VM.Mul),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "MOD     ", VM.Mod),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "POW     ", VM.Pow),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "DIV     ", VM.Div),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "IDIV    ", VM.IDiv),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "BAND    ", VM.BAnd),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "BOR     ", VM.BOr),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "BXOR    ", VM.BXor),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "SHL     ", VM.Shl),
            new OperationCode(0, 1, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "SHR     ", VM.Shr),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.N, InstructionMode.IABC,  "UNM     ", VM.Unm),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.N, InstructionMode.IABC,  "BNOT    ", VM.BNot),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.N, InstructionMode.IABC,  "NOT     ", VM.Not),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.N, InstructionMode.IABC,  "LEN     ", VM.Len),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.R, InstructionMode.IABC,  "CONCAT  ", VM.Concat),
            new OperationCode(0, 0, OperationArgs.R, OperationArgs.N, InstructionMode.IAsBx, "JMP     ", VM.Jump),
            new OperationCode(1, 0, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "EQ      ", VM.EQ),
            new OperationCode(1, 0, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "LT      ", VM.LT),
            new OperationCode(1, 0, OperationArgs.K, OperationArgs.K, InstructionMode.IABC,  "LE      ", VM.LE),
            new OperationCode(1, 0, OperationArgs.N, OperationArgs.U, InstructionMode.IABC,  "TEST    ", VM.Test),
            new OperationCode(1, 1, OperationArgs.R, OperationArgs.U, InstructionMode.IABC,  "TESTSET ", VM.TestSet),
            new OperationCode(0, 1, OperationArgs.U, OperationArgs.U, InstructionMode.IABC,  "CALL    ", VM.Func),
            new OperationCode(0, 1, OperationArgs.U, OperationArgs.U, InstructionMode.IABC,  "TAILCALL", VM.Func),
            new OperationCode(0, 0, OperationArgs.U, OperationArgs.N, InstructionMode.IABC,  "RETURN  ", VM.Func),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.N, InstructionMode.IAsBx, "FORLOOP ", VM.ForLoop),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.N, InstructionMode.IAsBx, "FORPREP ", VM.ForPrep),
            new OperationCode(0, 0, OperationArgs.N, OperationArgs.U, InstructionMode.IABC,  "TFORCALL", VM.Func),
            new OperationCode(0, 1, OperationArgs.R, OperationArgs.N, InstructionMode.IAsBx, "TFORLOOP", VM.Func),
            new OperationCode(0, 0, OperationArgs.U, OperationArgs.U, InstructionMode.IABC,  "SETLIST ", VM.SetList),
            new OperationCode(0, 1, OperationArgs.U, OperationArgs.N, InstructionMode.IABx,  "CLOSURE ", VM.Func),
            new OperationCode(0, 1, OperationArgs.U, OperationArgs.N, InstructionMode.IABC,  "VARARG  ", VM.Func),
            new OperationCode(0, 0, OperationArgs.U, OperationArgs.U, InstructionMode.IAx,   "EXTRAARG", VM.Func),
        };

        internal static OperationCode Get(byte c)
        {
            return codes[c];
        }
    }

    // 数据类型
    internal static class LuaType
    {
        public const int LUA_TNONE = -1; // no value
        public const int LUA_TNIL = 0;
        public const int LUA_TBOOLEAN = 1;
        public const int LUA_TLIGHTUSERDATA = 2;
        public const int LUA_TNUMBER = 3;
        public const int LUA_TSTRING = 4;
        public const int LUA_TTABLE = 5;
        public const int LUA_TFUNCTION = 6;
        public const int LUA_TUSERDATA = 7;
        public const int LUA_TTHREAD = 8;
    }
}
