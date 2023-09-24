Imports System.Data
Imports MySql.Data.MySqlClient
Imports System.Configuration
Imports Newtonsoft.Json
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports System.Web.UI.WebControls
Partial Class Home
    Inherits System.Web.UI.Page

    Dim ConnString As String
    Dim clsMaster As New cls_master
    Dim requestID As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ConnString = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
        LoadApplicantCounts()
        If Not Page.IsPostBack Then

        End If
    End Sub

    Private Sub LoadApplicantCounts()
        ' Define your connection string from the Web.config file
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("conn1").ConnectionString

        ' Create a SQL connection
        Using connection As New MySqlConnection(connectionString)
            ' Define your SQL query with CASE WHEN statement
            Dim query As String = "SELECT " &
               "COUNT(*) As TotalApplicants," &
               "SUM(CASE WHEN applicant_tbl.applicationStatus = 1 THEN 1 ELSE 0 END) As ApprovedApplicants," &
               "SUM(CASE WHEN applicant_tbl.applicationStatus = 2 THEN 1 ELSE 0 END) As OnProcessApplicants," &
               "SUM(CASE WHEN applicant_tbl.applicationStatus = 3 THEN 1 ELSE 0 END) As DeclinedApplicants " &
               " From applicant_tbl"

            ' Create a SQL command
            Using command As New MySqlCommand(query, connection)
                connection.Open()

                ' Execute the SQL command and fetch the counts
                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        ' Bind the counts to your card elements
                        lblTotalApplicants.InnerText = reader("TotalApplicants").ToString()
                        lblApprovedApplicants.InnerText = reader("ApprovedApplicants").ToString()
                        lblOnProcessApplicants.InnerText = reader("OnProcessApplicants").ToString()
                        lblDeclinedApplicants.InnerText = reader("DeclinedApplicants").ToString()
                    End If
                End Using
            End Using
        End Using
    End Sub

    'Protected Sub Home_Init(sender As Object, e As EventArgs) Handles Me.Init
    '    If Not Request.QueryString("ID") = Nothing Then
    '        requestID = clsMaster.Decrypt(Request.QueryString("ID").Replace(" ", "+"))
    '    End If

    '    ConnString = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
    'End Sub

    <WebMethod>
    Public Shared Function GetDataForChart(startDate As Date, endDate As Date) As String
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
        Dim connection As MySqlConnection = New MySqlConnection(connectionString)
        Dim chartData As New Dictionary(Of String, Integer)()

        Try
            connection.Open()
            Dim query As String = "SELECT createdDate, " &
            "SUM(CASE WHEN applicationStatus = 1 THEN 1 ELSE 0 END) AS approved_applicants, " &
            "SUM(CASE WHEN applicationStatus = 2 THEN 1 ELSE 0 END) AS pending_applicants, " &
            "SUM(CASE WHEN applicationStatus = 3 THEN 1 ELSE 0 END) AS declined_applicants " &
            "FROM applicant_tbl " &
            "WHERE createdDate >= @startDate AND createdDate <= @endDate " &
            "GROUP BY createdDate " &
            "ORDER BY createdDate"

            Dim cmd As New MySqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@startDate", startDate)
            cmd.Parameters.AddWithValue("@endDate", endDate)

            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                Dim dateValue As Date = Convert.ToDateTime(reader("createdDate"))
                Dim approvedApplicants As Integer = Convert.ToInt32(reader("approved_applicants"))
                Dim pendingApplicants As Integer = Convert.ToInt32(reader("pending_applicants"))
                Dim declinedApplicants As Integer = Convert.ToInt32(reader("declined_applicants"))

                ' Assuming your date format is "MM/DD/YYYY"
                Dim dateString As String = dateValue.ToString("MM/dd/yyyy")

                ' Add data to the dictionary
                chartData(dateString + "_Approved Applicants") = approvedApplicants
                chartData(dateString + "_On-Process Applicants") = pendingApplicants
                chartData(dateString + "_Declined Applicants") = declinedApplicants
            End While

            reader.Close()

            ' Serialize chartData to JSON
            Dim serializer As New JavaScriptSerializer()
            Dim jsonData As String = serializer.Serialize(chartData)

            Return jsonData

        Catch ex As Exception
            ' Handle any exceptions here and return an error message in JSON format
            Dim errorResponse As New Dictionary(Of String, String) From {{"error", ex.Message}}
            Dim serializer As New JavaScriptSerializer()
            Dim errorJson As String = serializer.Serialize(errorResponse)

            Return errorJson
        Finally
            connection.Close()
        End Try

    End Function
    Public Shared Function GetApplicantsData(startDate As Date, endDate As Date) As String
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
        Dim connection As MySqlConnection = New MySqlConnection(connectionString)

        Try
            connection.Open()
            Dim query As String = "SELECT ID, refID, fName, mName, lName, suffx, createdDate " &
            "FROM applicant_tbl " &
            "WHERE createdDate >= @startDate AND createdDate <= @endDate " &
            "ORDER BY createdDate"

            Dim cmd As New MySqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@startDate", startDate)
            cmd.Parameters.AddWithValue("@endDate", endDate)

            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            Dim tableData As New StringBuilder()
            While reader.Read()
                Dim id As Integer = Convert.ToInt32(reader("ID"))
                Dim referenceID As String = Convert.ToString(reader("refID"))
                Dim firstName As String = Convert.ToString(reader("fName"))
                Dim middleName As String = Convert.ToString(reader("mName"))
                Dim lastName As String = Convert.ToString(reader("lName"))
                Dim suffix As String = Convert.ToString(reader("suffx"))
                Dim applicationDate As Date = Convert.ToDateTime(reader("createdDate"))

                tableData.AppendFormat("<tr>")
                tableData.AppendFormat("<td>{0}</td>", id)
                tableData.AppendFormat("<td>{0}</td>", referenceID)
                tableData.AppendFormat("<td>{0}</td>", firstName)
                tableData.AppendFormat("<td>{0}</td>", middleName)
                tableData.AppendFormat("<td>{0}</td>", lastName)
                tableData.AppendFormat("<td>{0}</td>", suffix)
                tableData.AppendFormat("<td>{0}</td>", applicationDate.ToString("MM/dd/yyyy"))
                tableData.AppendFormat("</tr>")
            End While

            reader.Close()
            Return tableData.ToString()

        Catch ex As Exception
            ' Handle any exceptions here and return an error message in HTML format
            Dim errorResponse As String = String.Format("<tr><td colspan='7'>{0}</td></tr>", ex.Message)
            Return errorResponse
        Finally
            connection.Close()
        End Try
    End Function

End Class

