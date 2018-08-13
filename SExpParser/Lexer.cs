using System;
using System.Collections.Generic;
using System.Text;

namespace SExpParser
{
    public class Lexer
    {
        string text;
        int pos;
        char current_char;
        int current_line;
        int current_pos_in_line;

        public int CurrentLine { get => current_line; }
        public int CurrentPosInLine { get => current_pos_in_line; }
        public string CurrentLineSource
        {
            get
            {
                return text.Substring(pos - current_pos_in_line, current_pos_in_line);
            }
        }

        public Lexer(string t)
        {
            text = t;
            pos = 0;
            current_char = text[pos];
            current_line = 0;
            current_pos_in_line = 0;
        }

        public Lexer(Lexer other)
        {
            text = other.text;
            pos = other.pos;
            current_char = other.current_char;
            current_line = other.current_line;
            current_pos_in_line = other.current_pos_in_line;
        }

        public void ErrorExpecting(char c)
        {
            Error($"Expecting {c}, got {current_char}");
        }

        public void ErrorUnknownChar()
        {
            Error($"Unknown {current_char}");
        }

        public void Error(string msg)
        {
            throw new Exception($"Lexer error: {msg}\n at line {current_line}:{text.GetLine(current_line)}|{CurrentLineSource}");
        }

        void Advance()
        {
            pos++;
            current_pos_in_line++;
            if (pos > text.Length - 1)
            {
                current_char = '\0';
            }
            else
            {
                current_char = text[pos];
            }
        }
        char Peek()
        {
            if (pos > text.Length - 1)
            {
                current_char = '\0';
            }
            return text[pos + 1];
        }

        void SkipWhitespace()
        {
            while (current_char != '\0' && char.IsWhiteSpace(current_char))
                Advance();
        }

        void SkipComment()
        {
            while (current_char != '\0' && current_char != '\n')
            {
                Advance();
            }
        }

        Token Number()
        {
            string result = "";
            TokenType tokentype = TokenType.NUMBER;
            if (current_char == '-')
            {
                result += '-';
                Advance();
            }
            while (current_char != '\0' && current_char != '.' && current_char.IsNumeric())
            {
                result += current_char;
                Advance();
            }
            if (current_char == '.')
            {
                result += '.';
                Advance();
                if (current_char == '\0')
                {
                    result += '0';
                }
                else
                {
                    while (current_char != '\0' && current_char.IsNumeric())
                    {
                        result += current_char;
                        Advance();
                    }
                }
            }
            return new Token(tokentype, result);
        }

        Token Ident()
        {
            StringBuilder temp = new StringBuilder();
            while (current_char != '\0' && current_char.IsIdent())
            {
                temp.Append(current_char);
                Advance();
            }

            var result = temp.ToString();

            switch (result)
            {
                default:
                    return new Token(TokenType.IDENT, result);
            }
        }

        public Token GetNextToken()
        {
            while (current_char != '\0')
            {
                Token token;
                if (current_char == '\n')
                {
                    current_line++;
                    current_pos_in_line = 0;
                    Advance();
                }

                if (char.IsWhiteSpace(current_char))
                {
                    SkipWhitespace();
                    continue;
                }

                if (current_char == '/' && Peek() == '/')
                {
                    SkipComment();
                    continue;
                }

                //switch (current_char)
                //{
                //    case '+':
                //        token = new Token(TokenType.PLUS);
                //        break;
                //    case '-':
                //        token = new Token(TokenType.MINUS);
                //        break;
                //    case '*':
                //        token = new Token(TokenType.STAR);
                //        break;
                //    case '/':
                //        token = new Token(TokenType.SLASH);
                //        break;
                //    case ':':
                //        if (Peek() == '=')
                //        {
                //            token = new Token(TokenType.ASSIGN);
                //        }
                //        else
                //        {
                //            ErrorExpecting('=');
                //        }
                //        break;
                //    default:
                //        break;
                //}

                if (current_char == '(')
                {
                    Advance();
                    return new Token(TokenType.LPAREN, "(");
                }

                if (current_char == ')')
                {
                    Advance();
                    return new Token(TokenType.RPAREN, ")");
                }

                if (current_char.IsNumeric())
                {
                    return Number();
                }

                return Ident();
            }
            return new Token(TokenType.EOF, null);
        }

        public Token PeekNextToken()
        {
            Lexer peeker = new Lexer(this);
            return peeker.GetNextToken();
        }
    }
    static class ExtensionMethod
    {
        public static bool IsIdent(this char c)
        {
            // Return true if the character is between 0 or 9 inclusive or is an uppercase or
            // lowercase letter or underscore

            //return ((c >= '0' && c <= '9') ||
            //        (c >= 'A' && c <= 'Z') ||
            //        (c >= 'a' && c <= 'z') ||
            //         c == '_' ||
            //         c == '$' ||
            //         c == '@');

            return c != '(' && c != ')' && !c.IsWhiteSpace();
        }

        public static bool IsNumeric(this char c)
        {
            return (c >= '0' && c <= '9');
        }

        public static bool IsHexNumeric(this char c)
        {
            return IsNumeric(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
        }

        public static bool IsWhiteSpace(this char c)
        {
            return c == '\t' || c == ' ';
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string GetLine(this string t, int targetLine)
        {
            int currentLine = 0;
            for (int i = 0; i < t.Length; i++)
            {
                var c = t[i];
                if (c == '\n')
                {
                    currentLine++;
                    continue;
                }

                if (currentLine == targetLine)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = i; j < t.Length && t[j] != '\n'; j++)
                    {
                        sb.Append(t[j]);
                    }
                    return sb.ToString();
                }
            }
            return null;
        }
    }
}
