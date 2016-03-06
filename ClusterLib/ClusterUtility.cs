using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OODBCluserCS
{
    [Serializable]
    public class ClusterUtility
    {
        public enum Cardinality
        {
            ONE,
            MANY
        }

        public enum Relationship
        {
            INHERITANCE,
            AGGREGATION,
            COMPOSITION
        }

        public static int NotExisting = 0;
    }
}
