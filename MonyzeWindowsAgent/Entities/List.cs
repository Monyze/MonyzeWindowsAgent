using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities
{
    enum BracketType
    {
        scSquare,
        scCurly
    }

    class List : IEntity
    {
        public string indent = "";

        public string name;

        public BracketType bracketType = BracketType.scSquare;

        public List(string name_, string indent_ = "", BracketType bracketType_ = BracketType.scSquare)
        {
            indent = indent_;
            name = name_;
            bracketType = bracketType_;
        }

        public void Add(IEntity entity)
        {
            list.Add(entity);
        }

        public string Serialize()
        {
            string output = indent + "\"" + @name + "\":" + (bracketType == BracketType.scSquare ? "[" : "{");

            foreach (var entity in list)
            {
                output += "\r\n" + entity.Serialize() + ",";
            }

            output = output.TrimEnd(',');

            return output + "\r\n" + indent + (bracketType == BracketType.scSquare ? "]" : "}");
        }

        private List<IEntity> list = new List<IEntity>();
    }
}
