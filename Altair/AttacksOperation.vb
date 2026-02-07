Imports System.Net.NetworkInformation
Imports Altair.Common
Imports BitBoard = System.UInt128
Imports Key = System.UInt128
Imports Move = System.UInt32
Imports Rand = System.UInt128

Module AttacksOperation
    Public Function IsPinnedOnKing(ByVal bt As BoardTree, ByVal sq As Integer, ByVal idirec As Integer, ByVal c As Integer) As BitBoard
        Dim bb_occupied As BitBoard
        Dim bb_attacks As BitBoard
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        Select Case Math.Abs(idirec)
            Case Direction.Direc_File_U2d
                bb_attacks = ABB_File_Attacks(sq)(bb_occupied And ABB_File_Mask_Ex(sq))
                If (bb_attacks And ABB_Mask(bt.SQ_King(c))) > 0 Then
                    Return bb_attacks And (bt.BB_Piece(c Xor 1, Piece.Rook) Or bt.BB_Piece(c Xor 1, Piece.Dragon) Or bt.BB_Piece(c Xor 1, Piece.Lance))
                End If
            Case Direction.Direc_Rank_L2r
                bb_attacks = ABB_Rank_Attacks(sq)(bb_occupied And ABB_Rank_Mask_Ex(sq))
                If (bb_attacks And ABB_Mask(bt.SQ_King(c))) > 0 Then
                    Return bb_attacks And (bt.BB_Piece(c Xor 1, Piece.Rook) Or bt.BB_Piece(c Xor 1, Piece.Dragon))
                End If
            Case Direction.Direc_Diag1_U2d
                bb_attacks = ABB_Diag1_Attacks(sq)(bb_occupied And ABB_Diag1_Mask_Ex(sq))
                If (bb_attacks And ABB_Mask(bt.SQ_King(c))) > 0 Then
                    Return bb_attacks And (bt.BB_Piece(c Xor 1, Piece.Bishop) Or bt.BB_Piece(c Xor 1, Piece.Horse))
                End If
            Case Direction.Direc_Diag2_U2d
                bb_attacks = ABB_Diag2_Attacks(sq)(bb_occupied And ABB_Diag2_Mask_Ex(sq))
                If (bb_attacks And ABB_Mask(bt.SQ_King(c))) > 0 Then
                    Return bb_attacks And (bt.BB_Piece(c Xor 1, Piece.Bishop) Or bt.BB_Piece(c Xor 1, Piece.Horse))
                End If
        End Select
        Return 0
    End Function
    Public Function AttacksToPiece(ByVal bt As BoardTree, ByVal sq As Integer, ByVal c As Integer) As BitBoard
        Dim bb_ret As BitBoard
        Dim bb_occupied As BitBoard
        Dim bb_total_gold As BitBoard
        Dim bb_hdk As BitBoard
        Dim bb_bh As BitBoard
        Dim bb_rd As BitBoard
        Dim bb_lance_attacks As BitBoard
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        bb_ret = bt.BB_Piece(c, Piece.Pawn) And ABB_Piece_Attacks(c Xor 1, Piece.Pawn, sq)
        bb_ret = bb_ret Or (bt.BB_Piece(c, Piece.Knight) And ABB_Piece_Attacks(c Xor 1, Piece.Knight, sq))
        bb_ret = bb_ret Or (bt.BB_Piece(c, Piece.Silver) And ABB_Piece_Attacks(c Xor 1, Piece.Silver, sq))
        bb_total_gold = bt.BB_Piece(c, Piece.Gold) Or bt.BB_Piece(c, Piece.Pro_Pawn) Or bt.BB_Piece(c, Piece.Pro_Lance) Or bt.BB_Piece(c, Piece.Pro_Knight) Or bt.BB_Piece(c, Piece.Pro_Silver)
        bb_ret = bb_ret Or (bb_total_gold And ABB_Piece_Attacks(c Xor 1, Piece.Gold, sq))
        bb_hdk = bt.BB_Piece(c, Piece.Horse) Or bt.BB_Piece(c, Piece.Dragon) Or bt.BB_Piece(c, Piece.King)
        bb_ret = bb_ret Or (bb_hdk And ABB_Piece_Attacks(c Xor 1, Piece.King, sq))
        bb_bh = bt.BB_Piece(c, Piece.Bishop) Or bt.BB_Piece(c, Piece.Horse)
        bb_ret = bb_ret Or (bb_bh And ABB_Diagonal_Attacks(sq)(ABB_Diagonal_Mask_Ex(sq) And bb_occupied))
        bb_rd = bt.BB_Piece(c, Piece.Rook) Or bt.BB_Piece(c, Piece.Dragon)
        bb_ret = bb_ret Or (bb_rd And ABB_Cross_Attacks(sq)(ABB_Cross_Mask_Ex(sq) And bb_occupied))
        bb_lance_attacks = ABB_Lance_Attacks(c Xor 1, sq)(ABB_Lance_Mask_Ex(c Xor 1, sq) And bb_occupied)
        bb_ret = bb_ret Or bt.BB_Piece(c, Piece.Lance) And bb_lance_attacks
        Return bb_ret
    End Function
    Public Function AttacksToLongPiece(ByVal bt As BoardTree, ByVal sq As Integer, ByVal c As Integer) As BitBoard
        Dim bb_ret As BitBoard
        Dim bb_occupied As BitBoard
        Dim bb_bh As BitBoard
        Dim bb_rd As BitBoard
        Dim bb_lance_attacks As BitBoard
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        bb_bh = bt.BB_Piece(c, Piece.Bishop) Or bt.BB_Piece(c, Piece.Horse)
        bb_ret = bb_bh And ABB_Diagonal_Attacks(sq)(ABB_Diagonal_Mask_Ex(sq) And bb_occupied)
        bb_rd = bt.BB_Piece(c, Piece.Rook) Or bt.BB_Piece(c, Piece.Dragon)
        bb_ret = bb_ret Or (bb_rd And ABB_Cross_Attacks(sq)(ABB_Cross_Mask_Ex(sq) And bb_occupied))
        bb_lance_attacks = ABB_Lance_Attacks(c Xor 1, sq)(ABB_Lance_Mask_Ex(c Xor 1, sq) And bb_occupied)
        bb_ret = bb_ret Or bt.BB_Piece(c, Piece.Lance) And bb_lance_attacks
        Return bb_ret
    End Function
    Public Function IsMatePawnDrop(ByVal bt As BoardTree, ByVal sq_drop As Integer, ByVal c As Integer) As Boolean
        Dim bb_sum As BitBoard
        Dim bb_occupied As BitBoard
        Dim bb_total_gold As BitBoard
        Dim bb_bh As BitBoard
        Dim bb_rd As BitBoard
        Dim bb_hd As BitBoard
        Dim bb_move As BitBoard
        Dim ifrom As Integer
        Dim ito As Integer
        Dim iking As Integer
        Dim bret As Boolean
        If c = Color.White Then
            If (sq_drop - 9) >= 0 And bt.Board(sq_drop - 9) <> -Piece.King Then
                Return False
            End If
        Else
            If (sq_drop + 9) < Square_NB And bt.Board(sq_drop + 9) <> Piece.King Then
                Return False
            End If
        End If
        bb_sum = bt.BB_Piece(c, Piece.Knight) And ABB_Piece_Attacks(c Xor 1, Piece.Knight, sq_drop)
        bb_sum = bb_sum Or bt.BB_Piece(c, Piece.Silver) And ABB_Piece_Attacks(c Xor 1, Piece.Silver, sq_drop)
        bb_total_gold = bt.BB_Piece(c, Piece.Gold) Or bt.BB_Piece(c, Piece.Pro_Pawn) Or bt.BB_Piece(c, Piece.Pro_Lance) Or bt.BB_Piece(c, Piece.Pro_Knight) Or bt.BB_Piece(c, Piece.Pro_Silver)
        bb_sum = bb_sum Or (bb_total_gold And ABB_Piece_Attacks(c Xor 1, Piece.Gold, sq_drop))
        bb_occupied = bt.BB_Occupied(Common.Color.Black) Or bt.BB_Occupied(Color.White)
        bb_bh = bt.BB_Piece(c, Piece.Bishop) Or bt.BB_Piece(c, Piece.Horse)
        bb_sum = bb_sum Or (bb_bh And ABB_Diagonal_Attacks(sq_drop)(ABB_Diagonal_Mask_Ex(sq_drop) And bb_occupied))
        bb_rd = bt.BB_Piece(c, Piece.Rook) Or bt.BB_Piece(c, Piece.Dragon)
        bb_sum = bb_sum Or (bb_rd And ABB_Cross_Attacks(sq_drop)(ABB_Cross_Mask_Ex(sq_drop) And bb_occupied))
        bb_hd = bt.BB_Piece(c, Piece.Horse) Or bt.BB_Piece(c, Piece.Dragon)
        bb_sum = bb_sum Or (bb_hd And ABB_Piece_Attacks(c, Piece.King, sq_drop))
        While bb_sum > 0
            ifrom = Square(bb_sum)
            bb_sum = bb_sum Xor ABB_Mask(ifrom)
            If IsDiscoverKing(bt, ifrom, sq_drop, c) = True Then
                Continue While
            End If
            Return False
        End While
        iking = bt.SQ_King(c)
        bret = True
        bt.BB_Occupied(c Xor 1) = bt.BB_Occupied(c Xor 1) Xor ABB_Mask(sq_drop)
        bb_move = ABB_Piece_Attacks(c, Piece.King, iking) And ((Not bt.BB_Occupied(c)) And BB_Full)
        While bb_move > 0
            ito = Square(bb_move)
            If IsAttacked(bt, ito, c) = 0 Then
                bret = False
                Exit While
            End If
            bb_move = bb_move Xor ABB_Mask(ito)
        End While
        bt.BB_Occupied(c Xor 1) = bt.BB_Occupied(c Xor 1) Xor ABB_Mask(sq_drop)
        Return bret
    End Function
    Public Function IsAttacked(ByVal bt As BoardTree, ByVal sq As Integer, ByVal c As Integer) As BitBoard
        Dim bb_ret As BitBoard
        Dim bb_occupied As BitBoard
        Dim bb_total_gold As BitBoard
        Dim bb_hdk As BitBoard
        Dim bb_bh As BitBoard
        Dim bb_rd As BitBoard
        Dim bb_lance_attacks As BitBoard
        bb_ret = 0
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        If (sq + Delta_Table(c)) >= 0 And (sq + Delta_Table(c)) < Square_NB Then
            If (bt.Board(sq + Delta_Table(c)) = (Sign_Table(c) * Piece.Pawn)) Then
                bb_ret = ABB_Mask(sq + Delta_Table(c))
            End If
        End If
        bb_ret = bb_ret Or (bt.BB_Piece(c Xor 1, Piece.Knight) And ABB_Piece_Attacks(c, Piece.Knight, sq))
        bb_ret = bb_ret Or (bt.BB_Piece(c Xor 1, Piece.Silver) And ABB_Piece_Attacks(c, Piece.Silver, sq))
        bb_total_gold = bt.BB_Piece(c Xor 1, Piece.Gold) Or bt.BB_Piece(c Xor 1, Piece.Pro_Pawn) Or bt.BB_Piece(c Xor 1, Piece.Pro_Lance) Or bt.BB_Piece(c Xor 1, Piece.Pro_Knight) Or bt.BB_Piece(c Xor 1, Piece.Pro_Silver)
        bb_ret = bb_ret Or (bb_total_gold And ABB_Piece_Attacks(c, Piece.Gold, sq))
        bb_hdk = bt.BB_Piece(c Xor 1, Piece.Horse) Or bt.BB_Piece(c Xor 1, Piece.Dragon) Or bt.BB_Piece(c Xor 1, Piece.King)
        bb_ret = bb_ret Or (bb_hdk And ABB_Piece_Attacks(c Xor 1, Piece.King, sq))
        bb_bh = bt.BB_Piece(c Xor 1, Piece.Bishop) Or bt.BB_Piece(c Xor 1, Piece.Horse)
        bb_ret = bb_ret Or (bb_bh And ABB_Diagonal_Attacks(sq)(ABB_Diagonal_Mask_Ex(sq) And bb_occupied))
        bb_rd = bt.BB_Piece(c Xor 1, Piece.Rook) Or bt.BB_Piece(c Xor 1, Piece.Dragon)
        bb_ret = bb_ret Or (bb_rd And ABB_Cross_Attacks(sq)(ABB_Cross_Mask_Ex(sq) And bb_occupied))
        bb_lance_attacks = ABB_Lance_Attacks(c, sq)(ABB_Lance_Mask_Ex(c, sq) And bb_occupied)
        bb_ret = bb_ret Or (bt.BB_Piece(c Xor 1, Piece.Lance) And bb_lance_attacks)
        Return bb_ret
    End Function
    Public Function IsAttackedByLongPieces(ByVal bt As BoardTree, ByVal sq As Integer, ByVal c As Integer) As BitBoard
        Dim bb_ret As BitBoard
        Dim bb_occupied As BitBoard
        Dim bb_bh As BitBoard
        Dim bb_rd As BitBoard
        Dim bb_lance_attacks As BitBoard
        bb_ret = 0
        bb_occupied = bt.BB_Occupied(Color.Black) Or bt.BB_Occupied(Color.White)
        bb_bh = bt.BB_Piece(c Xor 1, Piece.Bishop) Or bt.BB_Piece(c Xor 1, Piece.Horse)
        bb_ret = bb_ret Or (bb_bh And ABB_Diagonal_Attacks(sq)(ABB_Diagonal_Mask_Ex(sq) And bb_occupied))
        bb_rd = bt.BB_Piece(c Xor 1, Piece.Rook) Or bt.BB_Piece(c Xor 1, Piece.Dragon)
        bb_ret = bb_ret Or (bb_rd And ABB_Cross_Attacks(sq)(ABB_Cross_Mask_Ex(sq) And bb_occupied))
        bb_lance_attacks = ABB_Lance_Attacks(c, sq)(ABB_Lance_Mask_Ex(c, sq) And bb_occupied)
        bb_ret = bb_ret Or (bt.BB_Piece(c Xor 1, Piece.Lance) And bb_lance_attacks)
        Return bb_ret
    End Function

    Public Function IsDiscoverKing(ByVal bt As BoardTree, ByVal ifrom As Integer, ByVal ito As Integer, ByVal c As Integer) As Boolean
        Dim idirec As Integer
        idirec = Adirec(bt.SQ_King(c), ifrom)
        If idirec <> Direction.Direc_Misc And idirec <> Adirec(bt.SQ_King(c), ito) And IsPinnedOnKing(bt, ifrom, idirec, c) <> 0 Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function IsDiscoverKing2(ByRef bt As BoardTree, ByVal ifrom As Integer, ByVal ito As Integer, ByVal c As Integer, ByVal ipiece As Integer) As Boolean
        Dim idirec As Integer
        idirec = Adirec(bt.SQ_King(c), ifrom)
        bt.BB_Piece(c, ipiece) = bt.BB_Piece(c, ipiece) Xor ABB_Mask(ifrom)
        bt.BB_Occupied(c) = bt.BB_Occupied(c) Xor ABB_Mask(ifrom)
        If idirec <> Direction.Direc_Misc And idirec <> Adirec(bt.SQ_King(c), ito) And IsPinnedOnKing(bt, ifrom, idirec, c) <> 0 Then
            bt.BB_Piece(c, ipiece) = bt.BB_Piece(c, ipiece) Xor ABB_Mask(ifrom)
            bt.BB_Occupied(c) = bt.BB_Occupied(c) Xor ABB_Mask(ifrom)
            Return True
        Else
            bt.BB_Piece(c, ipiece) = bt.BB_Piece(c, ipiece) Xor ABB_Mask(ifrom)
            bt.BB_Occupied(c) = bt.BB_Occupied(c) Xor ABB_Mask(ifrom)
            Return False
        End If
    End Function
End Module
