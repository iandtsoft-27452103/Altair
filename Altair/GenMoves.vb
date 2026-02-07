Imports Altair.Common
Imports BitBoard = System.UInt128
Imports Move = System.UInt32

Module GenMoves
    Public Sub GenDrop(ByVal bt As BoardTree, ByVal c As Integer, ByRef moves As List(Of Move))
        Dim i As Integer
        Dim sq As Integer
        Dim ifrom As Integer
        Dim ito As Integer
        Dim bb_occupied As BitBoard
        Dim bb_empty As BitBoard
        Dim bb As BitBoard
        Dim m As Move
        Dim bb_piece_can_drop As BitBoard() = {0, 0, 0, 0, 0, 0, 0, 0}
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        bb_empty = (Not bb_occupied) And BB_Full
        If bt.Hand(c) And Hand_Mask(Piece.Pawn) > 0 Then
            For i = File.File1 To File.File9
                bb = BB_File(i) And bt.BB_Piece(c, Piece.Pawn)
                If bb = 0 Then
                    bb_piece_can_drop(Piece.Pawn) = bb_piece_can_drop(Piece.Pawn) Or ((Not bt.BB_Piece(c, Piece.Pawn) And BB_Full And BB_Pawn_Lance_Can_Drop(c) And bb_empty And BB_File(i)))
                End If
            Next i
            sq = bt.SQ_King(c Xor 1) + Delta_Table(c Xor 1)
            bb = 0
            If sq >= 0 And sq < Square_NB Then
                bb = bb_piece_can_drop(Piece.Pawn) And ABB_Mask(sq)
                If bt.Board(sq) = Piece.Empty And bb > 0 Then
                    If IsMatePawnDrop(bt, sq, c Xor 1) Then
                        bb_piece_can_drop(Piece.Pawn) = bb_piece_can_drop(Piece.Pawn) Xor ABB_Mask(sq)
                    End If
                End If
            End If
        End If
        bb_piece_can_drop(Piece.Lance) = BB_Pawn_Lance_Can_Drop(c) And bb_empty
        bb_piece_can_drop(Piece.Knight) = BB_Knight_Can_Drop(c) And bb_empty
        bb_piece_can_drop(Piece.Silver) = BB_Others_Can_Drop And bb_empty
        bb_piece_can_drop(Piece.Gold) = bb_piece_can_drop(Piece.Silver)
        bb_piece_can_drop(Piece.Bishop) = bb_piece_can_drop(Piece.Silver)
        bb_piece_can_drop(Piece.Rook) = bb_piece_can_drop(Piece.Silver)
        For i = Piece.Pawn To Piece.Rook
            If (bt.Hand(c) And Hand_Mask(i)) > 0 Then
                bb = bb_piece_can_drop(i)
                While bb > 0
                    m = 0
                    ifrom = Square_NB + i - 1
                    ito = Square(bb)
                    bb = bb Xor ABB_Mask(ito)
                    m = Pack(ifrom, ito, i, 0, 0)
                    moves.Add(m)
                End While
            End If
        Next i
    End Sub
    Public Sub GenNoCap(ByVal bt As BoardTree, ByVal c As Integer, ByRef moves As List(Of Move))
        Dim bb_occupied As BitBoard
        Dim bb_empty As BitBoard
        Dim bb_from As BitBoard
        Dim bb_to As BitBoard
        Dim bb_can_promote As BitBoard
        Dim m As Move
        Dim ifrom As Integer
        Dim ito As Integer
        Dim i As Integer
        Dim flag_promo As Integer
        Dim piece_list As Integer() = {Piece.Gold, Piece.King, Piece.Pro_Pawn, Piece.Pro_Lance, Piece.Pro_Knight, Piece.Pro_Silver}
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        bb_empty = (Not bb_occupied) And BB_Full
        bb_from = bt.BB_Piece(c, Piece.Pawn)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Pawn, ifrom) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                    flag_promo = 1
                Else
                    flag_promo = 0
                End If
                m = Pack(ifrom, ito, Piece.Pawn, 0, flag_promo)
                moves.Add(m)
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Knight)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Knight, ifrom) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                bb_can_promote = BB_Rev_Color_Position(c) And ABB_Mask(ito)
                If bb_can_promote > 0 Then
                    m = Pack(ifrom, ito, Piece.Knight, 0, 1)
                    moves.Add(m)
                End If
                If (BB_Knight_Must_Promote(c) And ABB_Mask(ito)) = 0 Then
                    m = Pack(ifrom, ito, Piece.Knight, 0, 0)
                    moves.Add(m)
                End If
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Silver)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Silver, ifrom) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                bb_can_promote = BB_Rev_Color_Position(c) And (ABB_Mask(ifrom) Or ABB_Mask(ito))
                If bb_can_promote > 0 Then
                    m = Pack(ifrom, ito, Piece.Silver, 0, 1)
                    moves.Add(m)
                End If
                m = Pack(ifrom, ito, Piece.Silver, 0, 0)
                moves.Add(m)
            End While
        End While
        For i = 0 To piece_list.Length - 1
            bb_from = bt.BB_Piece(c, piece_list(i))
            While bb_from > 0
                ifrom = Square(bb_from)
                bb_from = bb_from Xor ABB_Mask(ifrom)
                bb_to = ABB_Piece_Attacks(c, piece_list(i), ifrom) And bb_empty
                While bb_to > 0
                    ito = Square(bb_to)
                    bb_to = bb_to Xor ABB_Mask(ito)
                    m = Pack(ifrom, ito, piece_list(i), 0, 0)
                    moves.Add(m)
                End While
            End While
        Next i
        bb_from = bt.BB_Piece(c, Piece.Lance)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Lance_Attacks(c, ifrom)(ABB_Lance_Mask_Ex(c, ifrom) And bb_occupied) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                bb_can_promote = BB_Rev_Color_Position(c) And ABB_Mask(ito)
                If bb_can_promote > 0 Then
                    m = Pack(ifrom, ito, Piece.Lance, 0, 1)
                    If (BB_Knight_Must_Promote(c) And ABB_Mask(ito)) = 0 Then
                        m = Pack(ifrom, ito, Piece.Lance, 0, 0)
                        moves.Add(m)
                    End If
                    m = Pack(ifrom, ito, Piece.Lance, 0, 1)
                    moves.Add(m)
                Else
                    m = Pack(ifrom, ito, Piece.Lance, 0, 0)
                    moves.Add(m)
                End If
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Bishop)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Diagonal_Attacks(ifrom)(ABB_Diagonal_Mask_Ex(ifrom) And bb_occupied) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                    flag_promo = 1
                Else
                    flag_promo = 0
                End If
                m = Pack(ifrom, ito, Piece.Bishop, 0, flag_promo)
                moves.Add(m)
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Horse)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = (ABB_Diagonal_Attacks(ifrom)(ABB_Diagonal_Mask_Ex(ifrom) And bb_occupied) Or ABB_Piece_Attacks(c, Piece.King, ifrom)) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Horse, 0, 0)
                moves.Add(m)
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Rook)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Cross_Attacks(ifrom)(ABB_Cross_Mask_Ex(ifrom) And bb_occupied) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                    flag_promo = 1
                Else
                    flag_promo = 0
                End If
                m = Pack(ifrom, ito, Piece.Rook, 0, flag_promo)
                moves.Add(m)
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Dragon)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = (ABB_Cross_Attacks(ifrom)(ABB_Cross_Mask_Ex(ifrom) And bb_occupied) Or ABB_Piece_Attacks(c, Piece.King, ifrom)) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Dragon, 0, 0)
                moves.Add(m)
            End While
        End While
    End Sub
    Public Sub GenCap(ByVal bt As BoardTree, ByVal c As Integer, ByRef moves As List(Of Move))
        Dim bb_occupied As BitBoard
        Dim bb_can_cap As BitBoard
        Dim bb_can_promote As BitBoard
        Dim bb_from As BitBoard
        Dim bb_to As BitBoard
        Dim m As Move
        Dim ifrom As Integer
        Dim ito As Integer
        Dim i As Integer
        Dim flag_promo As Integer
        Dim piece_list As Integer() = {Piece.Gold, Piece.King, Piece.Pro_Pawn, Piece.Pro_Lance, Piece.Pro_Knight, Piece.Pro_Silver}
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        bb_can_cap = bt.BB_Occupied(c Xor 1)
        bb_from = bt.BB_Piece(c, Piece.Pawn)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Pawn, ifrom) And bb_can_cap
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                    flag_promo = 1
                Else
                    flag_promo = 0
                End If
                m = Pack(ifrom, ito, Piece.Pawn, Math.Abs(bt.Board(ito)), flag_promo)
                moves.Add(m)
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Knight)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Knight, ifrom) And bb_can_cap
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                bb_can_promote = BB_Rev_Color_Position(c) And ABB_Mask(ito)
                If bb_can_promote > 0 Then
                    m = Pack(ifrom, ito, Piece.Knight, Math.Abs(bt.Board(ito)), 1)
                    moves.Add(m)
                End If
                If (BB_Knight_Must_Promote(c) And ABB_Mask(ito)) = 0 Then
                    m = Pack(ifrom, ito, Piece.Knight, Math.Abs(bt.Board(ito)), 0)
                    moves.Add(m)
                End If
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Silver)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Silver, ifrom) And bb_can_cap
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                bb_can_promote = BB_Rev_Color_Position(c) And ABB_Mask(ito)
                If bb_can_promote > 0 Then
                    m = Pack(ifrom, ito, Piece.Silver, Math.Abs(bt.Board(ito)), 1)
                    moves.Add(m)
                End If
                m = Pack(ifrom, ito, Piece.Silver, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
        For i = 0 To piece_list.Length - 1
            bb_from = bt.BB_Piece(c, piece_list(i))
            While bb_from > 0
                ifrom = Square(bb_from)
                bb_from = bb_from Xor ABB_Mask(ifrom)
                bb_to = ABB_Piece_Attacks(c, piece_list(i), ifrom) And bb_can_cap
                While bb_to > 0
                    ito = Square(bb_to)
                    bb_to = bb_to Xor ABB_Mask(ito)
                    m = Pack(ifrom, ito, piece_list(i), Math.Abs(bt.Board(ito)), 0)
                    moves.Add(m)
                End While
            End While
        Next i
        bb_from = bt.BB_Piece(c, Piece.Lance)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Lance_Attacks(c, ifrom)(ABB_Lance_Mask_Ex(c, ifrom) And bb_occupied) And bb_can_cap
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                bb_can_promote = BB_Rev_Color_Position(c) And ABB_Mask(ito)
                If bb_can_promote > 0 Then
                    m = Pack(ifrom, ito, Piece.Lance, Math.Abs(bt.Board(ito)), 1)
                    moves.Add(m)
                End If
                If (BB_Knight_Must_Promote(c) And ABB_Mask(ito)) = 0 Then
                    m = Pack(ifrom, ito, Piece.Lance, Math.Abs(bt.Board(ito)), 0)
                    moves.Add(m)
                End If
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Bishop)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Diagonal_Attacks(ifrom)(ABB_Diagonal_Mask_Ex(ifrom) And bb_occupied) And bb_can_cap
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If (BB_Rev_Color_Position(c) And ABB_Mask(ifrom)) > 0 Or (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                    flag_promo = 1
                Else
                    flag_promo = 0
                End If
                m = Pack(ifrom, ito, Piece.Bishop, Math.Abs(bt.Board(ito)), flag_promo)
                moves.Add(m)
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Horse)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = (ABB_Diagonal_Attacks(ifrom)(ABB_Diagonal_Mask_Ex(ifrom) And bb_occupied) Or ABB_Piece_Attacks(c, Piece.King, ifrom)) And bb_can_cap
            While (bb_to > 0)
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Horse, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Rook)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Cross_Attacks(ifrom)(ABB_Cross_Mask_Ex(ifrom) And bb_occupied) And bb_can_cap
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If (BB_Rev_Color_Position(c) And ABB_Mask(ifrom)) > 0 Or (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                    flag_promo = 1
                Else
                    flag_promo = 0
                End If
                m = Pack(ifrom, ito, Piece.Rook, Math.Abs(bt.Board(ito)), flag_promo)
                moves.Add(m)
            End While
        End While
        bb_from = bt.BB_Piece(c, Piece.Dragon)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = (ABB_Cross_Attacks(ifrom)(ABB_Cross_Mask_Ex(ifrom) And bb_occupied) Or ABB_Piece_Attacks(c, Piece.King, ifrom)) And bb_can_cap
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Dragon, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
    End Sub
    'btはByvalの方が良いか？
    Public Sub GenEvasion(ByRef bt As BoardTree, ByVal c As Integer, ByRef moves As List(Of Move))
        Dim sq_king As Integer
        Dim ifrom As Integer
        Dim ito As Integer
        Dim checker_num As Integer
        Dim sq_checker As Integer
        Dim i As Integer
        Dim sq As Integer
        Dim m As Move
        Dim idirec As Integer
        Dim ipiece As Integer
        Dim checker As Integer
        Dim bb_to As BitBoard
        Dim bb_not_color As BitBoard
        Dim bb_checker As BitBoard
        Dim bb_cap_checker As BitBoard
        Dim bb_object As BitBoard
        Dim bb_inter As BitBoard
        Dim bb_defender As BitBoard
        Dim bb_occupied As BitBoard
        Dim bb_empty As BitBoard
        Dim bb As BitBoard
        Dim bb0 As BitBoard
        Dim bb1 As BitBoard
        Dim bb_piece_can_drop As BitBoard() = {0, 0, 0, 0, 0, 0, 0, 0}
        Dim flag As Boolean

        sq_king = bt.SQ_King(c)
        ifrom = sq_king
        bt.BB_Occupied(c) = bt.BB_Occupied(c) Xor ABB_Mask(ifrom)
        bb_not_color = (Not bt.BB_Occupied(c)) And BB_Full
        bb_to = ABB_Piece_Attacks(c, Piece.King, sq_king) And bb_not_color
        While bb_to > 0
            ito = Square(bb_to)
            If IsAttacked(bt, ito, c) = 0 Then
                m = Pack(ifrom, ito, Piece.King, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End If
            bb_to = bb_to Xor ABB_Mask(ito)
        End While
        bt.BB_Occupied(c) = bt.BB_Occupied(c) Xor ABB_Mask(ifrom)
        bb_checker = AttacksToPiece(bt, sq_king, c Xor 1)
        checker_num = BitBoard.PopCount(bb_checker)
        If checker_num = 2 Then
            Return
        End If
        sq_checker = Square(bb_checker)
        bb_cap_checker = AttacksToPiece(bt, sq_checker, c)
        ito = sq_checker
        While bb_cap_checker > 0
            ifrom = Square(bb_cap_checker)
            bb_cap_checker = bb_cap_checker Xor ABB_Mask(ifrom)
            If ifrom = sq_king Then
                Continue While
            End If
            ipiece = Math.Abs(bt.Board(ifrom))
            idirec = Adirec(ifrom, ito)
            flag = False
            If IsPinnedOnKing(bt, ifrom, idirec, c) = 0 Then
                If Set_Piece_Can_Promote0.Contains(ipiece) And (ABB_Piece_Attacks(c, ipiece, ifrom) And ABB_Mask(sq_checker)) > 0 And (ABB_Piece_Attacks(c, ipiece, ifrom) And BB_Rev_Color_Position(c)) > 0 Then
                    m = Pack(ifrom, ito, ipiece, Math.Abs(bt.Board(ito)), 1)
                    DoMove(bt, m, c)
                    If IsAttacked(bt, sq_king, c) = 0 Then
                        moves.Add(m)
                    End If
                    UnDoMove(bt, m, c)
                    If ipiece = Piece.Pawn Then
                        flag = True
                    End If
                End If
                If (Set_Piece_Can_Promote1.Contains(ipiece)) Then
                    If (BB_Rev_Color_Position(c) And ABB_Mask(ifrom)) > 0 Or (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                        m = Pack(ifrom, ito, ipiece, Math.Abs(bt.Board(ito)), 1)
                        DoMove(bt, m, c)
                        If IsAttacked(bt, sq_king, c) = 0 Then
                            moves.Add(m)
                        End If
                        UnDoMove(bt, m, c)
                        If ipiece <> Piece.Silver Then
                            flag = True
                        End If
                    End If
                End If
                If flag = False Then
                    m = Pack(ifrom, ito, ipiece, Math.Abs(bt.Board(ito)), 0)
                    DoMove(bt, m, c)
                    If IsAttacked(bt, sq_king, c) = 0 Then
                        moves.Add(m)
                    End If
                    UnDoMove(bt, m, c)
                End If
            End If
        End While
        checker = Math.Abs(bt.Board(sq_checker))
        If Set_Long_Attack_Pieces.Contains(checker) = False Then
            Return
        End If
        If (bb_checker And ABB_Piece_Attacks(c, Piece.King, sq_king)) > 0 Then
            Return
        Else
            If Set_Long_Attack_Pieces.Contains(checker) Then
                bb_inter = ABB_Obstacles(sq_king, sq_checker)
                While bb_inter > 0
                    ito = Square(bb_inter)
                    bb_inter = bb_inter Xor ABB_Mask(ito)
                    bb_defender = AttacksToPiece(bt, ito, c)
                    While bb_defender > 0
                        ifrom = Square(bb_defender)
                        bb_defender = bb_defender Xor ABB_Mask(ifrom)
                        If ifrom = sq_king Then
                            Continue While
                        End If
                        ipiece = Math.Abs(bt.Board(ifrom))
                        idirec = Adirec(sq_king, ifrom)
                        flag = False
                        If idirec = Direction.Direc_Misc Or IsPinnedOnKing(bt, ifrom, idirec, c) = 0 Then
                            If Set_Piece_Can_Promote0.Contains(ipiece) Then
                                If ipiece <> Piece.Lance And (ABB_Piece_Attacks(c, ipiece, ifrom) And BB_Rev_Color_Position(c)) > 0 Then
                                    m = Pack(ifrom, ito, ipiece, Math.Abs(bt.Board(ito)), 1)
                                    moves.Add(m)
                                    If ipiece = Piece.Pawn Then
                                        flag = True
                                    End If
                                ElseIf ipiece = Piece.Lance Then
                                    bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
                                    bb0 = ABB_Lance_Attacks(c, ifrom)(ABB_Lance_Mask_Ex(c, ifrom) And bb_occupied)
                                    bb1 = BB_Rev_Color_Position(c) And ABB_Mask(ito)
                                    If bb0 > 0 And bb1 > 0 Then
                                        m = Pack(ifrom, ito, ipiece, Math.Abs(bt.Board(ito)), 1)
                                        moves.Add(m)
                                    End If
                                End If
                            End If
                            If Set_Piece_Can_Promote1.Contains(ipiece) Then
                                If (BB_Rev_Color_Position(c) And ABB_Mask(ifrom)) > 0 Or (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                                    m = Pack(ifrom, ito, ipiece, Math.Abs(bt.Board(ito)), 1)
                                    moves.Add(m)
                                    If ipiece <> Piece.Silver Then
                                        flag = True
                                    End If
                                End If
                            End If
                            If flag = False Then
                                If (ipiece = Piece.Knight Or ipiece = Piece.Lance) And (BB_Knight_Must_Promote(c) And ABB_Mask(ito)) > 0 Then
                                    Continue While
                                End If
                                m = Pack(ifrom, ito, ipiece, Math.Abs(bt.Board(ito)), 0)
                                moves.Add(m)
                            End If
                        End If
                    End While
                End While
            End If
            bb_empty = ABB_Obstacles(sq_king, sq_checker)
            bb_piece_can_drop(Piece.Pawn) = 0
            If bt.Hand(c) And Hand_Mask(Piece.Pawn) > 0 Then
                For i = File.File1 To File.File9
                    If (BB_File(i) And bt.BB_Piece(c, Piece.Pawn)) = 0 Then
                        bb = (Not bt.BB_Piece(c, Piece.Pawn) And BB_Full) And BB_Pawn_Lance_Can_Drop(c) And bb_empty And BB_File(i)
                        bb_piece_can_drop(Piece.Pawn) = bb_piece_can_drop(Piece.Pawn) Or bb
                    End If
                Next i
                sq = bt.SQ_King(c) + Delta_Table(c)
                If sq >= 0 And sq < Square_NB Then
                    If bt.Board(sq) = Piece.Empty And (bb_piece_can_drop(Piece.Pawn) And ABB_Mask(sq)) = 0 Then
                        If IsMatePawnDrop(bt, sq, c) Then
                            bb_piece_can_drop(Piece.Pawn) = bb_piece_can_drop(Piece.Pawn) Xor ABB_Mask(sq)
                        End If
                    End If
                End If
            End If
            bb_piece_can_drop(Piece.Lance) = BB_Pawn_Lance_Can_Drop(c) And bb_empty
            bb_piece_can_drop(Piece.Knight) = BB_Knight_Can_Drop(c) And bb_empty
            bb_piece_can_drop(Piece.Silver) = BB_Others_Can_Drop And bb_empty
            bb_piece_can_drop(Piece.Gold) = bb_piece_can_drop(Piece.Silver)
            bb_piece_can_drop(Piece.Bishop) = bb_piece_can_drop(Piece.Silver)
            bb_piece_can_drop(Piece.Rook) = bb_piece_can_drop(Piece.Silver)
            For i = Piece.Pawn To Piece.Rook
                If (bt.Hand(c) And Hand_Mask(i)) > 0 Then
                    bb_object = bb_piece_can_drop(i)
                    While bb_object > 0
                        ifrom = Square_NB + i - 1
                        ito = Square(bb_object)
                        bb_object = bb_object Xor ABB_Mask(ito)
                        m = Pack(ifrom, ito, i, 0, 0)
                        moves.Add(m)
                    End While
                End If
            Next i
        End If
    End Sub
    Public Sub GenCheck(ByVal bt As BoardTree, ByVal c As Integer, ByRef moves As List(Of Move))
        Dim bb_occupied As BitBoard
        Dim bb_move_to As BitBoard
        Dim bb_empty As BitBoard
        Dim bb_temp As BitBoard
        Dim bb_temp2 As BitBoard
        Dim bb_from As BitBoard
        Dim bb_to As BitBoard
        Dim bb_rd As BitBoard
        Dim bb_bh As BitBoard
        Dim bb_attacks As BitBoard
        Dim m As Move
        Dim idirec As Integer
        Dim ifrom As Integer
        Dim ito As Integer
        Dim flag_promo As Integer
        Dim opponent_color As Integer
        Dim sq_opponent_king As Integer
        Dim sq_object As Integer
        Dim sq_pawn As Integer

        opponent_color = c Xor 1
        sq_opponent_king = bt.SQ_King(opponent_color)
        sq_object = sq_opponent_king + Delta_Table(opponent_color)
        sq_pawn = sq_opponent_king + (2 * Delta_Table(opponent_color))

        '歩を突く手
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        bb_empty = (Not bb_occupied) And BB_Full
        bb_move_to = (bt.BB_Occupied(c Xor 1) Or bb_empty) And BB_Full
        If sq_pawn >= 0 And sq_pawn < Square_NB Then
            If (bt.Board(sq_pawn) = Sign_Table(opponent_color) * Piece.Pawn) And ((ABB_Mask(sq_pawn) And BB_Pawn_Mask(c)) > 0) And (ABB_Mask(sq_object) And bb_move_to) > 0 Then
                m = Pack(sq_pawn, sq_object, Piece.Pawn, Math.Abs(bt.Board(sq_object)), 0)
                moves.Add(m)
            End If
        End If
        ' 歩を成る手
        bb_from = bt.BB_Piece(c, Piece.Pawn)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = BB_Rev_Color_Position(c) And ABB_Piece_Attacks(c, Piece.Pawn, ifrom) And ABB_Piece_Attacks(opponent_color, Piece.King, sq_opponent_king) And bb_move_to
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Pawn, Math.Abs(bt.Board(ito)), 1)
                moves.Add(m)
            End While
        End While
        ' 歩を動かして龍・飛でDiscovered Checkにする手
        bb_from = ABB_Rank_Attacks(sq_opponent_king)(0) And bt.BB_Piece(c, Piece.Pawn)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_rd = bt.BB_Piece(c, Piece.Rook) Or bt.BB_Piece(c, Piece.Dragon)
            bb_temp = ABB_Rank_Attacks(ifrom)(0) And bb_rd
            bb_temp2 = ABB_Piece_Attacks(c, Piece.Pawn, ifrom) And bb_move_to
            If ((bt.BB_Piece(c, Piece.Rook) Or bt.BB_Piece(c, Piece.Dragon)) And ABB_Rank_Attacks(ifrom)(0)) > 0 And (ABB_Piece_Attacks(c, Piece.Pawn, ifrom) And bb_move_to) > 0 Then
                If (BB_Rev_Color_Position(c) And ABB_Piece_Attacks(c, Piece.Pawn, ifrom)) = 0 Then
                    flag_promo = 0
                Else
                    flag_promo = 1
                End If
                ito = ifrom + Delta_Table(c)
                m = Pack(ifrom, ito, Piece.Pawn, Math.Abs(bt.Board(ito)), flag_promo)
                moves.Add(m)
            End If
        End While
        ' 歩を動かして馬・角でDiscovered Checkにする手
        bb_from = ABB_Diag1_Attacks(sq_opponent_king)(0) And bt.BB_Piece(c, Piece.Pawn)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_bh = bt.BB_Piece(c, Piece.Bishop) Or bt.BB_Piece(c, Piece.Horse)
            bb_temp = ABB_Diag1_Attacks(ifrom)(0) And (bt.BB_Piece(c, Piece.Bishop) Or bt.BB_Piece(c, Piece.Horse))
            bb_temp2 = ABB_Piece_Attacks(c, Piece.Pawn, ifrom) And bb_move_to
            If ((ABB_Diag1_Attacks(ifrom)(0) And (bt.BB_Piece(c, Piece.Bishop) Or bt.BB_Piece(c, Piece.Horse))) > 0 And ((ABB_Piece_Attacks(c, Piece.Pawn, ifrom) And bb_move_to)) > 0) Then
                If ((BB_Rev_Color_Position(c) And ABB_Piece_Attacks(c, Piece.Pawn, ifrom)) = 0) Then
                    flag_promo = 0
                Else
                    flag_promo = 1
                End If
                ito = ifrom + Delta_Table(c)
                m = Pack(ifrom, ito, Piece.Pawn, Math.Abs(bt.Board(ito)), flag_promo)
                moves.Add(m)
            End If
        End While
        bb_from = ABB_Diag2_Attacks(sq_opponent_king)(0) And bt.BB_Piece(c, Piece.Pawn)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_bh = bt.BB_Piece(c, Piece.Bishop) Or bt.BB_Piece(c, Piece.Horse)
            bb_temp = ABB_Diag2_Attacks(ifrom)(0) And (bt.BB_Piece(c, Piece.Bishop) Or bt.BB_Piece(c, Piece.Horse))
            bb_temp2 = ABB_Piece_Attacks(c, Piece.Pawn, ifrom) And bb_move_to
            If ((ABB_Diag2_Attacks(ifrom)(0) And (bt.BB_Piece(c, Piece.Bishop) Or bt.BB_Piece(c, Piece.Horse))) > 0 And ((ABB_Piece_Attacks(c, Piece.Pawn, ifrom) And bb_move_to)) > 0) Then
                If ((BB_Rev_Color_Position(c) And ABB_Piece_Attacks(c, Piece.Pawn, ifrom)) = 0) Then
                    flag_promo = 0
                Else
                    flag_promo = 1
                End If
                ito = ifrom + Delta_Table(c)
                m = Pack(ifrom, ito, Piece.Pawn, Math.Abs(bt.Board(ito)), flag_promo)
                moves.Add(m)
            End If
        End While
        bb_temp = 0
        ' 歩打ち
        If sq_object >= 0 And sq_object < Square_NB Then
            bb_temp = BB_File(FileTable(sq_object)) And bt.BB_Piece(c, Piece.Pawn)
        End If
        If bb_temp = 0 And sq_object >= 0 And sq_object < Square_NB Then
            If (bt.Hand(c) And Hand_Mask(Piece.Pawn)) > 0 And bt.Board(sq_object) = Piece.Empty And IsMatePawnDrop(bt, sq_object, c Xor 1) = False Then
                m = Pack(Square_NB + Piece.Pawn - 1, sq_object, Piece.Pawn, 0, 0)
                moves.Add(m)
            End If
        End If
        '銀を動かす手
        bb_from = bt.BB_Piece(c, Piece.Silver)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Silver, ifrom) And ABB_Piece_Attacks(opponent_color, Piece.Silver, sq_opponent_king) And bb_move_to
            If idirec <> Direction.Direc_Misc And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = 0
                bb_to = bb_to Or AddBehindAttacks(bb_temp, idirec, sq_opponent_king) And ABB_Piece_Attacks(c, Piece.Silver, ifrom) And bb_move_to
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Silver, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
        '銀を成る手
        bb_from = bt.BB_Piece(c, Piece.Silver)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Silver, ifrom) And ABB_Piece_Attacks(opponent_color, Piece.Gold, sq_opponent_king) And bb_move_to
            If idirec <> Direction.Direc_Misc And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = 0
                bb_to = bb_to Or (AddBehindAttacks(bb_temp, idirec, sq_opponent_king) And ABB_Piece_Attacks(c, Piece.Silver, ifrom) And bb_move_to)
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If (BB_Rev_Color_Position(c) And ABB_Mask(ifrom)) > 0 Or (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                    m = Pack(ifrom, ito, Piece.Silver, Math.Abs(bt.Board(ito)), 1)
                    moves.Add(m)
                End If
            End While
        End While
        ' 銀を打つ手
        If (bt.Hand(c) And Hand_Mask(Piece.Silver)) > 0 Then
            bb_to = ABB_Piece_Attacks(opponent_color, Piece.Silver, sq_opponent_king) And bb_empty
            While (bb_to > 0)
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(Square_NB + Piece.Silver - 1, ito, Piece.Silver, 0, 0)
                moves.Add(m)
            End While
        End If
        ' 金を動かす手
        bb_from = bt.BB_Piece(c, Piece.Gold) Or bt.BB_Piece(c, Piece.Pro_Pawn) Or bt.BB_Piece(c, Piece.Pro_Lance) Or bt.BB_Piece(c, Piece.Pro_Knight) Or bt.BB_Piece(c, Piece.Pro_Silver)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Gold, ifrom) And ABB_Piece_Attacks(opponent_color, Piece.Gold, sq_opponent_king) And bb_move_to
            If idirec <> Direction.Direc_Misc And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = 0
                bb_to = bb_to Xor (AddBehindAttacks(bb_temp, idirec, sq_opponent_king) And ABB_Piece_Attacks(c, Piece.Gold, ifrom) And bb_move_to)
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Math.Abs(bt.Board(ifrom)), Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
        ' 金を打つ手
        If (bt.Hand(c) And Hand_Mask(Piece.Gold)) > 0 Then
            bb_to = ABB_Piece_Attacks(opponent_color, Piece.Gold, sq_opponent_king) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(Square_NB + Piece.Gold - 1, ito, Piece.Gold, 0, 0)
                moves.Add(m)
            End While
        End If
        ' 桂を動かす手
        bb_from = bt.BB_Piece(c, Piece.Knight)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Knight, ifrom) And ABB_Piece_Attacks(opponent_color, Piece.Knight, sq_opponent_king) And bb_move_to
            If idirec <> Direction.Direc_Misc And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = 0
                bb_to = bb_to Or (AddBehindAttacks(bb_temp, idirec, sq_opponent_king) And ABB_Piece_Attacks(c, Piece.Knight, ifrom) And bb_move_to)
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Knight, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
        ' 桂を成る手
        bb_from = bt.BB_Piece(c, Piece.Knight)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = ABB_Piece_Attacks(c, Piece.Knight, ifrom) And ABB_Piece_Attacks(opponent_color, Piece.Gold, sq_opponent_king) And bb_move_to
            If idirec <> Direction.Direc_Misc And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = 0
                bb_to = bb_to Or (AddBehindAttacks(bb_temp, idirec, sq_opponent_king) And ABB_Piece_Attacks(c, Piece.Knight, ifrom) And bb_move_to)
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If (BB_Rev_Color_Position(c) And ABB_Mask(ifrom)) > 0 Or (BB_Rev_Color_Position(c) And ABB_Mask(ito)) > 0 Then
                    m = Pack(ifrom, ito, Piece.Knight, Math.Abs(bt.Board(ito)), 1)
                    moves.Add(m)
                End If
            End While
        End While
        ' 桂を打つ手
        If (bt.Hand(c) And Hand_Mask(Piece.Knight)) > 0 Then
            bb_to = ABB_Piece_Attacks(opponent_color, Piece.Knight, sq_opponent_king) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(Square_NB + Piece.Knight - 1, ito, Piece.Knight, 0, 0)
                moves.Add(m)
            End While
        End If
        ' 玉を動かす手
        ifrom = bt.SQ_King(c)
        idirec = Adirec(sq_opponent_king, ifrom)
        If (idirec <> Direction.Direc_Misc) And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
            bb_temp = 0
            bb_to = AddBehindAttacks(bb_temp, idirec, sq_opponent_king) And ABB_Piece_Attacks(c, Piece.King, ifrom) And bb_move_to
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.King, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End If
        '香を動かす手 => Discovered Check以外は必ず駒を取る手になる
        bb_from = bt.BB_Piece(c, Piece.Lance)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Lance_Attacks(c, ifrom)(ABB_Lance_Mask_Ex(c, ifrom) And bb_occupied) And ((Not BB_Knight_Must_Promote(c)) And BB_Full And bb_move_to)
            bb_attacks = bb_to
            bb_to = bb_to And ABB_Lance_Attacks(c Xor 1, sq_opponent_king)(ABB_Lance_Mask_Ex(c Xor 1, sq_opponent_king) And bb_occupied)
            idirec = Adirec(sq_opponent_king, ifrom)
            If (idirec <> Direction.Direc_Misc) And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = bb_attacks And AddBehindAttacks(bb_temp, idirec, sq_opponent_king)
                bb_to = bb_to Or bb_temp
                If c = Color.Black Then
                    bb_to = bb_to And (BB_File(FileTable(ifrom)) And (BB_Rank(2) Or BB_Rank(3)))
                Else
                    bb_to = bb_to And (BB_File(FileTable(ifrom)) And (BB_Rank(6) Or BB_Rank(5)))
                End If
            End If
            While (bb_to > 0)
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Lance, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
        ' 香を成る手
        bb_from = bt.BB_Piece(c, Piece.Lance)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Lance_Attacks(c, ifrom)(ABB_Lance_Mask_Ex(c, ifrom) And bb_occupied)
            bb_attacks = bb_to
            bb_to = bb_to And (BB_Rev_Color_Position(c) And BB_Full And ABB_Piece_Attacks(opponent_color, Piece.Gold, sq_opponent_king) And bb_move_to)
            bb_to = bb_to And (ABB_Lance_Attacks(c Xor 1, sq_opponent_king)(ABB_Lance_Mask_Ex(c Xor 1, sq_opponent_king) And bb_occupied))
            idirec = Adirec(sq_opponent_king, ifrom)
            If (idirec <> Direction.Direc_Misc) And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = bb_attacks And AddBehindAttacks(bb_temp, idirec, sq_opponent_king)
                bb_to = bb_to Or bb_temp
                bb_to = bb_to And (BB_Color_Position(Color.Black) Or BB_Color_Position(Color.White))
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Lance, Math.Abs(bt.Board(ito)), 1)
                moves.Add(m)
            End While
        End While
        '香を打つ手
        If (bt.Hand(c) And Hand_Mask(Piece.Lance)) > 0 Then
            bb_to = ABB_Lance_Attacks(c Xor 1, sq_opponent_king)(ABB_Lance_Mask_Ex(c Xor 1, sq_opponent_king) And bb_occupied) And bb_empty
            While (bb_to > 0)
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(Square_NB + Piece.Lance - 1, ito, Piece.Lance, 0, 0)
                moves.Add(m)
            End While
        End If
        ' 飛を動かす手
        bb_from = bt.BB_Piece(c, Piece.Rook) And (BB_Color_Position(c) Or BB_DMZ)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Cross_Attacks(ifrom)(ABB_Cross_Mask_Ex(ifrom) And bb_occupied)
            bb_attacks = bb_to
            bb_to = bb_to And bb_move_to
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = bb_to And (ABB_Cross_Attacks(sq_opponent_king)(ABB_Cross_Mask_Ex(sq_opponent_king) And bb_occupied))
            bb_to = bb_to And (BB_Color_Position(c) Or BB_DMZ)
            If (idirec <> Direction.Direc_Misc) And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = bb_attacks And AddBehindAttacks(bb_temp, idirec, sq_opponent_king)
                bb_to = bb_to Or bb_temp
                bb_to = bb_to And (BB_Color_Position(c) Or BB_DMZ)
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Rook, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
        '飛を成る手
        bb_from = bt.BB_Piece(c, Piece.Rook)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Cross_Attacks(ifrom)(ABB_Cross_Mask_Ex(ifrom) And bb_occupied)
            bb_attacks = bb_to
            bb_to = bb_to And bb_move_to
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = bb_to And ((ABB_Cross_Attacks(sq_opponent_king)(ABB_Cross_Mask_Ex(sq_opponent_king) And bb_occupied)) Or ABB_Piece_Attacks(opponent_color, Piece.King, sq_opponent_king))
            If (idirec <> Direction.Direc_Misc) And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = bb_attacks And AddBehindAttacks(bb_temp, idirec, sq_opponent_king)
                bb_to = bb_to Or bb_temp
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If ((ABB_Mask(ifrom) And BB_Rev_Color_Position(c)) > 0 Or (ABB_Mask(ito) And BB_Rev_Color_Position(c)) > 0) Then
                    m = Pack(ifrom, ito, Piece.Rook, Math.Abs(bt.Board(ito)), 1)
                    moves.Add(m)
                End If
            End While
        End While
        '飛を打つ手
        If (bt.Hand(c) And Hand_Mask(Piece.Rook)) > 0 Then
            bb_to = ABB_Cross_Attacks(sq_opponent_king)(ABB_Cross_Mask_Ex(sq_opponent_king) And bb_occupied) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(Square_NB + Piece.Rook - 1, ito, Piece.Rook, 0, 0)
                moves.Add(m)
            End While
        End If
        '角を動かす手
        bb_from = bt.BB_Piece(c, Piece.Bishop) And (BB_Color_Position(c) Or BB_DMZ)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Diagonal_Attacks(ifrom)(ABB_Diagonal_Mask_Ex(ifrom) And bb_occupied)
            bb_attacks = bb_to
            bb_to = bb_to And bb_move_to
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = bb_to And (ABB_Diagonal_Attacks(sq_opponent_king)(ABB_Diagonal_Mask_Ex(sq_opponent_king) And bb_occupied))
            bb_to = bb_to And (BB_Color_Position(c) Or BB_DMZ)
            If (idirec <> Direction.Direc_Misc) And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = 0
                bb_to = bb_to Or (bb_attacks And AddBehindAttacks(bb_temp, idirec, sq_opponent_king))
                bb_to = bb_to And (BB_Color_Position(c) Or BB_DMZ)
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Bishop, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
        '角を成る手
        bb_from = bt.BB_Piece(c, Piece.Bishop)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Diagonal_Attacks(ifrom)(ABB_Diagonal_Mask_Ex(ifrom) And bb_occupied)
            bb_attacks = bb_to
            bb_to = bb_to And bb_move_to
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = bb_to And ((ABB_Diagonal_Attacks(sq_opponent_king)(ABB_Diagonal_Mask_Ex(sq_opponent_king) And bb_occupied)) Or (ABB_Piece_Attacks(opponent_color, Piece.King, sq_opponent_king)))
            If (idirec <> Direction.Direc_Misc) And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = 0
                bb_to = bb_to Or (bb_attacks And AddBehindAttacks(bb_temp, idirec, sq_opponent_king))
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                If ((ABB_Mask(ifrom) And BB_Rev_Color_Position(c)) > 0 Or (ABB_Mask(ito) And BB_Rev_Color_Position(c)) > 0) Then
                    m = Pack(ifrom, ito, Piece.Bishop, Math.Abs(bt.Board(ito)), 1)
                    moves.Add(m)
                End If
            End While
        End While
        ' 角を打つ手
        If (bt.Hand(c) And Hand_Mask(Piece.Bishop)) > 0 Then
            bb_to = ABB_Diagonal_Attacks(sq_opponent_king)(ABB_Diagonal_Mask_Ex(sq_opponent_king) And bb_occupied) And bb_empty
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(Square_NB + Piece.Bishop - 1, ito, Piece.Bishop, 0, 0)
                moves.Add(m)
            End While
        End If
        ' 龍を動かす手
        bb_from = bt.BB_Piece(c, Piece.Dragon)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Cross_Attacks(ifrom)(ABB_Cross_Mask_Ex(ifrom) And bb_occupied) Or ABB_Piece_Attacks(c, Piece.King, ifrom)
            bb_attacks = bb_to
            bb_to = bb_to And bb_move_to
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = bb_to And (ABB_Cross_Attacks(sq_opponent_king)(ABB_Cross_Mask_Ex(sq_opponent_king) And bb_occupied) Or ABB_Piece_Attacks(c, Piece.King, sq_opponent_king))
            If (idirec <> Direction.Direc_Misc) And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = 0
                bb_to = bb_attacks And AddBehindAttacks(bb_temp, idirec, sq_opponent_king)
            End If
            While (bb_to > 0)
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Dragon, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
        ' 馬を動かす手
        bb_from = bt.BB_Piece(c, Piece.Horse)
        While bb_from > 0
            ifrom = Square(bb_from)
            bb_from = bb_from Xor ABB_Mask(ifrom)
            bb_to = ABB_Diagonal_Attacks(ifrom)(ABB_Diagonal_Mask_Ex(ifrom) And bb_occupied) Or ABB_Piece_Attacks(c, Piece.King, ifrom)
            bb_attacks = bb_to
            bb_to = bb_to And bb_move_to
            idirec = Adirec(sq_opponent_king, ifrom)
            bb_to = bb_to And (ABB_Diagonal_Attacks(sq_opponent_king)(ABB_Diagonal_Mask_Ex(sq_opponent_king) And bb_occupied) Or ABB_Piece_Attacks(c, Piece.King, sq_opponent_king))
            If (idirec <> Direction.Direc_Misc) And IsPinnedOnKing(bt, ifrom, idirec, opponent_color) > 0 Then
                bb_temp = 0
                bb_to = bb_to Or (bb_attacks And AddBehindAttacks(bb_temp, idirec, sq_opponent_king))
            End If
            While bb_to > 0
                ito = Square(bb_to)
                bb_to = bb_to Xor ABB_Mask(ito)
                m = Pack(ifrom, ito, Piece.Horse, Math.Abs(bt.Board(ito)), 0)
                moves.Add(m)
            End While
        End While
    End Sub
    Private Function AddBehindAttacks(ByVal bb As BitBoard, ByVal idirec As Integer, ByVal ik As Integer) As BitBoard
        Dim bb_tmp As BitBoard
        bb_tmp = 0
        Select Case Math.Abs(idirec)
            Case Direction.Direc_Diag1_U2d
                bb_tmp = ABB_Diag1_Attacks(ik)(0)
            Case Direction.Direc_Diag2_U2d
                bb_tmp = ABB_Diag2_Attacks(ik)(0)
            Case Direction.Direc_File_U2d
                bb_tmp = ABB_File_Attacks(ik)(0)
            Case Direction.Direc_Rank_L2r
                bb_tmp = ABB_Rank_Attacks(ik)(0)
        End Select
        bb_tmp = (BB_Full And (Not bb_tmp)) Or bb
        Return bb_tmp
    End Function
End Module
