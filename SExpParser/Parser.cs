using System;
using System.Collections.Generic;
using System.Text;

namespace SExpParser
{
    public class Parser
    {
        Lexer lexer;

        Token current_token;

        public Parser(Lexer l)
        {
            lexer = l;
            current_token = lexer.GetNextToken();
        }

        void Error(string msg)
        {
            throw new Exception($"{msg} at ({lexer.CurrentLine}:{lexer.CurrentPosInLine}) : {lexer.CurrentLineSource} ");
        }

        void Error()
        {
            Error($"Invalid Token: {current_token}");
        }

        void Error(TokenType expecting)
        {
            Error($"Expecting: {expecting}");
        }

        void Eat(TokenType t)
        {
            if (current_token.Type == t)
            {
                current_token = lexer.GetNextToken();
            }
            else
            {
                Error(t);
            }
        }

        public ISNode Term()
        {
            string name;
            List<ISNode> childs = new List<ISNode>();

            Eat(TokenType.LPAREN);
            var token = current_token;
            Eat(TokenType.IDENT);
            name = token.Lexeme;
            Console.WriteLine($"term {name}");
            Console.WriteLine("(");
            while (current_token.Type != TokenType.RPAREN)
            {
                if (current_token.Type == TokenType.EOF)
                {
                    Error(TokenType.RPAREN);
                }
                childs.Add(Parse());
            }

            Eat(TokenType.RPAREN);

            Console.WriteLine(")");
            return new STerm(name, childs);
        }

        public ISNode Atom()
        {
            var token = current_token;
            switch (token.Type)
            {
                case TokenType.IDENT:
                    Eat(TokenType.IDENT);
                    Console.WriteLine($"ident {token.Lexeme}");
                    return new SVariable(token.Lexeme);
                case TokenType.NUMBER:
                    Eat(TokenType.NUMBER);
                    Console.WriteLine($"number {token.Lexeme}");
                    return new SConst(double.Parse(token.Lexeme));
                default:
                    return null;
            }
        }

        public ISNode Parse()
        {
            switch (current_token.Type)
            {
                case TokenType.IDENT:
                case TokenType.NUMBER:
                    return Atom();
                case TokenType.LPAREN:
                    return Term();
                default:
                    return null;
            }
        }
    }
}
