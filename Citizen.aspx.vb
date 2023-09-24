Imports System.Data
Imports System.Web.Services
Imports MySql.Data.MySqlClient
Imports System.IO

Partial Class Departments
    Inherits System.Web.UI.Page
    Dim ConnString As String
    Dim clsMaster As New cls_master
    Dim requestID As String

    Protected Sub Departments_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not Request.QueryString("ID") = Nothing Then
            requestID = clsMaster.Decrypt(Request.QueryString("ID").Replace(" ", "+"))
        End If

        ConnString = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
    End Sub

    Private Sub Departments_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try
                If Not requestID = Nothing Then
                    showDetails()
                Else
                    loadList()
                End If
            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
            End Try
        End If
    End Sub

    Private Sub loadList()
        Dim htmlStr As New StringBuilder

        With clsMaster
            For Each row In .dynamicQuery("SELECT ID, fName, DATE_FORMAT(bDay,'%m-%d-%Y')as bDay, sex, floor(TIMESTAMPDIFF(YEAR, bday, curdate())) AS Age, address, yor, case  when status = 1 then 'Active' else 'Inactive' end As status FROM proftest_tbl").AsEnumerable
                htmlStr.Append("<tr>")
                htmlStr.Append("<td style='width: 10%'><a href=""" + Master.activePage + "?ID=" + .Encrypt(row.Item("ID").ToString) + """ Class=""green""><i class=""fa fa-search bigger-130""></i> &nbsp" + row.Item("ID").ToString.ToUpper + "</a></td>")
                htmlStr.Append("<td>" + row.Item("fName").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("bDay").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("Sex").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("Age").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("Address").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("YoR").ToString + "</td>")
                htmlStr.Append("<td style=""width: 15%"" class=""text-center"">" + row.Item("status").ToString + "</td>")
                htmlStr.Append("</tr>")
            Next
        End With
        ph_list.Controls.Add(New Literal() With {.Text = htmlStr.ToString()})
    End Sub

    Private Sub showDetails()
        With clsMaster
            Dim foundRow As DataRow() = .dynamicQuery("SELECT ID, fName, mName, lName, bDate, sAddress, Case When test.citizen_tbl.Status = '1' Then 'Active' Else 'Inactive' End as Status, psID" &
           "                      From citizen_tbl where ID = '" & requestID & "'").Select
            If foundRow.Length > 0 Then
                div_status.Visible = True
                div_psID.Visible = True
                h4_title.InnerText = "View/Edit Record"


                txt_fName.Value = foundRow(0).Item("fName").ToString
                txt_mName.Value = foundRow(0).Item("mName").ToString
                txt_lName.Value = foundRow(0).Item("lName").ToString
                bDay.Value = Format(foundRow(0).Item("bDate"), "MM/dd/yyyy")
                Address.Value = foundRow(0).Item("sAddress").ToString
                dl_status.Value = foundRow(0).Item("status").ToString
                dl_psID.Value = foundRow(0).Item("psID").ToString

                ClientScript.RegisterStartupScript(Me.[GetType](), "showModalAdd", "showModalAdd();", True)
            Else
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, 'Selected record did not exists.');", True)
                Response.Redirect(Master.activePage)
            End If
        End With
    End Sub

    Protected Sub Save_Click()
        If Not requestID = Nothing Then
            CRUD("U")
        Else
            CRUD("C")
        End If
    End Sub

    Protected Sub CRUD(_mode As String)
        If IsPostBack Then
            Try
                Using con As New MySqlConnection(ConnString)
                    con.Open()
                    Dim sql As String = Nothing

                    If _mode = "C" Then
                        sql = "Insert into test_tbl (ID, fName, mName, lName, bDate, sAddress, psID) values (@ID, @fName, @mName, @lName, @bDate, @sAddress, @psID)"
                    ElseIf _mode = "U" Then
                        sql = "Update test_tbl SET fName = @fName, mName = @mName, lName = @lName, bDate = @bDate, sAddress =  @sAddress, status = @status, psID = @psID where ID = @ID"
                    End If

                    Using cmd As New MySqlCommand
                        cmd.Parameters.AddWithValue("@ID", requestID)
                        cmd.Parameters.AddWithValue("@fName", txt_fName.Value.Trim)
                        cmd.Parameters.AddWithValue("@mName", txt_mName.Value.Trim)
                        cmd.Parameters.AddWithValue("@lName", txt_lName.Value.Trim)
                        cmd.Parameters.AddWithValue("@bDate", Date.Parse(bDay.Value))
                        cmd.Parameters.AddWithValue("@sAddress", Address.Value.Trim)
                        cmd.Parameters.AddWithValue("@status", dl_status.Value)
                        cmd.Parameters.AddWithValue("@psID", dl_psID.Value)

                        cmd.Connection = con
                        cmd.CommandText = sql
                        cmd.ExecuteNonQuery()
                    End Using
                    con.Close()
                    con.Dispose()

                    Session("action") = _mode
                    Response.Redirect(Request.RawUrl)
                End Using
            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
            End Try
        End If
    End Sub

    <WebMethod>
    Public Shared Function validateEntry(mode As String, entry As String) As Boolean
        Dim clsMaster2 As New cls_master

        With clsMaster2
            If .dynamicQuery("Select ID from citizen_tbl where " + mode + " = '" & entry.Trim & "'").Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End With

    End Function


End Class
