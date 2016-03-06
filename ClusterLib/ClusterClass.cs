using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OODBCluserCS
{
    [Serializable]
    public class ClusterClass
    {
        static int lastObjectID = ClusterUtility.NotExisting;
        public int ClassID;
        public String ClassName;
        public List<ClusterObject> objects = new List<ClusterObject>();
        public List<ClusterRelationship> relationships = new List<ClusterRelationship>();

        public ClusterClass()
        {

        }
        public ClusterClass(int cID)
        {
            this.ClassID = cID;
        }

        public Type SetClassName
        {
            set
            {
                Type t = value;
                ClassName = t.Name;
            }
        }

        public static int GenerateNewObjectID()
        {
            return ++lastObjectID;
        }

    }
}
