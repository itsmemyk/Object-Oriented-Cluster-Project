using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace OODBCluserCS
{
    [Serializable]
    public class Stud
    {
        public String StudentStd;
    }

    [Serializable]
    public class Student : Stud
    {
        public int StudentID;
        public String StudentName;
        public Attendance [] attendances = new Attendance[5];
        public Marks mark = new Marks(10, 20, 30);

        public Student(int sID, String sName, String sStd)
        {
            this.StudentID = sID;
            this.StudentName = sName;
            this.StudentStd = sStd;
        }

        public override string ToString()
        {
            return "Id : " + StudentID + " \t Name : " + StudentName ;
        }

        public class Marks
        {
            public int Sub1;
            public int Sub2;
            public int Sub3;

            public Marks(int s1, int s2, int s3)
            {
                Sub1 = s1;
                Sub2 = s2;
                Sub3 = s3;
            }
        }

        public class MarkSheet
        {
            public int Total;
            public String Grade;

            public MarkSheet(int total, String grade)
            {
                Total = total;
                Grade = grade;
            }
        }
    }

    [Serializable]
    public class Attendance
    {
        public DateTime attendDate;
        public Boolean isPresent;

        public Attendance(DateTime attendDate, Boolean isPresent)
        {
            this.attendDate = attendDate;
            this.isPresent = isPresent;
        }
    }

    [Serializable]
    public class Department
    {
        public int DeptID;
        public String DeptName;
        public List<Employee> employees = new List<Employee>();

        public Department(int deptID, String deptName)
        {
            this.DeptID = deptID;
            this.DeptName = deptName;
        }

        public void AddNewEmployee(Employee newEmployee)
        {
            this.employees.Add(newEmployee);           
        }
    }


    [Serializable]
    public class Employee
    {
        public int EmpID;
        public String EmpName;
        public String Designation;

        public Employee(int eID, String eName, String desg)
        {
            this.EmpID = eID;
            this.EmpName = eName;
            this.Designation = desg;
        }

        public override string ToString()
        {
            return "(Id : " + EmpID + " \t Name : " + EmpName + " \t Designation : " + Designation+")";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            String filepath = @"h:\\OODB\\test.db";

            ClusterFileIO fileIO = new ClusterFileIO(filepath);

            /*
            Student s1 = new Student(1, "Mayank"," MSC.ICT");
            DateTime dt = DateTime.Today;

            s1.attendances[0] = new Attendance(dt.AddDays(1), true);
            s1.attendances[1] = new Attendance(dt.AddDays(2), true);
            s1.attendances[2] = new Attendance(dt.AddDays(3), true);
            s1.attendances[3] = new Attendance(dt.AddDays(4), true);
            s1.attendances[4] = new Attendance(dt.AddDays(5), true);

            Student s2 = new Student(2, "Kiran","MSC.ICT");            

            */

            Department d1 = new Department(1, "IT");
            
            Employee e1 = new Employee(1, "Mayank", "IT");
            Employee e2 = new Employee(2, "Param", "Sales");
            
            d1.AddNewEmployee(e1);
            d1.AddNewEmployee(e2);


            Department d2 = new Department(2, "Sales");

            Employee e3 = new Employee(3, "Sonu Lok", "Account");

            d2.AddNewEmployee(e3);
            
            //fileIO.ReadFromDisk();

            //fileIO.UpdateObject(3,e3);

            //fileIO.WriteToDisk();

            fileIO.StoreObject(d1);
            fileIO.StoreObject(d2);


            //fileIO.StoreObject(s1);
 //           fileIO.StoreObject(s2);

            fileIO.WriteToDisk(); // Commit


            fileIO.ReadFromDisk(); // Read it Again From Disk
            
            //foreach (ClusterClass clsClass in fileIO.GetStoredClasses())
            //{
            //    Console.WriteLine("\n\n " + clsClass.ClassID + " " + clsClass.ClassName);
            //    ViewClassDetails(fileIO, clsClass);
            //}

            ClusterClass clsClass = fileIO.GetClassByName("Department");
            ViewClassDetails(fileIO, clsClass);

            Console.ReadKey();
        }

        public static void ViewClassDetails(ClusterFileIO fileIO, ClusterClass clsClass)
        {

            if (clsClass.relationships.Any())
            {
                Console.WriteLine("\n\n\n\t Relationships \n");

                foreach (ClusterRelationship relation in clsClass.relationships)                
                    Console.WriteLine("\n\nRelationship : " + Enum.GetName(typeof(ClusterUtility.Relationship), relation.relationship) + " With " + fileIO.GetClassByID(relation.ID).ClassName);                
                
            }

            if( clsClass.objects.Any())
            {
                Console.WriteLine("\n\n\n\t Objects \n");

                foreach (ClusterObject clsObject in clsClass.objects)
                    ViewObjectDetails(fileIO, clsObject);
            }
            else
                Console.WriteLine("\n\n\n\t No Objects \n");


            Console.WriteLine("\n\n-----------------------------------------------------------------------------\n");
        }

        public static void ViewObjectDetails(ClusterFileIO fileIO, ClusterObject clsObject)
        {
            Console.WriteLine("\n\n\n Object ID : {0}\n",clsObject.ObjectID);

            Console.Write("\n Data      : ");

            foreach (ClusterField clsField in clsObject.Attributes)            
                Console.Write(clsField.FieldName + " : " + clsField.FieldValue + " \t");

            if (clsObject.relatedObjects.Any())
            {
                Console.WriteLine("\n\n\n\t All Related Objects\n");
                Console.WriteLine("\n\n*****************************************************************************\n");
                foreach (ClusterRelationship relation in clsObject.relatedObjects)
                {
                    ViewObjectDetails(fileIO, fileIO.GetObjectByID(relation.ID));
                    Console.WriteLine("\n\nRelationship : " + Enum.GetName(typeof(ClusterUtility.Relationship), relation.relationship) + ", Cardinality : " + Enum.GetName(typeof(ClusterUtility.Cardinality), relation.cardinality));                
                    Console.WriteLine("\n\n``````````````````````````````````````````````````````````````````````````````");
                }
                Console.WriteLine("\n\n*****************************************************************************\n");
            }
            
        }
    }
}
