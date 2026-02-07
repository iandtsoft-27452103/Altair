Imports Altair.Common
Imports BitBoard = System.UInt128
Imports Move = System.UInt32

Module Mate1Ply
    Public Function MateIn1Ply(ByVal bt As BoardTree, ByVal c As Integer) As Move
        Dim mate_move As Move
        Dim null_move As Move
        Dim bb_can_escape As BitBoard
        Dim bb_opp_king_attacks As BitBoard
        Dim bb_myside_attacks As BitBoard
        Dim bb_enemy_attacks As BitBoard
        Dim bb As BitBoard
        Dim bb2 As BitBoard
        Dim bb_occupied As BitBoard
        Dim bb_opponent_attacks_to_sq As BitBoard
        Dim bb_my_knight_attacks As BitBoard
        Dim sq_can_check_by_drop As Integer() = {0, 0, 0, 0, 0, 0, 0, 0}
        Dim sq_can_check_by_move As Integer() = {0, 0, 0, 0, 0, 0, 0, 0}
        Dim pos_array As Integer() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Dim pc_array As Integer() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Dim sq_can_escape As Integer() = {0, 0, 0, 0, 0, 0, 0, 0}
        Dim sq As Integer
        Dim sq2 As Integer
        Dim sq3 As Integer
        Dim myside_attacks_count As Integer
        Dim attacks_count As Integer
        Dim sq_object As Integer
        Dim cnt_pos As Integer
        Dim cnt_pc As Integer
        Dim cnt_d As Integer
        Dim cnt_m As Integer
        Dim cnt_e As Integer
        Dim idirec As Integer
        Dim idirec2 As Integer
        Dim idirec3 As Integer
        Dim pc As Integer
        Dim pc_promote As Integer
        Dim flag_promo As Integer
        Dim pos As Integer
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim index As Integer
        Dim counter As Integer
        Dim flag As Boolean
        Dim mate_flag As Boolean
        Dim hand As UInt32
        Dim opponent_color As Integer
        Dim sq_opponent_king As Integer
        Dim pt As Dictionary(Of Integer, List(Of Integer))
        Dim pcs As List(Of Integer)
        cnt_d = 0
        cnt_m = 0
        cnt_e = 0
        '// 敵玉の位置を取得する。
        opponent_color = c Xor 1
        sq_opponent_king = bt.SQ_King(opponent_color)
        bb_can_escape = BB_Full And (Not bt.BB_Occupied(opponent_color))
        hand = bt.Hand(c)
        bb_opp_king_attacks = ABB_Piece_Attacks(opponent_color, Piece.King, sq_opponent_king)
        While bb_opp_king_attacks > 0
            sq = Square(bb_opp_king_attacks)
            bb_opp_king_attacks = bb_opp_king_attacks Xor ABB_Mask(sq)
            bb_myside_attacks = AttacksToPiece(bt, sq, opponent_color)
            myside_attacks_count = BitBoard.PopCount(bb_myside_attacks)
            flag = False
            If myside_attacks_count >= 2 And bt.Board(sq) = Piece.Empty Then
                'If there are attacks from opponent pieces except king, opponents can capture the checker.
                flag = True
            End If
            If (bb_can_escape And ABB_Mask(sq)) > 0 Then
                'If there are attacks from your pieces, you maybe generate escape move.
                If IsAttacked(bt, sq, opponent_color) = 0 Then
                    sq_can_escape(cnt_e) = sq
                    cnt_e += 1
                End If
            End If
            If bt.Board(sq) = Piece.Empty And flag = False Then
                sq_can_check_by_drop(cnt_d) = sq
                cnt_d += 1
            End If
            bb_enemy_attacks = IsAttacked(bt, sq, c Xor 1)
            If bt.Board(sq) <> Piece.Empty And (bt.BB_Occupied(opponent_color) And ABB_Mask(sq)) > 0 And bb_enemy_attacks > 0 Then
                sq_can_check_by_move(cnt_m) = sq
                cnt_m += 1
            End If
            If myside_attacks_count < 2 And bt.Board(sq) = Piece.Empty And bb_enemy_attacks > 0 Then
                sq_can_check_by_move(cnt_m) = sq
                cnt_m += 1
            End If
        End While
        For i = 0 To cnt_d - 1
            sq = sq_can_check_by_drop(i)
            idirec = Adirec(sq, sq_opponent_king)
            pt = Piece_Table(opponent_color)
            bb = AttacksToPiece(bt, sq, opponent_color)
            cnt_pos = 0
            cnt_pc = 0
            While bb > 0
                pos = Square(bb)
                bb = bb Xor ABB_Mask(pos)
                pos_array(cnt_pos) = pos
                pc_array(cnt_pc) = bt.Board(pos)
                cnt_pos += 1
                cnt_pc += 1
            End While
            pcs = pt(idirec)
            If hand > 0 Then
                For j = 0 To pt.Count - 1
                    pc = pcs(j)
                    If pc > Piece.Rook Then
                        Exit For
                    End If
                    If pc <> Piece.Pawn And (hand And Hand_Mask(pc)) > 0 Then
                        If cnt_e = 0 Then
                            mate_move = Pack(Square_NB + pc - 1, sq, pc, 0, 0)
                            Return mate_move
                        End If
                        counter = 0
                        mate_flag = True
                        For k = 0 To cnt_e - 1
                            sq_object = sq_can_escape(k)
                            If sq = sq_object Then
                                counter += 1
                            End If
                            If IsCanEscape(bt, c, sq, pc, sq_opponent_king, sq_object, False) = False And IsCanCapture(bt, c, opponent_color, sq, True, -1, pc) = False Then
                                counter += 1
                            Else
                                mate_flag = False
                            End If
                        Next k
                        If counter = cnt_e And mate_flag = True Then
                            mate_move = Pack(Square_NB + pc - 1, sq, pc, 0, 0)
                            Return mate_move
                        End If
                    End If
                Next j
            End If
        Next i
        For i = 0 To cnt_m - 1
            sq = sq_can_check_by_move(i)
            idirec = Adirec(sq, sq_opponent_king)
            pt = Piece_Table(opponent_color)
            bb = AttacksToPiece(bt, sq, c)
            attacks_count = BitBoard.PopCount(bb)
            If attacks_count < 2 And bb > 0 Then
                pos = Square(bb)
                bb2 = AttacksToLongPiece(bt, pos, c)
                While bb2 > 0
                    sq2 = Square(bb2)
                    bb2 = bb2 Xor ABB_Mask(sq2)
                    idirec2 = Adirec(sq2, sq_opponent_king)
                    If idirec = idirec2 Then
                        If cnt_e = 0 Then
                            If IsCanCapture(bt, c, opponent_color, sq, False, pos, Math.Abs(bt.Board(pos))) = False Then
                                mate_move = Pack(pos, sq, Math.Abs(bt.Board(pos)), Math.Abs(bt.Board(sq)), 0)
                                Return mate_move
                            End If
                        ElseIf cnt_e = 1 Then
                            sq3 = sq_can_escape(0)
                            idirec3 = Adirec(sq3, sq_opponent_king)
                            If IsCanCapture(bt, c, opponent_color, sq, False, pos, Math.Abs(bt.Board(pos))) = False Then
                                If Math.Abs(idirec) = Math.Abs(idirec3) Then
                                    Select Case Math.Abs(idirec)
                                        Case Direction.Direc_File_U2d
                                            If Math.Abs(bt.Board(pos)) = Piece.Lance Or Math.Abs(bt.Board(pos)) = Piece.Rook Or Math.Abs(bt.Board(pos)) = Piece.Dragon Then
                                                mate_move = Pack(pos, sq, Math.Abs(bt.Board(pos)), Math.Abs(bt.Board(sq)), 0)
                                                Return mate_move
                                            End If
                                        Case Direction.Direc_Rank_L2r
                                            If Math.Abs(bt.Board(pos)) = Piece.Rook Or Math.Abs(bt.Board(pos)) = Piece.Dragon Then
                                                mate_move = Pack(pos, sq, Math.Abs(bt.Board(pos)), Math.Abs(bt.Board(sq)), 0)
                                                Return mate_move
                                            End If
                                        Case Direction.Direc_Diag1_U2d
                                            If Math.Abs(bt.Board(pos)) = Piece.Bishop Or Math.Abs(bt.Board(pos)) = Piece.Horse Then
                                                mate_move = Pack(pos, sq, Math.Abs(bt.Board(pos)), Math.Abs(bt.Board(sq)), 0)
                                                Return mate_move
                                            End If
                                        Case Direction.Direc_Diag2_U2d
                                            If Math.Abs(bt.Board(pos)) = Piece.Bishop Or Math.Abs(bt.Board(pos)) = Piece.Horse Then
                                                mate_move = Pack(pos, sq, Math.Abs(bt.Board(pos)), Math.Abs(bt.Board(sq)), 0)
                                                Return mate_move
                                            End If
                                    End Select
                                End If
                            End If
                        End If
                    Else
                        If cnt_e = 0 And (ABB_Piece_Attacks(c, Piece.Gold, sq) And ABB_Piece_Attacks(opponent_color, Piece.King, sq_opponent_king)) > 0 And (ABB_Mask(sq) And BB_Color_Position(opponent_color)) > 0 Then
                            bb_myside_attacks = AttacksToPiece(bt, sq, opponent_color)
                            myside_attacks_count = BitBoard.PopCount(bb_myside_attacks)
                            idirec3 = Adirec(pos, bt.SQ_King(c))
                            bt.BB_Occupied(c) = bt.BB_Occupied(c) Xor ABB_Mask(pos)
                            bb = IsPinnedOnKing(bt, pos, idirec3, c)
                            bt.BB_Occupied(c) = bt.BB_Occupied(c) Xor ABB_Mask(pos)
                            If myside_attacks_count < 2 And bb = 0 Then
                                mate_move = Pack(pos, sq, Math.Abs(bt.Board(pos)), Math.Abs(bt.Board(sq)), 1)
                                Return mate_move
                            End If
                        End If
                    End If
                End While
                Continue For
            End If
            cnt_pos = 0
            cnt_pc = 0
            While bb > 0
                pos = Square(bb)
                bb = bb Xor ABB_Mask(pos)
                pos_array(cnt_pos) = pos
                pc_array(cnt_pc) = bt.Board(pos)
                cnt_pos += 1
                cnt_pc += 1
            End While
            pcs = pt(idirec)
            If cnt_pos = 0 Then ' This Then maybe Not make sense.
                Continue For
            End If
            index = 0
            While index < cnt_pos
                pos = pos_array(index)
                pc = Math.Abs(pc_array(index))
                If pc = Piece.King Then
                    index += 1
                    Continue While
                End If
                idirec = Adirec(pos, sq_opponent_king)
                If IsDiscoverKing2(bt, pos, sq, c, pc) = True Then
                    index += 1
                    Continue While
                End If
                If pcs.Contains(pc) = True Then
                    If LongPieces2.Contains(pc) = True Then
                        If cnt_e = 0 Then
                            If LongPieces.Contains(pc) And IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                                If (ABB_Mask(sq) And BB_Color_Position(opponent_color)) > 0 Then
                                    flag_promo = 1
                                Else
                                    flag_promo = 0
                                End If
                                mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), flag_promo)
                                Return mate_move
                            End If
                        End If
                        flag = False
                        For j = 0 To cnt_e - 1
                            sq_object = sq_can_escape(j)
                            If sq = sq_object Then
                                Continue For
                            End If
                            If IsCanEscape(bt, c, sq, pc, sq_opponent_king, sq_object, False) = False And IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                                If (ABB_Mask(pos) And BB_Color_Position(opponent_color)) > 0 Or (ABB_Mask(sq) And BB_Color_Position(opponent_color)) > 0 Then
                                    flag_promo = 1
                                Else
                                    flag_promo = 0
                                End If
                                mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), flag_promo)
                                flag = True
                            Else
                                flag = False
                                mate_move = 0
                                Exit For
                            End If
                        Next j
                        If flag = True And mate_move <> 0 Then
                            Return mate_move
                        End If
                    ElseIf pc = Piece.Dragon Or pc = Piece.Horse Then
                        If cnt_e = 0 Then
                            If IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                                mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 0)
                                Return mate_move
                            End If
                        End If
                        flag = False
                        For j = 0 To cnt_e - 1
                            sq_object = sq_can_escape(j)
                            If sq = sq_object Then
                                Continue For
                            End If
                            If IsCanEscape(bt, c, sq, pc, sq_opponent_king, sq_object, False) = False And IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                                mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 0)
                                flag = True
                            Else
                                flag = False
                                mate_move = 0
                                Exit For
                            End If
                        Next j
                        If flag = True And mate_move <> 0 Then
                            Return mate_move
                        End If
                    Else
                        If GoldSilver.Contains(pc) Then
                            If cnt_e = 0 Then
                                If IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                                    mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 0)
                                    Return mate_move
                                End If
                            End If
                            flag = False
                            For j = 0 To cnt_e - 1
                                sq_object = sq_can_escape(j)
                                If sq = sq_object Then
                                    Continue For
                                End If
                                If IsCanEscape(bt, c, sq, pc, sq_opponent_king, sq_object, False) = False And IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                                    mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 0)
                                    flag = True
                                Else
                                    flag = False
                                    mate_move = 0
                                    'Exit For
                                End If
                            Next j
                            If flag = True And mate_move <> 0 Then
                                Return mate_move
                            End If
                            'Exit For
                        End If
                    End If
                    ' ここでは成った方が得な場合でも香不成、歩不成で詰ます。
                    ' 複雑な心境だが、flag_promoの判定を入れると少し遅くなる。
                End If
                If pc > Piece.Rook Then
                    index += 1
                    Continue While
                End If
                pc_promote = pc + Promote
                ' knight promote move
                ' Knight cannnot mate opponent king from neighbour 8 Square.
                If pcs.Contains(pc_promote) And pc = Piece.Knight And (BB_Rev_Color_Position(c) And ABB_Mask(sq)) > 0 Then
                    If cnt_e = 0 Then
                        If IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False And (ABB_Piece_Attacks(c, Piece.Gold, sq) And ABB_Mask(sq_opponent_king)) > 0 Then

                            mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 1)
                            Return mate_move
                        End If
                    End If
                    flag = False
                    For j = 0 To cnt_e - 1
                        sq_object = sq_can_escape(j)
                        If sq = sq_object Then
                            Continue For
                        End If
                        If IsCanEscape(bt, c, sq, pc, sq_opponent_king, sq_object, True) = False And IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                            mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 1)
                            flag = True
                        Else
                            flag = False
                            mate_move = 0
                            Exit For
                        End If
                    Next j
                    If flag = True And mate_move <> 0 Then
                        Return mate_move
                    End If
                End If
                ' lance promote move Or pawn promote move
                If pcs.Contains(pc_promote) And ShortPieces.Contains(pc) And (BB_Rev_Color_Position(c) And ABB_Mask(sq)) > 0 Then
                    If cnt_e = 0 Then
                        If IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False And (ABB_Piece_Attacks(c, Piece.Gold, sq) And ABB_Mask(sq_opponent_king)) > 0 Then
                            mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 1)
                            Return mate_move
                        End If
                    End If
                    flag = False
                    For j = 0 To cnt_e - 1
                        sq_object = sq_can_escape(j)
                        If sq = sq_object Then
                            Continue For
                        End If
                        If IsCanEscape(bt, c, sq, pc, sq_opponent_king, sq_object, True) = False And IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                            flag = True
                            mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 1)

                        Else
                            flag = False
                            mate_move = 0
                            Exit For
                        End If
                    Next j
                    If flag = True And mate_move = 0 Then
                        mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 1)
                        Return mate_move
                    End If
                End If
                ' silver promote move
                If pc = Piece.Silver Then
                    If pcs.Contains(pc_promote) And (BB_Rev_Color_Position(c) And ABB_Mask(sq)) > 0 Or (BB_Rev_Color_Position(c) And ABB_Mask(pos)) > 0 Then
                        If cnt_e = 0 Then
                            If IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False And (ABB_Piece_Attacks(c, Piece.Gold, sq) And ABB_Mask(sq_opponent_king)) > 0 Then
                                mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 1)
                                Return mate_move
                            End If
                        End If
                        flag = False
                        For j = 0 To cnt_e - 1
                            sq_object = sq_can_escape(j)
                            If sq = sq_object Then
                                Continue For
                            End If
                            If IsCanEscape(bt, c, sq, pc, sq_opponent_king, sq_object, True) = False And IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                                mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 1)
                                flag = True
                            Else
                                flag = False
                                mate_move = 0
                                Exit For
                            End If
                        Next j
                        If flag = True And mate_move <> 0 Then
                            Return mate_move
                        End If
                    End If
                End If
                If pc < Piece.Bishop Then
                    index += 1
                    Continue While
                End If
                ' rook promote move or bishop promote move
                If pcs.Contains(pc_promote) And LongPieces.Contains(pc) And (BB_Rev_Color_Position(c) And ABB_Mask(sq)) > 0 Or (BB_Rev_Color_Position(c) And ABB_Mask(pos)) > 0 Then
                    If cnt_e = 0 Then
                        If IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False And (ABB_Piece_Attacks(c, Piece.King, sq) And ABB_Mask(sq_opponent_king)) > 0 Then
                            If (ABB_Mask(pos) And BB_Color_Position(opponent_color)) > 0 Or (ABB_Mask(sq) And BB_Color_Position(opponent_color)) > 0 Then
                                flag_promo = 1
                            Else
                                flag_promo = 0 ' この場合は成らないか？
                            End If
                            mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), flag_promo)
                            Return mate_move
                        End If
                    End If
                    flag = False
                    For j = 0 To cnt_e - 1
                        sq_object = sq_can_escape(j)
                        If sq = sq_object Then
                            Continue For
                        End If
                        If IsCanEscape(bt, c, sq, pc, sq_opponent_king, sq_object, True) = False And IsCanCapture(bt, c, opponent_color, sq, False, pos, pc) = False Then
                            If (ABB_Mask(pos) And BB_Color_Position(opponent_color)) > 0 Or (ABB_Mask(sq) And BB_Color_Position(opponent_color)) > 0 Then
                                flag_promo = 1
                            Else
                                flag_promo = 0 ' この場合は成らないか？
                            End If
                            mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), flag_promo)
                            flag = True
                        Else
                            flag = False
                            mate_move = 0
                            Exit For
                        End If
                    Next j
                    If flag And mate_move = 0 Then
                        Return mate_move
                    End If
                End If
                index += 1
            End While
        Next i
        ' You cannot mate opponnent king from neighbour 8 square.
        ' You maybe mate opponnent move using knight.
        pc = Piece.Knight
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        bb = ABB_Piece_Attacks(opponent_color, pc, sq_opponent_king) And (((Not bb_occupied) And BB_Full) Or bt.BB_Occupied(opponent_color))
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            bb_opponent_attacks_to_sq = AttacksToPiece(bt, sq, opponent_color)
            If (hand And Hand_Mask(pc)) > 0 And bt.Board(sq) = Piece.Empty And cnt_e = 0 And bb_opponent_attacks_to_sq = 0 Then
                ' drop knight
                mate_move = Pack(Square_NB + pc - 1, sq, pc, 0, 0)
                Return mate_move
            End If
            bb_my_knight_attacks = ABB_Piece_Attacks(opponent_color, pc, sq) And bt.BB_Piece(c, Piece.Knight)
            If bb_my_knight_attacks > 0 And cnt_e = 0 And bb_opponent_attacks_to_sq = 0 Then
                pos = Square(bb_my_knight_attacks)
                bb_my_knight_attacks = bb_my_knight_attacks Xor ABB_Mask(pos)
                If IsDiscoverKing2(bt, pos, sq, c, pc) = True Then
                    Continue While
                End If
                mate_move = Pack(pos, sq, pc, Math.Abs(bt.Board(sq)), 0)
            End If
        End While
        If mate_move <> 0 Then
            Return mate_move
        End If
        Return null_move
    End Function

    Public Function IsCanEscape(ByVal bt As BoardTree, ByVal c As Integer, ByVal sq_checker As Integer, ByVal pc_checker As Integer, ByVal sq_opponent_king As Integer, ByVal sq_object As Integer, ByVal is_promo As Boolean) As Boolean
        Dim bb_occupied As BitBoard
        Dim bb_attacks As BitBoard
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        bb_occupied = bb_occupied Xor (ABB_Mask(sq_opponent_king) Or ABB_Mask(sq_object))
        bb_attacks = 0
        Select Case pc_checker
            Case Piece.Rook
                bb_attacks = ABB_Cross_Attacks(sq_checker)(ABB_Cross_Mask_Ex(sq_checker) And bb_occupied)
            Case Piece.Dragon
                bb_attacks = ABB_Cross_Attacks(sq_checker)(ABB_Cross_Mask_Ex(sq_checker) And bb_occupied)
                bb_attacks = bb_attacks Or ABB_Piece_Attacks(c, Piece.King, sq_checker)
            Case Piece.Bishop
                bb_attacks = ABB_Diagonal_Attacks(sq_checker)(ABB_Diagonal_Mask_Ex(sq_checker) And bb_occupied)
            Case Piece.Horse
                bb_attacks = ABB_Diagonal_Attacks(sq_checker)(ABB_Diagonal_Mask_Ex(sq_checker) And bb_occupied)
                bb_attacks = bb_attacks Or ABB_Piece_Attacks(c, Piece.King, sq_checker)
            Case Piece.Pawn
                If is_promo = True Then
                    bb_attacks = ABB_Piece_Attacks(c, Piece.Gold, sq_checker)
                Else
                    bb_attacks = ABB_Piece_Attacks(c, pc_checker, sq_checker)
                End If
            Case Piece.Knight
                If is_promo = True Then
                    bb_attacks = ABB_Piece_Attacks(c, Piece.Gold, sq_checker)
                Else
                    bb_attacks = ABB_Piece_Attacks(c, pc_checker, sq_checker)
                End If
            Case Piece.Silver
                If is_promo = True Then
                    bb_attacks = ABB_Piece_Attacks(c, Piece.Gold, sq_checker)
                Else
                    bb_attacks = ABB_Piece_Attacks(c, pc_checker, sq_checker)
                End If
            Case Piece.Lance
                If is_promo = True Then
                    bb_attacks = ABB_Piece_Attacks(c, Piece.Gold, sq_checker)
                Else
                    bb_attacks = ABB_Lance_Attacks(c, sq_checker)(ABB_Lance_Mask_Ex(c, sq_checker) And bb_occupied)
                End If
            Case Else
                bb_attacks = ABB_Piece_Attacks(c, pc_checker, sq_checker)
        End Select
        bb_attacks = bb_attacks And ABB_Mask(sq_object)
        If bb_attacks > 0 Then
            Return False
        End If
        Return True
    End Function
    Public Function IsCanCapture(ByVal bt As BoardTree, ByVal c As Integer, ByVal opponent_color As Integer, ByVal sq_object As Integer, ByVal is_drop As Boolean, ByVal ifrom As Integer, ByVal ipiece As Integer) As Boolean
        Dim bb_myside_attacks As BitBoard
        Dim bb_opp_attacks As BitBoard
        Dim bb As BitBoard
        Dim bb2 As BitBoard
        Dim bb3 As BitBoard
        Dim myside_attacks_count As Integer
        Dim opp_attacks_count As Integer
        Dim idirec As Integer
        Dim pcs As Integer() = {Piece.Pawn, Piece.Lance, Piece.Rook, Piece.Dragon}
        bb_myside_attacks = AttacksToPiece(bt, sq_object, c)
        myside_attacks_count = BitBoard.PopCount(bb_myside_attacks)
        bb_opp_attacks = AttacksToPiece(bt, sq_object, opponent_color)
        opp_attacks_count = BitBoard.PopCount(bb_opp_attacks)
        If opp_attacks_count > 1 Then
            Return True
        End If
        If opp_attacks_count = 1 And myside_attacks_count = 0 Then
            '敵玉の利きのみだが、味方の駒が対象マスに利いていない場合
            Return True
        End If
        If opp_attacks_count >= myside_attacks_count Then
            If opp_attacks_count = myside_attacks_count And is_drop = True Then
                ' 敵玉の利きのみで、味方の駒の利きはひとつだが、駒打ち王手の場合
                Return False
            End If
            If is_drop = True Then
                Return True
            End If
            bt.BB_Occupied(c) = bt.BB_Occupied(c) Xor ABB_Mask(ifrom)
            bt.BB_Piece(c, ipiece) = bt.BB_Piece(c, ipiece) Xor ABB_Mask(ifrom)
            bb = IsAttacked(bt, bt.SQ_King(opponent_color), c)
            bb2 = IsAttacked(bt, bt.SQ_King(c), c)
            bb3 = 0
            If pcs.Contains(ipiece) Then
                idirec = Adirec(ifrom, sq_object)
                If Math.Abs(idirec) = Direction.Direc_File_U2d Then
                    bb3 = IsAttacked(bt, sq_object, c)
                End If
            End If
            bt.BB_Occupied(c) = bt.BB_Occupied(c) Xor ABB_Mask(ifrom)
            bt.BB_Piece(c, ipiece) = bt.BB_Piece(c, ipiece) Xor ABB_Mask(ifrom)
            If bb2 > 0 Then
                Return True
            End If
            If bb > 0 Or bb3 > 0 Then
                Return False
            End If
            Return True
        End If
        Return False
    End Function
End Module
