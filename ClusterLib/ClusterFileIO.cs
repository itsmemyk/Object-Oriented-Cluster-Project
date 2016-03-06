using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace OODBCluserCS
{
    [Serializable]
    public class ClusterFileIO
    {
        String filePath;
        ClusterDatabase db;

        public ClusterFileIO()
        {
            db = new ClusterDatabase();
        }

        public ClusterFileIO(String filePath)
        {
            this.filePath = filePath;
            db = new ClusterDatabase();
        }
        public ClusterFileIO(String filePath, ClusterDatabase db)
        {
            this.filePath = filePath;
            this.db = db;
        }

        public ClusterClass[] GetStoredClasses()
        {
            return db.Classes.ToArray();
        }

        public void StoreObject(ClusterObject clsObj, Object objToStore)
        {            
            Type t = objToStore.GetType();

            foreach (FieldInfo field in t.GetFields())
            {
                Object val = field.GetValue(objToStore);
                String ns = val.GetType().UnderlyingSystemType.FullName;

                if (val.GetType().IsPrimitive || val.GetType().UnderlyingSystemType.FullName.StartsWith("System."))
                {
                    ClusterField attribute = new ClusterField(field, objToStore);
                    clsObj.Attributes.Add(attribute);
                }
                else if (val.GetType().IsClass)
                {                    
                    if (val.GetType().IsArray)
                    {
                        Array arrValues = val as Array;

                        for (int i = 0; i < arrValues.Length; i++)
                        {
                            Object objElement = arrValues.GetValue(i);
                            ClusterObject relatedClsObj = new ClusterObject(db.GenerateNewObjectID());
                            clsObj.relatedObjects.Add(new ClusterRelationship(ClusterUtility.Relationship.AGGREGATION, ClusterUtility.Cardinality.MANY, relatedClsObj.ObjectID));
                            this.StoreObject(relatedClsObj, objElement);
                        }
                    }
                    else
                    {
                        ClusterObject relatedClsObj = new ClusterObject(ClusterClass.GenerateNewObjectID());
                        clsObj.relatedObjects.Add(new ClusterRelationship(ClusterUtility.Relationship.AGGREGATION, ClusterUtility.Cardinality.ONE, relatedClsObj.ObjectID));
                        this.StoreObject(relatedClsObj, val);
                    }
                }
            }

            int class_ID = StoreInClass(t, clsObj);
            ClusterClass clsClass = db.findClassByID(class_ID);

            foreach (Type nestType in t.GetNestedTypes())
            {
                int relatedClassID = CreateOrCheckClass(nestType);
                clsClass.relationships.Add(new ClusterRelationship(ClusterUtility.Relationship.COMPOSITION, ClusterUtility.Cardinality.ONE, relatedClassID));
            }
        }        

        public void StoreObject(Object objToStore)
        {
            ClusterObject clsObj = new ClusterObject(db.GenerateNewObjectID());            

            Type t = objToStore.GetType();

            // Code For Storing All Field As Well As Aggregated Objects
            foreach (FieldInfo field in t.GetFields())
            {
                Object val = field.GetValue(objToStore);
                String ns = val.GetType().UnderlyingSystemType.FullName;
                Type innerType = val.GetType();

                if (innerType.IsGenericType == true)
                {
                    IList list = val as IList;

                    for(var i=0;i<list.Count;i++){
                        Object objElement = list[i];
                        ClusterObject relatedClsObj = new ClusterObject(db.GenerateNewObjectID());
                        clsObj.relatedObjects.Add(new ClusterRelationship(ClusterUtility.Relationship.AGGREGATION, ClusterUtility.Cardinality.MANY, relatedClsObj.ObjectID));
                        this.StoreObject(relatedClsObj, objElement);
                    }                    
                    
                }
                else if (val.GetType().IsPrimitive || val.GetType().UnderlyingSystemType.FullName.StartsWith("System."))
                { 
                    ClusterField attribute = new ClusterField(field, objToStore);
                    clsObj.Attributes.Add(attribute);
                }
                else if ( val.GetType().IsClass)
                {
                    if(val.GetType().IsArray)
                    {
                        Array arrValues = val as Array;

                        for (int i = 0; i < arrValues.Length; i++)
                        {
                            Object objElement = arrValues.GetValue(i);
                            ClusterObject relatedClsObj = new ClusterObject(db.GenerateNewObjectID());                          
                            clsObj.relatedObjects.Add(new ClusterRelationship(ClusterUtility.Relationship.AGGREGATION, ClusterUtility.Cardinality.MANY, relatedClsObj.ObjectID));                            
                            this.StoreObject(relatedClsObj, objElement);
                        }
                    }
                    else
                    {
                        ClusterObject relatedClsObj = new ClusterObject(db.GenerateNewObjectID());                        
                        clsObj.relatedObjects.Add(new ClusterRelationship(ClusterUtility.Relationship.AGGREGATION, ClusterUtility.Cardinality.ONE, relatedClsObj.ObjectID));
                        this.StoreObject(relatedClsObj, val);
                    }
                }
            }

            int class_ID = StoreInClass(t, clsObj);
            ClusterClass clsClass = db.findClassByID(class_ID);

            // Code for finding Composition Relationship
            foreach (Type nestType in t.GetNestedTypes())
            {
                int relatedClassID = CreateOrCheckClass(nestType);
                clsClass.relationships.Add(new ClusterRelationship(ClusterUtility.Relationship.COMPOSITION, ClusterUtility.Cardinality.ONE, relatedClassID));
            }

            // Code for finding Inheritance Relationship
            Type ChildType = t;
            Type ParentType = ChildType.BaseType;

            while( ParentType.FullName != "System.Object")
            {
                int relatedClassID = CreateOrCheckClass(ParentType);
                clsClass.relationships.Add(new ClusterRelationship(ClusterUtility.Relationship.INHERITANCE, ClusterUtility.Cardinality.ONE, relatedClassID));

                ChildType = ParentType;
                ParentType = ChildType.BaseType; 
            }
        }

        public void UpdateObject(int objID, Object objToStore)
        {
            ClusterObject clsObj = db.findObjectByID(objID);

            if (clsObj != null)
            {
                clsObj.Attributes.Clear();
                clsObj.relatedObjects.Clear();

                StoreObject(clsObj, objToStore);
            }            
        }

        public void RemoveObject(int objID)
        {
            ClusterObject clsObj = db.findObjectByID(objID);

            if (clsObj != null)
            {
                clsObj.Attributes.Clear();
                clsObj.relatedObjects.Clear();

                foreach (ClusterClass clsClass in db.Classes)
                {
                    if (clsClass.objects.Where(O => O.ObjectID == objID).Any())
                    {
                        clsClass.objects.Remove(clsObj);
                        break;
                    }
                }
            }            
        }

        public int StoreInClass(Type t, ClusterObject clsObj)
        {
            int class_ID = 0;

            if (!db.Classes.Where(C => C.ClassName.Equals(t.Name)).Any())
            {
                class_ID = db.GenerateNewClassID();
                ClusterClass clsCls = new ClusterClass(class_ID);
                clsCls.ClassName = t.Name;
                clsCls.objects.Add(clsObj);
                db.Classes.Add(clsCls);
            }
            else
            {
                ClusterClass clsCls = db.Classes.Where(C => C.ClassName.Equals(t.Name)).Single();

                if( ! clsCls.objects.Where(O => O.ObjectID == clsObj.ObjectID).Any())
                {
                    clsCls.objects.Add(clsObj);
                }
                
                class_ID = clsCls.ClassID;
            }

            return class_ID;
        }

        // Used to Retrieve Class ID, If Class is not exist then create new one and set its ID & Return it
        public int CreateOrCheckClass(Type t)
        {
            int class_ID = 0;

            if (!db.Classes.Where(C => C.ClassName.Equals(t.Name)).Any())
            {
                class_ID = db.GenerateNewClassID();
                ClusterClass clsCls = new ClusterClass(class_ID);
                clsCls.ClassName = t.Name;
                db.Classes.Add(clsCls);
            }
            else
            {
                ClusterClass clsCls = db.Classes.Where(C => C.ClassName.Equals(t.Name)).Single();                
                class_ID = clsCls.ClassID;                
            }
            return class_ID;
        }


        public void WriteToDisk()
        {
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, db);
            fs.Close();
        }


        public void ReadFromDisk()
        {
            FileStream fsr = null;

            try
            {
                fsr = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryFormatter formatterr = new BinaryFormatter();
                db = (ClusterDatabase)formatterr.Deserialize(fsr);                
            }
            catch(Exception e)
            {
                db = new ClusterDatabase();                
            }
            finally
            {
                fsr.Close();
            }
        }

        public ClusterObject GetObjectByID(int ID)
        {
            return db.findObjectByID(ID);
        }
        
        public ClusterClass GetClassByID(int ID)
        {
            return db.findClassByID(ID);
        }

        public ClusterClass GetClassByName(String Name)
        {
            return db.findClassByName(Name);
        }
    }
}
