using System;

namespace SExpParser
{
    public enum TokenType
    {
        IDENT,
        NUMBER,
        //PLUS,
        //MINUS,
        //STAR,
        //SLASH,
        //ASSIGN,
        //DEFINE,
        LPAREN,
        RPAREN,
        EOF
    }
    public class Token
    {
        public Token(TokenType type, string lexeme = "")
        {
            Type = type;
            Lexeme = lexeme;
        }

        public TokenType Type { get; }
        public string Lexeme { get; }

        public override string ToString()
        {
            return $"{Type} {Lexeme}";
        }
    }
}
