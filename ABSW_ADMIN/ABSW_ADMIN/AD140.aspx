﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AD140.aspx.vb" Inherits="ABSW_ADMIN.AD140" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Services/Repairs/Maintenance</title>
    <link rel="stylesheet" type="text/css" href="StyleAdmin.css" />
    <script language="javascript" type="text/javascript" src="Script/ScriptJS.js">
    </script>

    <script language="vbscript" type="text/vbscript">
    
    Sub cmdDelete_ASP_OnClick()
    Dim P : P = 0
    for each V in Form1.elements
	    if V.id = "cmdDelete_ASP" then
            P = P + 1
	    end if
    next
    If P > 0 then
        if msgbox("Are you sure you want to delete this record from database?", 36, "Confirm Delete!") = 6 then
            Form1.txtAction.value="Delete"
	        Form1.Submit
        else  
	        'msgbox "Current record not deleted!", , "Delete Record"
        end if
    Else
    End if
	    
End Sub

Sub cmdDelItem_ASP_OnClick()
	Dim P : P = 0
	for each V in Form1.elements
		'if v.type = "checkbox" then
		'if v.Checked = True then P = P + 1
		'end if

        if V.type = "checkbox" and right(V.id,6) = "chkSel" and (left(V.id,9) = "DataGrid1" or left(V.id,9) = "GridView1") then
           'msgbox "Found Control Type: " &  V.type & vbcrLF & "Found Control ID: " & V.id & vbcrLF & "Status: " & V.Checked
           if V.Checked = True then
              P = P + 1
           end if
        end if

	next
	
	if P > 0 then
	if msgbox("Are you sure you want to delete the selected item(s)?", 36, "Confirm Delete!") = 6 then
		Form1.txtAction.value="Delete_Item"
		Form1.Submit
	end if
	Else
		msgbox "You must select an item to delete!", , "Nothing to Delete"
	End if
End Sub

    </script>

</head>
<body>
    <form id="Form1" name="Form1" runat="server">
    <div class="div_container_02">
    <br />
    <table align="center" border="0" class="tbl_main">
        <tr>
            <td align="left" colspan="7" valign="top">
                <table border="0" class="tbl_butt">
                    <tr>
                        <td colspan="5" valign="top">
                            &nbsp;<asp:Button ID="cmdNew" CssClass="cmd_butt" Text="New Data" runat="server" />
                            &nbsp;<asp:Button ID="cmdSave" CssClass="cmd_butt"  Text="Save Data" runat="server" Visible="false" />
                            &nbsp;<asp:Button id="cmdSaveN" CssClass="cmd_butt" Text="Save Data" runat="server" OnClientClick="JavaSave_Rtn()" />
                            &nbsp;<asp:Button ID="cmdDel" CssClass="cmd_butt" Text="Delete Data" Enabled="false" runat="server" Visible="false" />
                            &nbsp;<asp:Button ID="cmdDelN" CssClass="cmd_butt" Text="Delete Data" runat="server" OnClientClick="JavaDel_Rtn()" />
                            &nbsp;<asp:Button ID="cmdPrint" CssClass="cmd_butt" Enabled="false" Text="Print" runat="server" />
                            &nbsp;
                        </td>
                        <td id="td_Return_Link" colspan="1" valign="top" runat="server">
                            &nbsp;|&nbsp;&nbsp;<a id="PageAnchor_Return_Link" runat="server" href="Blankpg.aspx" style="font-size:large; font-weight:bold;">CLOSE PAGE</a>
                            <%=PageLinks%>&nbsp;&nbsp;|
                        </td>
                        <td colspan="1" valign="top">    
        	                <div style="display: none;">
        	                    &nbsp;&nbsp;Status:&nbsp;<asp:textbox id="txtAction" Visible="true" runat="server" EnableViewState="false" Width="50px"></asp:textbox>&nbsp;
        	                </div>
                            
        	            </td>
                    </tr>                    
                </table>
            </td>
        </tr>

                <tr>
                    <td colspan="7" valign="top"><asp:Label ID="lblMessage" Text="Status:" ForeColor="Red" runat="server"></asp:Label></td>
                </tr>    

        <tr>
            <td colspan="7" valign="top">
                <table align="left" width="100%">
                    <tr>
                        <td align="left" colspan="2" valign="top" class="tbl_caption">Repairs/Maintenance Data Entry</td>
                    </tr>
                    <tr>
                        <td colspan="2" valign="top"></td>
                    </tr>
                    <tr>
                        <td align="right" valign="top"><asp:Label ID="Label1" Text="Trans No:" runat="server"></asp:Label>&nbsp;</td>
                        <td valign="top"><asp:TextBox ID="txtTransNum" MaxLength="15" runat="server"></asp:TextBox></td>
                    </tr>

                    <tr>
                        <td align="right" valign="top"><asp:Label ID="lblTransNum" Text="Transaction Type:" runat="server"></asp:Label>&nbsp;</td>
                        <td valign="top"><asp:DropDownList ID="ddlTransType" AutoPostBack="true" Width="300" runat="server">
                        </asp:DropDownList>
                            &nbsp;<asp:TextBox ID="txtTransType" Enabled="false" Width="60" MaxLength="4" runat="server" Visible=false></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top"><asp:Label ID="lblBraNum" Text="Branch:" runat="server"></asp:Label>&nbsp;</td>
                        <td valign="top"><asp:DropDownList ID="ddlBraNum" AutoPostBack="true" Width="300" runat="server"></asp:DropDownList>                       
                            &nbsp;<asp:TextBox ID="txtBraNum" Enabled="false" Width="60" MaxLength="4" runat="server" Visible=false></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top"><asp:Label ID="lblDeptNum" Text="Department:" runat="server"></asp:Label>&nbsp;</td>
                        <td valign="top"><asp:DropDownList ID="ddlDeptNum" Width="300" runat="server"></asp:DropDownList>                       
                            &nbsp;<asp:TextBox ID="txtDeptNum" Visible="false" Enabled="false" Width="60" MaxLength="5" runat="server"></asp:TextBox>
                            &nbsp;<asp:TextBox ID="txtDeptName" Visible="false" Enabled="false" Width="60" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    
                    <tr>
                        <td align="right" valign="top"><asp:Label ID="lblTransDate" Text="Transaction Date:" runat="server"></asp:Label>&nbsp;</td>
                        <td valign="top"><asp:TextBox ID="txtTransDate" MaxLength="10" runat="server"></asp:TextBox>&nbsp;&nbsp;dd/mm/yyyy</td>                       
                    </tr>
                    <tr>
                        <td align="right" valign="top"><asp:Label ID="lblTransAmt" Text="Amount =N=:" runat="server"></asp:Label>&nbsp;</td>
                        <td valign="top"><asp:TextBox ID="txtTransAmt" MaxLength="15" runat="server"></asp:TextBox>&nbsp;</td>
                    </tr>
                    <tr>
                        <td align="right" valign="top"><asp:Label ID="lblTransDescr" Text="Description:" runat="server"></asp:Label>&nbsp;</td>
                        <td valign="top"><asp:TextBox ID="txtTransDescr" MaxLength="90" runat="server" Width="400px"></asp:TextBox>&nbsp;</td>
                    </tr>
                    <tr>
                        <td align="right" valign="top"><asp:Label ID="lblServiceComp" Text="Service Company:" runat="server"></asp:Label>&nbsp;</td>
                        <td valign="top"><asp:DropDownList ID="ddlServiceComp" AutoPostBack="true" Width="300" runat="server"></asp:DropDownList>                       
                            &nbsp;<asp:TextBox ID="txtServiceComp" Enabled="false" Width="60" MaxLength="10" runat="server" Visible=false></asp:TextBox>
                        </td>
                    </tr>
                    
                    <tr>
                        <td align="right" valign="top"><asp:Label ID="Label2" Text="Service HRS:" runat="server"></asp:Label>&nbsp;</td>
                        <td valign="top"><asp:TextBox ID="txtServiceHrs" MaxLength="15" runat="server"></asp:TextBox></td>
                    </tr>

                    <tr>
                        <td colspan="2" valign="top">&nbsp;</td>
                    </tr>

                </table>
            </td>
                
        </tr>

                <tr>
                    <td colspan="7" valign="top">
                    <table align="center" width="95%" style="border-style:groove;">
                        <tr>
                            <td align="left" colspan="4" valign="top" class="tbl_caption">Transaction Details</td>
                        </tr>
			           <tr>
                            <td align="left" colspan="4" valign="top" style=" background-color: #ccccee; height: 26px">
					            &nbsp;&nbsp;<asp:Button ID="cmdDelItem_ASP" Enabled="true" Font-Bold="true" Text="Delete Item" runat="server" />
					            &nbsp;&nbsp;<asp:Button ID="cmdDelItem" Enabled="false" Font-Bold="true" Visible="false" Text="Delete Item" runat="server" />
				                </td>
			            </tr>

                        <tr>
                            <td colspan="4" valign="top" style="width:auto;">
                                <asp:GridView id="grdData" Font-Size="Small" BackColor="White" bordercolor="Silver" borderstyle="Solid" runat="server"
                                    horizontalalign="Left" DataSourceID="ods"
                                    AutoGenerateColumns="false" AllowPaging="true" PageSize="10"
                                    selectedindex="0" PagerSettings-Position="TopAndBottom" PagerSettings-Mode="NextPreviousFirstLast"
                                    PagerSettings-FirstPageText="First" PagerSettings-NextPageText="Next"
                                    PagerSettings-PreviousPageText="Previous" PagerSettings-LastPageText="Last"
                                    EmptyDataText="No data available." Width="100%">
                        
	                                <RowStyle></RowStyle>
	                                <PagerStyle></PagerStyle>
	                                <HeaderStyle BackColor="#9ACD32" ForeColor="White"></HeaderStyle>
                                    <selectedrowstyle backcolor="LightCyan" forecolor="DarkBlue" font-bold="true"/>  
                        
                                    <Columns>
                                        <asp:TemplateField>
        			                        <ItemTemplate>
        						                <asp:CheckBox id="chkSel" runat="server"></asp:CheckBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:HyperLinkField DataTextField="rbId" DataNavigateUrlFields="rbId,TransNo"
                                            DataNavigateUrlFormatString="~/AD140.aspx?idd={0},{1}" HeaderText="ID">
                                        </asp:HyperLinkField>
                            
                                        <asp:HyperLinkField DataTextField="TransNo" DataNavigateUrlFields="rbId,TransNo"
                                            DataNavigateUrlFormatString="~/AD140.aspx?idd={0},{1}" HeaderText="Trans #">
                                        </asp:HyperLinkField>
                            
                                        <asp:BoundField DataField="TransDate" HeaderText="Trans Date" DataFormatString="{0:d}"/>
                                        <asp:TemplateField  HeaderText="Trans. Amt">
                                          <ItemTemplate>
                                           <asp:Label ID="lblTransAmt" runat="server" DataFormatString="{0:N2}" Text='<%#Eval("TransAmount") %>' />
                                          </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Description" HeaderText="Trans Description"/>
                            
                                    </Columns>
  
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>

                    </td>    
                </tr>


                <tr>
                    <td colspan="7" valign="top">&nbsp;</td>
        <asp:ObjectDataSource ID="ods" runat="server" SelectMethod="RepairsBills" TypeName="CustodianAdmin.Data.RepairsBillRepository">
    </asp:ObjectDataSource>  
                </tr>    

    </table>

    </div>
    </form>
</body>
</html>
