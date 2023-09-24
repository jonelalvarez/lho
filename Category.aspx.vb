Imports System.Data
Imports System.Web.Services
Imports MySql.Data.MySqlClient
Imports System.IO

Partial Class Category
    Inherits System.Web.UI.Page
    Dim ConnString As String
    Dim clsMaster As New cls_master
    Dim requestID As String

    Protected Sub Category_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not Request.QueryString("ID") = Nothing Then
            requestID = clsMaster.Decrypt(Request.QueryString("ID").Replace(" ", "+"))
        End If

        ConnString = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
    End Sub

    Private Sub Category_Load(sender As Object, e As EventArgs) Handles Me.Load
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
            For Each row In .dynamicQuery("Select category_tbl.ID, category_tbl.CategoryName As `Category Name`, Case When category_tbl.Status = '1' Then 'Active' Else 'Inactive' End As Status " &
                                          "From category_tbl").AsEnumerable
                htmlStr.Append("<tr>")
                htmlStr.Append("<td style='width: 25%'><a href=""" + Master.activePage + "?ID=" + .Encrypt(row.Item("ID").ToString) + """ Class=""green""><i class=""fa fa-search bigger-130""></i> &nbsp" + row.Item("ID").ToString.ToUpper + "</a></td>")
                htmlStr.Append("<td>" + row.Item("Category Name").ToString + "</td>")
                htmlStr.Append("<td style=""width: 15%"" class=""text-center"">" + row.Item("Status").ToString + "</td>")
                htmlStr.Append("</tr>")
            Next
        End With
        ph_list.Controls.Add(New Literal() With {.Text = htmlStr.ToString()})
    End Sub

    Private Sub showDetails()
        With clsMaster
            Dim foundRow As DataRow() = .dynamicQuery("Select category_tbl.ID, category_tbl.CategoryName, category_tbl.Status From category_tbl where ID = '" & requestID & "'").Select

            If foundRow.Length > 0 Then
                dl_status.Visible = True
                h4_title.InnerText = "View/Edit Record"

                txt_category.Value = foundRow(0).Item("CategoryName").ToString
                dl_status.Value = foundRow(0).Item("Status").ToString


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
                        sql = "Insert into category_tbl (ID, CategoryName) values (@ID, @CategoryName)"
                    ElseIf _mode = "U" Then
                        sql = "Update category_tbl SET ID = @ID, CategoryName = @CategoryName, Status = @Status, ID = @ID"
                    End If

                    Using cmd As New MySqlCommand
                        cmd.Parameters.AddWithValue("@ID", requestID)
                        cmd.Parameters.AddWithValue("@CategoryName", txt_category.Value.Trim)
                        cmd.Parameters.AddWithValue("@Status", dl_status.Value)

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



End Class