using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace OODBCluserCS
{
    [Serializable]
    public class ClusterObject
    {
        public int ObjectID;
        public List<ClusterField> Attributes = new List<ClusterField>();        
        public List<ClusterRelationship> relatedObjects = new List<ClusterRelationship>();

        public ClusterObject()
        {

        }

        public ClusterObject(int objID)
        {
            this.ObjectID = objID;
        }

        public String[] GetHeaderFields()
        {
            String[] headers = new String[this.Attributes.Count];

            return headers;
        }

        public override string ToString()
        {
            String str = "";
            String del = "";

            foreach (var item in Attributes)
            {
                str += del + item.FieldValue;
                del = ",";
            }

            return str;
        }

        public Object GetValueByField(String fieldName)
        {
            Object returnValue = null;

            if(Attributes.Where(A => A.FieldName == fieldName).Any())
            {
                returnValue = Attributes.Where(A => A.FieldName == fieldName).Single().FieldValue;
            }
            return returnValue;
        } 
    }
}
