<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EmployeeWebApp._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product Management</title>
    <link rel="stylesheet" type="text/css" href="css/bootstrap.min.css" />
    <link rel="stylesheet" type="text/css" href="css/style.css" />

    <script type="text/javascript" src="bootstrap.min.js"></script>
    <script type="text/javascript" src="jquery.js"></script>
    <script type="text/javascript" src="custom.js"></script>
</head>
<body>
    <form id="form1" runat="server">    
    <section id="Content">
        <div class="container">
            <div class="row">
                <div class="col-md-4">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <div class="panel-title">
                                <label id="lblTitle" runat="server"> Add New Product </label>
                                <asp:HiddenField ID="hdnOperation" runat="server" Value="Add" />
                            </div>
                        </div>
                        <div class="panel-body">
                            <asp:HiddenField id="txtObjectId" runat="server" Value="0" />   
                            <div class="form-group">
                                <label> Product Name </label>                               
                                <asp:TextBox id="txtProductName" runat="server" class="form-control" />                                
                            </div>
                            <div class="form-group">
                                <label> Product Description </label>                               
                                <asp:TextBox TextMode="MultiLine" id="txtProductDesc" runat="server" class="form-control" />                                
                            </div>
                            <div class="form-group">
                                <label> Product Price </label>                               
                                <asp:TextBox id="txtProductPrice" runat="server" class="form-control" />                                
                            </div>                   
                            <div class="form-group">
                                <label> Product Qty </label>                               
                                <asp:TextBox id="txtProductQty" runat="server" class="form-control" />                                
                            </div>                   
                            <div class="form-group">
                                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-success btn-lg" Text="Save" OnClick="btnSave_Click"/>
                                <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-default btn-lg" Text="Cancel" OnClick="btnCancel_Click"/>                                
                            </div>         
                        </div>
                    </div> 
                </div>
                <div class="col-md-8">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <div class="panel-title">Products</div>
                        </div>
                        <div class="panel-body">
                            <asp:Repeater ID="productGrid" runat="server"  >
                                <HeaderTemplate>                                   
                                    <table class="table table-condensed table-hover">
                                        <thead>
                                            <tr>
                                                <th  class="text-center">Sr.No.</th>
                                                <th  class="text-center">Title</th>
                                                <th  class="text-center">Description</th>
                                                <th  class="text-center">Price</th>
                                                <th  class="text-center">Qty</th>
                                                <th  class="text-center">Edit</th>
                                            </tr>
                                        </thead>
                                        <tbody>                            
                                </HeaderTemplate>

                                <ItemTemplate>
                                    <tr class="text-center">
                                        <td><%# Container.ItemIndex+1 %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem,"Title") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem,"Description") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem,"Price") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem,"Qty") %></td>
                                        <td>
                                            <asp:Button ID="btnEdit" runat="server" OnCommand="btnEdit_Command" CommandName="ObjectID" CommandArgument='<%#Eval("ObjectID")%>' CssClass="btn btn-warning" Text="Edit"/>
                                            <asp:Button ID="btnRemove" runat="server" OnCommand="btnRemove_Command" CommandName="ObjectID" CommandArgument='<%#Eval("ObjectID")%>' CssClass="btn btn-danger" Text="Remove"/>
                                        </td>
                                    </tr>                                
                                </ItemTemplate>
                                <FooterTemplate>
                                        </tbody>
                                    </table>
                                </FooterTemplate>
                                
                            </asp:Repeater>                                                                    
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>        
    </form>
</body>
</html>
</html>
