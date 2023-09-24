Imports System.Data
Imports System.Web.Services
Imports MySql.Data.MySqlClient
Imports System.IO

Partial Class Applicant
    Inherits System.Web.UI.Page
    Dim ConnString As String
    Dim clsMaster As New cls_master
    Dim requestID As String

    Protected Sub Applicant_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not Request.QueryString("ID") = Nothing Then
            requestID = clsMaster.Decrypt(Request.QueryString("ID").Replace(" ", "+"))
        End If

        ConnString = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
    End Sub

    Private Sub Applicant_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try

                Purok()
                If Not requestID = Nothing Then
                    showDetails()
                End If
            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
            End Try
        End If
    End Sub
    Private Sub showDetails()
        With clsMaster
            Dim foundRow As DataRow() = .dynamicQuery("Select ID, FirstName, MiddleName, LastName, Suffix, CivilStatus, Birthdate, ContactNumber, VotersID, ShortAddress, Residency, ApplicationID, PurokID " &
                                                      " From applicant_tbl where ID = '" & requestID & "'").Select
            If foundRow.Length > 0 Then
                app_status.Visible = True
                h4_title.InnerText = "View/Edit Record"


                application_status.Value = foundRow(0).Item("ApplicationID").ToString
                txt_fName.Value = foundRow(0).Item("FirstName").ToString
                txt_mName.Value = foundRow(0).Item("MiddleName").ToString
                txt_lName.Value = foundRow(0).Item("LastName").ToString
                div_suffix.Value = foundRow(0).Item("Suffix").ToString
                dl_civil.Value = foundRow(0).Item("CivilStatus").ToString
                bDay.Value = Format(foundRow(0).Item("Birthdate"), "MM/dd/yyyy")
                div_contact.Value = foundRow(0).Item("ContactNumber").ToString
                div_voters.Value = foundRow(0).Item("VotersID").ToString
                div_residency.Value = foundRow(0).Item("Residency").ToString
                Address.Value = foundRow(0).Item("ShortAddress").ToString
                dl_psID.Value = foundRow(0).Item("PurokID").ToString


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
                        sql = "Insert into applicant_tbl (ID, FirstName, MiddleName, LastName, Suffix, Birthdate, ShortAddress, PurokID) values (@ID, @FirstName, @MiddleName, @LastName, @Suffix, @Birthdate, @ShortAddress, @PurokID)"
                    ElseIf _mode = "U" Then
                        sql = "Update applicant_tbl SET ApplicationID = @ApplicationID, FirstName = @FirstName, MiddleName = @MiddleName, LastName = @LastName, Suffix = @Suffix, CivilStatus = @CivilStatus, ContactNumber = @ContactNumber, VotersID = @VotersID, Residency = @Residency, Birthdate = @Birthdate, ShortAddress =  @ShortAddress, PurokID = @PurokID where ID = @ID"
                    End If

                    Using cmd As New MySqlCommand
                        cmd.Parameters.AddWithValue("@ID", requestID)
                        cmd.Parameters.AddWithValue("@ApplicationID", application_status.Value.Trim)
                        cmd.Parameters.AddWithValue("@FirstName", txt_fName.Value.Trim)
                        cmd.Parameters.AddWithValue("@MiddleName", txt_mName.Value.Trim)
                        cmd.Parameters.AddWithValue("@LastName", txt_lName.Value.Trim)
                        cmd.Parameters.AddWithValue("@Suffix", div_suffix.Value.Trim)
                        cmd.Parameters.AddWithValue("@CivilStatus", dl_civil.Value.Trim)
                        cmd.Parameters.AddWithValue("@ContactNumber", div_contact.Value.Trim)
                        cmd.Parameters.AddWithValue("@VotersID", div_voters.Value.Trim)
                        cmd.Parameters.AddWithValue("@Residency", div_residency.Value.Trim)
                        cmd.Parameters.AddWithValue("@Birthdate", Date.Parse(bDay.Value))
                        cmd.Parameters.AddWithValue("@ShortAddress", Address.Value.Trim)
                        cmd.Parameters.AddWithValue("@PurokID", dl_psID.Value)

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
            If .dynamicQuery("Select ID from applicant_tbl where " + mode + " = '" & entry.Trim & "'").Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End With

    End Function
    Protected Sub Purok()

        With clsMaster
            dl_psID.Items.Clear()
            For Each row In .dynamicQuery("Select ID, psName From ps_tbl Where Status = 1 Order By psName").AsEnumerable
                dl_psID.Items.Add(New ListItem(row.Item("psName").ToString, row.Item("ID").ToString))
            Next
            dl_psID.Items.Insert(0, New ListItem("Please select 1", ""))


        End With

    End Sub

    <WebMethod>
    Public Shared Function filterDataTable(_from As String, _to As String, status As String, category As String) As String
        Dim htmlStr As New StringBuilder
        Dim clsMaster As New cls_master

        With clsMaster
            ' Your SQL query with the original date parameters _from and _to
            Dim sqlQuery As String = "SELECT applicant_tbl.ID, Concat_Ws(' ', applicant_tbl.fName, applicant_tbl.mName, applicant_tbl.lName, applicant_tbl.suffx) As FullName, " &
                                      " Concat_Ws(' ', applicant_tbl.sAddress, ps_tbl.psName, ',', brgy_tbl.brgyName) As Address, " &
                                      " applicant_tbl.applicationStatus CASE WHEN applicant_tbl.applicationStatus = 1 THEN 'New Applicants' WHEN applicant_tbl.applicationStatus = 2 THEN 'Approved Applicants' " &
                                      " WHEN  applicant_tbl.applicationStatus = 3 THEN 'On-process Applicants' " &
                                      " WHEN  applicant_tbl.applicationStatus = 4 THEN 'Declined Applicants' " &
                                      " ELSE 'Unknown Status' END As ApplicationStatus, category_tbl.categoryName As Category, date_format (applicant_tbl.createdDate, '%M %d, %Y') As ApplicationDate " &
                                      " FROM applicant_tbl " &
                                      " INNER JOIN ps_tbl ON ps_tbl.ID = applicant_tbl.psID " &
                                      " INNER JOIN brgy_tbl ON brgy_tbl.ID = ps_tbl.bID " &
                                      " INNER JOIN category_tbl ON category_tbl.ID = applicant_tbl.categoryID " &
                                      " WHERE applicant_tbl.ApplicationDate BETWEEN '" & _from & "' AND '" & _to & "'"

            ' Check if "" is selected for status and adjust the SQL query accordingly
            If status <> "" Then
                sqlQuery &= " AND applicant_tbl.applicationStatus = '" & status & "'"
            End If

            ' Check if "" is selected for category and adjust the SQL query accordingly
            If category <> "" Then
                sqlQuery &= " AND applicant_tbl.categoryID = '" & category & "'"
            End If

            For Each row In .dynamicQuery(sqlQuery).AsEnumerable
                htmlStr.Append("<tr>")
                htmlStr.Append("<td style='width: 10%'><a href=""Applicant.aspx?ID=" + .Encrypt(row.Item("ID").ToString) + """ Class=""green""><i class=""fa fa-search bigger-130""></i> &nbsp" + row.Item("ID").ToString.ToUpper + "</a></td>")
                htmlStr.Append("<td style=""width: 20%"">" + row.Item("FullName").ToString + "</td>")
                htmlStr.Append("<td style=""width: 30%"">" + row.Item("Address").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("Category").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("ApplicationStatus").ToString + "</td>")
                htmlStr.Append("<td style=""width: 15%"" class=""text-center"">" + row.Item("ApplicationDate").ToString + "</td>")
                htmlStr.Append("</tr>")
            Next
        End With

        Return htmlStr.ToString
    End Function

End Class