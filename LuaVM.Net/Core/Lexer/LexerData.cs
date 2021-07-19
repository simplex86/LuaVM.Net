using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LuaVM.Net.Core
{
    public class LexerData
    {
        // 源码
        public string chunk { get; set; } = string.Empty;
        // 源文件名
        public string name { get; set; } = string.Empty;
        // 行号
        public int line { get; set; } = 0;

        // 构造函数
        public LexerData(string name, string chunk, int line)
        {
            this.name = name;
            this.chunk = chunk;
            this.line = line;
        }

        // Token
        public void NextToken(out int num, out int type, out string token)
        {
            SkipWhiteSpace();

            num   = line;
            type  = TokenType.EOF;
            token = "EOF";

            if (chunk.Length > 0)
            {
                switch (chunk[0])
                {
                    case ';':
                        Skip(1); num = line; type = TokenType.SEP_SEMI; token = ";";
                        break;
                    case ',':
                        Skip(1); num = line; type = TokenType.SEP_COMMA; token = ",";
                        break;
                    case '(':
                        Skip(1); num = line; type = TokenType.SEP_LPAREN; token = "(";
                        break;
                    case ')':
                        Skip(1); num = line; type = TokenType.SEP_RPAREN; token = ")";
                        break;
                    case ']':
                        Skip(1); num = line; type = TokenType.SEP_RBRACK; token = "]";
                        break;
                    case '{':
                        Skip(1); num = line; type = TokenType.SEP_LCURLY; token = "{";
                        break;
                    case '}':
                        Skip(1); num = line; type = TokenType.SEP_RCURLY; token = "}";
                        break;
                    case '+':
                        Skip(1); num = line; type = TokenType.OP_ADD; token = "+";
                        break;
                    case '-':
                        Skip(1); num = line; type = TokenType.OP_MINUS; token = "-";
                        break;
                    case '*':
                        Skip(1); num = line; type = TokenType.OP_MUL; token = "*";
                        break;
                    case '^':
                        Skip(1); num = line; type = TokenType.OP_POW; token = "^";
                        break;
                    case '%':
                        Skip(1); num = line; type = TokenType.OP_MOD; token = "%";
                        break;
                    case '&':
                        Skip(1); num = line; type = TokenType.OP_BIT_AND; token = "&";
                        break;
                    case '|':
                        Skip(1); num = line; type = TokenType.OP_BIT_OR; token = "|";
                        break;
                    case '#':
                        Skip(1); num = line; type = TokenType.OP_LEN; token = "#";
                        break;
                    case ':':
                        if (chunk.StartsWith("::"))
                        {
                            Skip(2); num = line; type = TokenType.SEP_LABEL; token = "::";
                        }
                        else
                        {
                            Skip(1); num = line; type = TokenType.SEP_COLON; token = ":";
                        }
                        break;
                    case '/':
                        if (chunk.StartsWith("//"))
                        {
                            Skip(2); num = line; type = TokenType.OP_IDIV; token = "//";
                        }
                        else
                        {
                            Skip(1); num = line; type = TokenType.OP_DIV; token = "/";
                        }
                        break;
                    case '~':
                        if (chunk.StartsWith("~="))
                        {
                            Skip(2); num = line; type = TokenType.OP_NE; token = "~=";
                        }
                        else
                        {
                            Skip(1); num = line; type = TokenType.OP_WAVE; token = "~";
                        }
                        break;
                    case '=':
                        if (chunk.StartsWith("=="))
                        {
                            Skip(2); num = line; type = TokenType.OP_EQ; token = "==";
                        }
                        else
                        {
                            Skip(1); num = line; type = TokenType.OP_ASSIGN; token = "=";
                        }
                        break;
                    case '<':
                        if (chunk.StartsWith("<<"))
                        {
                            Skip(2); num = line; type = TokenType.OP_BIT_SHL; token = "<<";
                        }
                        else if (chunk.StartsWith("<="))
                        {
                            Skip(2); num = line; type = TokenType.OP_LE; token = "<=";
                        }
                        else
                        {
                            Skip(1); num = line; type = TokenType.OP_LT; token = "<";
                        }
                        break;
                    case '>':
                        if (chunk.StartsWith(">>"))
                        {
                            Skip(2); num = line; type = TokenType.OP_BIT_SHR; token = ">>";
                        }
                        else if (chunk.StartsWith(">="))
                        {
                            Skip(2); num = line; type = TokenType.OP_GE; token = ">=";
                        }
                        else
                        {
                            Skip(1); num = line; type = TokenType.OP_GT; token = ">";
                        }
                        break;
                    case '.':
                        if (chunk.StartsWith("..."))
                        {
                            Skip(3); num = line; type = TokenType.VARARG; token = "...";
                        }
                        else if (chunk.StartsWith(".."))
                        {
                            Skip(2); num = line; type = TokenType.OP_CONCAT; token = "..";
                        }
                        else
                        {
                            Skip(1); num = line; type = TokenType.SEP_DOT; token = ".";
                        }
                        break;
                    case '[':
                        if (chunk.StartsWith("[[") || chunk.StartsWith("[="))
                        {
                            num = line; type = TokenType.STRING; token = ScanLongString();
                        }
                        else
                        {
                            Skip(1); num = line; type = TokenType.SEP_LBRACK; token = "[";
                        }
                        break;
                    case '\'':
                    case '"':
                        num = line; type = TokenType.STRING; token = ScanShortString();
                        break;
                    default:
                        break;
                }
            }
        }

        // 跳过空格、制表符、注释、换行等空白字符
        private void SkipWhiteSpace()
        {
            while (chunk.Length > 0)
            {
                if (chunk.StartsWith("--"))
                {
                    SkipComment();
                }
                else if (chunk.StartsWith("\r\n") || chunk.StartsWith("\n\r"))
                {
                    Skip(2);
                    line++;
                }
                else if (IsNewLine(chunk[0]))
                {
                    Skip(1);
                    line++;
                }
                else if (IsWhiteSpace(chunk[0]))
                {
                    Skip(1);
                }
                else
                {
                    break;
                }
            }
        }

        // 跳过注释
        private void SkipComment()
        {
            Skip(2);
            if (chunk.StartsWith("["))
            {
                // 长注释，TODO
            }
            // 短注释
            while (chunk.Length > 0 && !IsNewLine(chunk[0]))
            {
                Skip(1);
            }
        }

        // 跳过指定长度的字符
        private void Skip(int len)
        {
            chunk = chunk.Substring(len);
        }

        // 是否是新行
        private bool IsNewLine(char c)
        {
            return c == '\n' || c == '\r';
        }

        // 是否是空白字符
        // 包括制表符、换行符、回车符、空格等
        private bool IsWhiteSpace(char c)
        {
            if (c == '\t' || c == '\n' || c == '\v' || c == '\f' || c == '\r' || c == ' ')
            {
                return true;
            }
            return false;
        }

        // 长字符串字面量
        private string ScanLongString()
        {
            var matches = Regex.Matches(chunk, "^[=*[");
            if (matches.Count == 0)
            {
                Error("");
                return string.Empty;
            }

            var openingLongBracket = matches[0].Value;
            var closingLongBracket = openingLongBracket.Replace("[", "]");
            var closingLongBracketIndex = chunk.IndexOf(closingLongBracket);

            if (closingLongBracketIndex < 0)
            {
                Error("");
                return string.Empty;
            }

            var str = chunk.Substring(openingLongBracket.Length, closingLongBracketIndex);
            Skip(closingLongBracketIndex + closingLongBracket.Length);

            str = Regex.Replace(str, "\r\n|\n\\n|\r", "\n");
            line += str.Count(c => (c == '\n'));

            if (str.Length > 0 && str[0] == '\n')
            {
                str = str.Substring(1);
            }

            return str;
        }

        // 短字符串字面量
        private string ScanShortString()
        {
            var str = string.Empty;

            return str;
        }

        // 输出错误信息
        private void Error(string info)
        {
            var text = info;
            Console.WriteLine(text);
        }
    }
}
