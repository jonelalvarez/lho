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
                Barangay()
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
            For Each row In .dynamicQuery("Select ps_tbl.ID, ps_tbl.PurokName As `Purok Sitio`, brgy_tbl.BarangayName As Barangay, Case When brgy_tbl.Status = '1' Then 'Active' Else 'Inactive' End As Status " &
                                          " From ps_tbl Inner Join brgy_tbl On brgy_tbl.ID = ps_tbl.bID").AsEnumerable
                htmlStr.Append("<tr>")
                htmlStr.Append("<td style='width: 25%'><a href=""" + Master.activePage + "?ID=" + .Encrypt(row.Item("ID").ToString) + """ Class=""green""><i class=""fa fa-search bigger-130""></i> &nbsp" + row.Item("ID").ToString.ToUpper + "</a></td>")
                htmlStr.Append("<td>" + row.Item("Purok Sitio").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("Barangay").ToString + "</td>")
                htmlStr.Append("<td style=""width: 15%"" class=""text-center"">" + row.Item("Status").ToString + "</td>")
                htmlStr.Append("</tr>")
            Next
        End With
        ph_list.Controls.Add(New Literal() With {.Text = htmlStr.ToString()})
    End Sub

    Private Sub showDetails()
        With clsMaster
            Dim foundRow As DataRow() = .dynamicQuery("Select ps_tbl.ID, ps_tbl.PurokName, ps_tbl.Status From ps_tbl where ID = '" & requestID & "'").Select

            If foundRow.Length > 0 Then
                dl_status.Visible = True
                dl_brgy.Visible = True
                h4_title.InnerText = "View/Edit Record"

                txt_name.Value = foundRow(0).Item("PurokName").ToString
                dl_status.Value = foundRow(0).Item("Status").ToString
                dl_brgy.Value = foundRow(0).Item("Status").ToString

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
                        sql = "Insert into ps_tbl (bID, PurokName) values (@bID, @PurokName)"
                    ElseIf _mode = "U" Then
                        sql = "Update ps_tbl SET ID = @ID, PurokName = @PurokName, Status = @Status, bID = @bID where ID = @ID"
                    End If

                    Using cmd As New MySqlCommand
                        cmd.Parameters.AddWithValue("@ID", requestID)
                        cmd.Parameters.AddWithValue("@PurokName", txt_name.Value.Trim)
                        cmd.Parameters.AddWithValue("@Status", dl_status.Value)
                        cmd.Parameters.AddWithValue("@bID", dl_brgy.Value)

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
            If .dynamicQuery("Select ID from ps_tbl where " + mode + " = '" & entry.Trim & "'").Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End With

    End Function

    'DOWN LIST FOR ADD AND UPDATE'
    Protected Sub Barangay()

        With clsMaster
            dl_brgy.Items.Clear()
            For Each row In .dynamicQuery("Select ID, BarangayName From brgy_tbl Where Status = 1 Order By BarangayName").AsEnumerable
                dl_brgy.Items.Add(New ListItem(row.Item("BarangayName").ToString, row.Item("ID").ToString))
            Next
            dl_brgy.Items.Insert(0, New ListItem("Please select 1", ""))


        End With

    End Sub

    <WebMethod>
    Public Shared Function validatePurokSitio(brgyID As String, entry As String) As Boolean
        Dim clsMaster2 As New cls_master

        With clsMaster2
            If .dynamicQuery("Select ID from ps_tbl where bID = '" + brgyID + "' and PurokName = '" & entry.Trim & "'").Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End With

    End Function

End Class