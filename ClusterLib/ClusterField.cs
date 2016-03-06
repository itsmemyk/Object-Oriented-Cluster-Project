using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OODBCluserCS
{
    [Serializable]
    public class ClusterField
    {
        public String FieldName;
        public Object FieldValue;

        public ClusterField()
        {

        }
        public ClusterField(FieldInfo field, Object OnObject)
        {
            this.FieldName = field.Name;
            this.FieldValue = field.GetValue(OnObject);     
        }
    }
}
