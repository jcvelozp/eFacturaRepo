﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.18449
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace AutorizacionComp
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute([Namespace]:="http://autorizacion.ws/", ConfigurationName:="AutorizacionComp.AutorizacionComprobante")>  _
    Public Interface AutorizacionComprobante
        
        'CODEGEN: Generating message contract since element name key from namespace  is not marked nillable
        <System.ServiceModel.OperationContractAttribute(Action:="http://autorizacion.ws/AutorizacionComprobante/autorizacionComprobanteRequest", ReplyAction:="http://autorizacion.ws/AutorizacionComprobante/autorizacionComprobanteResponse")>  _
        Function autorizacionComprobante(ByVal request As AutorizacionComp.autorizacionComprobanteRequest) As AutorizacionComp.autorizacionComprobanteResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://autorizacion.ws/AutorizacionComprobante/autorizacionComprobanteRequest", ReplyAction:="http://autorizacion.ws/AutorizacionComprobante/autorizacionComprobanteResponse")>  _
        Function autorizacionComprobanteAsync(ByVal request As AutorizacionComp.autorizacionComprobanteRequest) As System.Threading.Tasks.Task(Of AutorizacionComp.autorizacionComprobanteResponse)
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class autorizacionComprobanteRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute(Name:="autorizacionComprobante", [Namespace]:="http://autorizacion.ws/", Order:=0)>  _
        Public Body As AutorizacionComp.autorizacionComprobanteRequestBody
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal Body As AutorizacionComp.autorizacionComprobanteRequestBody)
            MyBase.New
            Me.Body = Body
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.Runtime.Serialization.DataContractAttribute([Namespace]:="")>  _
    Partial Public Class autorizacionComprobanteRequestBody
        
        <System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue:=false, Order:=0)>  _
        Public key As String
        
        <System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue:=false, Order:=1)>  _
        Public clave_autorizacion As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal key As String, ByVal clave_autorizacion As String)
            MyBase.New
            Me.key = key
            Me.clave_autorizacion = clave_autorizacion
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class autorizacionComprobanteResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute(Name:="autorizacionComprobanteResponse", [Namespace]:="http://autorizacion.ws/", Order:=0)>  _
        Public Body As AutorizacionComp.autorizacionComprobanteResponseBody
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal Body As AutorizacionComp.autorizacionComprobanteResponseBody)
            MyBase.New
            Me.Body = Body
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.Runtime.Serialization.DataContractAttribute([Namespace]:="")>  _
    Partial Public Class autorizacionComprobanteResponseBody
        
        <System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue:=false, Order:=0)>  _
        Public [return] As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal [return] As String)
            MyBase.New
            Me.[return] = [return]
        End Sub
    End Class
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Public Interface AutorizacionComprobanteChannel
        Inherits AutorizacionComp.AutorizacionComprobante, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Partial Public Class AutorizacionComprobanteClient
        Inherits System.ServiceModel.ClientBase(Of AutorizacionComp.AutorizacionComprobante)
        Implements AutorizacionComp.AutorizacionComprobante
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String)
            MyBase.New(endpointConfigurationName)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As String)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal binding As System.ServiceModel.Channels.Binding, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(binding, remoteAddress)
        End Sub
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function AutorizacionComp_AutorizacionComprobante_autorizacionComprobante(ByVal request As AutorizacionComp.autorizacionComprobanteRequest) As AutorizacionComp.autorizacionComprobanteResponse Implements AutorizacionComp.AutorizacionComprobante.autorizacionComprobante
            Return MyBase.Channel.autorizacionComprobante(request)
        End Function
        
        Public Function autorizacionComprobante(ByVal key As String, ByVal clave_autorizacion As String) As String
            Dim inValue As AutorizacionComp.autorizacionComprobanteRequest = New AutorizacionComp.autorizacionComprobanteRequest()
            inValue.Body = New AutorizacionComp.autorizacionComprobanteRequestBody()
            inValue.Body.key = key
            inValue.Body.clave_autorizacion = clave_autorizacion
            Dim retVal As AutorizacionComp.autorizacionComprobanteResponse = CType(Me,AutorizacionComp.AutorizacionComprobante).autorizacionComprobante(inValue)
            Return retVal.Body.[return]
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function AutorizacionComp_AutorizacionComprobante_autorizacionComprobanteAsync(ByVal request As AutorizacionComp.autorizacionComprobanteRequest) As System.Threading.Tasks.Task(Of AutorizacionComp.autorizacionComprobanteResponse) Implements AutorizacionComp.AutorizacionComprobante.autorizacionComprobanteAsync
            Return MyBase.Channel.autorizacionComprobanteAsync(request)
        End Function
        
        Public Function autorizacionComprobanteAsync(ByVal key As String, ByVal clave_autorizacion As String) As System.Threading.Tasks.Task(Of AutorizacionComp.autorizacionComprobanteResponse)
            Dim inValue As AutorizacionComp.autorizacionComprobanteRequest = New AutorizacionComp.autorizacionComprobanteRequest()
            inValue.Body = New AutorizacionComp.autorizacionComprobanteRequestBody()
            inValue.Body.key = key
            inValue.Body.clave_autorizacion = clave_autorizacion
            Return CType(Me,AutorizacionComp.AutorizacionComprobante).autorizacionComprobanteAsync(inValue)
        End Function
    End Class
End Namespace
