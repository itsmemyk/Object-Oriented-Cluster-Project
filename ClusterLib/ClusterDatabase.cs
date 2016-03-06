using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OODBCluserCS
{
    [Serializable]
    public class ClusterDatabase
    {
        int lastClassID;
        int lastObjectID;
        public List<ClusterClass> Classes = new List<ClusterClass>();

        public ClusterDatabase()
        {
            lastClassID = ClusterUtility.NotExisting;
            lastObjectID = ClusterUtility.NotExisting;
        }

        public int GenerateNewClassID()
        {
            return ++lastClassID;
        }
        public int GenerateNewObjectID()
        {
            return ++lastObjectID;
        }

        public ClusterClass findClassByID(int classID)
        {
            foreach (ClusterClass clsCls in Classes)
            {
                if (clsCls.ClassID == classID)
                    return clsCls;             
            }
            return null;
        }
        public ClusterClass findClassByName(String className)
        {
            foreach (ClusterClass clsCls in Classes)
            {
                if (clsCls.ClassName.Equals(className))
                    return clsCls;
            }
            return null;
        }


        public ClusterObject findObjectByID(int objID)
        {
            foreach (ClusterClass clsCls in Classes)
            {
                foreach(ClusterObject clsObj in clsCls.objects )
                {
                    if (clsObj.ObjectID == objID)
                        return clsObj;
                }
            }
            return null;
        }
    }
}
