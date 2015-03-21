Imports Microsoft.Reporting.WinForms

Public Interface IReportTemplate
    Property DataSource As DataTable
    Property DataSourceName As String
    Property MultipleDataSource As Dictionary(Of String, DataTable)
    Property ReportFileName As String
    Property ReportProject As String
    Property Title As String
    ReadOnly Property ReportFullName As String
    Property PathExportFile As String
    Property ReportFileNameRDLC As String
    Function Print(ByVal parameter As List(Of ReportParameter)) As Boolean
End Interface
