Imports System.IO
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Text
Imports Altair.Common
Imports BitBoard = System.UInt128
Imports Move = System.UInt32

Module Test
    Private Sub InitTestModule()
        IniRand(5489)
        IniRandomTable()
        Init.Init()
    End Sub
    Public Sub TestDropMove()
        InitTestModule()
        Dim comments_sfen As List(Of String)
        Dim comments_answer As List(Of String)
        Dim str_sfen As List(Of String)
        Dim str_moves As List(Of String)
        Dim str_move As String()
        Dim moves As List(Of Move)
        Dim i As Integer
        Dim j As Integer
        Dim s As String
        Dim sw As StreamWriter
        Dim bt As BoardTree
        bt = Board.Init()
        Clear(bt)
        comments_sfen = New List(Of String)
        str_sfen = ReadTestFile("test_data_drop.txt", comments_sfen)
        comments_answer = New List(Of String)
        str_moves = ReadTestFile("answer_data_drop.txt", comments_answer)
        sw = New StreamWriter("debug_log_gendrop.txt")
        moves = New List(Of Move)
        For i = 0 To comments_sfen.Count - 1
            Clear(bt)
            bt = ToBoard(str_sfen(i))
            str_move = str_moves(i).Split(" ")
            moves.Clear()
            GenDrop(bt, bt.RootColor, moves)
            s = comments_answer(i)
            sw.WriteLine(s)
            s = ""
            For j = 0 To moves.Count - 1
                s = s & Move2CSA(moves(j)) & " "
            Next j
            sw.WriteLine(s)
        Next i
        sw.Close()
    End Sub
    Public Sub TestNoCapMove()
        InitTestModule()
        Dim comments_sfen As List(Of String)
        Dim comments_answer As List(Of String)
        Dim str_sfen As List(Of String)
        Dim str_moves As List(Of String)
        Dim str_move As String()
        Dim moves As List(Of Move)
        Dim i As Integer
        Dim j As Integer
        Dim s As String
        Dim sw As StreamWriter
        Dim bt As BoardTree
        bt = Board.Init()
        Clear(bt)
        comments_sfen = New List(Of String)
        str_sfen = ReadTestFile("test_data_gennocap.txt", comments_sfen)
        comments_answer = New List(Of String)
        str_moves = ReadTestFile("answer_data_gennocap.txt", comments_answer)
        sw = New StreamWriter("debug_log_nocap.txt")
        moves = New List(Of Move)
        For i = 0 To comments_sfen.Count - 1
            Clear(bt)
            bt = ToBoard(str_sfen(i))
            str_move = str_moves(i).Split(" ")
            moves.Clear()
            GenNoCap(bt, bt.RootColor, moves)
            s = comments_answer(i)
            sw.WriteLine(s)
            s = ""
            For j = 0 To moves.Count - 1
                s = s & Move2CSA(moves(j)) & " "
            Next j
            sw.WriteLine(s)
        Next i
        sw.Close()
    End Sub
    Public Sub TestCapMove()
        InitTestModule()
        Dim comments_sfen As List(Of String)
        Dim comments_answer As List(Of String)
        Dim str_sfen As List(Of String)
        Dim str_moves As List(Of String)
        Dim str_move As String()
        Dim moves As List(Of Move)
        Dim i As Integer
        Dim j As Integer
        Dim s As String
        Dim sw As StreamWriter
        Dim bt As BoardTree
        bt = Board.Init()
        Clear(bt)
        comments_sfen = New List(Of String)
        str_sfen = ReadTestFile("test_data_gencap.txt", comments_sfen)
        comments_answer = New List(Of String)
        str_moves = ReadTestFile("answer_data_gencap.txt", comments_answer)
        sw = New StreamWriter("debug_log_cap.txt")
        moves = New List(Of Move)
        For i = 0 To comments_sfen.Count - 1
            Clear(bt)
            bt = ToBoard(str_sfen(i))
            str_move = str_moves(i).Split(" ")
            moves.Clear()
            GenCap(bt, bt.RootColor, moves)
            s = comments_answer(i)
            sw.WriteLine(s)
            s = ""
            For j = 0 To moves.Count - 1
                s = s & Move2CSA(moves(j)) & " "
            Next j
            sw.WriteLine(s)
        Next i
        sw.Close()
    End Sub
    Public Sub TestEvasionMove()
        InitTestModule()
        Dim comments_sfen As List(Of String)
        Dim comments_answer As List(Of String)
        Dim str_sfen As List(Of String)
        Dim str_moves As List(Of String)
        Dim str_move As String()
        Dim moves As List(Of Move)
        Dim i As Integer
        Dim j As Integer
        Dim s As String
        Dim sw As StreamWriter
        Dim bt As BoardTree
        bt = Board.Init()
        Clear(bt)
        comments_sfen = New List(Of String)
        str_sfen = ReadTestFile("test_data_evasion.txt", comments_sfen)
        comments_answer = New List(Of String)
        str_moves = ReadTestFile("answer_data_evasion.txt", comments_answer)
        sw = New StreamWriter("debug_log_evasion.txt")
        moves = New List(Of Move)
        For i = 0 To comments_sfen.Count - 1
            Clear(bt)
            bt = ToBoard(str_sfen(i))
            str_move = str_moves(i).Split(" ")
            moves.Clear()
            GenEvasion(bt, bt.RootColor, moves)
            s = comments_answer(i)
            sw.WriteLine(s)
            s = ""
            For j = 0 To moves.Count - 1
                s = s & Move2CSA(moves(j)) & " "
            Next j
            sw.WriteLine(s)
        Next i
        sw.Close()
    End Sub

    Public Sub TestCheckMove()
        InitTestModule()
        Dim comments_sfen As List(Of String)
        Dim comments_answer As List(Of String)
        Dim str_sfen As List(Of String)
        Dim str_moves As List(Of String)
        Dim str_move As String()
        Dim moves As List(Of Move)
        Dim i As Integer
        Dim j As Integer
        Dim s As String
        Dim sw As StreamWriter
        Dim bt As BoardTree
        bt = Board.Init()
        Clear(bt)
        comments_sfen = New List(Of String)
        str_sfen = ReadTestFile("test_data_check.txt", comments_sfen)
        comments_answer = New List(Of String)
        str_moves = ReadTestFile("answer_data_check.txt", comments_answer)
        sw = New StreamWriter("debug_log_check.txt")
        moves = New List(Of Move)
        For i = 0 To comments_sfen.Count - 1
            Clear(bt)
            bt = ToBoard(str_sfen(i))
            str_move = str_moves(i).Split(" ")
            moves.Clear()
            GenCheck(bt, bt.RootColor, moves)
            s = comments_answer(i)
            sw.WriteLine(s)
            s = ""
            For j = 0 To moves.Count - 1
                s = s & Move2CSA(moves(j)) & " "
            Next j
            sw.WriteLine(s)
        Next i
        sw.Close()
    End Sub

    Public Sub TestCheckMove2()
        InitTestModule()
        Dim comments_sfen As List(Of String)
        Dim comments_answer As List(Of String)
        Dim str_sfen As List(Of String)
        Dim str_moves As List(Of String)
        Dim str_move As String()
        Dim moves As List(Of Move)
        Dim i As Integer
        Dim j As Integer
        Dim s As String
        Dim sw As StreamWriter
        Dim bt As BoardTree
        bt = Board.Init()
        Clear(bt)
        comments_sfen = New List(Of String)
        str_sfen = ReadTestFile("test_data_b_check_additional.txt", comments_sfen)
        comments_answer = New List(Of String)
        str_moves = ReadTestFile("answer_data_b_check_additional.txt", comments_answer)
        sw = New StreamWriter("debug_log_b_check_additional.txt")
        moves = New List(Of Move)
        For i = 0 To comments_sfen.Count - 1
            Clear(bt)
            bt = ToBoard(str_sfen(i))
            str_move = str_moves(i).Split(" ")
            moves.Clear()
            GenCheck(bt, bt.RootColor, moves)
            s = comments_answer(i)
            sw.WriteLine(s)
            s = ""
            For j = 0 To moves.Count - 1
                s = s & Move2CSA(moves(j)) & " "
            Next j
            sw.WriteLine(s)
        Next i
        sw.Close()
    End Sub

    Public Sub TestCheckMove3()
        InitTestModule()
        Dim comments_sfen As List(Of String)
        Dim comments_answer As List(Of String)
        Dim str_sfen As List(Of String)
        Dim str_moves As List(Of String)
        Dim str_move As String()
        Dim moves As List(Of Move)
        Dim i As Integer
        Dim j As Integer
        Dim s As String
        Dim sw As StreamWriter
        Dim bt As BoardTree
        bt = Board.Init()
        Clear(bt)
        comments_sfen = New List(Of String)
        str_sfen = ReadTestFile("test_data_w_check_additional.txt", comments_sfen)
        comments_answer = New List(Of String)
        str_moves = ReadTestFile("answer_data_w_check_additional.txt", comments_answer)
        sw = New StreamWriter("debug_log_w_check_additional.txt")
        moves = New List(Of Move)
        For i = 0 To comments_sfen.Count - 1
            Clear(bt)
            bt = ToBoard(str_sfen(i))
            str_move = str_moves(i).Split(" ")
            moves.Clear()
            GenCheck(bt, bt.RootColor, moves)
            s = comments_answer(i)
            sw.WriteLine(s)
            s = ""
            For j = 0 To moves.Count - 1
                s = s & Move2CSA(moves(j)) & " "
            Next j
            sw.WriteLine(s)
        Next i
        sw.Close()
    End Sub

    Public Sub TestMate1Ply()
        InitTestModule()
        Dim comments_sfen As List(Of String)
        Dim comments_answer As List(Of String)
        Dim str_sfen As List(Of String)
        Dim str_moves As List(Of String)
        Dim str_move As String()
        Dim i As Integer
        Dim s As String
        Dim mate_move As Move
        Dim sw As StreamWriter
        Dim bt As BoardTree
        bt = Board.Init()
        Clear(bt)
        comments_sfen = New List(Of String)
        str_sfen = ReadTestFile("test_data_b_mate1ply.txt", comments_sfen)
        comments_answer = New List(Of String)
        str_moves = ReadTestFile("answer_data_b_mate1ply.txt", comments_answer)
        sw = New StreamWriter("debug_log_b_mate1ply.txt")
        For i = 0 To comments_sfen.Count - 1
            Clear(bt)
            bt = ToBoard(str_sfen(i))
            If i = 364 Then
                Dim debug = 0
            End If
            str_move = str_moves(i).Split(" ")
            mate_move = MateIn1Ply(bt, bt.RootColor)
            s = comments_answer(i)
            sw.WriteLine(s)
            s = ""
            If mate_move <> 0 Then
                s = Move2CSA(mate_move)
            End If
            sw.WriteLine(s)
        Next i
        sw.Close()
    End Sub

    Public Sub TestMate1Ply2()
        InitTestModule()
        Dim comments_sfen As List(Of String)
        Dim comments_answer As List(Of String)
        Dim str_sfen As List(Of String)
        Dim str_moves As List(Of String)
        Dim str_move As String()
        Dim i As Integer
        Dim s As String
        Dim mate_move As Move
        Dim sw As StreamWriter
        Dim bt As BoardTree
        bt = Board.Init()
        Clear(bt)
        comments_sfen = New List(Of String)
        str_sfen = ReadTestFile("test_data_w_mate1ply.txt", comments_sfen)
        comments_answer = New List(Of String)
        str_moves = ReadTestFile("answer_data_w_mate1ply.txt", comments_answer)
        sw = New StreamWriter("debug_log_w_mate1ply.txt")
        For i = 0 To comments_sfen.Count - 1
            Clear(bt)
            bt = ToBoard(str_sfen(i))
            If i = 364 Then
                Dim debug = 0
            End If
            str_move = str_moves(i).Split(" ")
            mate_move = MateIn1Ply(bt, bt.RootColor)
            s = comments_answer(i)
            sw.WriteLine(s)
            s = ""
            If mate_move <> 0 Then
                s = Move2CSA(mate_move)
            End If
            sw.WriteLine(s)
        Next i
        sw.Close()
    End Sub

    Public Sub TestRepetition()
        InitTestModule()
        Dim sw As StreamWriter
        Dim bt As BoardTree
        Dim records As List(Of Record)
        Dim r As Record
        Dim m As Move
        Dim c As Integer
        Dim i As Integer
        Dim tr As TT
        Dim iret As Integer
        bt = Board.Init()
        'Clear(bt)
        sw = New StreamWriter("debug_log_repetition.txt")
        records = ReadRecords("test_repetition.txt")
        r = records(0)
        tr = New TT()
        InitTT(tr)
        c = 0
        For i = 0 To r.ply - 1
            m = CSA2Move(bt, r.str_moves(i))
            DoMove(bt, m, c)
            tr.is_check(bt.CurrentHash) = False
            c = c Xor 1
        Next i
        iret = IsRepetition(bt, tr)
        sw.Close()
    End Sub

    Public Sub TestDeclarationWin()
        InitTestModule()
        Dim s As String
        Dim ls As List(Of String)
        Dim i As Integer
        Dim l As Integer
        Dim iret As Integer
        Dim bt As BoardTree
        ls = New List(Of String)
        bt = Board.Init()
        s = "+L+NSGKGS+N+L/1+R5+B1/+P+P+P+P+P+P+P+P+P/9/9/9/+p+p+p+p+p+p+p+p+p/1+r5+b1/+l+nsgkgs+n+l b - 1" ' 後手勝ち
        ls.Add(s)
        s = "+L+NSGKGS+N+L/+P+R5+B1/+P+P+P+P+P+P+P+P+P/9/9/9/+p+p+p+p+p+p+p+p1/1+r5+b1/+l+nsgkgs+n+l b - 1" ' 先手勝ち
        ls.Add(s)
        s = "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1" ' どちらの勝ちでもない → 初期局面
        ls.Add(s)
        s = "+L+NSGK4/1+R7/+P+P+P+P+P4/9/9/9/+p+p+p+p+p4/7+b1/+l+nsgk4 b BGSNL4Prgsnl4p 1" ' 後手勝ち
        ls.Add(s)
        s = "+L+NSGK4/1+R7/+P+P+P+P+P4/9/9/9/+p+p+p+p+p4/7+b1/+l+nsgk4 b BGSNL5Prgsnl3p 1" ' 先手勝ち
        ls.Add(s)
        s = "4k4/9/9/9/9/9/9/4p4/4K4 b RB2G2S2N2L9Prb2g2s2n2l8p 1" ' どちらの勝ちでもない → 先手玉が王手
        ls.Add(s)
        s = "4k4/4P4/9/9/9/9/9/9/4K4 b RB2G2S2N2L8Prb2g2s2n2l9p 1" ' どちらの勝ちでもない → 後手玉が王手
        ls.Add(s)
        s = "+L+NSGKGS+N+L/1+R5+B1/+P+P+P+P+P+P+P+P+P/9/9/8k/+p+p+p+p+p+p+p+p+p/1+r5+b1/+l+nsg1gs+n+l b - 1" ' どちらの勝ちでもない → 後手玉が宣言勝ちの位置にいない
        ls.Add(s)
        s = "+L+NSG1GS+N+L/+P+R5+B1/+P+P+P+P+P+P+P+P+P/K8/9/9/+p+p+p+p+p+p+p+p1/1+r5+b1/+l+nsgkgs+n+l b - 1" ' どちらの勝ちでもない → 先手玉が宣言勝ちの位置にいない
        ls.Add(s)
        s = "+L+NSGK4/1+R7/+P+P+P+P+P4/9/9/8k/+p+p+p+p+p4/7+b1/+l+nsg5 b BGSNL4Prgsnl4p 1" ' どちらの勝ちでもない → 後手玉が宣言勝ちの位置にいない
        ls.Add(s)
        s = "+L+NSG5/1+R7/+P+P+P+P+P4/K8/9/9/+p+p+p+p+p4/7+b1/+l+nsgk4 b BGSNL5Prgsnl3p 1" ' どちらの勝ちでもない → 先手玉が宣言勝ちの位置にいない
        ls.Add(s)
        l = ls.Count
        For i = 0 To l - 1
            Clear(bt)
            bt = ToBoard(ls(i))
            iret = IsDeclarationWin(bt)
            If iret = 0 Then
                Console.WriteLine("宣言勝ちの局面ではない。")
            ElseIf iret = 1 Then
                Console.WriteLine("先手勝ち。")
            ElseIf iret = 2 Then
                Console.WriteLine("後手勝ち。")
            End If
        Next i
    End Sub

    Public Sub TestDoMove()
        InitTestModule()
        Dim bt As BoardTree
        Dim records As List(Of Record)
        Dim r As Record
        Dim m As Move
        Dim c As Integer
        Dim i As Integer
        Dim tr As TT
        'Dim iret As Integer
        bt = Board.Init()
        'Clear(bt)
        records = ReadRecords("20220403_nhk_hai.txt")
        r = records(0)
        tr = New TT()
        InitTT(tr)
        c = 0
        For i = 0 To r.ply - 1
            m = CSA2Move(bt, r.str_moves(i))
            DoMove(bt, m, c)
            c = c Xor 1
        Next i
        OutBoard(bt)
    End Sub

    Public Sub TestUnDoMove()
        InitTestModule()
        Dim bt As BoardTree
        Dim bt_copy As BoardTree
        Dim records As List(Of Record)
        Dim r As Record
        Dim m As Move
        Dim c As Integer
        Dim i As Integer
        Dim tr As TT
        'Dim iret As Integer
        bt = Board.Init()
        'Clear(bt)
        records = ReadRecords("20220410_nhk_hai.txt")
        r = records(0)
        tr = New TT()
        InitTT(tr)
        c = 0
        For i = 0 To r.ply - 1
            m = CSA2Move(bt, r.str_moves(i))
            bt_copy = DeepCopy(bt, False)
            DoMove(bt, m, c)
            UnDoMove(bt, m, c)
            VerifyBoard(bt, bt_copy)
            DoMove(bt, m, c)
            c = c Xor 1
        Next i
        'OutBoard(bt)
    End Sub

    Public Sub TestMate()
        InitTestModule()
        Dim str_sfen As List(Of String)
        Dim bt As BoardTree
        Dim mst As MateSearchTree
        bt = Board.Init()
        str_sfen = New List(Of String)
        '詰みがあるもの -> 先後入れ替えで5局面か6局面ずつ
        '詰みがないもの -> 先後入れ替えでn局面ずつ -> nをいくつにするか？
        '13手詰めの問題は玉方の設定を持ち駒（残り全部）にしないと
        '早詰みと判定されてしまうので外した。
        '佐藤康光先生の本から抜粋
        str_sfen.Add("6s2/6R2/6Bk1/6p2/7N1/9/9/9/9 b GN 1") '問題1 先手番 5手詰め
        str_sfen.Add("9/9/9/9/1n7/2P6/1Kb6/2r6/2S6 w gn 1") '問題1 後手番 5手詰め
        str_sfen.Add("5s1nl/5s1k1/4+Pp1pp/6R2/7N1/9/9/9/9 b 2GN 1") '問題80 先手番 7手詰め
        str_sfen.Add("9/9/9/9/1n7/2r6/PP1P+p4/1K1S5/LN1S5 w 2gn 1") '問題80 後手番 7手詰め
        str_sfen.Add("8l/7R1/5Sg1k/6N1s/6p2/9/9/9/9 b BGS 1") '問題115 先手番 9手詰め
        str_sfen.Add("9/9/9/9/2P6/S1n6/K1Gs5/1r7/L8 w bgs 1") '問題115 後手番 9手詰め
        str_sfen.Add("5p2l/4Br2k/4s1gpp/6N+R1/9/9/9/9/9 b 2S 1") '問題140 先手番 11手詰め
        str_sfen.Add("9/9/9/9/9/1+rn6/PPG1S4/K2Rb4/L2P5 w 2s 1") '問題140 後手番 11手詰め
        str_sfen.Add("7nl/4kl1+R1/1+L6p/2pppPp2/1s3p3/1pPP2P2/n1G1B3P/2G6/2K1+b4 b RG2SNL2Pgsn4p 1") '2022年4月3日のNHK杯▲木村九段対△黒田五段戦の終盤戦
        '以下は1個しかテストしていない。
        str_sfen.Add("lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1") '初期局面 先手番 詰みなし
        str_sfen.Add("lnsgkgsnl/1r5b1/ppppppppp/9/9/2P6/PP1PPPPPP/1B5R1/LNSGKGSNL w - 1") '初期局面から▲7六歩と指した局面 後手番 詰みなし
        str_sfen.Add("lnsgk2nl/1r4gs1/p1pppp1pp/6p2/1p5P1/2P6/PPSPPPP1P/7R1/LN1GKGSNL b Bb 1") 'ランダム局面 先手番 詰みなし
        str_sfen.Add("lr5nl/3g1kg2/2n1pssp1/p1p2pp1p/1p1PP2P1/P1P2PP1P/1PSS2N2/1KG2G3/LN2R3L w BPb 1") 'ランダム局面 後手番 詰みなし
        mst = InitMateSearchTree(5)
        mst.BTree = ToBoard(str_sfen(8))
        GenCheck(mst.BTree, mst.BTree.RootColor, mst.RootCheckMoves)
        MateSearchWrapper(mst, 5)
    End Sub

    Public Sub OutBoard(ByVal bt As BoardTree)
        Dim s As String
        Dim i As Integer
        Dim pc As Integer
        Dim c As Integer
        Dim bb As BitBoard
        Dim sq As Integer
        Dim n As Integer
        s = "盤面の配列"
        Console.WriteLine(s)
        s = " 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 "
        Console.WriteLine(s)
        For i = 0 To Square_NB - 1
            pc = bt.Board(i)
            If FileTable(i) = File.File1 Then
                s = ""
            End If
            If pc <> 0 Then
                If pc > 0 Then
                    s = s & " " & Str_Piece_JP(Math.Abs(pc))
                Else
                    s = s & "v" & Str_Piece_JP(Math.Abs(pc))
                End If
            Else
                s = s & " 　"
            End If
            If FileTable(i) = File.File9 Then
                s = s & "|" & vbCrLf
                Console.WriteLine(s)
            Else
                s = s & "|"
            End If
        Next i
        Console.WriteLine()
        s = "ビットボードの駒"
        Console.WriteLine(s)
        c = Color.Black
        bb = bt.BB_Piece(c, Piece.Pawn)
        s = "先手の歩："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Lance)
        s = "先手の香："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Knight)
        s = "先手の桂："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Silver)
        s = "先手の銀："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Gold)
        s = "先手の金："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Bishop)
        s = "先手の角："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Rook)
        s = "先手の飛："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.King)
        s = "先手の玉："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Pro_Pawn)
        s = "先手のと："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Pro_Lance)
        s = "先手の成香："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Pro_Knight)
        s = "先手の成桂："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Pro_Silver)
        s = "先手の成銀："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Horse)
        s = "先手の馬："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Dragon)
        s = "先手の龍："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        Console.WriteLine()
        c = Color.White
        bb = bt.BB_Piece(c, Piece.Pawn)
        s = "後手の歩："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Lance)
        s = "後手の香："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Knight)
        s = "後手の桂："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Silver)
        s = "後手の銀："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Gold)
        s = "後手の金："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Bishop)
        s = "後手の角："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Rook)
        s = "後手の飛："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.King)
        s = "後手の玉："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Pro_Pawn)
        s = "後手のと："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Pro_Lance)
        s = "後手の成香："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Pro_Knight)
        s = "後手の成桂："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Pro_Silver)
        s = "後手の成銀："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Horse)
        s = "後手の馬："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        bb = bt.BB_Piece(c, Piece.Dragon)
        s = "後手の龍："
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        Console.WriteLine()
        s = "先手玉の位置：" & bt.SQ_King(Color.Black).ToString()
        Console.WriteLine(s)
        s = "後手玉の位置：" & bt.SQ_King(Color.White).ToString()
        Console.WriteLine(s)
        Console.WriteLine()
        s = "先手のOccupied："
        Console.WriteLine(s)
        bb = bt.BB_Occupied(Color.Black)
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        s = "後手のOccupied："
        Console.WriteLine(s)
        bb = bt.BB_Occupied(Color.White)
        While bb > 0
            sq = Square(bb)
            bb = bb Xor ABB_Mask(sq)
            s = s & sq.ToString() & ","
        End While
        Console.WriteLine(s)
        Console.WriteLine()
        s = "先手の持ち駒："
        Console.WriteLine(s)
        For i = Piece.Pawn To Piece.Rook
            n = (bt.Hand(0) And Hand_Mask(i)) >> Hand_Rev_Bit(i)
            s = Str_Piece_JP(i) & "：" & n.ToString()
            Console.WriteLine(s)
        Next i
        Console.WriteLine()
        s = "後手の持ち駒："
        Console.WriteLine(s)
        For i = Piece.Pawn To Piece.Rook
            n = (bt.Hand(1) And Hand_Mask(i)) >> Hand_Rev_Bit(i)
            s = Str_Piece_JP(i) & "：" & n.ToString()
            Console.WriteLine(s)
        Next i
    End Sub

    Public Sub VerifyBoard(ByVal bt_before As BoardTree, ByVal bt_after As BoardTree)
        Dim i As Integer
        For i = Piece.Pawn To Piece.Dragon
            If bt_before.BB_Piece(Color.Black, i) <> bt_after.BB_Piece(Color.Black, i) Then
                Console.WriteLine("先手の" & Str_Piece_JP(i) & "のビットボードが一致しません。")
            End If
            If bt_before.BB_Piece(Color.White, i) <> bt_after.BB_Piece(Color.White, i) Then
                Console.WriteLine("後手の" & Str_Piece_JP(i) & "のビットボードが一致しません。")
            End If
        Next i
        If bt_before.BB_Occupied(Color.Black) <> bt_after.BB_Occupied(Color.Black) Then
            Console.WriteLine("先手のOccupiedのビットボードが一致しません。")
        End If
        If bt_before.BB_Occupied(Color.White) <> bt_after.BB_Occupied(Color.White) Then
            Console.WriteLine("後手のOccupiedのビットボードが一致しません。")
        End If
        For i = 0 To Square_NB - 1
            If bt_before.Board(i) <> bt_after.Board(i) Then
                Console.WriteLine("盤面の配列が一致しません。")
                Console.WriteLine("不一致の位置：" & i.ToString())
            End If
        Next i
        If bt_before.SQ_King(Color.Black) <> bt_after.SQ_King(Color.Black) Then
            Console.WriteLine("先手玉の位置が一致しません。")
        End If
        If bt_before.SQ_King(Color.White) <> bt_after.SQ_King(Color.White) Then
            Console.WriteLine("後手玉の位置が一致しません。")
        End If
        If bt_before.Hand(0) <> bt_after.Hand(0) Then
            Console.WriteLine("先手の持ち駒が一致しません。")
        End If
        If bt_before.Hand(1) <> bt_after.Hand(1) Then
            Console.WriteLine("後手の持ち駒が一致しません。")
        End If
        If bt_before.RootColor <> bt_after.RootColor Then
            Console.WriteLine("手番が一致しません。")
        End If
        If bt_before.CurrentHash <> bt_after.CurrentHash Then
            Console.WriteLine("CurrentHashが一致しません。")
        End If
        If bt_before.PrevHash <> bt_after.PrevHash Then
            Console.WriteLine("PrevHashが一致しません。")
        End If
        If bt_before.Ply <> bt_after.Ply Then
            Console.WriteLine("Plyが一致しません。")
        End If
        For i = 0 To Ply_Max - 1
            If bt_before.Hash(i) <> bt_after.Hash(i) Then
                Console.WriteLine("Hashが一致しません。")
                Console.WriteLine("不一致のPly：" & i.ToString())
            End If
        Next i
    End Sub
End Module
