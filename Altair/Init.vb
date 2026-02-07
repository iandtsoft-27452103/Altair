Imports Altair.Common
Imports System.Math
Imports System.IO
Imports BitBoard = System.UInt128
Imports Key = System.UInt128
Module Init

    Public Sub Init()
        InitPieceAttacks()
        InitObs()
        InitLongAttacks()
        InitPieceTable()
    End Sub

    Private Sub InitPieceAttacks()
        Dim color As Integer
        Dim pc As Integer
        Dim ifrom As Integer
        ABB_Piece_Attacks = New BitBoard(Color_NB - 1, Piece_NB - 1, Square_NB - 1) {}
        color = Common.Color.Black
        For pc = Piece.Pawn To Piece.Dragon
            For ifrom = 0 To Square_NB - 1
                Select Case pc
                    Case Piece.Pawn
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                    Case Piece.Knight
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Knight_L_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Knight_R_D2u)
                    Case Piece.Silver
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_U2d)
                    Case Piece.Gold
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.Pro_Pawn
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.Pro_Lance
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.Pro_Knight
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.Pro_Silver
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.King
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_U2d)
                End Select
            Next ifrom
        Next pc
        color = Common.Color.White
        For pc = Piece.Pawn To Piece.Dragon
            For ifrom = 0 To Square_NB - 1
                Select Case pc
                    Case Piece.Pawn
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                    Case Piece.Knight
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Knight_L_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Knight_R_U2d)
                    Case Piece.Silver
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_U2d)
                    Case Piece.Gold
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.Pro_Pawn
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.Pro_Lance
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.Pro_Knight
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.Pro_Silver
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                    Case Piece.King
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_File_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_D2u)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_L2r)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Rank_R2l)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag1_U2d)
                        SetAttacks(color, pc, ifrom, ifrom + Direction.Direc_Diag2_U2d)
                End Select
            Next ifrom
        Next pc
    End Sub

    Private Sub SetAttacks(ByVal color As Integer, ByVal pc As Integer, ByVal ifrom As Integer, ByVal ito As Integer)
        Dim rd As Integer
        Dim r As Integer
        Dim f As Integer
        Dim fd As Integer
        If ifrom >= 0 And ifrom < Square_NB And ito >= 0 And ito < Square_NB Then
            r = RankTable(ifrom) - RankTable(ito)
            rd = Abs(r)
            If pc = Piece.Knight Then
                If rd <> 2 Then
                    Return
                End If
            Else
                If rd <> 0 And rd <> 1 Then
                    Return
                End If
                f = FileTable(ifrom) - FileTable(ito)
                fd = Abs(f)
                If fd <> 0 And fd <> 1 Then
                    Return
                End If
            End If
        End If
        If ifrom >= 0 And ifrom < Square_NB And ito >= 0 And ito < Square_NB Then
            ABB_Piece_Attacks(color, pc, ifrom) = ABB_Piece_Attacks(color, pc, ifrom) Or ABB_Mask(ito)
        End If
    End Sub

    Private Sub InitObs()
        Dim ifrom As Integer
        Dim ito As Integer
        Dim dist As Integer
        Dim delta As Integer
        Dim i As Integer
        Dim d As Direction
        Dim bb As BitBoard
        ABB_Obstacles = New BitBoard(Square_NB - 1, Square_NB - 1) {}
        For ifrom = 0 To Square_NB - 1
            For ito = 0 To Square_NB - 1
                d = Adirec(ifrom, ito)
                bb = 0
                Select Case d
                    Case Direction.Direc_Rank_L2r
                        If FileTable(ifrom) = File.File9 Then
                            Continue For
                        End If
                        dist = Abs(ifrom - ito)
                        If dist >= 2 Then
                            For i = 0 To dist - 2
                                delta = (i + 1) * Direction.Direc_Rank_L2r
                                bb = ABB_Mask(ifrom + delta) Or bb
                            Next i
                            ABB_Obstacles(ifrom, ito) = bb
                        End If
                    Case Direction.Direc_Rank_R2l
                        If FileTable(ifrom) = File.File1 Then
                            Continue For
                        End If
                        dist = Abs(ifrom - ito)
                        If dist >= 2 Then
                            For i = 0 To dist - 2
                                delta = (i + 1) * Direction.Direc_Rank_R2l
                                bb = ABB_Mask(ifrom + delta) Or bb
                            Next i
                            ABB_Obstacles(ifrom, ito) = bb
                        End If
                    Case Direction.Direc_File_U2d
                        If RankTable(ifrom) = Rank.Rank9 Then
                            Continue For
                        End If
                        dist = Abs(ifrom - ito)
                        If dist >= 18 Then
                            dist = dist / 9
                            For i = 0 To dist - 2
                                delta = (i + 1) * Direction.Direc_File_U2d
                                bb = ABB_Mask(ifrom + delta) Or bb
                            Next i
                            ABB_Obstacles(ifrom, ito) = bb
                        End If
                    Case Direction.Direc_File_D2u
                        If RankTable(ifrom) = Rank.Rank1 Then
                            Continue For
                        End If
                        dist = Abs(ifrom - ito)
                        If dist >= 18 Then
                            dist = dist / 9
                            For i = 0 To dist - 2
                                delta = (i + 1) * Direction.Direc_File_D2u
                                bb = ABB_Mask(ifrom + delta) Or bb
                            Next i
                            ABB_Obstacles(ifrom, ito) = bb
                        End If
                    Case Direction.Direc_Diag1_U2d
                        If FileTable(ifrom) = File.File1 Or RankTable(ifrom) = Rank.Rank9 Then
                            Continue For
                        End If
                        dist = Abs(ifrom - ito)
                        If dist >= 16 Then
                            dist = dist / 8
                            For i = 0 To dist - 2
                                delta = (i + 1) * Direction.Direc_Diag1_U2d
                                bb = ABB_Mask(ifrom + delta) Or bb
                            Next i
                            ABB_Obstacles(ifrom, ito) = bb
                        End If
                    Case Direction.Direc_Diag1_D2u
                        If FileTable(ifrom) = File.File9 Or RankTable(ifrom) = Rank.Rank1 Then
                            Continue For
                        End If
                        dist = Abs(ifrom - ito)
                        If dist >= 16 Then
                            dist = dist / 8
                            For i = 0 To dist - 2
                                delta = (i + 1) * Direction.Direc_Diag1_D2u
                                bb = ABB_Mask(ifrom + delta) Or bb
                            Next i
                            ABB_Obstacles(ifrom, ito) = bb
                        End If
                    Case Direction.Direc_Diag2_U2d
                        If FileTable(ifrom) = File.File9 Or RankTable(ifrom) = Rank.Rank9 Then
                            Continue For
                        End If
                        dist = Abs(ifrom - ito)
                        If dist >= 20 Then
                            dist = dist / 10
                            For i = 0 To dist - 2
                                delta = (i + 1) * Direction.Direc_Diag2_U2d
                                bb = ABB_Mask(ifrom + delta) Or bb
                            Next i
                            ABB_Obstacles(ifrom, ito) = bb
                        End If
                    Case Direction.Direc_Diag2_D2u
                        If FileTable(ifrom) = File.File1 Or RankTable(ifrom) = Rank.Rank1 Then
                            Continue For
                        End If
                        dist = Abs(ifrom - ito)
                        If dist >= 20 Then
                            dist = dist / 10
                            For i = 0 To dist - 2
                                delta = (i + 1) * Direction.Direc_Diag2_D2u
                                bb = ABB_Mask(ifrom + delta) Or bb
                            Next i
                            ABB_Obstacles(ifrom, ito) = bb
                        End If
                End Select
            Next ito
        Next ifrom
    End Sub

    Private Sub InitLongAttacks()
        Dim s As IEnumerable(Of String) = System.IO.File.ReadLines("abb_file_mask_ex.txt")
        Dim ss As String() = s.ToArray()
        Dim sss As String()
        Dim limit As Integer = ss.Length
        Dim i As Integer
        Dim c As Integer
        Dim sq As Integer
        Dim key As BitBoard
        Dim value As BitBoard
        Dim f As Integer
        ABB_File_Mask_Ex = New BitBoard(Square_NB - 1) {}
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            ABB_File_Mask_Ex(i) = BitBoard.Parse(sss(1))
        Next i
        s = System.IO.File.ReadLines("abb_rank_mask_ex.txt")
        ss = s.ToArray()
        ABB_Rank_Mask_Ex = New BitBoard(Square_NB - 1) {}
        limit = ss.Length
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            ABB_Rank_Mask_Ex(i) = BitBoard.Parse(sss(1))
        Next i
        s = System.IO.File.ReadLines("abb_diag1_mask_ex.txt")
        ss = s.ToArray()
        ABB_Diag1_Mask_Ex = New BitBoard(Square_NB - 1) {}
        limit = ss.Length
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            ABB_Diag1_Mask_Ex(i) = BitBoard.Parse(sss(1))
        Next i
        s = System.IO.File.ReadLines("abb_diag2_mask_ex.txt")
        ss = s.ToArray()
        ABB_Diag2_Mask_Ex = New BitBoard(Square_NB - 1) {}
        limit = ss.Length
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            ABB_Diag2_Mask_Ex(i) = BitBoard.Parse(sss(1))
        Next i
        s = System.IO.File.ReadLines("abb_cross_mask_ex.txt")
        ss = s.ToArray()
        ABB_Cross_Mask_Ex = New BitBoard(Square_NB - 1) {}
        limit = ss.Length
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            ABB_Cross_Mask_Ex(i) = BitBoard.Parse(sss(1))
        Next i
        s = System.IO.File.ReadLines("abb_diagonal_mask_ex.txt")
        ss = s.ToArray()
        ABB_Diagonal_Mask_Ex = New BitBoard(Square_NB - 1) {}
        limit = ss.Length
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            ABB_Diagonal_Mask_Ex(i) = BitBoard.Parse(sss(1))
        Next i
        s = System.IO.File.ReadLines("abb_file_attacks.txt")
        ss = s.ToArray()
        limit = ss.Length
        ABB_File_Attacks = New Dictionary(Of BitBoard, BitBoard)(Square_NB - 1) {}
        For i = 0 To Square_NB - 1
            ABB_File_Attacks(i) = New Dictionary(Of BitBoard, BitBoard)()
        Next i
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            sq = Integer.Parse(sss(0)) - 1
            key = BitBoard.Parse(sss(1))
            value = BitBoard.Parse(sss(2))
            ABB_File_Attacks(sq)(key) = value
        Next i
        s = System.IO.File.ReadLines("abb_rank_attacks.txt")
        ss = s.ToArray()
        limit = ss.Length
        ABB_Rank_Attacks = New Dictionary(Of BitBoard, BitBoard)(Square_NB - 1) {}
        For i = 0 To Square_NB - 1
            ABB_Rank_Attacks(i) = New Dictionary(Of BitBoard, BitBoard)()
        Next i
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            sq = Integer.Parse(sss(0)) - 1
            key = BitBoard.Parse(sss(1))
            value = BitBoard.Parse(sss(2))
            ABB_Rank_Attacks(sq)(key) = value
        Next i
        s = System.IO.File.ReadLines("abb_diag1_attacks.txt")
        ss = s.ToArray()
        limit = ss.Length
        ABB_Diag1_Attacks = New Dictionary(Of BitBoard, BitBoard)(Square_NB - 1) {}
        For i = 0 To Square_NB - 1
            ABB_Diag1_Attacks(i) = New Dictionary(Of BitBoard, BitBoard)()
        Next i
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            sq = Integer.Parse(sss(0)) - 1
            key = BitBoard.Parse(sss(1))
            value = BitBoard.Parse(sss(2))
            ABB_Diag1_Attacks(sq)(key) = value
        Next i
        s = System.IO.File.ReadLines("abb_diag2_attacks.txt")
        ss = s.ToArray()
        limit = ss.Length
        ABB_Diag2_Attacks = New Dictionary(Of BitBoard, BitBoard)(Square_NB - 1) {}
        For i = 0 To Square_NB - 1
            ABB_Diag2_Attacks(i) = New Dictionary(Of BitBoard, BitBoard)()
        Next i
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            sq = Integer.Parse(sss(0)) - 1
            key = BitBoard.Parse(sss(1))
            value = BitBoard.Parse(sss(2))
            ABB_Diag2_Attacks(sq)(key) = value
        Next i
        s = System.IO.File.ReadLines("abb_cross_attacks.txt")
        ss = s.ToArray()
        limit = ss.Length
        ABB_Cross_Attacks = New Dictionary(Of BitBoard, BitBoard)(Square_NB - 1) {}
        For i = 0 To Square_NB - 1
            ABB_Cross_Attacks(i) = New Dictionary(Of BitBoard, BitBoard)()
        Next i
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            sq = Integer.Parse(sss(0)) - 1
            key = BitBoard.Parse(sss(1))
            value = BitBoard.Parse(sss(2))
            ABB_Cross_Attacks(sq)(key) = value
        Next i
        s = System.IO.File.ReadLines("abb_diagonal_attacks.txt")
        ss = s.ToArray()
        limit = ss.Length
        ABB_Diagonal_Attacks = New Dictionary(Of BitBoard, BitBoard)(Square_NB - 1) {}
        For i = 0 To Square_NB - 1
            ABB_Diagonal_Attacks(i) = New Dictionary(Of BitBoard, BitBoard)()
        Next i
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            sq = Integer.Parse(sss(0)) - 1
            key = BitBoard.Parse(sss(1))
            value = BitBoard.Parse(sss(2))
            ABB_Diagonal_Attacks(sq)(key) = value
        Next i
        s = System.IO.File.ReadLines("abb_lance_mask_ex.txt")
        ss = s.ToArray()
        ABB_Lance_Mask_Ex = New BitBoard(Color_NB, Square_NB - 1) {}
        limit = ss.Length
        For i = 0 To Square_NB - 1
            sss = ss(i).Split(",")
            ABB_Lance_Mask_Ex(0, i) = BitBoard.Parse(sss(2))
        Next i
        For i = 0 To Square_NB - 1
            sss = ss(i + Square_NB).Split(",")
            ABB_Lance_Mask_Ex(1, i) = BitBoard.Parse(sss(2))
        Next i
        s = System.IO.File.ReadLines("abb_lance_attacks.txt")
        ss = s.ToArray()
        limit = ss.Length
        ABB_Lance_Attacks = New Dictionary(Of BitBoard, BitBoard)(Color_NB, Square_NB - 1) {}
        For i = 0 To Square_NB - 1
            ABB_Lance_Attacks(0, i) = New Dictionary(Of BitBoard, BitBoard)()
            ABB_Lance_Attacks(1, i) = New Dictionary(Of BitBoard, BitBoard)()
        Next i
        For i = 0 To limit - 1
            sss = ss(i).Split(",")
            c = Integer.Parse(sss(0)) - 1
            sq = Integer.Parse(sss(1)) - 1
            key = BitBoard.Parse(sss(2))
            value = BitBoard.Parse(sss(3))
            ABB_Lance_Attacks(c, sq)(key) = value
        Next i
        ABB_Stomach_Attacks = New BitBoard(Square_NB) {}
        For i = 0 To Square_NB - 1
            f = FileTable(i)
            If f = File.File1 Then
                value = ABB_Mask(i + 1)
            ElseIf f = File.File9 Then
                value = ABB_Mask(i - 1)
            Else
                value = ABB_Mask(i - 1) Or ABB_Mask(i + 1)
            End If
            ABB_Stomach_Attacks(i) = value
        Next i
        ABB_2Up_Attacks = New BitBoard(Color_NB, Square_NB) {}
        c = Color.Black
        For i = 0 To Square_NB - 1
            If i >= 0 And i < 18 Then
                value = 0
            Else
                value = ABB_Mask(i - 18)
            End If
            ABB_2Up_Attacks(c, i) = value
        Next i
        c = Color.White
        For i = 0 To Square_NB - 1
            If i >= 63 And i < Square_NB Then
                value = 0
            Else
                value = ABB_Mask(i + 18)
            End If
            ABB_2Up_Attacks(c, i) = value
        Next i
        ABB_3Up_Attacks = New BitBoard(Color_NB, Square_NB) {}
        c = Color.Black
        For i = 0 To Square_NB - 1
            If i >= 0 And i < 27 Then
                value = 0
            Else
                value = ABB_Mask(i - 27)
            End If
            ABB_3Up_Attacks(c, i) = value
        Next i
        c = Color.White
        For i = 0 To Square_NB - 1
            If i >= 54 And i < Square_NB Then
                value = 0
            Else
                value = ABB_Mask(i + 27)
            End If
            ABB_3Up_Attacks(c, i) = value
        Next i
        ABB_2Up_3Sq_Attacks = New BitBoard(Color_NB, Square_NB) {}
        c = Color.Black
        For i = 0 To Square_NB - 1
            If i >= 0 And i < 18 Then
                value = 0
            Else
                f = FileTable(i)
                If f = File.File1 Then
                    value = ABB_Mask(i - 17) Or ABB_Mask(i - 18)
                ElseIf f = File.File9 Then
                    value = ABB_Mask(i - 18) Or ABB_Mask(i - 19)
                Else
                    value = ABB_Mask(i - 17) Or ABB_Mask(i - 18) Or ABB_Mask(i - 19)
                End If
            End If
            ABB_2Up_3Sq_Attacks(c, i) = value
        Next i
        c = Color.Black
        For i = 0 To Square_NB - 1
            If i >= 63 And i < Square_NB Then
                value = 0
            Else
                f = FileTable(i)
                If f = File.File1 Then
                    value = ABB_Mask(i + 18) Or ABB_Mask(i + 19)
                ElseIf f = File.File9 Then
                    value = ABB_Mask(i + 17) Or ABB_Mask(i + 18)
                Else
                    value = ABB_Mask(i + 17) Or ABB_Mask(i + 18) Or ABB_Mask(i + 19)
                End If
            End If
            ABB_2Up_3Sq_Attacks(c, i) = value
        Next i
        ABB_Diag_Back_Attacks = New BitBoard(Color_NB, Square_NB) {}
        c = Color.Black
        For i = 0 To Square_NB - 1
            If i > 71 Then
                value = 0
            Else
                f = FileTable(i)
                If f = File.File1 Then
                    value = ABB_Mask(i + 10)
                ElseIf f = File.File9 Then
                    value = ABB_Mask(i + 8)
                Else
                    value = ABB_Mask(i + 8) Or ABB_Mask(i + 10)
                End If
            End If
            ABB_Diag_Back_Attacks(c, i) = value
        Next i
        c = Color.White
        For i = 0 To Square_NB - 1
            If i < 9 Then
                value = 0
            Else
                f = FileTable(i)
                If f = File.File1 Then
                    value = ABB_Mask(i - 8)
                ElseIf f = File.File9 Then
                    value = ABB_Mask(i - 10)
                Else
                    value = ABB_Mask(i - 8) Or ABB_Mask(i - 10)
                End If
            End If
            ABB_Diag_Back_Attacks(c, i) = value
        Next i
    End Sub

    Private Sub InitPieceTable()
        Dim i As Integer
        Dim list As List(Of Integer)()
        list = New List(Of Integer)(16) {}
        Piece_Table = New Dictionary(Of Integer, List(Of Integer))(1) {}
        Piece_Table(0) = New Dictionary(Of Integer, List(Of Integer))
        Piece_Table(1) = New Dictionary(Of Integer, List(Of Integer))
        For i = 0 To 15
            list(i) = New List(Of Integer)
        Next
        list(0).Add(Piece.Silver)
        list(0).Add(Piece.Gold)
        list(0).Add(Piece.Bishop)
        list(0).Add(Piece.Pro_Pawn)
        list(0).Add(Piece.Pro_Lance)
        list(0).Add(Piece.Pro_Knight)
        list(0).Add(Piece.Pro_Silver)
        list(0).Add(Piece.Horse)
        list(0).Add(Piece.Dragon)
        Piece_Table(0).Add(Direction.Direc_Diag2_U2d, list(0))
        list(1).Add(Piece.Pawn)
        list(1).Add(Piece.Lance)
        list(1).Add(Piece.Silver)
        list(1).Add(Piece.Gold)
        list(1).Add(Piece.Rook)
        list(1).Add(Piece.Pro_Pawn)
        list(1).Add(Piece.Pro_Lance)
        list(1).Add(Piece.Pro_Knight)
        list(1).Add(Piece.Pro_Silver)
        list(1).Add(Piece.Horse)
        list(1).Add(Piece.Dragon)
        Piece_Table(0).Add(Direction.Direc_File_U2d, list(1))
        list(2).Add(Piece.Silver)
        list(2).Add(Piece.Gold)
        list(2).Add(Piece.Bishop)
        list(2).Add(Piece.Pro_Pawn)
        list(2).Add(Piece.Pro_Lance)
        list(2).Add(Piece.Pro_Knight)
        list(2).Add(Piece.Pro_Silver)
        list(2).Add(Piece.Horse)
        list(2).Add(Piece.Dragon)
        Piece_Table(0).Add(Direction.Direc_Diag1_U2d, list(2))
        list(3).Add(Piece.Gold)
        list(3).Add(Piece.Rook)
        list(3).Add(Piece.Pro_Pawn)
        list(3).Add(Piece.Pro_Lance)
        list(3).Add(Piece.Pro_Knight)
        list(3).Add(Piece.Pro_Silver)
        list(3).Add(Piece.Horse)
        list(3).Add(Piece.Dragon)
        Piece_Table(0).Add(Direction.Direc_Rank_L2r, list(3))
        list(4).Add(Piece.Gold)
        list(4).Add(Piece.Rook)
        list(4).Add(Piece.Pro_Pawn)
        list(4).Add(Piece.Pro_Lance)
        list(4).Add(Piece.Pro_Knight)
        list(4).Add(Piece.Pro_Silver)
        list(4).Add(Piece.Horse)
        list(4).Add(Piece.Dragon)
        Piece_Table(0).Add(Direction.Direc_Rank_R2l, list(4))
        list(5).Add(Piece.Silver)
        list(5).Add(Piece.Bishop)
        list(5).Add(Piece.Horse)
        list(5).Add(Piece.Dragon)
        Piece_Table(0).Add(Direction.Direc_Diag1_D2u, list(5))
        list(6).Add(Piece.Gold)
        list(6).Add(Piece.Rook)
        list(6).Add(Piece.Pro_Pawn)
        list(6).Add(Piece.Pro_Lance)
        list(6).Add(Piece.Pro_Knight)
        list(6).Add(Piece.Pro_Silver)
        list(6).Add(Piece.Horse)
        list(6).Add(Piece.Dragon)
        Piece_Table(0).Add(Direction.Direc_File_D2u, list(6))
        list(7).Add(Piece.Silver)
        list(7).Add(Piece.Bishop)
        list(7).Add(Piece.Horse)
        list(7).Add(Piece.Dragon)
        Piece_Table(0).Add(Direction.Direc_Diag2_D2u, list(7))
        list(8).Add(Piece.Silver)
        list(8).Add(Piece.Bishop)
        list(8).Add(Piece.Horse)
        list(8).Add(Piece.Dragon)
        Piece_Table(1).Add(Direction.Direc_Diag2_U2d, list(8))
        list(9).Add(Piece.Gold)
        list(9).Add(Piece.Rook)
        list(9).Add(Piece.Pro_Pawn)
        list(9).Add(Piece.Pro_Lance)
        list(9).Add(Piece.Pro_Knight)
        list(9).Add(Piece.Pro_Silver)
        list(9).Add(Piece.Horse)
        list(9).Add(Piece.Dragon)
        Piece_Table(1).Add(Direction.Direc_File_U2d, list(9))
        list(10).Add(Piece.Silver)
        list(10).Add(Piece.Bishop)
        list(10).Add(Piece.Horse)
        list(10).Add(Piece.Dragon)
        Piece_Table(1).Add(Direction.Direc_Diag1_U2d, list(10))
        list(11).Add(Piece.Gold)
        list(11).Add(Piece.Rook)
        list(11).Add(Piece.Pro_Pawn)
        list(11).Add(Piece.Pro_Lance)
        list(11).Add(Piece.Pro_Knight)
        list(11).Add(Piece.Pro_Silver)
        list(11).Add(Piece.Horse)
        list(11).Add(Piece.Dragon)
        Piece_Table(1).Add(Direction.Direc_Rank_L2r, list(11))
        list(12).Add(Piece.Gold)
        list(12).Add(Piece.Rook)
        list(12).Add(Piece.Pro_Pawn)
        list(12).Add(Piece.Pro_Lance)
        list(12).Add(Piece.Pro_Knight)
        list(12).Add(Piece.Pro_Silver)
        list(12).Add(Piece.Horse)
        list(12).Add(Piece.Dragon)
        Piece_Table(1).Add(Direction.Direc_Rank_R2l, list(12))
        list(13).Add(Piece.Silver)
        list(13).Add(Piece.Gold)
        list(13).Add(Piece.Bishop)
        list(13).Add(Piece.Pro_Pawn)
        list(13).Add(Piece.Pro_Lance)
        list(13).Add(Piece.Pro_Knight)
        list(13).Add(Piece.Pro_Silver)
        list(13).Add(Piece.Horse)
        list(13).Add(Piece.Dragon)
        Piece_Table(1).Add(Direction.Direc_Diag1_D2u, list(13))
        list(14).Add(Piece.Pawn)
        list(14).Add(Piece.Lance)
        list(14).Add(Piece.Silver)
        list(14).Add(Piece.Gold)
        list(14).Add(Piece.Rook)
        list(14).Add(Piece.Pro_Pawn)
        list(14).Add(Piece.Pro_Lance)
        list(14).Add(Piece.Pro_Knight)
        list(14).Add(Piece.Pro_Silver)
        list(14).Add(Piece.Horse)
        list(14).Add(Piece.Dragon)
        Piece_Table(1).Add(Direction.Direc_File_D2u, list(14))
        list(15).Add(Piece.Silver)
        list(15).Add(Piece.Gold)
        list(15).Add(Piece.Bishop)
        list(15).Add(Piece.Pro_Pawn)
        list(15).Add(Piece.Pro_Lance)
        list(15).Add(Piece.Pro_Knight)
        list(15).Add(Piece.Pro_Silver)
        list(15).Add(Piece.Horse)
        list(15).Add(Piece.Dragon)
        Piece_Table(1).Add(Direction.Direc_Diag2_D2u, list(15))
    End Sub
End Module
