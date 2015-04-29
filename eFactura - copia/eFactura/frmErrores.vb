Public Class frmErrores

    Private Sub frmErrores_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ConsultarTipoDoc()
            btnBuscar_Click(sender, e)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Dim ds As New DataSet
        Dim cls As New basXML

        ds = cls.consError(cmbTipoDoc.SelectedValue)
        dgvDocumento.DataSource = Nothing
        dgvDocumento.DataSource = ds.Tables(0)
        dgvDocumento.Columns(2).Width = 800
        For i = 0 To dgvDocumento.ColumnCount - 1
            dgvDocumento.Columns(i).ReadOnly = True
        Next
    End Sub

    Private Sub ConsultarTipoDoc()
        Try
            Dim ds As New DataSet
            Dim cls As New basXML
            ds = cls.ConsultarTipoDoc
            cmbTipoDoc.DataSource = ds.Tables(0)
            cmbTipoDoc.DisplayMember = "nombre"
            cmbTipoDoc.ValueMember = "codigo"
        Catch ex As Exception
        End Try
    End Sub

    Private Sub cmbTipoDoc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTipoDoc.SelectedIndexChanged
        Try

        Catch ex As Exception

        End Try
    End Sub

    Private Sub cmbTipoDoc_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cmbTipoDoc.SelectionChangeCommitted
        Try
            btnBuscar_Click(sender, e)
        Catch ex As Exception

        End Try
    End Sub
End Class