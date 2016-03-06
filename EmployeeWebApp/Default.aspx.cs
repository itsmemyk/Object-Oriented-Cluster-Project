using OODBCluserCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EmployeeWebApp
{
    [Serializable]
    public class Product
    {
        public String Title;
        public String Description;
        public Double Price;
        public int Qty;

        public Product()
        {

        }
        public Product(String Title, String Description, Double Price, int Qty)
        {            
            this.Title = Title;
            this.Description = Description;
            this.Price = Price;
            this.Qty = Qty;
        }

    }

    public partial class _Default : System.Web.UI.Page
    {
        static ClusterFileIO fileIO = new ClusterFileIO(@"h:\\OODB\\ProductDB.txt");

        protected void Page_Load(object sender, EventArgs e)
        {
            if( ! Page.IsPostBack)
                RefreshGrid();            
        }

        private void RefreshGrid()
        {
            // Set Up-Default Value
            lblTitle.InnerText = "Add New Product";
            hdnOperation.Value = "Add";
            txtObjectId.Value = "0";
            txtProductName.Text = "";
            txtProductDesc.Text = "";
            txtProductPrice.Text = "";
            txtProductQty.Text = "";


            // Persist Data From Database
            fileIO.ReadFromDisk();

            // Firing A Query
            // Select * From Product
            ClusterClass clsClass = fileIO.GetClassByName("Product");

            if (clsClass != null)
            {
                if (clsClass.objects.Any())
                {
                    var result = from Obj in clsClass.objects
                                 select new { ObjectID = Obj.ObjectID, Title = Obj.GetValueByField("Title"), Name = Obj.GetValueByField("EmpName"), Description = Obj.GetValueByField("Description"), Price = Obj.GetValueByField("Price"), Qty = Obj.GetValueByField("Qty") };

                    productGrid.DataSource = result.ToList();
                    productGrid.DataBind();
                }
            }
        }
        
        protected void btnSave_Click(object sender, EventArgs e)
        {            
            int objectId = Convert.ToInt32(txtObjectId.Value);

            Product p = new Product();            
            p.Title = txtProductName.Text;
            p.Description = txtProductDesc.Text;
            p.Price = Convert.ToDouble(txtProductPrice.Text);
            p.Qty = Convert.ToInt32(txtProductQty.Text);

            if (hdnOperation.Value.Equals("Add"))
                fileIO.StoreObject(p);
            else
                fileIO.UpdateObject(objectId, p);

            fileIO.WriteToDisk();

            RefreshGrid();
        }

        protected void btnEdit_Command(object sender, CommandEventArgs e)
        {
            //Get Passed Id
            int Id = Convert.ToInt32(e.CommandArgument.ToString());

            // Load Data of Product
            ClusterObject clsObj  = fileIO.GetObjectByID(Id);

            // Disable ID Field           
            // Set Operation to Edit
            hdnOperation.Value = "Edit";
            lblTitle.InnerText = "Edit Product";

            // Set-Up Loaded Data
            txtObjectId.Value = Id.ToString();
            txtProductName.Text = clsObj.GetValueByField("Title").ToString();
            txtProductDesc.Text = clsObj.GetValueByField("Description").ToString();
            txtProductPrice.Text = clsObj.GetValueByField("Price").ToString();
            txtProductQty.Text = clsObj.GetValueByField("Qty").ToString();
        }

        protected void btnRemove_Command(object sender, CommandEventArgs e)
        {
            int Id = Convert.ToInt32(e.CommandArgument.ToString());
            fileIO.RemoveObject(Id);
            fileIO.WriteToDisk();

            RefreshGrid();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }                
    }
}