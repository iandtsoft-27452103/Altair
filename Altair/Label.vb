Imports Move = System.UInt32

Module LabelModule
    Public Function MakeOutputLabel(ByVal m As Move) As Label
        Dim d As Integer
        Dim lbl As Label
        Dim ifrom As Integer
        Dim ito As Integer
        Dim pc As Integer
        Dim is_promo As Integer
        ifrom = GetFrom(m)
        ito = GetTo(m)
        pc = GetPiece(m)
        is_promo = IsPromote(m)
        If ifrom >= Square_NB Then
            Select Case pc
                Case Piece.Pawn
                    lbl = Label.DROP_PAWN
                Case Piece.Lance
                    lbl = Label.DROP_LANCE
                Case Piece.Knight
                    lbl = Label.DROP_KNIGHT
                Case Piece.Silver
                    lbl = Label.DROP_SILVER
                Case Piece.Gold
                    lbl = Label.DROP_GOLD
                Case Piece.Bishop
                    lbl = Label.DROP_BISHOP
                Case Piece.Rook
                    lbl = Label.DROP_ROOK
            End Select
        Else
            d = Adirec(ifrom, ito)
            If is_promo = 0 Then
                Select Case d
                    Case Direction.Direc_Diag1_U2d
                        lbl = Label.DOWN_LEFT
                    Case Direction.Direc_Diag1_D2u
                        lbl = Label.UP_RIGHT
                    Case Direction.Direc_Diag2_U2d
                        lbl = Label.DOWN_RIGHT
                    Case Direction.Direc_Diag2_D2u
                        lbl = Label.UP_LEFT
                    Case Direction.Direc_File_U2d
                        lbl = Label.DOWN
                    Case Direction.Direc_File_D2u
                        lbl = Label.UP
                    Case Direction.Direc_Rank_L2r
                        lbl = Label.RIGHT
                    Case Direction.Direc_Rank_R2l
                        lbl = Label.LEFT
                    Case Direction.Direc_Knight_L_U2d
                        lbl = Label.DOWN_LEFT_KNIGHT
                    Case Direction.Direc_Knight_R_U2d
                        lbl = Label.DOWN_RIGHT_KNIGHT
                    Case Direction.Direc_Knight_L_D2u
                        lbl = Label.UP_LEFT_KNIGHT
                    Case Direction.Direc_Knight_R_D2u
                        lbl = Label.UP_RIGHT_KNIGHT
                End Select
            Else
                Select Case d
                    Case Direction.Direc_Diag1_U2d
                        lbl = Label.DOWN_LEFT_PRO
                    Case Direction.Direc_Diag1_D2u
                        lbl = Label.UP_RIGHT_PRO
                    Case Direction.Direc_Diag2_U2d
                        lbl = Label.DOWN_RIGHT_PRO
                    Case Direction.Direc_Diag2_D2u
                        lbl = Label.UP_LEFT_PRO
                    Case Direction.Direc_File_U2d
                        lbl = Label.DOWN_PRO
                    Case Direction.Direc_File_D2u
                        lbl = Label.UP_PRO
                    Case Direction.Direc_Rank_L2r
                        lbl = Label.RIGHT_PRO
                    Case Direction.Direc_Rank_R2l
                        lbl = Label.LEFT_PRO
                    Case Direction.Direc_Knight_L_U2d
                        lbl = Label.DOWN_LEFT_KNIGHT_PRO
                    Case Direction.Direc_Knight_R_U2d
                        lbl = Label.DOWN_RIGHT_KNIGHT_PRO
                    Case Direction.Direc_Knight_L_D2u
                        lbl = Label.UP_LEFT_KNIGHT_PRO
                    Case Direction.Direc_Knight_R_D2u
                        lbl = Label.UP_RIGHT_KNIGHT_PRO
                End Select
            End If
        End If
        Return lbl
    End Function
    Public Function MakeOutputLSTMLabel(ByVal m As Move, ByRef sq As Integer) As Label
        Dim ifrom As Integer
        Dim ito As Integer
        Dim pc As Integer
        Dim is_promo As Integer
        Dim h As Integer
        ifrom = GetFrom(m)
        ito = GetTo(m)
        sq = ito
        pc = GetPiece(m)
        is_promo = IsPromote(m)
        If ifrom < Square_NB Then
            h = ((is_promo << 14) + (ito << 7) + ifrom)
        Else
            h = ((is_promo << 14) + (ito << 7) + (ifrom + pc - 1))
        End If
        Return LabelTable(h)
    End Function
End Module