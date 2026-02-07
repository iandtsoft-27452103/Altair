Module TTModule
    Public Sub InitTT(ByRef tt As TT)
        tt.value = New Dictionary(Of UInt128, Integer)
        tt.color = New Dictionary(Of UInt128, Color)
        tt.is_check = New Dictionary(Of UInt128, Boolean)
        tt.move = New Dictionary(Of UInt128, UInteger)
        tt.ply = New Dictionary(Of UInt128, Integer)
    End Sub
End Module
