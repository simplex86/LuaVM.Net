﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public LexerData(string name, string chunk, int line)
        {
            this.name = name;
            this.chunk = chunk;
            this.line = line;
        }

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
                        Skip(1); num = line; type = TokenType.SEP_SEMI; token = ".";
                        break;
                    case ',':
                        Skip(1); num = line; type = TokenType.SEP_COMMA; token = ",";
                        break;
                    // TODO
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
    }
}