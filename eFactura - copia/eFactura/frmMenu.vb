Public Class frmMenu

    Private Sub DocumentosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DocumentosToolStripMenuItem.Click
        Dim frm As New frmControlDoc
        frm.MdiParent = Me
        frm.Show()
    End Sub

    Private Sub ErroresToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ErroresToolStripMenuItem.Click
        Dim frm As New frmErrores
        frm.MdiParent = Me
        frm.Show()
    End Sub

    Private Sub ConfiguracionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfiguracionToolStripMenuItem.Click
        Dim frm As New ReportUtilities.FrmConfiguraciones
        frm.ShowDialog()
    End Sub

    
End Class