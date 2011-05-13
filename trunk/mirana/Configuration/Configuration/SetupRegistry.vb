Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.Win32
Imports Microsoft.Win32.Registry
Imports System
Imports System.IO
Imports System.Security.Permissions

<Assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum, ViewAndModify:="HKEY_LOCAL_MACHINE")> 
Public Class SetupRegistry

    Private RegPath As String

    Private Sub GetRegPath(ByVal AcadLang As String)
        'check the system type 'later development
        'If Environment.OSVersion.VersionString.Contains("32-bit") Then
        '    RegPath = "software"
        'ElseIf Environment.OSVersion.VersionString.Contains("64-bit") Then
        '    RegPath = "software\Wow6432Node"
        'End If

        RegPath = "software\Wow6432Node"

        'check the autocad language
        If AcadLang.ToLower = "english" Then
            RegPath = RegPath & "\Autodesk\AutoCAD\R16.2\ACAD-4001:409\Applications\AcadFR"
        ElseIf AcadLang.ToLower = "japanese" Then
            RegPath = RegPath & "\Autodesk\AutoCAD\R16.2\ACAD-4001:411\Applications\AcadFR"
        End If
    End Sub

    Public Sub StartSetup(ByVal AcadLang As String, ByRef Message As String)
        Try
            GetRegPath(AcadLang)

            Dim Db As String
            Db = Environment.CurrentDirectory & "\FR.mdb"
            ListTmp.AddRange(New Object() {"<configuration>", "   <connectionStrings>", "      <remove name=""LocalSqlServer"" />", "      <add name=""AccessDataSource"" connectionString=""Provider=Microsoft.Jet.OLEDB" & _
            ".4.0;Data Source=&quot;" & Db & "&quot;""", "         providerName=""System.Data.OleDb"" />", "   </connectionStrings>"})

            Call read()
            Call Write()

            Message = ">>> Modify acad.exe.config." + vbCrLf

            Dim WriteKey As RegistryKey = Registry.LocalMachine.CreateSubKey(RegPath)

            WriteKey.SetValue("DESCRIPTION", "LOAD ACADFR")
            WriteKey.SetValue("LOADER", Environment.CurrentDirectory & "\App\AcadFR.dll")
            WriteKey.SetValue("MANAGED", 1)
            WriteKey.SetValue("LOADCTRLS", 2)

            Message = Message + ">>> Add autoload setting to registry" + vbCrLf

            WriteKey = Nothing

            Message = Message + ">>> Setup complete!"
        Catch ex As Exception
            Message = Message + ">>> Setup failed"
        End Try
    End Sub

    Public Sub RemoveSetup(ByVal AcadLang As String, ByRef Message As String)
        Try
            GetRegPath(AcadLang)

            ListRead.Clear()

            Call read()
            If ListRead.Item(1) <> "<startup>" Then
                Dim sw As StreamWriter = New StreamWriter("C:\Program Files (x86)\AutoCAD 2006\acad.exe.config")
                sw.WriteLine("<configuration>")
                For i = 6 To ListRead.Count - 1
                    sw.WriteLine(ListRead.Item(i))
                    'MsgBox(Listread.Items.Item(i))
                Next
                sw.Close()

                Message = ">>> Revert the acad.exe.config." + vbCrLf

                RegPath = RTrim(RegPath).Remove(69)

                Dim key As RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(RegPath, True)
                key.DeleteSubKey("AcadFR")

                Message = Message + "Delete the autoload setting." + vbCrLf
                Message = Message + "Autoload removal complete."
            End If

        Catch ex As Exception
            Message = Message + "Autoload removal failed."
        End Try

    End Sub

    Private ListRead As List(Of String)
    Private ListTmp As List(Of String)

    Public Sub read()
        Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser("C:\Program Files (x86)\AutoCAD 2006\acad.exe.config")
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.SetDelimiters(",")
            Dim currentRow As String()
            While Not MyReader.EndOfData
                Try
                    currentRow = MyReader.ReadFields()
                    Dim currentField As String
                    For Each currentField In currentRow
                        ListRead.Add(currentField)
                    Next
                Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
                    MsgBox("Line " & ex.Message & _
                    "is not valid and will be skipped.")
                End Try
            End While
        End Using
    End Sub

    Private Sub Write()
        'Create an instance of StreamWriter to write text to a file.
        Dim i As Integer = 1
        If ListRead.Count > 1 Then
            If ListRead.Item(1) = "<connectionStrings>" Then 'jika sudah pernah dikonfigurasi
                Dim sw As StreamWriter = New StreamWriter("C:\Program Files (x86)\AutoCAD 2006\acad.exe.config")
                'template konfigurasi
                For i = 0 To ListTmp.Count - 1
                    sw.WriteLine(ListTmp.Item(i))
                Next
                'perbedaannya sumber konfigurasi
                For i = 6 To ListRead.Count - 1
                    sw.WriteLine(ListRead.Item(i))
                Next
                sw.Close()
            Else ' Add some text to the file.
                Dim sw As StreamWriter = New StreamWriter("C:\Program Files (x86)\AutoCAD 2006\acad.exe.config")
                'template konfigurasi
                For i = 0 To ListTmp.Count - 1
                    sw.WriteLine(ListTmp.Item(i))
                Next
                'sumber konfigurasi
                For i = 1 To ListRead.Count - 1
                    sw.WriteLine(ListRead.Item(i))
                Next
                sw.Close()
            End If
        Else
            ListTmp.Clear()
            ListTmp.AddRange(New Object() {"<configuration>", "   <connectionStrings>", "      <remove name=""LocalSqlServer"" />", "      <add name=""AccessDataSource"" connectionString=""Provider=Microsoft.Jet.OLEDB" & _
                         ".4.0;Data Source=&quot;D:\firman\Working Folder\Configuration\Configuration\bin\" & _
                         "Debug\FR.mdb&quot;""", "         providerName=""System.Data.OleDb"" />", "   </connectionStrings>", "<startup>", "<!--We always use the latest version of the framework installed on the computer. " & _
                         "If you", "are having problems then explicitly specify .NET 1.1 by uncommenting the followin" & _
                         "g line.", "<supportedRuntime version=""v1.1.4322""/>", "-->", "</startup>", "</configuration>"})
            Dim sw As StreamWriter = New StreamWriter("C:\Program Files (x86)\AutoCAD 2006\acad.exe.config")
            'template konfigurasi
            For i = 0 To ListTmp.Count - 1
                sw.WriteLine(ListTmp.Item(i))
            Next
            'sumber konfigurasi
            'For i = 1 To Listread.Items.Count - 1
            '    sw.WriteLine(Listread.Items.Item(i))
            'Next
            sw.Close()
        End If
    End Sub
End Class
