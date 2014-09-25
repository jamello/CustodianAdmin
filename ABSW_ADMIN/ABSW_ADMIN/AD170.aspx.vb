Imports System.Data
Imports CustodianAdmin.Data
Imports CustodianAdmin.Model
Imports CustodianAdmin.Repositories

Partial Public Class AD170
    Inherits System.Web.UI.Page

    Protected FirstMsg As String
    Protected PageLinks As String

    Protected strPOP_UP As String

    Dim ibRepo As InsurancePremRepository
    Dim acRepo As AdminCodeRepository
    Dim ibill As InsurancePrem
    Dim updateFlag As Boolean
    Dim strKey As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'SessionProvider.RebuildSchema()

        If Not Page.IsPostBack Then
            ibRepo = New InsurancePremRepository
            acRepo = New AdminCodeRepository

            Session("ibRepo") = ibRepo
            updateFlag = False
            Session("updateFlag") = updateFlag
            strKey = Request.QueryString("idd")
            Session("strKey") = strKey


            SetComboBinding(ddlBraNum, acRepo.GetBranches(), "BranchName", "BranchNo")
            SetComboBinding(ddlDeptNum, acRepo.GetDepartments(), "LongDesc", "dId")
            SetComboBinding(cmbInsurerName, acRepo.GetAdminCodes("004"), "ItemDesc", "ItemCode")
            SetComboBinding(cmbBrokerName, acRepo.GetAdminCodes("005"), "ItemDesc", "ItemCode")

            If strKey IsNot Nothing Then
                FillValues()
            Else
                ibRepo = CType(Session("ibRepo"), InsurancePremRepository)
            End If

        Else 'post back

            ibRepo = CType(Session("ibRepo"), InsurancePremRepository)

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
            ibill = New InsurancePrem

            'lblError.Visible = False
            ibill.BranchCode = ddlBraNum.SelectedValue.ToString()
            ibill.Department = ddlDeptNum.SelectedValue.ToString()
            ibill.TransDescription = txtTransDescr.Text
            ibill.EntryDate = Date.Now
            ibill.EntryFlag = "A"
            ibill.OperatorId = "001"
            ibill.TransNo = txtTransNum.Text
            ibill.TransDate = ValidDate(txtTransDate.Text)
            ibill.SumInsured = Math.Round(CType(txtSumInsured.Text, Decimal), 2)
            ibill.PremiumAmt = Math.Round(CType(txtPremAmt.Text, Decimal), 2)
            ibill.TransactionType = cmbTransType.SelectedValue

            ibill.PolicyNo = txtPolicyNo.Text
            ibill.InsurerName = cmbInsurerName.SelectedValue
            ibill.BrokerName = cmbBrokerName.SelectedValue
            ibill.EndDate = ValidDate(txtEndDate.Text)
            ibill.StartDate = ValidDate(txtStartDate.Text)

            ibRepo.Save(ibill)
            Session("ibill") = ibill
        Else
            ibRepo = CType(Session("ibRepo"), InsurancePremRepository)
            ibill = CType(Session("ibill"), InsurancePrem)

            ibill.BranchCode = ddlBraNum.SelectedValue.ToString()
            ibill.Department = ddlDeptNum.SelectedValue.ToString()
            ibill.TransDescription = txtTransDescr.Text
            ibill.TransDate = ValidDate(txtTransDate.Text)
            ibill.TransactionType = cmbTransType.SelectedValue

            ibill.SumInsured = Math.Round(CType(txtSumInsured.Text, Decimal), 2)
            ibill.PremiumAmt = Math.Round(CType(txtPremAmt.Text, Decimal), 2)
            ibill.TransactionType = cmbTransType.SelectedValue

            ibill.PolicyNo = txtPolicyNo.Text
            ibill.InsurerName = cmbInsurerName.SelectedValue
            ibill.BrokerName = cmbBrokerName.SelectedValue
            ibill.EndDate = ValidDate(txtEndDate.Text)
            ibill.StartDate = ValidDate(txtStartDate.Text)

            ibRepo.Save(ibill)

        End If
        grdData.DataBind()
        initializeFields()

    End Sub
    Private Sub initializeFields()
        txtBraNum.Text = String.Empty
        txtDeptName.Text = String.Empty
        txtDeptNum.Text = String.Empty
        txtTransDate.Text = String.Empty
        txtTransDescr.Text = String.Empty
        txtTransNum.Text = String.Empty
        ddlBraNum.SelectedIndex = 0
        ddlDeptNum.SelectedIndex = 0
        cmbTransType.SelectedIndex = 0
        txtTransDescr.Text = String.Empty
        txtSumInsured.Text = String.Empty
        txtPremAmt.Text = String.Empty
        txtPolicyNo.Text = String.Empty
        txtInsurerName.Text = String.Empty
        txtEndDate.Text = String.Empty
        txtStartDate.Text = String.Empty
        cmbInsurerName.SelectedIndex = 0
        cmbBrokerName.SelectedIndex = 0

    End Sub
    Private Sub SetComboBinding(ByVal toBind As ListControl, ByVal dataSource As Object, ByVal displayMember As String, ByVal valueMember As String)
        toBind.DataTextField = displayMember
        toBind.DataValueField = valueMember
        toBind.DataSource = dataSource
        toBind.DataBind()
    End Sub

    Private Sub FillValues()
        strKey = CType(Session("strKey"), String)
        ibRepo = CType(Session("ibRepo"), InsurancePremRepository)
        ibill = CType(Session("ibill"), InsurancePrem)

        ibill = ibRepo.GetById(strKey)

        If ibill IsNot Nothing Then
            With ibill
                txtTransDate.Text = ValidDateFromDB(.TransDate)
                txtTransDescr.Text = .TransDescription

                ddlBraNum.SelectedValue = ibill.BranchCode
                ddlDeptNum.SelectedValue = ibill.Department
                txtTransNum.Text = ibill.TransNo
                cmbTransType.SelectedValue = .TransactionType

                txtSumInsured.Text = ibill.SumInsured
                txtPremAmt.Text = ibill.PremiumAmt

                txtPolicyNo.Text = ibill.PolicyNo
                txtEndDate.Text = ValidDateFromDB(ibill.EndDate)
                txtStartDate.Text = ValidDateFromDB(ibill.StartDate)
                cmbInsurerName.SelectedValue = ibill.InsurerName
                cmbBrokerName.SelectedValue = ibill.BrokerName


                Session("ibill") = ibill
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
        ibRepo = CType(Session("ibRepo"), InsurancePremRepository)
        ibill = CType(Session("ibill"), InsurancePrem)
        Try
            ibRepo.Delete(ibill)
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