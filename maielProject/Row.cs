using System;
using System.Collections.Generic;
using System.Text;

namespace maielProject
{
    public class Row
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public string Reference { get; set; }
        public string Client { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string Matriculation { get; set; }
        public string TypeCargo { get; set; }
        public DateTime Priority { get; set; }
        public DateTime RegistryDate { get; set; }
        public DateTime BlokedTime { get; set; }
        public string POD { get; set; }
        public string Park { get; set; }
        public string KindEquipment { get; set; }
        public int DepotIdBlocking { get; set; }
        public DateTime ExpiredAssignmentDate { get; set; }
        public string Vessel { get; set; }
        public string Voyage { get; set; }
        public string POL { get; set; }

        public bool EqualsByReference(string reference)
        {
            return this.Reference == reference;
        }        

        public override bool Equals(object obj)
        {
            return obj is Row row &&
                   Guid.Equals(row.Guid);
        }

        public override int GetHashCode()
        {
            return -737073652 + Guid.GetHashCode();
        }
    }
}
