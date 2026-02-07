Imports Altair.Common
Imports Move = System.UInt32
Module CSA
    Public Function CSA2Move(ByVal bt As BoardTree, ByVal s_csa As String) As Move
        Dim ifrom As Integer
        Dim ito As Integer
        Dim ipiece As Integer
        Dim icap_piece As Integer
        Dim flag_promo As Integer
        ifrom = Array.IndexOf(Str_CSA, s_csa.Substring(0, 2))
        ito = Array.IndexOf(Str_CSA, s_csa.Substring(2, 2))
        flag_promo = 0
        If ifrom < Square_NB Then
            ipiece = Math.Abs(bt.Board(ifrom))
        Else
            ipiece = CSA_TO_PC(s_csa.Substring(4, 2))
            ifrom += ipiece - 1
        End If
        icap_piece = Math.Abs(bt.Board(ito))
        If ipiece < Piece.King And CSA_TO_PC(s_csa.Substring(4, 2)) > Piece.King Then
            flag_promo = 1
        End If
        Return Pack(ifrom, ito, ipiece, icap_piece, flag_promo)
    End Function
    Public Function Move2CSA(ByVal move As Move) As String
        Dim str As String
        str = Str_CSA(GetFrom(move))
        str = str & Str_CSA(GetTo(move))
        If IsPromote(move) = 0 Then
            str = str & Str_Piece(GetPiece(move))
        Else
            str = str & Str_Piece(GetPiece(move) + Promote)
        End If
        Return str
    End Function
End Module
