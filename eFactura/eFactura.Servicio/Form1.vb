Imports ReportUtilities
Public Class Form1

    Private Sub bgwService_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgwService.DoWork
        While True
            If e.Cancel Then
                Exit While
            End If
            Dim ctrl As New frmControlDoc
            ctrl.InformacionComprobante = New frmControlDoc.SetInformacionComprobante(AddressOf Informacion)
            e.Result = "Ocurrio un problema al subir los comprobantes"
            If ctrl.Generar() Then
                e.Result = "Se generaron conrrectamente los documentos"
            End If
            System.Threading.Thread.Sleep(3000)
        End While
    End Sub

    Private Sub btnIniciar_Click(sender As Object, e As EventArgs) Handles btnIniciar.Click
        If Not bgwService.IsBusy Then
            EnableControl(False)
            bgwService.RunWorkerAsync()
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EnableControl(True)
    End Sub

    Private auxiliar As String
    Public Sub Info()
        lblSecuencia.Text = auxiliar
    End Sub
    Public Sub Informacion(ByVal value As String)
        If lblSecuencia.InvokeRequired Then
            auxiliar = value
            lblSecuencia.Invoke(New MethodInvoker(AddressOf Info))
        End If
    End Sub

    Private Sub EnableControl(ByRef value As Boolean)
        btnIniciar.Enabled = value
        btnCancelar.Enabled = Not value
    End Sub

    Private Sub bgwService_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwService.RunWorkerCompleted
        EnableControl(True)
        'txtLog.Text &= Now & ": " & e.Result & Environment.NewLine
    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        bgwService.CancelAsync()
    End Sub

    Private Sub bgwService_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles bgwService.ProgressChanged
        ProgressBar1.Value = e.ProgressPercentage
    End Sub

    Private Sub btnConfiguraciones_Click(sender As Object, e As EventArgs) Handles btnConfiguraciones.Click
        Dim config As New FrmConfiguraciones
        config.ShowDialog()
    End Sub



End Class
