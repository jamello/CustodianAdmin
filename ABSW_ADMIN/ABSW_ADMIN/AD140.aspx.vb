Imports System.Data
Imports CustodianAdmin.Data
Imports CustodianAdmin.Model
Imports CustodianAdmin.Repositories

Partial Public Class AD140
    Inherits System.Web.UI.Page

    Protected FirstMsg As String
    Protected PageLinks As String

    Protected strPOP_UP As String
    Dim rbRepo As RepairsBillRepository
    Dim acRepo As AdminCodeRepository
    Dim rbill As RepairsBill
    Dim updateFlag As Boolean
    Dim strKey As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'SessionProvider.RebuildSchema()


        If Not Page.IsPostBack Then
            rbRepo = New RepairsBillRepository
            acRepo = New AdminCodeRepository

            Session("rbRepo") = rbRepo
            updateFlag = False
            Session("updateFlag") = updateFlag
            strKey = Request.QueryString("idd")
            Session("strKey") = strKey

            SetComboBinding(ddlBraNum, acRepo.GetBranches(), "BranchName", "BranchNo")
            SetComboBinding(ddlDeptNum, acRepo.GetDepartments(), "LongDesc", "dId")

            SetComboBinding(ddlServiceComp, rbRepo.GetAdminCodes("002"), "ItemDesc", "ItemCode")
            SetComboBinding(ddlTransType, rbRepo.GetAdminCodes("003"), "ItemDesc", "ItemCode")

            If strKey IsNot Nothing Then
                FillValues()
            Else
                rbRepo = CType(Session("rbRepo"), RepairsBillRepository)
            End If

        Else 'post back

            rbRepo = CType(Session("rbRepo"), RepairsBillRepository)

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
            rbill = New RepairsBill

            'lblError.Visible = False
            rbill.BranchCode = ddlBraNum.SelectedValue.ToString()
            rbill.Department = ddlDeptNum.SelectedValue.ToString()
            rbill.Description = txtTransDescr.Text
            rbill.EntryDate = Date.Now
            rbill.EntryFlag = "A"
            rbill.OperatorId = "001"
            rbill.TransNo = txtTransNum.Text
            rbill.TransDate = ValidDate(txtTransDate.Text)
            rbill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)
            rbill.ServiceCoy = ddlServiceComp.SelectedValue
            rbill.ServiceHrs = CType(txtServiceHrs.Text, Integer)

            rbRepo.Save(rbill)
            Session("rbill") = rbill
        Else
            rbRepo = CType(Session("rbRepo"), RepairsBillRepository)
            rbill = CType(Session("rbill"), RepairsBill)

            rbill.BranchCode = ddlBraNum.SelectedValue.ToString()
            rbill.Department = ddlDeptNum.SelectedValue.ToString()
            rbill.Description = txtTransDescr.Text
            rbill.TransNo = txtTransNum.Text
            rbill.TransDate = ValidDate(txtTransDate.Text)
            rbill.TransAmount = Math.Round(CType(txtTransAmt.Text, Decimal), 2)
            rbill.ServiceCoy = ddlServiceComp.SelectedValue
            rbill.ServiceHrs = CType(txtServiceHrs.Text, Integer)

            rbRepo.Save(rbill)

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
        ddlServiceComp.SelectedIndex = 0
        txtServiceHrs.Text = String.Empty
        txtTransNum.Text = String.Empty

    End Sub
    Private Sub SetComboBinding(ByVal toBind As ListControl, ByVal dataSource As Object, ByVal displayMember As String, ByVal valueMember As String)
        toBind.DataTextField = displayMember
        toBind.DataValueField = valueMember
        toBind.DataSource = dataSource
        toBind.DataBind()
    End Sub

    Private Sub FillValues()
        strKey = CType(Session("strKey"), String)
        rbRepo = CType(Session("rbRepo"), RepairsBillRepository)
        rbill = CType(Session("rbill"), RepairsBill)

        rbill = rbRepo.GetById(strKey)

        If rbill IsNot Nothing Then
            With rbill
                txtTransDate.Text = ValidDateFromDB(.TransDate)
                txtTransDescr.Text = .Description

                ddlBraNum.SelectedValue = rbill.BranchCode
                ddlDeptNum.SelectedValue = rbill.Department
                txtTransNum.Text = rbill.TransNo
                txtTransAmt.Text = Math.Round(.TransAmount, 2)
                txtServiceHrs.Text = .ServiceHrs
                ddlServiceComp.SelectedValue = .ServiceCoy

                Session("rbill") = rbill
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
        rbRepo = CType(Session("rbRepo"), RepairsBillRepository)
        rbill = CType(Session("rbill"), RepairsBill)
        Try
            rbRepo.Delete(rbill)
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