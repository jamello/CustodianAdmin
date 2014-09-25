Imports System.Data
Imports CustodianAdmin.Data
Imports CustodianAdmin.Model
Imports CustodianAdmin.Repositories

Partial Public Class AD150
    Inherits System.Web.UI.Page

    Protected FirstMsg As String
    Protected PageLinks As String

    Protected strPOP_UP As String

    Dim dbRepo As DieselBillRepository
    Dim acRepo As AdminCodeRepository
    Dim dbill As DieselBill
    Dim updateFlag As Boolean
    Dim strKey As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'SessionProvider.RebuildSchema()
        txtTransAmt.Attributes.Add("disabled", "disabled")

        If Not Page.IsPostBack Then
            dbRepo = New DieselBillRepository
            acRepo = New AdminCodeRepository

            Session("dbRepo") = dbRepo
            updateFlag = False
            Session("updateFlag") = updateFlag
            strKey = Request.QueryString("idd")
            Session("strKey") = strKey


            SetComboBinding(ddlBraNum, acRepo.GetBranches(), "BranchName", "BranchNo")
            SetComboBinding(ddlDeptNum, acRepo.GetDepartments(), "LongDesc", "dId")
            SetComboBinding(cmbServiceComp, acRepo.GetAdminCodes("002"), "ItemDesc", "ItemCode")

            If strKey IsNot Nothing Then
                FillValues()
            Else
                dbRepo = CType(Session("dbRepo"), DieselBillRepository)
            End If

        Else 'post back

            dbRepo = CType(Session("dbRepo"), DieselBillRepository)

            Me.Validate()
            If (Not Me.IsValid) Then
                Dim msg As String
                ' Loop through all validation controls to see which 
                ' generated the error(s).
                Dim oValidator As IValidator
                For Each oValidator In Validators
                    If oValidator.IsValid = False Then
                        msg = msg & "\n" & oValidator.ErrorMessage
                    End If
                Next

                'lblError.Text = msg
                'lblError.Visible = True
                'publicMsgs = "javascript:alert('" + msg + "')"
            End If



        End If




        Try
            strPOP_UP = CType(Request.QueryString("popup"), String)
        Catch ex As Exception
            strPOP_UP = "NO"
        End Try

        If UCase(Trim(strPOP_UP)) = "YES" Then
            Me.PageAnchor_Return_Link.Visible = False
            PageLinks = "<a href='javascript:void(0);' onclick='javascript:window.close();'>CLOSE PAGE...</a>"
        Else
            Me.PageAnchor_Return_Link.Visible = True
            PageLinks = ""
        End If

    End Sub

    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click, cmdSaveN.Click
        updateFlag = CType(Session("updateFlag"), String)

        If Not updateFlag Then 'if new record
            dbill = New DieselBill

            'lblError.Visible = False
            dbill.BranchCode = ddlBraNum.SelectedValue.ToString()
            dbill.Department = ddlDeptNum.SelectedValue.ToString()
            dbill.TransDescription = txtTransDescr.Text
            dbill.EntryDate = Date.Now
            dbill.EntryFlag = "A"
            dbill.OperatorId = "001"
            dbill.TransNo = txtTransNum.Text
            dbill.TransDate = ValidDate(txtTransDate.Text)
            dbill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)


            dbill.UnitPrice = Math.Round(CType(txtTransRate.Text, Decimal), 2)
            dbill.TransactionType = cmbTransType.SelectedValue
            dbill.Quantity = Math.Round(CType(txtTransQty.Text, Decimal), 2)
            dbill.SupplyCompany = cmbServiceComp.SelectedValue


            dbRepo.Save(dbill)
            Session("dbill") = dbill
        Else
            dbRepo = CType(Session("dbRepo"), DieselBillRepository)
            dbill = CType(Session("dbill"), DieselBill)

            dbill.BranchCode = ddlBraNum.SelectedValue.ToString()
            dbill.Department = ddlDeptNum.SelectedValue.ToString()
            dbill.TransDescription = txtTransDescr.Text
            dbill.TransDate = ValidDate(txtTransDate.Text)
            dbill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)

            dbill.UnitPrice = Math.Round(CType(txtTransRate.Text, Decimal), 2)
            dbill.TransactionType = cmbTransType.SelectedValue
            dbill.Quantity = Math.Round(CType(txtTransQty.Text, Decimal), 2)
            dbill.SupplyCompany = cmbServiceComp.SelectedValue

            dbRepo.Save(dbill)

        End If
        grdData.DataBind()
        initializeFields()




    End Sub
    Private Sub initializeFields()
        txtBraNum.Text = String.Empty
        txtDeptName.Text = String.Empty
        txtDeptNum.Text = String.Empty
        txtTransAmt.Text = String.Empty
        txtTransDate.Text = String.Empty
        txtTransDescr.Text = String.Empty
        txtTransNum.Text = String.Empty
        ddlBraNum.SelectedIndex = 0
        ddlDeptNum.SelectedIndex = 0
        txtServiceComp.Text = String.Empty
        txtTransQty.Text = String.Empty
        txtTransRate.Text = String.Empty
        cmbTransType.SelectedIndex = 0
        cmbServiceComp.SelectedIndex = 0


    End Sub
    Private Sub SetComboBinding(ByVal toBind As ListControl, ByVal dataSource As Object, ByVal displayMember As String, ByVal valueMember As String)
        toBind.DataTextField = displayMember
        toBind.DataValueField = valueMember
        toBind.DataSource = dataSource
        toBind.DataBind()
    End Sub

    Private Sub FillValues()
        strKey = CType(Session("strKey"), String)
        dbRepo = CType(Session("dbRepo"), DieselBillRepository)
        dbill = CType(Session("dbill"), DieselBill)

        dbill = dbRepo.GetById(strKey)

        If dbill IsNot Nothing Then
            With dbill
                txtTransDate.Text = ValidDateFromDB(.TransDate)
                txtTransDescr.Text = .TransDescription

                ddlBraNum.SelectedValue = dbill.BranchCode
                ddlDeptNum.SelectedValue = dbill.Department
                txtTransNum.Text = dbill.TransNo
                txtTransAmt.Text = Math.Round(.TransAmount, 2)
                txtTransRate.Text = Math.Round(.UnitPrice, 2)
                txtTransQty.Text = Math.Round(.Quantity, 2)
                cmbTransType.SelectedValue = .TransactionType
                cmbServiceComp.SelectedValue = .SupplyCompany

                Session("dbill") = dbill
            End With

            updateFlag = True
            Session("updateFlag") = updateFlag
            grdData.DataBind()


        End If
    End Sub
    Private Function ValidDate(ByVal DateValue As String) As DateTime
        Dim dateparts() As String = DateValue.Split(Microsoft.VisualBasic.ChrW(47))
        Dim strDateTest As String = dateparts(1) & "/" & dateparts(0) & "/" & dateparts(2)
        Dim dateIn As Date = Format(CDate(strDateTest), "MM/dd/yyyy")
        Return dateIn
    End Function
    Private Function ValidDateFromDB(ByVal DateValue As Date) As String
        Dim dateparts() As String = DateValue.Date.ToString.Split(Microsoft.VisualBasic.ChrW(47))
        Dim strDateTest As String = dateparts(1) & "/" & dateparts(0) & "/" & Left(dateparts(2), 4)
        Return strDateTest
    End Function

    Protected Sub cmdNew_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdNew.Click
        initializeFields()
        updateFlag = False 'Switches to first time load
        Session("updateFlag") = updateFlag


    End Sub

    Protected Sub cmdDel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDel.Click, cmdDelN.Click

        Dim msg As String = String.Empty
        dbRepo = CType(Session("dbRepo"), DieselBillRepository)
        dbill = CType(Session("dbill"), DieselBill)
        Try
            dbRepo.Delete(dbill)
            msg = "Delete Successful"
            ' lblError.Text = msg
            grdData.DataBind()

        Catch ex As Exception
            msg = ex.Message
            'lblError.Text = msg
            'lblError.Visible = True
            'publicMsgs = "javascript:alert('" + msg + "')"
        End Try
        initializeFields()


    End Sub
End Class