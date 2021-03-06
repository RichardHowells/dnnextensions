﻿'
' Lightbox Gallery Module for DotNetNuke
' Project Contributors - Will Strohl (http://www.WillStrohl.com), Armand Datema (http://www.schwingsoft.com)
'
'Copyright (c) 2009-2012, Will Strohl
'All rights reserved.
'
'Redistribution and use in source and binary forms, with or without modification, are 
'permitted provided that the following conditions are met:
'
'Redistributions of source code must retain the above copyright notice, this list of 
'conditions and the following disclaimer.
'
'Redistributions in binary form must reproduce the above copyright notice, this list 
'of conditions and the following disclaimer in the documentation and/or other 
'materials provided with the distribution.
'
'Neither the name of Will Strohl, Armand Datema, Lightbox Gallery, nor the names of its contributors may be 
'used to endorse or promote products derived from this software without specific prior 
'written permission.
'
'THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
'EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
'OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
'SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
'INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
'TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
'BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
'CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
'ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
'DAMAGE.
'

Imports DotNetNuke
Imports DotNetNuke.Common
Imports System
Imports System.Data
Imports Microsoft.ApplicationBlocks.Data

Namespace WillStrohl.Modules.Lightbox

    Public Class SqlDataProvider
        Inherits DataProvider
        
#Region " Private Members "

        Private Const ProviderType As String = "data"

        Private p_providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private p_connectionString As String = String.Empty
        Private p_providerPath As String = String.Empty
        Private p_objectQualifier As String = String.Empty
        Private p_databaseOwner As String = String.Empty

        ' local string values
        Private Const c_ConnectionStringName As String = "connectionStringName"
        Private Const c_ConnectionString As String = "connectionString"
        Private Const c_ProviderPath As String = "providerPath"
        Private Const c_ObjectQualifier As String = "objectQualifier"
        Private Const c_Underscore As String = "_"
        Private Const c_DatabaseOwner As String = "databaseOwner"
        Private Const c_Period As String = "."
        Private Const c_SProc_Prefix As String = "wns_lightbox_"

        ' sproc names
        Private Const c_AddLightbox As String = "AddLightbox"
        Private Const c_UpdateLightbox As String = "UpdateLightbox"
        Private Const c_DeleteLightbox As String = "DeleteLightbox"
        Private Const c_GetLightbox As String = "GetLightbox"
        Private Const c_GetLightboxes As String = "GetLightboxes"
        Private Const c_GetLightboxIds As String = "GetLightboxIds"
        Private Const c_GetLightboxCount As String = "GetLightboxCount"
        Private Const c_UpdateDisplayOrder As String = "UpdateDisplayOrder"
        Private Const c_DoesDisplayOrderNeedUpdate As String = "DoesDisplayOrderNeedUpdate"

        Private Const c_AddSetting As String = "AddSetting"
        Private Const c_UpdateSetting As String = "UpdateSetting"
        Private Const c_DeleteSetting As String = "DeleteSetting"
        Private Const c_GetSettings As String = "GetSettings"

        Private Const c_AddImage As String = "AddImage"
        Private Const c_DeleteImageByFileName As String = "DeleteImageByFileName"
        Private Const c_DeleteImageById As String = "DeleteImageById"
        Private Const c_GetImageByFileName As String = "GetImageByFileName"
        Private Const c_GetImageById As String = "GetImageById"
        Private Const c_GetImages As String = "GetImages"
        Private Const c_UpdateImage As String = "UpdateImage"
        Private Const c_GetImageCount As String = "GetImageCount"

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        ''' <history>
        ''' 20101022 - wstrohl - Changed the connection string to use a different method of retrieval
        ''' </history>
        Public Sub New()

            ' Read the configuration specific information for this provider
            Dim objProvider As Framework.Providers.Provider = CType(p_providerConfiguration.Providers(p_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            ' Read the attributes for this provider
            If Not String.IsNullOrEmpty(objProvider.Attributes(c_ConnectionStringName)) Then
                p_connectionString = Utilities.Config.GetConnectionString
            Else
                p_connectionString = objProvider.Attributes(c_ConnectionString)
            End If

            p_providerPath = objProvider.Attributes(c_ProviderPath)

            p_objectQualifier = objProvider.Attributes(c_ObjectQualifier)
            If Not String.IsNullOrEmpty(p_objectQualifier) And p_objectQualifier.EndsWith(c_Underscore) = False Then
                p_objectQualifier = String.Concat(p_objectQualifier, c_Underscore)
            End If

            ' Add willstrohl_ to the beginning of the sprocs
            p_objectQualifier = String.Concat(p_objectQualifier, c_SProc_Prefix)

            p_databaseOwner = objProvider.Attributes(c_DatabaseOwner)
            If Not String.IsNullOrEmpty(p_databaseOwner) And p_databaseOwner.EndsWith(c_Period) = False Then
                p_databaseOwner = String.Concat(p_databaseOwner, c_Period)
            End If

        End Sub

#End Region

#Region " Properties "

        Public ReadOnly Property ConnectionString() As String
            Get
                Return p_connectionString
            End Get
        End Property

        Public ReadOnly Property ProviderPath() As String
            Get
                Return p_providerPath
            End Get
        End Property

        Public ReadOnly Property ObjectQualifier() As String
            Get
                Return p_objectQualifier
            End Get
        End Property

        Public ReadOnly Property DatabaseOwner() As String
            Get
                Return p_databaseOwner
            End Get
        End Property

#End Region

#Region " Data Provider Implementation "

        Public Overrides Function AddLightbox(ByVal ModuleId As Integer, ByVal GalleryName As String, ByVal GalleryDescription As String, ByVal GalleryFolder As String, ByVal DisplayOrder As Integer, ByVal HideTitleDescription As Boolean, ByVal LastUpdatedBy As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, String.Concat(DatabaseOwner, ObjectQualifier, c_AddLightbox), ModuleId, GetNull(GalleryName), GetNull(GalleryDescription), GetNull(GalleryFolder), DisplayOrder, HideTitleDescription, LastUpdatedBy), Integer)
        End Function
        Public Overrides Sub DeleteLightbox(ByVal LightboxId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, String.Concat(DatabaseOwner, ObjectQualifier, c_DeleteLightbox), LightboxId)
        End Sub
        Public Overrides Function GetLightboxes(ByVal ModuleId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, String.Concat(DatabaseOwner, ObjectQualifier, c_GetLightboxes), ModuleId), IDataReader)
        End Function
        Public Overrides Function GetLightbox(ByVal LightboxId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, String.Concat(DatabaseOwner, ObjectQualifier, c_GetLightbox), LightboxId), IDataReader)
        End Function
        Public Overrides Function GetLightboxIds(ByVal ModuleId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, String.Concat(DatabaseOwner, ObjectQualifier, c_GetLightboxIds), ModuleId), IDataReader)
        End Function
        Public Overrides Sub UpdateLightbox(ByVal LightboxId As Integer, ByVal ModuleId As Integer, ByVal GalleryName As String, ByVal GalleryDescription As String, ByVal GalleryFolder As String, ByVal DisplayOrder As Integer, ByVal HideTitleDescription As Boolean, ByVal LastUpdatedBy As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, String.Concat(DatabaseOwner, ObjectQualifier, c_UpdateLightbox), LightboxId, ModuleId, GetNull(GalleryName), GetNull(GalleryDescription), GetNull(GalleryFolder), DisplayOrder, HideTitleDescription, LastUpdatedBy)
        End Sub
        Public Overrides Function GetLightboxCount(ByVal ModuleId As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, String.Concat(DatabaseOwner, ObjectQualifier, c_GetLightboxCount), ModuleId), Integer)
        End Function
        Public Overrides Sub UpdateDisplayOrder()
            SqlHelper.ExecuteNonQuery(ConnectionString, String.Concat(DatabaseOwner, ObjectQualifier, c_UpdateDisplayOrder))
        End Sub
        Public Overrides Function DoesDisplayOrderNeedUpdate() As Boolean
            Return CType(SqlHelper.ExecuteScalar(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_DoesDisplayOrderNeedUpdate)), Boolean)
        End Function


        Public Overloads Overrides Function AddSetting(ByVal LightboxId As Integer, ByVal Padding As Integer, ByVal Margin As Integer, ByVal Opacity As Boolean, ByVal Modal As Boolean, ByVal Cyclic As Boolean, ByVal OverlayShow As Boolean, ByVal OverlayOpacity As Decimal, ByVal OverlayColor As String, ByVal TitleShow As Boolean, ByVal TitlePosition As String, ByVal Transition As String, ByVal Speed As Integer, ByVal ChangeSpeed As Integer, ByVal ShowCloseButton As Boolean, ByVal ShowNavArrows As Boolean, ByVal EnableEscapeButton As Boolean, ByVal OnStart As String, ByVal OnCancel As String, ByVal OnComplete As String, ByVal OnCleanup As String, ByVal OnClosed As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_AddSetting), LightboxId, Padding, Margin, Opacity, Modal, Cyclic, OverlayShow, OverlayOpacity, OverlayColor, TitleShow, TitlePosition, Transition, Speed, ChangeSpeed, ShowCloseButton, ShowNavArrows, EnableEscapeButton, OnStart, OnCancel, OnComplete, OnCleanup, OnClosed), Integer)
        End Function
        Public Overloads Overrides Sub UpdateSetting(ByVal SettingId As Integer, ByVal LightboxId As Integer, ByVal Padding As Integer, ByVal Margin As Integer, ByVal Opacity As Boolean, ByVal Modal As Boolean, ByVal Cyclic As Boolean, ByVal OverlayShow As Boolean, ByVal OverlayOpacity As Decimal, ByVal OverlayColor As String, ByVal TitleShow As Boolean, ByVal TitlePosition As String, ByVal Transition As String, ByVal Speed As Integer, ByVal ChangeSpeed As Integer, ByVal ShowCloseButton As Boolean, ByVal ShowNavArrows As Boolean, ByVal EnableEscapeButton As Boolean, ByVal OnStart As String, ByVal OnCancel As String, ByVal OnComplete As String, ByVal OnCleanup As String, ByVal OnClosed As String)
            SqlHelper.ExecuteNonQuery(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_UpdateSetting), SettingId, LightboxId, Padding, Margin, Opacity, Modal, Cyclic, OverlayShow, OverlayOpacity, OverlayColor, TitleShow, TitlePosition, Transition, Speed, ChangeSpeed, ShowCloseButton, ShowNavArrows, EnableEscapeButton, OnStart, OnCancel, OnComplete, OnCleanup, OnClosed)
        End Sub
        Public Overrides Sub DeleteSetting(ByVal SettingId As Integer)
            SqlHelper.ExecuteScalar(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_DeleteSetting), SettingId)
        End Sub
        Public Overrides Function GetSettings(ByVal LightboxId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_GetSettings), LightboxId), IDataReader)
        End Function


        Public Overrides Function AddImage(ByVal LightboxId As Integer, ByVal FileName As String, ByVal Title As String, ByVal Description As String, ByVal DisplayOrder As Integer, ByVal LastUpdatedBy As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_AddImage), LightboxId, FileName, Title, Description, DisplayOrder, LastUpdatedBy), Integer)
        End Function
        Public Overrides Sub DeleteImageByFileName(ByVal LightboxId As Integer, ByVal FileName As String)
            SqlHelper.ExecuteNonQuery(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_DeleteImageByFileName), LightboxId, FileName)
        End Sub
        Public Overrides Sub DeleteImageById(ByVal ImageId As Integer)
            SqlHelper.ExecuteNonQuery(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_DeleteImageById), ImageId)
        End Sub
        Public Overrides Function GetImageByFileName(ByVal LightboxId As Integer, ByVal FileName As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_GetImageByFileName), LightboxId, FileName), IDataReader)
        End Function
        Public Overrides Function GetImageById(ByVal ImageId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_GetImageById), ImageId), IDataReader)
        End Function
        Public Overrides Function GetImages(ByVal LightboxId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_GetImages), LightboxId), IDataReader)
        End Function
        Public Overrides Sub UpdateImage(ByVal ImageId As Integer, ByVal LightboxId As Integer, ByVal FileName As String, ByVal Title As String, ByVal Description As String, ByVal DisplayOrder As Integer, ByVal LastUpdatedBy As Integer)
            SqlHelper.ExecuteNonQuery(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_UpdateImage), ImageId, LightboxId, FileName, Title, Description, DisplayOrder, LastUpdatedBy)
        End Sub
        Public Overrides Function GetImageCount(ByVal LightboxId As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(Me.ConnectionString, String.Concat(Me.DatabaseOwner, Me.ObjectQualifier, c_GetImageCount), LightboxId), Integer)
        End Function

#End Region

#Region " Private Helper Methods "

        Private Function GetNull(ByVal Field As Object) As Object
            Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
        End Function

#End Region

        End Class

End Namespace