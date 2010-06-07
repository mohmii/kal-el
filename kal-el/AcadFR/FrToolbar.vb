Imports System
Imports System.Type
Imports System.CLSCompliantAttribute
Imports System.Reflection
Imports System.Runtime.InteropServices

Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Interop
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports FR

<Assembly: ExtensionApplication(GetType(FrToolbarApp))> 
<Assembly: CommandClass(GetType(SelectionCommand))> 
<Assembly: CommandClass(GetType(adskClass))> 
<Assembly: CommandClass(GetType(DatabaseConn))> 

'class for testing toolbar
Public Class FrToolbarApp
    Implements Autodesk.AutoCAD.Runtime.IExtensionApplication

    'Private Shared FrToolbarModule As System.Reflection.Module
    Public Shared ModulePath As String

    Public Sub Initialize() Implements Autodesk.AutoCAD.Runtime.IExtensionApplication.Initialize
        ' Create an AutoCAD toolbar with 3 buttons linked to the 10 AutoCAD commands defined below
        Dim FrToolbarModule As System.Reflection.Module = System.Reflection.Assembly.GetExecutingAssembly().GetModules()(0)
        ModulePath = FrToolbarModule.FullyQualifiedName

        Try
            ModulePath = ModulePath.Substring(0, ModulePath.LastIndexOf("\"))
            ModulePath = ModulePath.Substring(0, ModulePath.LastIndexOf("\"))
        Catch
            MsgBox("Error with Module Path")
            Exit Sub
        End Try

        Dim acadApp As Autodesk.AutoCAD.Interop.AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication
        Dim hwTb As Autodesk.AutoCAD.Interop.AcadToolbar = acadApp.MenuGroups.Item(0).Toolbars.Add("FR Tool")

        Dim tbBut0 As Autodesk.AutoCAD.Interop.AcadToolbarItem = hwTb.AddToolbarButton(0, "Machining Feature", "FR - Machining Feature", "_palette ")
        tbBut0.SetBitmaps(ModulePath + "\Images\app1.bmp", ModulePath + "\Images\app1.bmp")

        Dim tbBut1 As Autodesk.AutoCAD.Interop.AcadToolbarItem = hwTb.AddToolbarButton(1, "Preferences", "FR - Preferences", "_pref ")
        tbBut1.SetBitmaps(ModulePath + "\Images\app2.bmp", ModulePath + "\Images\app2.bmp")

        Dim tbBut2 As Autodesk.AutoCAD.Interop.AcadToolbarItem = hwTb.AddToolbarButton(2, "Feature Database", "FR - Feature Database", "_loadform ")
        tbBut2.SetBitmaps(ModulePath + "\Images\app3.bmp", ModulePath + "\Images\app3.bmp")

        'Dim tbBut3 As Autodesk.AutoCAD.Interop.AcadToolbarItem = hwTb.AddToolbarButton(3, "Feature Database", "FR - Feature Database", "_tesbreak ")
        'tbBut3.SetBitmaps(ModulePath + "\Images\app3.bmp", ModulePath + "\Images\app3.bmp")
        
    End Sub

    Public Sub Terminate() Implements Autodesk.AutoCAD.Runtime.IExtensionApplication.Terminate

    End Sub

End Class