Imports System.IO
Imports System.Text
Imports Altair.Common
Imports BitBoard = System.UInt128
Imports Move = System.UInt32

Module IO
    Public Function ReadRecords(ByVal file_name As String) As List(Of Record)
        Dim AppPath As String
        Dim FilePath As String
        Dim line As String
        Dim i As Integer
        Dim r As Record
        Dim records As List(Of Record)
        Dim s As String()
        Dim sr As StreamReader
        AppPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
        FilePath = AppPath & "\\" & file_name
        sr = New StreamReader(FilePath, Encoding.UTF8)
        records = New List(Of Record)
        Do
            line = sr.ReadLine()
            s = line.Split(",")
            r = New Record()
            r.str_moves = New List(Of String)
            If s(0) = "B" Then
                r.winner = 0
            ElseIf s(0) = "W" Then
                r.winner = 1
            Else
                r.winner = 2 'Case of Draw
            End If
            r.ply = s.Length - 2
            For i = 2 To r.ply + 1
                r.str_moves.Add(s(i))
            Next i
            records.Add(r)
            If sr.EndOfStream Then
                Exit Do
            End If
        Loop
        sr.Close()
        Return records
    End Function
    Public Function ReadTestFile(ByVal file_name As String, ByRef comments As List(Of String)) As List(Of String)
        Dim AppPath As String
        Dim FilePath As String
        Dim str_return As List(Of String)
        Dim line As String
        Dim flag As Integer
        Dim sr As StreamReader
        AppPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
        FilePath = AppPath & "\\" & file_name
        str_return = New List(Of String)
        line = ""
        flag = 0
        sr = New StreamReader(FilePath, Encoding.UTF8)
        Do
            line = sr.ReadLine()
            If flag = 0 Then
                comments.Add(line)
            Else
                str_return.Add(line)
            End If
            flag = flag Xor 1
            If sr.EndOfStream Then
                Exit Do
            End If
        Loop
        sr.Close()
        Return str_return
    End Function
End Module
