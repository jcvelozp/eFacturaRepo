Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Drawing.Imaging
Imports System.Drawing.Printing
Imports System.Collections.Generic
Imports Microsoft.Reporting.WinForms

 
Public Class PrintReport
    Implements IDisposable
    Private m_currentPageIndex As Integer
    Private m_streams As IList(Of Stream)
    Private disposedValue As Boolean = False        ' To detect redundant calls

    Private m_report As LocalReport = New LocalReport()
    Private m_printerName As String = "Microsoft Office Document Image Writer"
    Private m_OutputFormat As OutputFormat = OutputFormat.Printer
    Private m_FileName As String
    Private m_path As String
    Private results As Byte()

    Public seedNr As Integer = 0

    Enum OutputFormat As Integer
        Printer
        PDF
        Excel
    End Enum

#Region "Property"

    Public Property FileName() As String
        Get
            Return m_FileName
        End Get
        Set(ByVal value As String)
            m_FileName = value
        End Set
    End Property

    Public Property PathExportFile() As String
        Get
            Return m_path
        End Get
        Set(ByVal value As String)
            m_path = value
            If Not Directory.Exists(value) Then
                Try
                    If (Not String.IsNullOrEmpty(value)) Then
                        Dim di As DirectoryInfo = Directory.CreateDirectory(value)
                        Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(value))
                    End If
                Catch ex As Exception
                    MsgBox(ex.ToString())
                End Try
            End If
        End Set
    End Property

    Public Property Format() As OutputFormat
        Get
            Return m_OutputFormat
        End Get
        Set(ByVal value As OutputFormat)
            m_OutputFormat = value
        End Set
    End Property

    Public Property Report() As LocalReport
        Get
            Return m_report
        End Get
        Set(ByVal value As LocalReport)
            m_report = value
        End Set
    End Property

    Public Property PrinterName() As String
        Get
            Return m_printerName
        End Get
        Set(ByVal value As String)
            m_printerName = value
        End Set
    End Property

#End Region

    Private Function CreateStream(ByVal name As String, ByVal fileNameExtension As String, ByVal encoding As Encoding, ByVal mimeType As String, ByVal willSeek As Boolean) As Stream
        Dim stream As Stream
        Try
            Dim fName As String = name & CStr(seedNr) & "." & fileNameExtension
            fName = Path.Combine(Path.GetTempPath, fName)
            stream = New FileStream(fName, FileMode.Create)
            m_streams.Add(stream)
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "CreateStream", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Throw
        End Try
        Return stream
    End Function

    Private Function GetDeviceInfo(ByVal outFormat As OutputFormat) As String
        Dim deviceInfo As String = _
         "<DeviceInfo>" + _
         "  <OutputFormat>EMF</OutputFormat>" + _
         "</DeviceInfo>"

        Select Case outFormat
            Case OutputFormat.Printer
                deviceInfo = "<DeviceInfo>" + _
                             "  <OutputFormat>EMF</OutputFormat>" + _
                             "</DeviceInfo>"
            Case OutputFormat.PDF, OutputFormat.EXCEL
                deviceInfo = "<DeviceInfo>" + _
                             "  <SimplePageHeaders>True</SimplePageHeaders>" + _
                             "</DeviceInfo>"
        End Select
        Return deviceInfo
    End Function

    Private Sub Export(ByVal report As LocalReport)

        'Dim deviceInfo As String = _
        ' "<DeviceInfo>" + _
        ' "  <OutputFormat>EMF</OutputFormat>" + _
        ' "</DeviceInfo>"
        Dim deviceInfo As String = GetDeviceInfo(Me.m_OutputFormat)
        Dim mimeType As String = String.Empty
        Dim encoding As String = String.Empty
        Dim extension As String = String.Empty
        Dim streamids As String() = Nothing

        Dim warnings() As Warning = Nothing
        m_streams = New List(Of Stream)()
        'report.Render("Image", deviceInfo, AddressOf CreateStream, warnings)



        Select Case m_OutputFormat
            Case OutputFormat.Printer
                report.Render("Image", deviceInfo, AddressOf CreateStream, warnings)
                Dim stream As Stream
                For Each stream In m_streams
                    stream.Position = 0
                Next
            Case OutputFormat.PDF, OutputFormat.EXCEL
                results = report.Render(m_OutputFormat.ToString, deviceInfo, mimeType, encoding, extension, streamids, warnings)

                'Create a file stream and write the report to it

                Dim _file As String

                If String.IsNullOrEmpty(PathExportFile) Then
                    _file = FileName
                Else
                    _file = PathExportFile & "/" & FileName
                End If

                Using _stream As FileStream = File.OpenWrite(_file)

                    _stream.Write(results, 0, results.Length)
                End Using
                 
        End Select
         
    End Sub

    Private Sub PrintPage(ByVal sender As Object, ByVal ev As PrintPageEventArgs)
        Dim pageImage As New Metafile(m_streams(m_currentPageIndex))
        ev.Graphics.DrawImage(pageImage, ev.PageBounds)

        m_currentPageIndex += 1
        ev.HasMorePages = (m_currentPageIndex < m_streams.Count)
    End Sub

    Private Sub Print()
        'Const printerName As String = "Microsoft Office Document Image Writer"

        If m_streams Is Nothing Or m_streams.Count = 0 Then
            Return
        End If

        Dim printDoc As New PrintDocument()
        printDoc.PrinterSettings.PrinterName = m_printerName
        If Not printDoc.PrinterSettings.IsValid Then
            Dim msg As String = String.Format("Can't find printer ""{0}"".", m_printerName)
            MessageBox.Show(msg, "Print Report", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Console.WriteLine(msg)
            Return
        End If
        AddHandler printDoc.PrintPage, AddressOf PrintPage
        printDoc.Print()
    End Sub

    Public Sub Print_Report()
        Export(m_report)
        Select Case m_OutputFormat
            Case OutputFormat.Printer
                m_currentPageIndex = 0
                Print()
            Case OutputFormat.PDF, OutputFormat.EXCEL
                WriteFiler()
        End Select
    End Sub

    Private Sub WriteFiler()

    End Sub

#Region " IDisposable Support "
    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.


        If Not (m_streams Is Nothing) Then
            Dim stream As Stream
            For Each stream In m_streams
                stream.Close()
            Next
            m_streams = Nothing
        End If

        'Dispose(True)
        'GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
