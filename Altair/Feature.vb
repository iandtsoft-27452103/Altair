Imports BitBoard = System.UInt128
Module Feature
    Public Function MakeInputFeatures(ByVal bt As BoardTree, ByVal c As Integer) As Single()
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim l As Integer
        Dim sq As Integer
        Dim delta As Integer
        Dim n_hand As Integer
        Dim bb_piece As BitBoard
        Dim pc_list() As Integer = {Piece.Pawn, Piece.Lance, Piece.Knight, Piece.Silver, Piece.Gold, Piece.Bishop, Piece.Rook, Piece.King, Piece.Pro_Pawn, Piece.Pro_Lance, Piece.Pro_Knight, Piece.Pro_Silver, Piece.Horse, Piece.Dragon}
        Dim hand_max() As Integer = {0, 18, 4, 4, 4, 4, 2, 2}
        Dim inputData(1 * 105 * 9 * 9 - 1) As Single
        delta = 0
        For i = 0 To 1
            For j = 0 To pc_list.Length - 1
                bb_piece = bt.BB_Piece(i, pc_list(j))
                While bb_piece > 0
                    sq = Square(bb_piece)
                    bb_piece = bb_piece Xor ABB_Mask(sq)
                    inputData(delta + sq) = 1.0F
                End While
                delta += Square_NB
            Next j
            For j = Piece.Pawn To Piece.Rook
                n_hand = bt.Hand(i) And Hand_Mask(j)
                k = 0
                If n_hand > 0 Then
                    While n_hand > 0
                        n_hand -= Hand_Hash(j)
                        Array.Fill(inputData, 1.0F, delta, Square_NB)
                        delta += Square_NB
                        k += 1
                    End While
                End If
                l = hand_max(j) - k
                k = 0
                While k < l
                    Array.Fill(inputData, 0.0F, delta, Square_NB)
                    delta += Square_NB
                    k += 1
                End While
            Next j
        Next i
        If c = 0 Then
            Array.Fill(inputData, 1.0F, delta, Square_NB)
        End If
        Return inputData
    End Function
End Module
