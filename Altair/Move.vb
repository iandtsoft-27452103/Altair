Imports BitBoard = System.UInt128
Imports Rand = System.UInt128
Imports Key = System.UInt128
Imports Move = System.UInt32

Module MoveModule
    Public Function GetTo(ByVal m As Move) As Integer
        Return m And 127
    End Function
    Public Function GetFrom(ByVal m As Move) As Integer
        Return (m >> 7) And 127
    End Function
    Public Function IsPromote(ByVal m As Move) As Integer
        Return (m >> 14) And 1
    End Function
    Public Function GetPiece(ByVal m As Move) As Integer
        Return (m >> 15) And 15
    End Function
    Public Function GetCapPiece(ByVal m As Move) As Integer
        Return (m >> 19) And 15
    End Function

    'xxxxxxxx xxxxxxxx x1111111 To位置
    'xxxxxxxx xx111111 1xxxxxxx From位置
    'xxxxxxxx x1xxxxxx xxxxxxxx 成る手かどうか
    'xxxxx111 1xxxxxxx xxxxxxxx 動かした駒の種類
    'x1111xxx xxxxxxxx xxxxxxxx 捕獲した駒
    Public Function Pack(ByVal ifrom As Integer, ByVal ito As Integer, ByVal pc As Piece, ByVal cap_pc As Piece, ByVal flag_promo As Integer) As Move
        Return (cap_pc << 19) Or (pc << 15) Or (flag_promo << 14) Or (ifrom << 7) Or ito
    End Function
    Public Function GetNullMove() As Move
        Return (1 << 23)
    End Function
End Module
