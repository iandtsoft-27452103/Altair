Imports System.Text.Json
Imports Altair.Common
Imports BitBoard = System.UInt128
Imports Move = System.UInt32
Module Mate
    Public Function InitMateSearchTree(ByVal ply_limit As Integer) As MateSearchTree
        Dim mst As MateSearchTree
        mst.max_ply = ply_limit
        mst.move_cur = New Move(mst.max_ply + 1) {}
        mst.mate_proc = New List(Of List(Of Move))
        mst.no_mate_proc = New List(Of List(Of Move))
        mst.first_move = 0
        mst.second_move = 0
        mst.is_abort = False
        mst.is_mate_root = False
        mst.BTree = Board.Init()
        Board.Clear(mst.BTree) 'これは不要かもしれない。
        mst.RootCheckMoves = New List(Of Move)
        mst.root_str_pv = ""
        Return mst
    End Function
    Public Sub MateSearchWrapper(ByRef mst As MateSearchTree, ByVal depth_max As Integer)
        Dim rest_depth As Integer
        Dim i As Integer
        rest_depth = 1
        With mst
            While rest_depth <= depth_max
                .max_ply = rest_depth
                For i = 1 To .max_ply + 1
                    .move_cur(i) = 0
                Next i
                .mate_proc.Clear()
                .no_mate_proc.Clear()

                '後で直す
                .is_mate_root = Offend(mst, mst.BTree.RootColor, rest_depth, 1)

                If .is_mate_root = True Then
                    Exit While
                End If

                If .is_abort = True Then
                    .is_mate_root = False
                    Exit While
                End If

                rest_depth += 2
            End While

            If .is_mate_root = True And .is_abort = False Then
                Console.WriteLine("詰みあり")
                '後で直す
                .root_str_pv = ""
                .root_str_pv = OutResult(mst, rest_depth)
                Console.WriteLine(.root_str_pv)
            End If
        End With
    End Sub
    Private Function OutResult(ByRef mst As MateSearchTree, rest_depth As Integer)
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim l As List(Of List(Of Move))
        Dim nl As List(Of List(Of Move))
        Dim b As Boolean
        Dim c As Integer
        Dim str_pv As String
        Dim str_color As String() = {"+", "-"}
        Dim idxes As List(Of Integer)
        Dim s As String
        With mst
            l = mst.mate_proc
            nl = mst.no_mate_proc
            b = False
            str_pv = ""
            idxes = New List(Of Integer)
            For i = 0 To l.Count - 1
                s = (i + 1).ToString() & " / " & l.Count.ToString()
                Console.WriteLine(s)
                For j = 0 To nl.Count - 1
                    b = False
                    For k = 0 To nl(j).Count - 2
                        If l(i)(k) <> nl(j)(k) Then
                            b = True
                            Exit For
                        End If
                    Next k
                    If b = False Then
                        idxes.Add(i)
                    End If
                Next j
            Next i
            For i = 0 To l.Count - 1
                If idxes.Contains(i) = True Then
                    Continue For
                End If
                str_pv = ""
                c = .BTree.RootColor
                For j = 0 To rest_depth - 1
                    str_pv = str_pv & str_color(c)
                    str_pv = str_pv & CSA.Move2CSA(l(i)(j))
                    If j <> rest_depth - 1 Then
                        str_pv = str_pv & ", "
                    End If
                    c = c Xor 1
                Next j
                Console.WriteLine(str_pv)
            Next i
        End With
        Return str_pv
    End Function
    Public Function Offend(ByRef mst As MateSearchTree, ByVal c As Integer, ByVal rest_depth As Integer, ByVal ply As Integer) As Boolean
        Dim is_mate As Boolean
        Dim checkMoves As List(Of Move)
        Dim moves As List(Of Move)
        Dim i As Integer
        Dim j As Integer
        With mst
            If .is_abort = True Then
                Return False
            End If

            '王手を生成する
            If ply > 1 Then
                checkMoves = New List(Of Move)
                GenCheck(.BTree, c, checkMoves)
            Else
                checkMoves = mst.RootCheckMoves
            End If

            '// 王手が生成されなかったら詰まない
            If checkMoves.Count = 0 Then
                Return False
            End If

            For i = 0 To checkMoves.Count - 1
                If .is_abort = True Then
                    Return False
                End If

                .move_cur(ply) = checkMoves(i)

                If GetCapPiece(.move_cur(ply)) = Piece.King Then
                    Continue For
                End If

                If GetPiece(.move_cur(ply)) = Piece.Empty Then ' たまにこのような状態になる。原因は特定できていない。
                    Continue For
                End If

                DoMove(.BTree, .move_cur(ply), c)

                ' たまにこのような状態になる。原因は特定できていない。
                If IsAttacked(.BTree, .BTree.SQ_King(c Xor 1), c Xor 1) = 0 Then
                    UnDoMove(.BTree, .move_cur(ply), c)
                    Continue For
                End If

                ' Discoverd Checkになってしまった場合
                If IsAttacked(.BTree, .BTree.SQ_King(c), c) <> 0 Then
                    UnDoMove(.BTree, .move_cur(ply), c)
                    Continue For
                End If

                is_mate = Defend(mst, c Xor 1, rest_depth - 1, ply + 1)

                If is_mate = True Then
                    If ply = .max_ply Then
                        moves = New List(Of Move)
                        For j = 1 To ply
                            moves.Add(.move_cur(j))
                        Next j
                        .mate_proc.Add(moves)
                    End If
                    UnDoMove(.BTree, .move_cur(ply), c)
                    Return True
                End If

                UnDoMove(.BTree, .move_cur(ply), c)
            Next i
        End With
        Return False
    End Function
    Public Function Defend(ByRef mst As MateSearchTree, ByVal c As Integer, ByVal rest_depth As Integer, ByVal ply As Integer) As Boolean
        Dim is_mate As Boolean
        Dim mate_count As Integer
        Dim evasionMoves As List(Of Move)
        Dim moves As List(Of Move)
        Dim i As Integer
        mate_count = 0
        evasionMoves = New List(Of Move)
        With mst
            If .is_abort = True Then
                Return False
            End If
            '王手を避ける手を生成する
            GenEvasion(.BTree, c, evasionMoves)

            ' 残り深さが1で王手を避ける手が生成されたら詰みではない
            If rest_depth = 0 And evasionMoves.Count > 0 Then
                Return False
            End If

            '王手を避ける手が生成されなかったら詰みである
            If evasionMoves.Count = 0 Then
                Return True
            End If
            For i = 0 To evasionMoves.Count - 1
                If .is_abort = True Then
                    Return False
                End If

                .move_cur(ply) = evasionMoves(i)

                DoMove(.BTree, .move_cur(ply), c)

                If IsAttacked(.BTree, .BTree.SQ_King(c), c) <> 0 Then
                    UnDoMove(.BTree, .move_cur(ply), c)
                    Continue For
                End If

                is_mate = Offend(mst, c Xor 1, rest_depth - 1, ply + 1)

                If is_mate = False Then
                    moves = New List(Of Move)
                    For j = 0 To ply - 1
                        moves.Add(.move_cur(j + 1))
                    Next j
                    .no_mate_proc.Add(moves)

                    ' ※守備側で不詰みがあった場合、1つ上の攻撃側の手までの手順は全部不詰みとなる。
                    UnDoMove(.BTree, .move_cur(ply), c)
                    Return False
                Else
                    mate_count += 1
                End If

                UnDoMove(.BTree, .move_cur(ply), c)
            Next i

            If ply = 2 And mate_count = evasionMoves.Count Then
                .first_move = .move_cur(1)
                .second_move = .move_cur(2)
            End If
        End With
        Return True 'どの手を指しても詰みだった場合
    End Function
End Module
