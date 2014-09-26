Imports System.Data
Imports System.Data.OleDb
Imports CustodianAdmin.Data
Imports CustodianAdmin.Model
Imports CustodianAdmin.Repositories

Partial Public Class AD120
    Inherits System.Web.UI.Page
    Protected FirstMsg As String
    Protected PageLinks As String
    Protected strPOP_UP As String
    Dim ebRepo As ElectricBillRepository
    Dim acRepo As AdminCodeRepository

    Dim ebill As ElectricityBill
    Dim updateFlag As Boolean
    Dim strKey As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' SessionProvider.RebuildSchema()

        If Not Page.IsPostBack Then
            ebRepo = New ElectricBillRepository
            acRepo = New AdminCodeRepository

            Session("ebRepo") = ebRepo

            updateFlag = False
            Session("updateFlag") = updateFlag
            strKey = Request.QueryString("idd")
            Session("strKey") = strKey


            SetComboBinding(ddlBraNum, acRepo.GetBranches(), "BranchName", "BranchNo")
            SetComboBinding(ddlDeptNum, acRepo.GetDepartments(), "LongDesc", "dId")

            If strKey IsNot Nothing Then
                FillValues()
            Else
                ebRepo = CType(Session("ebRepo"), ElectricBillRepository)
            End If

        Else 'post back

            ebRepo = CType(Session("ebRepo"), ElectricBillRepository)

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
            ebill = New ElectricityBill

            'lblError.Visible = False
            ebill.BranchCode = ddlBraNum.SelectedValue.ToString()
            ebill.Department = ddlDeptNum.SelectedValue.ToString()
            ebill.MeterNo = txtTransNum.Text
            ebill.PeriodPaidFor = txtPeriodPaidFor.Text
            ebill.AccountNo = txtAccountNo.Text
            ebill.EntryDate = Date.Now
            ebill.EntryFlag = "A"
            ebill.OperatorId = "001"
            ebill.TransDate = ValidDate(txtTransDate.Text)
            ebill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)


            ebRepo.Save(ebill)
            Session("ebill") = ebill
        Else
            ebRepo = CType(Session("ebRepo"), ElectricBillRepository)
            ebill = CType(Session("ebill"), ElectricityBill)

            ebill.BranchCode = ddlBraNum.SelectedValue.ToString()
            ebill.Department = ddlDeptNum.SelectedValue.ToString()

            'ebill.MeterNo = txtTransNum.Text
            ebill.PeriodPaidFor = txtPeriodPaidFor.Text
            ebill.AccountNo = txtAccountNo.Text
            ' ebill.EntryDate = Date.Now
            'ebill.EntryFlag = "A"
            'ebill.OperatorId = "001"
            ebill.TransDate = ValidDate(txtTransDate.Text)
            ebill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)
            ebRepo.Save(ebill)
            Session("ebill") = ebill


        End If
        grdData.DataBind()
        initializeFields()

    End Sub
    Private Sub initializeFields()
        ddlBraNum.SelectedIndex = -1
        ddlDeptNum.SelectedIndex = -1
        txtTransNum.Text = String.Empty
        txtPeriodPaidFor.Text = String.Empty
        txtAccountNo.Text = String.Empty
        txtTransDate.Text = String.Empty
        txtTransAmt.Text = "0.00"

    End Sub
    Private Sub SetComboBinding(ByVal toBind As ListControl, ByVal dataSource As Object, ByVal displayMember As String, ByVal valueMember As String)
        toBind.DataTextField = displayMember
        toBind.DataValueField = valueMember
        toBind.DataSource = dataSource
        toBind.DataBind()
    End Sub

    Private Sub FillValues()
        strKey = CType(Session("strKey"), String)
        ebRepo = CType(Session("ebRepo"), ElectricBillRepository)
        ebill = CType(Session("ebill"), ElectricityBill)

        ebill = ebRepo.GetById(strKey)

        If ebill IsNot Nothing Then
            With ebill
                ddlBraNum.SelectedValue = ebill.BranchCode
                ddlDeptNum.SelectedValue = ebill.Department
                txtTransNum.Text = ebill.MeterNo
                txtPeriodPaidFor.Text = ebill.PeriodPaidFor
                txtAccountNo.Text = ebill.AccountNo
                txtTransDate.Text = ValidDateFromDB(ebill.TransDate)
                txtTransAmt.Text = Math.Round(CType(ebill.TransAmount, Decimal), 2)
                Session("ebill") = ebill
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

    Protected Sub cmdDel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDel.Click, cmdDelN.Click
        Dim msg As String = String.Empty
        ebRepo = CType(Session("ebRepo"), ElectricBillRepository)
        ebill = CType(Session("ebill"), ElectricityBill)
        Try
            ebRepo.Delete(ebill)
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

    Protected Sub cmdNew_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdNew.Click
        initializeFields()
        updateFlag = False 'Switches to first time load
        Session("updateFlag") = updateFlag

    End Sub
End Class