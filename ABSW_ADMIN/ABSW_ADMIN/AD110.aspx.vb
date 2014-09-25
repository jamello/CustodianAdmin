Imports System.Data
Imports CustodianAdmin.Data
Imports CustodianAdmin.Model
Imports CustodianAdmin.Repositories

Partial Public Class AD110
    Inherits System.Web.UI.Page

    Protected FirstMsg As String
    Protected PageLinks As String

    Protected strPOP_UP As String
    Dim tbRepo As TelephoneBillRepository
    Dim acRepo As AdminCodeRepository
    Dim tbill As TelephoneBill
    Dim updateFlag As Boolean
    Dim strKey As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'SessionProvider.RebuildSchema()
        txtTelUsersName.Attributes.Add("disabled", "disabled")

        If Not Page.IsPostBack Then
            tbRepo = New TelephoneBillRepository
            acRepo = New AdminCodeRepository

            Session("tbRepo") = tbRepo
            updateFlag = False
            Session("updateFlag") = updateFlag
            strKey = Request.QueryString("idd")
            Session("strKey") = strKey


            SetComboBinding(ddlBraNum, acRepo.GetBranches(), "BranchName", "BranchNo")
            SetComboBinding(ddlDeptNum, acRepo.GetDepartments(), "LongDesc", "dId")

            If strKey IsNot Nothing Then
                FillValues()
            Else
                tbRepo = CType(Session("tbRepo"), TelephoneBillRepository)
            End If

        Else 'post back

            tbRepo = CType(Session("tbRepo"), TelephoneBillRepository)

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
            tbill = New TelephoneBill

            'lblError.Visible = False
            tbill.BranchCode = ddlBraNum.SelectedValue.ToString()
            tbill.Department = ddlDeptNum.SelectedValue.ToString()
            tbill.UserName = txtTelUsersName.Text
            tbill.Description = txtTransDescr.Text
            tbill.EntryDate = Date.Now
            tbill.EntryFlag = "A"
            tbill.OperatorId = "001"
            tbill.TelephoneNo = txtTransNum.Text
            tbill.TransDate = ValidDate(txtTransDate.Text)
            tbill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)


            tbRepo.Save(tbill)
            Session("tbill") = tbill
        Else
            tbRepo = CType(Session("tbRepo"), TelephoneBillRepository)
            tbill = CType(Session("tbill"), TelephoneBill)

            tbill.BranchCode = ddlBraNum.SelectedValue.ToString()
            tbill.Department = ddlDeptNum.SelectedValue.ToString()
            tbill.UserName = txtTelUsersName.Text
            tbill.Description = txtTransDescr.Text
            ' tbill.EntryDate = Date.Now
            'tbill.EntryFlag = "A"
            'tbill.OperatorId = "001"
            tbill.TelephoneNo = txtTransNum.Text
            tbill.TransDate = ValidDate(txtTransDate.Text)
            tbill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)
            tbRepo.Save(tbill)

        End If
        grdData.DataBind()
        initializeFields()


    End Sub
    Private Sub initializeFields()
        txtBraName.Text = String.Empty
        txtBraNum.Text = String.Empty
        txtDeptName.Text = String.Empty
        txtDeptNum.Text = String.Empty
        txtTransAmt.Text = String.Empty
        txtTransDate.Text = String.Empty
        txtTelUsersName.Text = String.Empty
        txtTransDescr.Text = String.Empty
        txtTransNum.Text = String.Empty
        ddlBraNum.SelectedIndex = 0
        ddlDeptNum.SelectedIndex = 0

    End Sub
    Private Sub SetComboBinding(ByVal toBind As ListControl, ByVal dataSource As Object, ByVal displayMember As String, ByVal valueMember As String)
        toBind.DataTextField = displayMember
        toBind.DataValueField = valueMember
        toBind.DataSource = dataSource
        toBind.DataBind()
    End Sub

    Private Sub FillValues()
        strKey = CType(Session("strKey"), String)
        tbRepo = CType(Session("tbRepo"), TelephoneBillRepository)
        tbill = CType(Session("tbill"), TelephoneBill)

        tbill = tbRepo.GetById(strKey)

        If tbill IsNot Nothing Then
            With tbill
                txtTelUsersName.Text = .UserName
                txtTransDate.Text = ValidDateFromDB(.TransDate)
                txtTransDescr.Text = .Description

                ddlBraNum.SelectedValue = tbill.BranchCode
                ddlDeptNum.SelectedValue = tbill.Department
                txtTransNum.Text = tbill.TelephoneNo
                txtTransAmt.Text = Math.Round(.TransAmount, 2)
                Session("tbill") = tbill
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
        tbRepo = CType(Session("tbRepo"), TelephoneBillRepository)
        tbill = CType(Session("tbill"), TelephoneBill)
        Try
            tbRepo.Delete(tbill)
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

    <System.Web.Services.WebMethod()> _
Public Shared Function GetMiscAdminInfo(ByVal _clascod As String, ByVal _itmcode As String) As String
        Dim codeinfo As String = String.Empty
        Dim admRepo As New AdminCodeRepository()
        'Dim crit As String = 

        Try
            codeinfo = admRepo.GetMiscAdminInfo(_clascod, _itmcode)
            Return codeinfo
        Finally
            If codeinfo = "<NewDataSet />" Then
                Throw New Exception()
            End If
        End Try

    End Function

    Protected Sub cmdPrint_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdPrint.Click

    End Sub
End Class