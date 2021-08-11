using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace LuaVM.Net.Core
{
    // 词法分析器
    public class Lexer
    {
        // 源码
        public string chunk { get; private set; } = string.Empty;
        // 源文件名
        public string name { get; private set; } = string.Empty;
        // 行号
        public int line { get; private set; } = 0;

        // 下一个token，用于预览
        private Token nextToken = null;

        // 构造函数
        public Lexer(string name, string chunk, int line)
        {
            this.name = name;
            this.chunk = chunk;
            this.line = line;
        }

        // Token
        public Token NextToken()
        {
            if (nextToken != null)
            {
                var token = nextToken;
                nextToken = null;
                line = token.line;

                return token;
            }

            SkipWhiteSpace();

            if (chunk.Length > 0)
            {
                var c = chunk[0];
                switch (c)
                {
                    case ';':
                        Skip(1);
                        return new Token(TokenType.SEP_SEMI, ";", line);
                    case ',':
                        Skip(1);
                        return new Token(TokenType.SEP_COMMA, ",", line);
                    case '(':
                        Skip(1);
                        return new Token(TokenType.SEP_LPAREN, "(", line);
                    case ')':
                        Skip(1);
                        return new Token(TokenType.SEP_RPAREN, ")", line);
                    case ']':
                        Skip(1);
                        return new Token(TokenType.SEP_RBRACK, "]", line);
                    case '{':
                        Skip(1);
                        return new Token(TokenType.SEP_LCURLY, "{", line);
                    case '}':
                        Skip(1);
                        return new Token(TokenType.SEP_RCURLY, "}", line);
                    case '+':
                        Skip(1);
                        return new Token(TokenType.OP_ADD, "+", line);
                    case '-':
                        Skip(1);
                        return new Token(TokenType.OP_MINUS, "-", line);
                    case '*':
                        Skip(1);
                        return new Token(TokenType.OP_MUL, "*", line);
                    case '^':
                        Skip(1);
                        return new Token(TokenType.OP_POW, "^", line);
                    case '%':
                        Skip(1);
                        return new Token(TokenType.OP_MOD, "%", line);
                    case '&':
                        Skip(1);
                        return new Token(TokenType.OP_BIT_AND, "&", line);
                    case '|':
                        Skip(1);
                        return new Token(TokenType.OP_BIT_OR, "|", line);
                    case '#':
                        Skip(1);
                        return new Token(TokenType.OP_LEN, "#", line);
                    case ':':
                        if (chunk.StartsWith("::"))
                        {
                            Skip(2);
                            return new Token(TokenType.SEP_LABEL, "::", line);
                        }
                        Skip(1);
                        return new Token(TokenType.SEP_COLON, ":", line);
                    case '/':
                        if (chunk.StartsWith("//"))
                        {
                            Skip(2);
                            return new Token(TokenType.OP_IDIV, "//", line);
                        }
                        Skip(1);
                        return new Token(TokenType.OP_DIV, "/", line);
                    case '~':
                        if (chunk.StartsWith("~="))
                        {
                            Skip(2);
                            return new Token(TokenType.OP_NE, "=", line);
                        }
                        Skip(1);
                        return new Token(TokenType.OP_WAVE, "~", line);
                    case '=':
                        if (chunk.StartsWith("=="))
                        {
                            Skip(2);
                            return new Token(TokenType.OP_EQ, "==", line);
                        }
                        
                        Skip(1);
                        return new Token(TokenType.OP_ASSIGN, "=", line);
                    case '<':
                        if (chunk.StartsWith("<<"))
                        {
                            Skip(2);
                            return new Token(TokenType.OP_BIT_SHL, "<<", line);
                        }
                        if (chunk.StartsWith("<="))
                        {
                            Skip(2);
                            return new Token(TokenType.OP_LE, "<=", line);
                        }
                        
                        Skip(1);
                        return new Token(TokenType.OP_LT, "<", line);
                    case '>':
                        if (chunk.StartsWith(">>"))
                        {
                            Skip(2);
                            return new Token(TokenType.OP_BIT_SHR, ">>", line);
                        }
                        if (chunk.StartsWith(">="))
                        {
                            Skip(2);
                            return new Token(TokenType.OP_GE, ">=", line);
                        }
                        
                        Skip(1);
                        return new Token(TokenType.OP_GT, ">", line);
                    case '.':
                        if (chunk.StartsWith("..."))
                        {
                            Skip(3);
                            return new Token(TokenType.VARARG, "...", line);
                        }
                        if (chunk.StartsWith(".."))
                        {
                            Skip(2);
                            return new Token(TokenType.OP_CONCAT, "..", line);
                        }
                        if (chunk.Length == 1 || !IsDigit(chunk[0]))
                        {
                            Skip(1);
                            return new Token(TokenType.SEP_DOT, ".", line);
                        }
                        break;
                    case '[':
                        if (chunk.StartsWith("[[") || chunk.StartsWith("[="))
                        {
                            return new Token(TokenType.STRING, ScanLongString(), line);
                        }
                        Skip(1);
                        return new Token(TokenType.SEP_LBRACK, "[", line);
                    case '\'':
                    case '"':
                        return new Token(TokenType.STRING, ScanShortString(), line);
                    default:
                        break;
                }
                // 数字字面量
                if (c == ',' || IsDigit(c))
                {
                    return new Token(TokenType.NUMBER, ScanNumber(), line);
                }
                // 标识符
                if (c == '_' || IsLetter(c))
                {
                    var token = ScanIdentifier();
                    if (Keywords.Has(token))
                    {
                        var type = Keywords.Get(token);
                        return new Token(type, token, line);
                    }
                    return new Token(TokenType.IDENTIFIER, token, line);
                }

                Error.Commit($"lexer error: unexpected symbol near {c}");
            }

            return new Token(TokenType.EOF, "eof", line);
        }

        // 提前预览下一个token的类型
        public int LookAhead()
        {
            if (nextToken != null)
            {
                return nextToken.type;
            }

            var curline = line;
            nextToken = NextToken();
            line = curline;

            return nextToken.type;
        }

        // 下一个指定类型的token
        public Token NextTokenOfType(int type)
        {
            var token = NextToken();
            if (token.type != type)
            {
                Error.Commit($"lexer error: syntax error near \'{token.text}\'");
            }

            return token;
        }

        // 获取下一个标识符
        public Token NextIdentifier()
        {
            return NextTokenOfType(TokenType.IDENTIFIER);
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
                // 长注释
                var match = Regex.Match(chunk, "^\\[=*\\[");
                if (match.Success)
                {
                    ScanLongString();
                    return;
                }
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
            var match = Regex.Match(chunk, "^\\[=*\\[");
            if (!match.Success)
            {
                Error.Commit("lexer error: ");
                return string.Empty;
            }

            var openingLongBracket = match.Value;
            var closingLongBracket = openingLongBracket.Replace("[", "]");
            var closingLongBracketIndex = chunk.IndexOf(closingLongBracket);

            if (closingLongBracketIndex < 0)
            {
                Error.Commit("lexer error: ");
                return string.Empty;
            }

            var str = chunk.Substring(openingLongBracket.Length, closingLongBracketIndex);
            Skip(closingLongBracketIndex + closingLongBracket.Length);

            str = Regex.Replace(str, "\r\n|\n\r|\n|\r", "\n");
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
            var match = Regex.Match(chunk, "([\'\"])(?:\\\\(?:\r\n |[\\s\\S])|[^\\\\\r\n])*?\\1");
            if (!match.Success)
            {
                Error.Commit("lexer error: ");
                return string.Empty;
            }

            var str = match.Value;
            Skip(str.Length);
            str = str.Substring(1, str.Length - 2);

            if (str.IndexOf('\\') >= 0)
            {
                var lines = Regex.Matches(str, "\r\n|\n\r|\n|\r");
                line += lines.Count;

                str = Escape(str);
            }

            return str;
        }

        // 处理转义字符
        private string Escape(string str)
        {
            // TODO 待实现
            return str;
        }

        // 数字字面量
        private string ScanNumber()
        {
            string pattern = @"^0[xX][0-9a-fA-F]*|[+\-]?\d+(\.\d*)?([eE][+\-]?\d+)?|[+\-]?\d+(\.\d*)?|[+\-]?\d*\.(\d*)";
            return Scan(pattern);
        }

        // 是否是数字
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        // 标识符
        private string ScanIdentifier()
        {
            string pattern = @"^[_\d\w]+";
            return Scan(pattern);
        }

        // 是否是字母
        private bool IsLetter(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        }

        // 根据正则表达式查找字符串
        private string Scan(string pattern)
        {
            var match = Regex.Match(chunk, pattern);
            if (!match.Success)
            {
                Error.Commit("lexer error: ");
                return string.Empty;
            }

            var token = match.Value;
            Skip(token.Length);

            return token;
        }
    }
}
