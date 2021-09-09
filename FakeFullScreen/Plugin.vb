Imports System.Runtime.InteropServices

Namespace Udrakoloader

    Public Class Plugin

        Private Shared processGame As Process = Nothing
        Private Shared processName As String = String.Empty

        Public Shared Function EntryPoint(ByVal pwzArgument As String) As Integer
            processGame = Process.GetCurrentProcess()
            processName = processGame.ProcessName
            Dim tskThread As New Task(ScriptAsyc, TaskCreationOptions.LongRunning)
            tskThread.Start()
            Return 0
        End Function

        Public Shared Sub DIH_DLLMain(ByVal DIH_ProcessID As Integer)
            processGame = Process.GetProcessById(DIH_ProcessID)
            processName = processGame.ProcessName
            Dim tskThread As New Task(ScriptAsyc, TaskCreationOptions.LongRunning)
            tskThread.Start()
        End Sub
        
        Private Shared Procede As Boolean = False

        Private Shared ScriptAsyc As New Action(
   Sub()

      For i As Integer = 0 To 2

           Dim placement = GetPlacement(processGame.MainWindowHandle)
           If placement.showCmd.ToString = "Normal" Then
               Dim FakeFullSc As Boolean = FullScreenEmulation(processName)
               Procede = True
           End If

           If Procede = True Then
               If placement.showCmd.ToString = "Maximized" Then
                   Dim FakeFullSc As Boolean = FullScreenEmulation(processName)
                   Exit For
               End If
           End If

           i -= 1
       Next

   End Sub)

        Public Shared Function FullScreenEmulation(ByVal ProcessName As String) As Boolean
            Try
                If ProcessName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) Then ProcessName = ProcessName.Remove(ProcessName.Length - ".exe".Length)
                Dim HWND As IntPtr = Process.GetProcessesByName(ProcessName).First.MainWindowHandle
                For i As Integer = 0 To 2
                    SetWindowStyle.SetWindowStyle(HWND, SetWindowStyle.WindowStyles.WS_BORDER)
                    SetWindowState.SetWindowState(HWND, SetWindowState.WindowState.Maximize)
                Next
                BringMainWindowToFront(ProcessName)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

#Region " Set Focus "

        <System.Runtime.InteropServices.DllImport("user32.dll")>
        Private Shared Function SetForegroundWindow(ByVal hwnd As IntPtr) As Integer
        End Function

        Private Shared FisrsFocus As Boolean = False

        Public Shared Sub BringMainWindowToFront(ByVal processName As String)
            If FisrsFocus = False Then
                Dim bProcess As Process = Process.GetProcessesByName(processName).FirstOrDefault()

                If bProcess IsNot Nothing Then
                    SetForegroundWindow(bProcess.MainWindowHandle)
                End If
                FisrsFocus = True
            End If
        End Sub

#End Region

#Region " Check FakeFullscreen "

        Private Shared Function GetPlacement(ByVal hwnd As IntPtr) As WINDOWPLACEMENT
            Dim placement As WINDOWPLACEMENT = New WINDOWPLACEMENT()
            placement.length = Marshal.SizeOf(placement)
            GetWindowPlacement(hwnd, placement)
            Return placement
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Shared Function GetWindowPlacement(ByVal hWnd As IntPtr, ByRef lpwndpl As WINDOWPLACEMENT) As Boolean
        End Function

        <Serializable>
        <StructLayout(LayoutKind.Sequential)>
        Friend Structure WINDOWPLACEMENT
            Public length As Integer
            Public flags As Integer
            Public showCmd As ShowWindowCommands
            Public ptMinPosition As System.Drawing.Point
            Public ptMaxPosition As System.Drawing.Point
            Public rcNormalPosition As System.Drawing.Rectangle
        End Structure

        Friend Enum ShowWindowCommands As Integer
            Hide = 0
            Normal = 1
            Minimized = 2
            Maximized = 3
        End Enum

#End Region

    End Class

End Namespace

