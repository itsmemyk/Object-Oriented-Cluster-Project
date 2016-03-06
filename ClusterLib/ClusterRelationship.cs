using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OODBCluserCS
{
    [Serializable]
    public class ClusterRelationship
    {
        public ClusterUtility.Relationship relationship;
        public ClusterUtility.Cardinality cardinality;
        public int ID;

        public ClusterRelationship(ClusterUtility.Relationship relationship, ClusterUtility.Cardinality cardinality, int objId)
        {
            this.relationship = relationship;
            this.cardinality = cardinality;
            this.ID = objId;
        }
    }
}
