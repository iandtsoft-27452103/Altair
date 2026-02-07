Imports Altair.MoveModule
Imports BitBoard = System.UInt128
Imports Move = System.UInt32
Imports Rand = System.UInt128

Module Board
    Public Function Init() As BoardTree
        Dim bt As BoardTree
        Dim i As Integer
        bt.BB_Occupied = New BitBoard(Color_NB - 1) {}
        bt.BB_Occupied(Color.Black) = BB_Rank(6) Or BB_Rank(8) Or ABB_Mask(64) Or ABB_Mask(70)
        bt.BB_Occupied(Color.White) = BB_Rank(0) Or BB_Rank(2) Or ABB_Mask(10) Or ABB_Mask(16)
        bt.BB_Piece = New BitBoard(Color_NB - 1, Piece_NB - 1) {}
        bt.BB_Piece(Color.Black, Piece.Pawn) = BB_Rank(6)
        bt.BB_Piece(Color.Black, Piece.Lance) = ABB_Mask(72) Or ABB_Mask(80)
        bt.BB_Piece(Color.Black, Piece.Knight) = ABB_Mask(73) Or ABB_Mask(79)
        bt.BB_Piece(Color.Black, Piece.Silver) = ABB_Mask(74) Or ABB_Mask(78)
        bt.BB_Piece(Color.Black, Piece.Gold) = ABB_Mask(75) Or ABB_Mask(77)
        bt.BB_Piece(Color.Black, Piece.Bishop) = ABB_Mask(64)
        bt.BB_Piece(Color.Black, Piece.Rook) = ABB_Mask(70)
        bt.BB_Piece(Color.Black, Piece.King) = ABB_Mask(76)
        bt.BB_Piece(Color.White, Piece.Pawn) = BB_Rank(2)
        bt.BB_Piece(Color.White, Piece.Lance) = ABB_Mask(0) Or ABB_Mask(8)
        bt.BB_Piece(Color.White, Piece.Knight) = ABB_Mask(1) Or ABB_Mask(7)
        bt.BB_Piece(Color.White, Piece.Silver) = ABB_Mask(2) Or ABB_Mask(6)
        bt.BB_Piece(Color.White, Piece.Gold) = ABB_Mask(3) Or ABB_Mask(5)
        bt.BB_Piece(Color.White, Piece.Bishop) = ABB_Mask(16)
        bt.BB_Piece(Color.White, Piece.Rook) = ABB_Mask(10)
        bt.BB_Piece(Color.White, Piece.King) = ABB_Mask(4)
        bt.SQ_King = New Integer(Color_NB - 1) {}
        bt.SQ_King(Color.Black) = 76
        bt.SQ_King(Color.White) = 4
        bt.Hand = New Integer(Color_NB - 1) {}
        bt.Hand(Color.Black) = 0
        bt.Hand(Color.White) = 0
        bt.Board = New Integer(Square_NB - 1) {}
        bt.Board(0) = -Piece.Lance
        bt.Board(8) = -Piece.Lance
        bt.Board(1) = -Piece.Knight
        bt.Board(7) = -Piece.Knight
        bt.Board(2) = -Piece.Silver
        bt.Board(6) = -Piece.Silver
        bt.Board(3) = -Piece.Gold
        bt.Board(5) = -Piece.Gold
        bt.Board(4) = -Piece.King
        bt.Board(10) = -Piece.Rook
        bt.Board(16) = -Piece.Bishop
        For i = 18 To 26
            bt.Board(i) = -Piece.Pawn
        Next i
        bt.Board(72) = Piece.Lance
        bt.Board(80) = Piece.Lance
        bt.Board(73) = Piece.Knight
        bt.Board(79) = Piece.Knight
        bt.Board(74) = Piece.Silver
        bt.Board(78) = Piece.Silver
        bt.Board(75) = Piece.Gold
        bt.Board(77) = Piece.Gold
        bt.Board(76) = Piece.King
        bt.Board(64) = Piece.Bishop
        bt.Board(70) = Piece.Rook
        For i = 54 To 62
            bt.Board(i) = Piece.Pawn
        Next i
        bt.RootColor = Color.Black
        bt.Ply = 1
        bt.PrevHash = 0
        bt.EvalArray = New Integer(Ply_Max - 1) {}
        bt.RootMoves = New Move(Moves_Max - 1) {} '後で変更するかもしれない。
        bt.Hash = New Rand(Ply_Max - 1) {}
        bt.CurrentHash = HashFunc(bt)
        bt.Hash(1) = bt.CurrentHash
        Return bt
    End Function

    Public Sub Clear(ByRef BTree As BoardTree)
        Dim c As Integer
        Dim pc As Integer
        Dim ply As Integer
        BTree.BB_Occupied(Color.Black) = 0
        BTree.BB_Occupied(Color.White) = 0
        For c = Color.Black To Color.White
            For pc = Piece.Pawn To Piece.Dragon
                BTree.BB_Piece(c, pc) = 0
            Next pc
        Next c
        BTree.Hand(Color.Black) = 0
        BTree.Hand(Color.White) = 0
        BTree.CurrentHash = HashFunc(BTree)
        BTree.RootColor = Color.Black
        BTree.SQ_King(Color.Black) = 0
        BTree.SQ_King(Color.White) = 0
        BTree.Ply = 1
        BTree.PrevHash = 0
        For ply = 0 To Ply_Max - 1
            BTree.EvalArray(0) = 0
        Next ply
    End Sub

    Public Function DeepCopy(ByVal bt As BoardTree, ByVal flag As Boolean) As BoardTree
        Dim bt_base As BoardTree
        Dim c As Integer
        Dim pc As Integer
        Dim sq As Integer
        Dim ply As Integer
        Dim i As Integer
        bt_base = Init()
        For c = Color.Black To Color.White
            bt_base.BB_Occupied(c) = bt.BB_Occupied(c)
            bt_base.SQ_King(c) = bt.SQ_King(c)
            For pc = 0 To Piece_NB - 1
                bt_base.BB_Piece(c, pc) = bt.BB_Piece(c, pc)
            Next pc
            bt_base.Hand(c) = bt.Hand(c)
        Next c
        bt_base.RootColor = bt.RootColor
        bt_base.Ply = bt.Ply
        bt_base.CurrentHash = bt.CurrentHash
        bt_base.PrevHash = bt.PrevHash
        For sq = 0 To Square_NB - 1
            bt_base.Board(sq) = bt.Board(sq)
        Next sq
        For ply = 0 To Ply_Max - 1
            If ply <> 0 And bt.Hash(ply) = 0 Then
                Exit For
            End If
            bt_base.Hash(ply) = bt.Hash(ply)
            bt_base.EvalArray(ply) = bt.EvalArray(ply)
        Next ply
        If flag = True Then
            i = 0
            bt_base.RootMoves = New Move(Moves_Max - 1) {}
            While bt.RootMoves(i) <> 0
                bt_base.RootMoves(i) = bt.RootMoves(i)
                i += 1
            End While
        End If
        Return bt_base
    End Function

    Public Sub DoMove(ByRef bt As BoardTree, ByVal m As Move, ByVal color As Integer)
        Dim ifrom As Integer
        Dim ito As Integer
        Dim ipiece As Integer
        Dim is_promote As Integer
        Dim icap_piece As Integer
        Dim index As Integer
        Dim bb_set_clear As BitBoard
        bt.PrevHash = bt.CurrentHash
        ifrom = GetFrom(m)
        ito = GetTo(m)
        ipiece = GetPiece(m)
        is_promote = IsPromote(m)
        If ifrom >= Square_NB Then
            bt.BB_Piece(color, ipiece) = bt.BB_Piece(color, ipiece) Xor ABB_Mask(ito)
            bt.CurrentHash = bt.CurrentHash Xor PieceRand(color, ipiece, ito)
            bt.Hand(color) -= Hand_Hash(ipiece)
            bt.Board(ito) = -Sign_Table(color) * ipiece
            bt.BB_Occupied(color) = bt.BB_Occupied(color) Xor ABB_Mask(ito)
        Else
            bb_set_clear = ABB_Mask(ifrom) Or ABB_Mask(ito)
            bt.BB_Occupied(color) = bt.BB_Occupied(color) Xor bb_set_clear
            bt.Board(ifrom) = Piece.Empty
            If is_promote <> 0 Then
                bt.BB_Piece(color, ipiece) = bt.BB_Piece(color, ipiece) Xor ABB_Mask(ifrom)
                bt.BB_Piece(color, ipiece + Promote) = bt.BB_Piece(color, ipiece + Promote) Xor ABB_Mask(ito)
                bt.CurrentHash = bt.CurrentHash Xor PieceRand(color, ipiece, ifrom) Xor PieceRand(color, ipiece + Promote, ito)
                bt.Board(ito) = -Sign_Table(color) * (ipiece + Promote)
            Else
                If ipiece = Piece.King Then
                    bt.SQ_King(color) = ito
                End If
                bt.BB_Piece(color, ipiece) = bt.BB_Piece(color, ipiece) Xor bb_set_clear
                bt.CurrentHash = bt.CurrentHash Xor PieceRand(color, ipiece, ifrom) Xor PieceRand(color, ipiece, ito)
                bt.Board(ito) = -Sign_Table(color) * ipiece
            End If
            icap_piece = GetCapPiece(m)
            index = icap_piece
            If icap_piece > 0 Then
                If icap_piece > Piece.King Then
                    index -= Promote
                End If
                bt.Hand(color) += Hand_Hash(index)
                bt.BB_Piece(color Xor 1, icap_piece) = bt.BB_Piece((color Xor 1), icap_piece) Xor ABB_Mask(ito)
                bt.CurrentHash = bt.CurrentHash Xor PieceRand(color Xor 1, icap_piece, ito)
                bt.BB_Occupied(color Xor 1) = bt.BB_Occupied(color Xor 1) Xor ABB_Mask(ito)
            End If
        End If
        bt.Hash(bt.Ply) = bt.PrevHash
        bt.Hash(bt.Ply + 1) = bt.CurrentHash
        bt.Ply += 1
    End Sub
    Public Sub UnDoMove(ByRef bt As BoardTree, ByVal m As Move, ByVal color As Integer)
        Dim ifrom As Integer
        Dim ito As Integer
        Dim ipiece As Integer
        Dim is_promote As Integer
        Dim icap_piece As Integer
        Dim index As Integer
        Dim bb_set_clear As BitBoard
        bt.CurrentHash = bt.PrevHash
        ifrom = GetFrom(m)
        ito = GetTo(m)
        ipiece = GetPiece(m)
        is_promote = IsPromote(m)
        If ifrom >= Square_NB Then
            bt.BB_Piece(color, ipiece) = bt.BB_Piece(color, ipiece) Xor ABB_Mask(ito)
            bt.Hand(color) += Hand_Hash(ipiece)
            bt.Board(ito) = Piece.Empty
            bt.BB_Occupied(color) = bt.BB_Occupied(color) Xor ABB_Mask(ito)
        Else
            bb_set_clear = ABB_Mask(ifrom) Or ABB_Mask(ito)
            bt.BB_Occupied(color) = bt.BB_Occupied(color) Xor bb_set_clear
            bt.Board(ifrom) = -Sign_Table(color) * ipiece
            If is_promote > 0 Then
                bt.BB_Piece(color, ipiece) = bt.BB_Piece(color, ipiece) Xor ABB_Mask(ifrom)
                bt.BB_Piece(color, ipiece + Promote) = bt.BB_Piece(color, ipiece + Promote) Xor ABB_Mask(ito)
            Else
                If ipiece = Piece.King Then
                    bt.SQ_King(color) = ifrom
                End If
                bt.BB_Piece(color, ipiece) = bt.BB_Piece(color, ipiece) Xor bb_set_clear
            End If
            icap_piece = GetCapPiece(m)
            index = icap_piece
            If icap_piece > 0 Then
                If icap_piece > Piece.King Then
                    index -= Promote
                End If
                bt.Hand(color) -= Hand_Hash(index)
                bt.BB_Piece(color Xor 1, icap_piece) = bt.BB_Piece(color Xor 1, icap_piece) Xor ABB_Mask(ito)
                bt.BB_Occupied(color Xor 1) = bt.BB_Occupied(color Xor 1) Xor ABB_Mask(ito)
                bt.Board(ito) = Sign_Table(color) * icap_piece
            Else
                bt.Board(ito) = Piece.Empty
            End If
        End If
        bt.PrevHash = bt.Hash(bt.Ply - 2)
        bt.Hash(bt.Ply) = 0
        bt.Ply -= 1
    End Sub
    Public Sub DoNullMove(ByRef bt As BoardTree)
        bt.Hash(bt.Ply + 1) = bt.CurrentHash
        bt.Ply += 1
    End Sub
    Public Sub UnDoNullMove(ByRef bt As BoardTree)
        bt.Hash(bt.Ply) = 0
        bt.Ply -= 1
    End Sub

    ' 戻り値
    ' 0: 宣言勝ちの局面ではない。
    ' 1: 先手の勝ち
    ' 2: 後手の勝ち
    'b_tekijin_piece_count, w_tekijin_piece_count
    Public Function IsDeclarationWin(ByVal bt As BoardTree) As Integer
        Dim bb0 As BitBoard
        Dim bb1 As BitBoard
        Dim bb_object As BitBoard
        Dim bb_temp As BitBoard
        Dim i As Integer
        Dim black_score As Integer
        Dim white_score As Integer
        Dim b_tekijin_piece_count As Integer
        Dim w_tekijin_piece_count As Integer
        Dim b_hand_piece_count As Integer()
        Dim w_hand_piece_count As Integer()
        Dim b_board_piece_count As Integer()
        Dim w_board_piece_count As Integer()
        black_score = 0
        white_score = 0
        b_tekijin_piece_count = 0
        w_tekijin_piece_count = 0
        b_hand_piece_count = New Integer(Piece.Rook + 1) {}
        w_hand_piece_count = New Integer(Piece.Rook + 1) {}
        b_board_piece_count = New Integer(Piece_NB) {}
        w_board_piece_count = New Integer(Piece_NB) {}
        bb0 = bt.BB_Piece(Color.Black, Piece.King) And BB_White_Position
        bb1 = bt.BB_Piece(Color.White, Piece.King) And BB_Black_Position
        If bb0 = 0 And bb1 = 0 Then
            Return 0
        End If
        If bb0 > 0 Then
            For i = Piece.Pawn To Piece.Rook
                b_hand_piece_count(i) = (bt.Hand(Color.Black) And Hand_Mask(i)) >> Hand_Rev_Bit(i)
                If i >= Piece.Bishop Then
                    black_score += 5 * b_hand_piece_count(i)
                Else
                    black_score += b_hand_piece_count(i)
                End If
            Next i
            For i = Piece.Pawn To Piece.Dragon
                If i = Piece.None Then
                    Continue For
                End If
                bb_object = bt.BB_Piece(Color.Black, i) And BB_Rev_Color_Position(Color.Black)
                b_board_piece_count(i) = BitBoard.PopCount(bb_object)
                b_tekijin_piece_count += b_board_piece_count(i)
                bb_temp = BB_DMZ Or BB_Rev_Color_Position(Color.White)
                bb_object = bb_temp And bt.BB_Piece(Color.Black, i)
                b_board_piece_count(i) += BitBoard.PopCount(bb_object)
                If i = Piece.King Then
                    Continue For
                End If
                If i = Piece.Bishop Or i = Piece.Rook Or i >= Piece.Horse Then
                    black_score += 5 * b_board_piece_count(i)
                Else
                    black_score += b_board_piece_count(i)
                End If
            Next i
        End If
        If bb1 > 0 Then
            For i = Piece.Pawn To Piece.Rook
                w_hand_piece_count(i) = (bt.Hand(Color.White) And Hand_Mask(i)) >> Hand_Rev_Bit(i)
                If i > Piece.Bishop Then
                    white_score += 5 * w_hand_piece_count(i)
                Else
                    white_score += w_hand_piece_count(i)
                End If
            Next i
            For i = Piece.Pawn To Piece.Dragon
                If i = Piece.None Then
                    Continue For
                End If
                bb_object = bt.BB_Piece(Color.White, i) And BB_Rev_Color_Position(Color.White)
                w_board_piece_count(i) = BitBoard.PopCount(bb_object)
                w_tekijin_piece_count += w_board_piece_count(i)
                bb_temp = BB_DMZ Or BB_Rev_Color_Position(Color.Black)
                bb_object = bb_temp And bt.BB_Piece(Color.White, i)
                w_board_piece_count(i) += BitBoard.PopCount(bb_object)
                If i = Piece.King Then
                    Continue For
                End If
                If i = Piece.Bishop Or i = Piece.Rook Or i >= Piece.Horse Then
                    white_score += 5 * w_board_piece_count(i)
                Else
                    white_score += w_board_piece_count(i)
                End If
            Next i
        End If
        If bb0 > 0 And black_score >= 28 And b_tekijin_piece_count >= 10 Then
            Return 1
        End If
        If bb1 > 0 And white_score >= 27 And w_tekijin_piece_count >= 10 Then
            Return 2
        End If
        Return 0
    End Function
    Public Function IsRepetition(ByVal bt As BoardTree, ByVal tt As TT) As Integer
        Dim i As Integer
        Dim counter As Integer
        Dim limit As Integer
        Dim b As Boolean
        limit = bt.Ply - 12
        If limit < 1 Then
            Return 0
        End If
        counter = 0
        i = bt.Ply
        While i >= limit
            If bt.CurrentHash = bt.Hash(i) Then
                counter += 1
            End If
            i -= 1
        End While
        '手抜きのため、同一局面3回で千日手と判定する。<= Idea from Apery C++ Ver.
        If counter > 2 Then
            If tt.is_check.ContainsKey(bt.CurrentHash) Then
                b = tt.is_check(bt.CurrentHash)
                If b = False Then
                    '千日手
                    Return 1
                Else
                    '// 連続王手の千日手
                    Return 2
                End If
            End If
        End If
        Return 0
    End Function
End Module
