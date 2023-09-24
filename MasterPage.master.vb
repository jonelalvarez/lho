Imports System.Data
Imports System.Configuration
Imports MySql.Data.MySqlClient
Imports System.Drawing
Imports System.IO
Imports System.Data.OleDb
Imports System.Net
Imports System.Security.Cryptography
Imports System.Web.Services

Partial Class MasterPage
    Inherits System.Web.UI.MasterPage
    Public activePage As String
    Public ConnString As String

    Public myModules As New List(Of String)()
    Public globalSettings As DataTable
    Public userID, userName, userType, userPassword, moduleAccess, headID, deptID As String
    Public dateNow As DateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"))
    Public savePath As String
    Dim clsMaster As New cls_master

    Protected Sub Page_Error(sender As Object, e As EventArgs) Handles Me.Error
        Response.Redirect("404.aspx")
    End Sub

    Private Sub MasterPage_Init(sender As Object, e As EventArgs) Handles Me.Init
        
    End Sub

    Private Sub MasterPage_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("action") = Nothing Then
            Dim action As String = Session("action")
            Dim remarks As String = Nothing '"Existing record successfuly updated!"
            Select Case action
                Case "C"
                    remarks = "New record successfully created!"
                    Exit Select
                Case "U"
                    remarks = "Record updated successfully!"
                    Exit Select
                Case "X"
                    remarks = "Cancelled record successfully!"
                    Exit Select
                Case "404"
                    remarks = "Select record did not exist!"
                    Exit Select
                Case Else
                    remarks = action
                    Exit Select
            End Select
            Page.ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(" + If(action.ToString = "404", "0", "1") + ", '" + remarks + "');", True)
            Session("action") = Nothing
        End If

        
    End Sub

End Class

