Imports System.Data
Imports CustodianAdmin.Data
Imports CustodianAdmin.Model
Imports CustodianAdmin.Repositories

Partial Public Class AD160
    Inherits System.Web.UI.Page

    Protected FirstMsg As String
    Protected PageLinks As String

    Protected strPOP_UP As String
    Dim obRepo As OutsourceBillRepository
    Dim acRepo As AdminCodeRepository
    Dim obill As OutsourceBill
    Dim updateFlag As Boolean
    Dim strKey As String



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'SessionProvider.RebuildSchema()

        If Not Page.IsPostBack Then
            obRepo = New OutsourceBillRepository
            acRepo = New AdminCodeRepository

            Session("obRepo") = obRepo
            updateFlag = False
            Session("updateFlag") = updateFlag
            strKey = Request.QueryString("idd")
            Session("strKey") = strKey


            SetComboBinding(ddlBraNum, acRepo.GetBranches(), "BranchName", "BranchNo")
            SetComboBinding(ddlDeptNum, acRepo.GetDepartments(), "LongDesc", "dId")

            If strKey IsNot Nothing Then
                FillValues()
            Else
                obRepo = CType(Session("obRepo"), OutsourceBillRepository)
            End If

        Else 'post back

            obRepo = CType(Session("obRepo"), OutsourceBillRepository)

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
            obill = New OutsourceBill

            'lblError.Visible = False
            obill.BranchCode = ddlBraNum.SelectedValue.ToString()
            obill.Department = ddlDeptNum.SelectedValue.ToString()
            obill.TransDescription = txtTransDescr.Text
            obill.EntryDate = Date.Now
            obill.EntryFlag = "A"
            obill.OperatorId = "001"
            obill.TransNo = txtTransNum.Text
            obill.TransDate = ValidDate(txtTransDate.Text)
            obill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)


            obill.TransactionType = cmbTransType.SelectedValue
            obill.NoOfStaff = Math.Round(CType(txtNoOfStaff.Text, Decimal), 2)
            obill.SupplyCompany = txtServiceComp.Text


            obRepo.Save(obill)
            Session("obill") = obill
        Else
            obRepo = CType(Session("obRepo"), OutsourceBillRepository)
            obill = CType(Session("obill"), OutsourceBill)

            obill.BranchCode = ddlBraNum.SelectedValue.ToString()
            obill.Department = ddlDeptNum.SelectedValue.ToString()
            obill.TransDescription = txtTransDescr.Text
            obill.TransDate = ValidDate(txtTransDate.Text)
            obill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)

            obill.TransactionType = cmbTransType.SelectedValue
            obill.NoOfStaff = Math.Round(CType(txtNoOfStaff.Text, Decimal), 2)
            obill.SupplyCompany = txtServiceComp.Text

            obRepo.Save(obill)

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
        txtNoOfStaff.Text = String.Empty
        cmbTransType.SelectedIndex = 0
        txtTransDescr.Text = String.Empty

    End Sub
    Private Sub SetComboBinding(ByVal toBind As ListControl, ByVal dataSource As Object, ByVal displayMember As String, ByVal valueMember As String)
        toBind.DataTextField = displayMember
        toBind.DataValueField = valueMember
        toBind.DataSource = dataSource
        toBind.DataBind()
    End Sub

    Private Sub FillValues()
        strKey = CType(Session("strKey"), String)
        obRepo = CType(Session("obRepo"), OutsourceBillRepository)
        obill = CType(Session("obill"), OutsourceBill)

        obill = obRepo.GetById(strKey)

        If obill IsNot Nothing Then
            With obill
                txtTransDate.Text = ValidDateFromDB(.TransDate)
                txtTransDescr.Text = .TransDescription

                ddlBraNum.SelectedValue = obill.BranchCode
                ddlDeptNum.SelectedValue = obill.Department
                txtTransNum.Text = obill.TransNo
                txtTransAmt.Text = Math.Round(.TransAmount, 2)
                txtNoOfStaff.Text = Math.Round(.NoOfStaff, 2)
                txtServiceComp.Text = .SupplyCompany
                cmbTransType.SelectedValue = .TransactionType

                Session("obill") = obill
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
        obRepo = CType(Session("obRepo"), OutsourceBillRepository)
        obill = CType(Session("obill"), OutsourceBill)
        Try
            obRepo.Delete(obill)
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