Imports Microsoft.Reporting.WinForms

Public Class ReportMaster
    Implements IReportTemplate

#Region "Vars"
    Private _datasource As DataTable
    Private _datasourcename As String
    Private _reportname As String
    Private _reportnameRDLC As String
    Private _reportproject As String
    Private _title As String
    Private _type As System.Type
    Private _pathexportfile As String
    Private _multipleDataSource As New Dictionary(Of String, DataTable)
#End Region

#Region "Properties"

    Public Property TypeStream As System.Type
        Get
            Return _type
        End Get
        Set(value As System.Type)
            _type = value
        End Set
    End Property

    Public Property DataSource As DataTable Implements IReportTemplate.DataSource
        Get
            Return _datasource
        End Get
        Set(value As DataTable)
            _datasource = value
        End Set
    End Property

    Public Property MultipleDataSource As Dictionary(Of String, DataTable) Implements IReportTemplate.MultipleDataSource
        Get
            Return _multipleDataSource
        End Get
        Set(value As Dictionary(Of String, DataTable))
            _multipleDataSource = value
        End Set
    End Property
    Public Property DataSourceName As String Implements IReportTemplate.DataSourceName
        Get
            Return _datasourcename
        End Get
        Set(value As String)
            _datasourcename = value
        End Set
    End Property

    Public Property ReportFileName As String Implements IReportTemplate.ReportFileName
        Get
            Return _reportname
        End Get
        Set(value As String)
            _reportname = value
        End Set
    End Property

    Public Property ReportFileNameRDLC As String Implements IReportTemplate.ReportFileNameRDLC
        Get
            Return _reportnameRDLC
        End Get
        Set(value As String)
            _reportnameRDLC = value
        End Set
    End Property

    Public Property ReportProject As String Implements IReportTemplate.ReportProject
        Get
            Return _reportproject
        End Get
        Set(value As String)
            _reportproject = value
        End Set
    End Property

    Public ReadOnly Property ReportFullName As String Implements IReportTemplate.ReportFullName
        Get
            Return ReportProject & "." & ReportFileNameRDLC & ".rdlc"
        End Get
    End Property

    Public Property Title As String Implements IReportTemplate.Title
        Get
            Return _title
        End Get
        Set(value As String)
            _title = value
        End Set
    End Property

    Public Property PathExportFile As String Implements IReportTemplate.PathExportFile
        Get
            Return _pathexportfile
        End Get
        Set(value As String)
            _pathexportfile = value
        End Set
    End Property

#End Region

    Public Function Print() As Boolean
        Return Print(Nothing)
    End Function

    Public Function Print(parameter As List(Of Microsoft.Reporting.WinForms.ReportParameter)) As Boolean Implements IReportTemplate.Print
        Try
            'Dim rds As New ReportDataSource
            'rds.Name = DataSourceName
            'rds.Value = DataSource

            Dim rds As New List(Of ReportDataSource)
            If Me._multipleDataSource.Count = 0 Then
                Dim _rds As New ReportDataSource
                _rds.Name = DataSourceName
                _rds.Value = DataSource
                rds.Add(_rds)
            Else
                For Each source As KeyValuePair(Of String, DataTable) In _multipleDataSource
                    Dim _rds As New ReportDataSource
                    _rds.Name = source.Key
                    _rds.Value = source.Value
                    rds.Add(_rds)
                Next
            End If

            If parameter Is Nothing Then
                parameter = New List(Of ReportParameter)
            End If
            Dim stream As IO.Stream = Nothing
            Dim assembly As Reflection.Assembly = Reflection.Assembly.GetAssembly(TypeStream)

            stream = assembly.GetManifestResourceStream(ReportFullName)
            Dim frm As New FrmReportViewer(rds.ToArray, parameter, stream)
            frm.Text = Title
            frm.PathExportFile = PathExportFile
            If Not String.IsNullOrEmpty(frm.PathExportFile) Then
                frm.ExportToPDF(ReportFileName & ".pdf")
            Else
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "Error al Cargar Assembly", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
        Return True
    End Function

End Class
