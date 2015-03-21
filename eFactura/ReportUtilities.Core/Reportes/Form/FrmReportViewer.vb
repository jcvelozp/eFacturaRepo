Imports Microsoft.Reporting.WinForms
Imports System.IO
Imports System.Windows.Forms

Public Class FrmReportViewer

#Region "Var"
    Dim rds() As ReportDataSource
    Dim params As List(Of ReportParameter)
    Dim stream As Stream
    Dim _printed As Boolean = False
    Dim m_path As String
#End Region

#Region "Properties"
    Public Property PathExportFile() As String
        Get
            Return m_path
        End Get
        Set(ByVal value As String)
            m_path = value
        End Set
    End Property
#End Region

#Region "Constructor"

    Sub New(ByVal dataSource As ReportDataSource, ByVal reportParams As List(Of ReportParameter), ByVal reportStream As Stream)
        InitializeComponent()
        rds = New ReportDataSource() {dataSource}
        params = reportParams
        stream = reportStream
    End Sub

    Sub New(ByVal dataSources() As ReportDataSource, ByVal reportParams As List(Of ReportParameter), ByVal reportStream As Stream)
        InitializeComponent()
        rds = dataSources
        params = reportParams
        stream = reportStream
    End Sub

#End Region

#Region "Property"
    Public Property Printed() As Boolean
        Get
            Return _printed
        End Get
        Set(value As Boolean)
            _printed = value
        End Set
    End Property
#End Region

    Private Sub rptViewer_Print(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles rptViewer.Print
        _printed = True
    End Sub

    Private Sub Frm_ReportViewer_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                Me.Close()
        End Select
    End Sub

    Private Sub Frm_ReportViewer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            LocalizeReport(Me.rptViewer.LocalReport, stream)
            For Each ds As ReportDataSource In rds
                Me.rptViewer.LocalReport.DataSources.Add(ds)
            Next

            Me.rptViewer.LocalReport.SetParameters(params)

            Me.rptViewer.RefreshReport()
        Catch ex As Exception
            'Common.Write_Log(Common.GetApplicationPath(), "ErrorLog.txt", ex.Message, "Frm_ReportViewer_Load", ex.ToString)
            MessageBox.Show(ex.ToString, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    'Public Sub ExportToPDF(ByVal filename As String)
    Public Sub ExportToPDF(ByVal filename As String)
        Try
            LocalizeReport(Me.rptViewer.LocalReport, stream)
            For Each ds As ReportDataSource In rds
                Me.rptViewer.LocalReport.DataSources.Add(ds)
            Next
            Me.rptViewer.LocalReport.SetParameters(params)

            Dim pr As New PrintReport
            pr.Report = Me.rptViewer.LocalReport
            pr.seedNr = 0
            pr.Format = PrintReport.OutputFormat.PDF
            pr.FileName = filename
            pr.PathExportFile = PathExportFile
            pr.Print_Report()

        Catch ex As Exception
            MessageBox.Show(ex.ToString, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    'Public Sub ExportToPDF(ByVal filename As String)
    'Public Sub ExportToXLS(ByVal filename As String)
    '    Try
    '        LocalizeReport(Me.rptViewer.LocalReport, stream)
    '        For Each ds As ReportDataSource In rds
    '            Me.rptViewer.LocalReport.DataSources.Add(ds)
    '        Next
    '        Me.rptViewer.LocalReport.SetParameters(params)

    '        'Dim pr As New PrintReport
    '        'pr.Report = Me.rptViewer.LocalReport

    '        'pr.seedNr = 0
    '        'pr.Format = PrintReport.OutputFormat.EXCEL
    '        'pr.FileName = filename
    '        'pr.Print_Report()

    '    Catch ex As Exception
    '        BL_Mobile.WriteLog(BL_Mobile.GetApplicationPath(), "ErrorLog.txt", ex.Message, "ExportToXLS", ex.ToString)
    '        OC_MessageBox.Show(ex.Message, Me.Text, ex.ToString, MessageBoxButtons.OK, My.Resources.ErrorImage)
    '    End Try
    'End Sub

#Region "resource localizable"
    Private Sub LocalizeReport(ByVal report As LocalReport, ByVal xmlStream As Stream)
        'Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("de-DE")
        Dim doc As New Xml.XmlDocument()
        Try
            doc.Load(xmlStream)
        Catch generatedExceptionName As Xml.XmlException
            ' TIP: If your web site was published with the updatable option switched off
            ' you must copy your reports to the target location manually
            Return
        End Try
        ' Create an XmlNamespaceManager to resolve the default namespace.
        Dim nsmgr As New Xml.XmlNamespaceManager(doc.NameTable)
        nsmgr.AddNamespace("nm", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")
        nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")

        'Go through the nodes of XML document and localize the text of nodes Value, ToolTip, Label. 
        For Each nodeName As String In New String() {"Value", "ToolTip", "Label"}
            For Each node As Xml.XmlNode In doc.DocumentElement.SelectNodes([String].Format("//nm:{0}[@rd:LocID]", nodeName), nsmgr)
                Dim nodeValue As String = node.InnerText
                If Not [String].IsNullOrEmpty(nodeValue) AndAlso Not nodeValue.StartsWith("=") Then
                    Try
                        Dim localizedValue As String = My.Resources.ResourceManager.GetString(node.Attributes("rd:LocID").Value)
                        'HttpContext.GetLocalResourceObject(report.ReportPath, node.Attributes("rd:LocID").Value).toString()
                        If Not [String].IsNullOrEmpty(localizedValue) Then
                            node.InnerText = localizedValue
                        End If
                        ' if the specified resource is not a String
                    Catch generatedExceptionName As InvalidCastException
                    End Try
                End If
            Next
        Next
        report.ReportEmbeddedResource = [String].Empty
        'Load the updated RDLC document into LocalReport object.
        Using rdlcOutputStream As New IO.StringReader(doc.DocumentElement.OuterXml)
            ' TIP: If the loaded report definition contains any subreports, you must call LoadSubreportDefinition
            report.LoadReportDefinition(rdlcOutputStream)
        End Using
    End Sub
#End Region


#Region "Direct Actions"

    Public Enum Action
        ExportPDF
        ExportXLS
        MailPDF
        MailXLS
        Print
    End Enum

    Public Sub DirectAction(ByVal ac As Action, ByVal param As String)
        Try
            LocalizeReport(Me.rptViewer.LocalReport, stream)
            For Each ds As ReportDataSource In rds
                Me.rptViewer.LocalReport.DataSources.Add(ds)
            Next
            Me.rptViewer.LocalReport.SetParameters(params)

            Dim pr As New PrintReport
            pr.Report = Me.rptViewer.LocalReport
            pr.seedNr = Now.Second

            Dim mailPath As String = param
            Select Case ac
                Case Action.MailPDF
                    pr.Format = PrintReport.OutputFormat.PDF

                    mailPath = Path.Combine(Path.GetTempPath, mailPath & ".pdf")
                    File.Delete(mailPath)
                    pr.FileName = mailPath

                Case Action.MailXLS
                    pr.Format = PrintReport.OutputFormat.Excel

                    mailPath = Path.Combine(Path.GetTempPath, mailPath & ".xls")
                    File.Delete(mailPath)
                    pr.FileName = mailPath

                Case Action.ExportPDF
                    pr.Format = PrintReport.OutputFormat.PDF
                    pr.FileName = param

                Case Action.ExportXLS
                    pr.Format = PrintReport.OutputFormat.Excel
                    pr.FileName = param

                Case Action.Print
                    pr.Format = PrintReport.OutputFormat.Printer
                    pr.PrinterName = param
            End Select

            pr.Print_Report()

            If ac = Action.MailPDF Or ac = Action.MailXLS Then
                'MailHelper.SendMail("", "", param, "", mailPath, True)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


#End Region

   
    'Private Sub InitializeComponent()
    '    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmReportViewer))
    '    Me.SuspendLayout()
    '    '
    '    'FrmReportViewer
    '    '
    '    resources.ApplyResources(Me, "$this")
    '    Me.Name = "FrmReportViewer"
    '    Me.ResumeLayout(False)

    'End Sub
End Class