using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maielProject
{
    public class RowInfo
    {
        public Row Row { get; set; }
        public State State { get; set; }

        public RowInfo()
        {
            this.State = State.none;
        }

        public override bool Equals(object obj)
        {
            return obj is RowInfo info &&
                   EqualityComparer<Row>.Default.Equals(Row, info.Row);
        }

        public override int GetHashCode()
        {
            return -343017389 + EqualityComparer<Row>.Default.GetHashCode(Row);
        }
    }

    public enum State
    {
        delete,
        add,
        update,
        none
    }


}
