using System.Collections.Generic;

namespace SExpParser
{
    public interface IVisitor
    {
        void Visit(STerm term);
        void Visit(SVariable var);
        void Visit(SConst cons);
    }

    public interface ISNode
    {
        void Accept(IVisitor visitor);
    }

    public class STerm : ISNode
    {
        public string Name { get; }
        public List<ISNode> Childs { get; }
        public STerm(string name, List<ISNode> childs)
        {
            Name = name;
            Childs = childs;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class SVariable : ISNode
    {
        public string Name { get; }
        public SVariable(string name)
        {
            Name = name;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class SConst : ISNode
    {
        public double Value { get; }
        public SConst(double value)
        {
            Value = value;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
