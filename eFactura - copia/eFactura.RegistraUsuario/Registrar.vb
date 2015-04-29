Imports ReportUtilities
Module Registrar

    Sub Main(ByVal args() As String)
        Try
            If (args.Length > 0) Then
                Dim ruc As String = args(0)

                Dim i As New Interfaz
                Dim u = i.GetUsuario(ruc)
                If (u IsNot Nothing) Then
                    Dim correos As New List(Of String)
                    correos.Add(u.email)
                    Dim html As String = i.PlantillaRegistro(u.nombre_completo, u.usuario, u.password_ini)
                    If i.EnviarCorreo("Notificacion de Registro", html, ReportUtilities.Tools.Configuraciones.UsuarioEmail, correos) Then
                        Logs.WriteLog("Se envio el correo electronico al usuario:" + u.id_usuario + " al correo: " + u.email + " con la contraseña inicial:" + u.password_ini)
                    Else
                        Throw New Exception("No fue posible enviar el correo al usuario:" + u.cedula + "con correo:" + u.email)
                    End If
                Else
                    Throw New Exception("No se a encontrado el usuario con el ruc o cédula:" + ruc)
                End If
            Else
                Throw New Exception("No se a enviado el número de cédula o r.u.c a la interfaz")
            End If
        Catch ex As Exception
            Logs.WriteErrorLog(ex)
        End Try
    End Sub

End Module
