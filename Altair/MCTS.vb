Imports System.Threading
Imports BitBoard = System.UInt128
Imports Move = System.UInt32

Module MCTS
    'I do not use array intentionally.
    Public mcts_tree0 As MCTSTree
    Public mcts_tree1 As MCTSTree
    Public mcts_tree2 As MCTSTree
    Public mcts_tree3 As MCTSTree
    Public mcts_tree4 As MCTSTree
    Public mcts_tree5 As MCTSTree
    Public Function InitNode() As Node
        Dim nd As Node
        nd = New Node
        With nd
            .color = Color.Black
            .ParentIndex = Integer.MaxValue
            .ThisIndex = Integer.MaxValue
            .TrialCount = 0
            .PlayoutCount = 0
            .WinCount = 0
            .DrawCount = 0
            .LostCount = 0
            .EvalCount = 0
            .WinRateSum = 0.0F
            .LostRateSum = 0.0F
            .IsLeaf = True
            .ChildIndexes = New List(Of Integer)
            .move = 0
            .PolicyResult = 0.0F
        End With
        Return nd
    End Function
    Public Function InitMCTSTree() As MCTSTree
        Dim mcts_t As MCTSTree
        mcts_t = New MCTSTree
        With mcts_t
            .BTree = Board.Init()
            .NodeList = New List(Of Node)
            .PlayOutCount = 0
            .RootOutput = Array.Empty(Of Single)()
            .value_lambda = 0.5F
            .nthr = 30
            .TaskNumber = 0
            .SearchTimeLimit = 0
            .TimeBuffer = 500
            .sw = New Stopwatch()
            .queue_from_main_thread_p = New Queue(Of String)()
            .queue_to_main_thread_p = New Queue(Of String)
            .queue_from_main_thread_v = New Queue(Of String)()
            .queue_to_main_thread_v = New Queue(Of String)
            .tt = New TT2()
            InitTT2(.tt)
            .is_abort = False
            .is_finished = False
            .VectorList = ""
        End With
        Return mcts_t
    End Function

    'TotalParam is not implemented.
    Public Function TotalParam(ByVal num_tasks As Integer, ByRef f As List(Of Single), ByRef t As List(Of Integer), ByVal use_draw As Boolean) As List(Of Move)
        Dim value_max As Integer
        Dim max_index As Integer
        Dim limit As Integer
        Dim num_root_moves As Integer
        Dim trial_count_array As List(Of Integer)
        Dim win_rate_array As List(Of Single)
        Dim return_moves As List(Of Move)
        Dim return_win_rate_array As List(Of Single)
        Dim return_trial_count_array As List(Of Integer)
        Dim i As Integer
        Dim j As Integer
        Dim temp_tree As MCTSTree

        trial_count_array = New List(Of Integer)
        win_rate_array = New List(Of Single)
        return_trial_count_array = New List(Of Integer)
        return_moves = New List(Of Move)
        return_win_rate_array = New List(Of Single)
        num_root_moves = mcts_tree0.BTree.RootMoves.Length

        Select Case num_root_moves
            Case 1
                limit = 1
            Case 2
                limit = 2
            Case Else
                limit = 3
        End Select

        If num_tasks > 0 Then
            For i = 1 To num_tasks - 1
                For j = 0 To num_root_moves - 1
                    temp_tree = GetMCTSObject(i)
                    mcts_tree0.NodeList(j + 1).TrialCount += temp_tree.NodeList(j + 1).TrialCount
                    mcts_tree0.NodeList(j + 1).WinCount += temp_tree.NodeList(j + 1).WinCount
                    mcts_tree0.NodeList(j + 1).DrawCount += temp_tree.NodeList(j + 1).DrawCount
                    mcts_tree0.NodeList(j + 1).LostCount += temp_tree.NodeList(j + 1).LostCount
                Next j
            Next i
        End If
        For i = 0 To num_root_moves - 1
            trial_count_array.Add(mcts_tree0.NodeList(i + 1).TrialCount)
        Next i
        For i = 0 To num_root_moves - 1
            If use_draw = True Then
                If trial_count_array(i) = 0 Then
                    win_rate_array.Add(0)
                Else
                    win_rate_array.Add(CSng(mcts_tree0.NodeList(i + 1).WinCount) + CSng(mcts_tree0.NodeList(i + 1).DrawCount) * 0.5 / CSng(trial_count_array(i)))
                End If
            Else
                If trial_count_array(i) = 0 Then
                    win_rate_array.Add(0)
                Else
                    win_rate_array.Add(CSng(mcts_tree0.NodeList(i + 1).WinCount) / CSng(trial_count_array(i)))
                End If
            End If
        Next i

        For i = 0 To limit - 1
            value_max = trial_count_array.Max()
            max_index = Array.IndexOf(trial_count_array.ToArray(), value_max)
            return_trial_count_array.Add(trial_count_array(max_index))
            trial_count_array(max_index) = Integer.MinValue
            return_moves.Add(mcts_tree0.BTree.RootMoves(max_index))
            return_win_rate_array.Add(win_rate_array(max_index))
        Next i

        f = return_win_rate_array
        t = return_trial_count_array

        Return return_moves
    End Function

    'Before calling this function, You must initialize MCTSTree object.
    Public Function GetMCTSObject(ByVal i As Integer)
        Dim obj As MCTSTree
        Select Case i
            Case 0
                obj = mcts_tree0
            Case 1
                obj = mcts_tree1
            Case 2
                obj = mcts_tree2
            Case 3
                obj = mcts_tree3
            Case 4
                obj = mcts_tree4
            Case 5
                obj = mcts_tree5
        End Select
        Return obj
    End Function
    Public Sub GenRootMoves(ByRef m As MCTSTree)
        Dim i As Integer
        Dim ifrom As Integer
        Dim ito As Integer
        Dim moves As List(Of Move)
        Dim legal_moves As List(Of Move)
        Dim c As Integer
        moves = New List(Of Move)
        legal_moves = New List(Of Move)
        If IsAttacked(m.BTree, m.BTree.SQ_King(m.BTree.RootColor), m.BTree.RootColor) = 0 Then
            GenCap(m.BTree, m.BTree.RootColor, moves)
            GenNoCap(m.BTree, m.BTree.RootColor, moves)
            GenDrop(m.BTree, m.BTree.RootColor, moves)
        Else
            GenEvasion(m.BTree, m.BTree.RootColor, moves)
        End If

        'remove illegal move.
        For i = 0 To moves.Count - 1
            ifrom = GetFrom(moves(i))
            ito = GetTo(moves(i))
            If ifrom < Square_NB Then
                'the case of discovered check
                If IsPinnedOnKing(m.BTree, ifrom, Adirec(ifrom, ito), c) <> 0 Then
                    Continue For
                End If
            End If
            If GetCapPiece(moves(i)) = Piece.King Then
                'the case of capture opponent king
                Continue For
            End If
            legal_moves.Add(moves(i))
        Next i
        m.BTree.RootMoves = legal_moves.ToArray()
    End Sub
    ' For CNN.
    Public Sub Root(ByRef mcts_t As MCTSTree)
        Dim root_node As Node
        Dim node As Node
        Dim i As Integer
        Dim elapsed As Long
        Dim t As Integer
        Dim u As Single
        Dim q As Single
        Dim ucb1_array As Single()
        Dim max_index As Integer
        Dim bt As BoardTree
        Dim result As Integer
        'Dim result_array As List(Of Single)
        'result_array = New List(Of Single)
        With mcts_t
            .NodeList.Clear()
            root_node = InitNode()
            root_node.ThisIndex = 0
            .NodeList.Add(root_node)
            For i = 0 To .BTree.RootMoves.Length - 1
                node = InitNode()
                node.color = .BTree.RootColor
                node.move = .BTree.RootMoves(i)
                node.ParentIndex = 0
                node.ThisIndex = i + 1
                .NodeList(0).ChildIndexes.Add(i + 1)
                node.PolicyResult = .RootOutput(i)
                'result_array(i) = .RootOutput(i)
                .NodeList.Add(node)
            Next i


            While True
                elapsed = .sw.ElapsedMilliseconds
                If .SearchTimeLimit < elapsed Then
                    Exit While
                End If

                If .is_abort = True Then
                    Exit While
                End If

                ucb1_array = New Single(.BTree.RootMoves.Length - 1) {}
                t = 0
                i = 1
                While i < .BTree.RootMoves.Length
                    t += .NodeList(i).PlayoutCount
                    i += 1
                End While
                i = 1
                While i < .BTree.RootMoves.Length
                    u = .NodeList(i).PolicyResult * CSng(Math.Sqrt(t)) / CSng(.NodeList(i).PlayoutCount + 1)
                    q = 0.0F
                    If .NodeList(i).EvalCount > 0 And .NodeList(i).PlayoutCount > 0 Then
                        q = (CSng(1 - .value_lambda) * (.NodeList(i).WinRateSum / CSng(.NodeList(i).EvalCount)) + .value_lambda * (CSng(.NodeList(i).WinCount) / CSng(.NodeList(i).PlayoutCount)))
                    End If
                    ucb1_array(i - 1) = u + q
                    i += 1
                End While

                max_index = Array.IndexOf(ucb1_array, ucb1_array.Max()) + 1
                DoMove(.BTree, .BTree.RootMoves(max_index - 1), .BTree.RootColor)
                If .NodeList(max_index).IsLeaf = True Then
                    elapsed = .sw.ElapsedMilliseconds
                    If .SearchTimeLimit < (elapsed + .TimeBuffer) Then
                        Exit While
                    End If
                    If .is_abort = True Then
                        Exit While
                    End If
                    If .NodeList(max_index).TrialCount >= .nthr Then
                        ExpandNode(mcts_t, .BTree.RootColor Xor 1, max_index, .BTree.Ply + 1)
                        If .is_abort = True Then
                            Exit While
                        End If
                    Else
                        bt = Board.Init()
                        bt = DeepCopy(.BTree, False)
                        result = PlayOut(mcts_t, bt, .BTree.RootColor Xor 1, max_index, .BTree.Ply + 1)
                        EvalNode(mcts_t, max_index, .BTree.RootColor Xor 1)
                        If .is_abort = True Then
                            Exit While
                        End If
                        UpdateParam(mcts_t, max_index, result)
                    End If
                Else

                    elapsed = .sw.ElapsedMilliseconds
                    If .SearchTimeLimit < (elapsed + .TimeBuffer) Then
                        Exit While
                    End If
                    If .is_abort = True Then
                        Exit While
                    End If
                    DescendNode(mcts_t, .BTree.RootColor Xor 1, max_index, .nthr, .BTree.Ply + 1)
                End If
                UnDoMove(.BTree, .BTree.RootMoves(max_index - 1), .BTree.RootColor)
            End While
        End With
    End Sub
    ' For RNN.
    Public Sub Root2(ByRef mcts_t As MCTSTree)
        Dim root_node As Node
        Dim node As Node
        Dim i As Integer
        Dim elapsed As Long
        Dim t As Integer
        Dim u As Single
        Dim q As Single
        Dim ucb1_array As Single()
        Dim max_index As Integer
        Dim bt As BoardTree
        Dim result As Integer
        'Dim result_array As List(Of Single)
        'result_array = New List(Of Single)
        With mcts_t
            .NodeList.Clear()
            root_node = InitNode()
            root_node.ThisIndex = 0
            .NodeList.Add(root_node)
            For i = 0 To .BTree.RootMoves.Length - 1
                node = InitNode()
                node.color = .BTree.RootColor
                node.move = .BTree.RootMoves(i)
                node.ParentIndex = 0
                node.ThisIndex = i + 1
                .NodeList(0).ChildIndexes.Add(i + 1)
                node.PolicyResult = .RootOutput(i)
                'result_array(i) = .RootOutput(i)
                .NodeList.Add(node)
            Next i


            While True
                elapsed = .sw.ElapsedMilliseconds
                If .SearchTimeLimit < elapsed Then
                    Exit While
                End If

                If .is_abort = True Then
                    Exit While
                End If

                ucb1_array = New Single(.BTree.RootMoves.Length - 1) {}
                t = 0
                i = 1
                While i < .BTree.RootMoves.Length
                    t += .NodeList(i).PlayoutCount
                    i += 1
                End While
                i = 1
                While i < .BTree.RootMoves.Length
                    u = .NodeList(i).PolicyResult * CSng(Math.Sqrt(t)) / CSng(.NodeList(i).PlayoutCount + 1)
                    q = 0.0F
                    If .NodeList(i).EvalCount > 0 And .NodeList(i).PlayoutCount > 0 Then
                        q = (CSng(1 - .value_lambda) * (.NodeList(i).WinRateSum / CSng(.NodeList(i).EvalCount)) + .value_lambda * (CSng(.NodeList(i).WinCount) / CSng(.NodeList(i).PlayoutCount)))
                    End If
                    ucb1_array(i - 1) = u + q
                    i += 1
                End While

                max_index = Array.IndexOf(ucb1_array, ucb1_array.Max()) + 1

                DoMove(.BTree, .BTree.RootMoves(max_index - 1), .BTree.RootColor)
                If .NodeList(max_index).IsLeaf = True Then
                    elapsed = .sw.ElapsedMilliseconds
                    If .SearchTimeLimit < (elapsed + .TimeBuffer) Then
                        Exit While
                    End If
                    If .is_abort = True Then
                        Exit While
                    End If
                    If .NodeList(max_index).TrialCount >= .nthr Then
                        ExpandNode2(mcts_t, .BTree.RootColor Xor 1, max_index, .BTree.Ply + 1)
                        If .is_abort = True Then
                            Exit While
                        End If
                    Else
                        bt = Board.Init()
                        bt = DeepCopy(.BTree, False)
                        result = PlayOut(mcts_t, bt, .BTree.RootColor Xor 1, max_index, .BTree.Ply + 1)
                        EvalNode2(mcts_t, max_index, .BTree.RootColor Xor 1)
                        If .is_abort = True Then
                            Exit While
                        End If
                        UpdateParam(mcts_t, max_index, result)
                    End If
                Else

                    elapsed = .sw.ElapsedMilliseconds
                    If .SearchTimeLimit < (elapsed + .TimeBuffer) Then
                        Exit While
                    End If
                    If .is_abort = True Then
                        Exit While
                    End If
                    DescendNode2(mcts_t, .BTree.RootColor Xor 1, max_index, .nthr, .BTree.Ply + 1)
                End If
                UnDoMove(.BTree, .BTree.RootMoves(max_index - 1), .BTree.RootColor)
            End While
        End With
    End Sub

    Private Function MakeInputVectors(ByRef mcts_t As MCTSTree, ByVal parent_index As Integer) As String
        Dim str_out As String = ""
        Dim current_node As Node
        Dim m As Move
        Dim sq As Integer
        Dim temp_label As Label
        Dim h As Integer
        Dim temp_index As Integer = parent_index
        With mcts_t
            While True
                current_node = .NodeList(temp_index)
                m = current_node.move
                sq = GetTo(m)
                temp_label = MakeOutputLSTMLabel(m, sq)
                h = (temp_label << 7) Or sq
                If current_node.ParentIndex = 0 Then
                    str_out = str_out & h.ToString()
                    Exit While
                Else
                    str_out = str_out & h.ToString() & ","
                    temp_index = current_node.ParentIndex
                End If
            End While
        End With
        Return str_out
    End Function

    Private Sub ExpandNode(ByRef mcts_t As MCTSTree, ByVal c As Integer, ByVal parent_index As Integer, ByVal ply As Integer)
        Dim mate_move As Move
        Dim bb_ret As BitBoard
        Dim iret As Integer
        Dim evasion_moves As List(Of Move)
        Dim current_index As Integer
        Dim flag As Boolean
        Dim str_throw As String
        Dim str_receive As String
        Dim s As String()
        Dim moves As List(Of Move)
        Dim n As Node
        Dim i As Integer
        Dim s2 As String()
        Dim m As Move
        Dim ifrom As Integer
        Dim ito As Integer
        Dim bt As BoardTree
        Dim result As Integer
        mate_move = 0
        With mcts_t
            bb_ret = IsAttacked(.BTree, .BTree.SQ_King(c), c)
            If bb_ret = 0 And MateIn1Ply(.BTree, c) <> 0 Then
                'the case of mate in 1 ply.
                .NodeList(parent_index).WinRateSum = 0.0F
                .NodeList(parent_index).LostRateSum = 1.0F
                .NodeList(parent_index).EvalCount += 1
                Return
            Else
                If IsAttacked(.BTree, .BTree.SQ_King(c Xor 1), c Xor 1) <> 0 Then
                    iret = IsDeclarationWin(.BTree)
                    If iret = c + 1 Then
                        .NodeList(parent_index).WinRateSum = 0.0F
                        .NodeList(parent_index).LostRateSum = 1.0F
                        .NodeList(parent_index).EvalCount += 1
                        Return
                    ElseIf (c = 0 And iret = 2) Or (c = 1 And iret = 1) Then
                        .NodeList(parent_index).WinRateSum = 1.0F
                        .NodeList(parent_index).LostRateSum = 0.0F
                        .NodeList(parent_index).EvalCount += 1
                        Return
                    End If
                End If
            End If
            If bb_ret > 0 Then
                evasion_moves = New List(Of Move)
                GenEvasion(.BTree, c, evasion_moves)
                If evasion_moves.Count = 0 Then
                    'this node is check mate.
                    .NodeList(parent_index).WinRateSum = 1.0F
                    .NodeList(parent_index).LostRateSum = 0.0F
                    .NodeList(parent_index).EvalCount += 1
                    Return
                End If
            End If
            flag = False
            str_throw = ToSFEN(.BTree, c)
            str_throw = "p," & str_throw
            .queue_to_main_thread_p.Enqueue(str_throw)
            While .queue_from_main_thread_p.Count = 0
                Thread.Sleep(1)
                If .is_abort = True Then
                    Return
                End If
            End While
            str_receive = .queue_from_main_thread_p.Dequeue() 'string data format is like "7776FU 0.65,2226FU 0.25,5556FU 0.1" .
            .queue_from_main_thread_p.Clear()
            'the case of mate
            If str_receive = "mate" Then
                .NodeList(parent_index).WinRateSum = 1.0F
                .NodeList(parent_index).LostRateSum = 0.0F
                .NodeList(parent_index).EvalCount += 1
                Return
            End If
            s = Split(str_receive, ",")
            moves = New List(Of Move)
            For i = 0 To s.Length - 1
                'n = New Node
                n = InitNode()
                n.color = c
                s2 = Split(s(i), " ")
                m = CSA2Move(.BTree, s2(0))
                ifrom = GetFrom(m)
                ito = GetTo(m)
                If ifrom < Square_NB Then
                    'the case of discovered check
                    If IsPinnedOnKing(.BTree, ifrom, Adirec(ifrom, ito), c) <> 0 Then
                        Continue For
                    End If
                End If
                If GetCapPiece(m) = Piece.King Then
                    'the case of capture opponent king
                    Continue For
                End If
                DoMove(.BTree, m, c)
                'the case of discovered check
                If IsAttacked(.BTree, .BTree.SQ_King(c), c) <> 0 Then
                    UnDoMove(.BTree, m, c)
                    Continue For
                End If
                n.ThisIndex = .NodeList.Count
                n.ParentIndex = .NodeList(parent_index).ThisIndex
                n.move = m
                moves.Add(m)

                .NodeList(parent_index).ChildIndexes.Add(n.ThisIndex)
                n.PolicyResult = CSng(s2(1)) 'already executed softmax function.
                .NodeList.Add(n)
                current_index = n.ThisIndex
                'bt = New BoardTree
                bt = Board.Init()
                bt = DeepCopy(.BTree, False)
                result = PlayOut(mcts_t, bt, c Xor 1, current_index, ply + 1)
                EvalNode(mcts_t, current_index, c Xor 1)
                UpdateParam(mcts_t, current_index, result)
                UnDoMove(.BTree, m, c)
                flag = True
            Next i
            If flag = True Then
                mcts_t.NodeList(parent_index).IsLeaf = False
            End If
        End With
    End Sub

    Private Sub ExpandNode2(ByRef mcts_t As MCTSTree, ByVal c As Integer, ByVal parent_index As Integer, ByVal ply As Integer)
        Dim mate_move As Move
        Dim bb_ret As BitBoard
        Dim iret As Integer
        Dim evasion_moves As List(Of Move)
        Dim current_index As Integer
        Dim flag As Boolean
        Dim str_throw As String
        Dim str_receive As String
        Dim s As String()
        Dim moves As List(Of Move)
        Dim n As Node
        Dim i As Integer
        Dim s2 As String()
        Dim m As Move
        Dim ifrom As Integer
        Dim ito As Integer
        Dim bt As BoardTree
        Dim result As Integer
        mate_move = 0
        With mcts_t
            bb_ret = IsAttacked(.BTree, .BTree.SQ_King(c), c)
            If bb_ret = 0 And MateIn1Ply(.BTree, c) <> 0 Then
                'the case of mate in 1 ply.
                .NodeList(parent_index).WinRateSum = 0.0F
                .NodeList(parent_index).LostRateSum = 1.0F
                .NodeList(parent_index).EvalCount += 1
                Return
            Else
                If IsAttacked(.BTree, .BTree.SQ_King(c Xor 1), c Xor 1) <> 0 Then
                    iret = IsDeclarationWin(.BTree)
                    If iret = c + 1 Then
                        .NodeList(parent_index).WinRateSum = 0.0F
                        .NodeList(parent_index).LostRateSum = 1.0F
                        .NodeList(parent_index).EvalCount += 1
                        Return
                    ElseIf (c = 0 And iret = 2) Or (c = 1 And iret = 1) Then
                        .NodeList(parent_index).WinRateSum = 1.0F
                        .NodeList(parent_index).LostRateSum = 0.0F
                        .NodeList(parent_index).EvalCount += 1
                        Return
                    End If
                End If
            End If
            If bb_ret > 0 Then
                evasion_moves = New List(Of Move)
                GenEvasion(.BTree, c, evasion_moves)
                If evasion_moves.Count = 0 Then
                    'this node is check mate.
                    .NodeList(parent_index).WinRateSum = 1.0F
                    .NodeList(parent_index).LostRateSum = 0.0F
                    .NodeList(parent_index).EvalCount += 1
                    Return
                End If
            End If
            flag = False
            str_throw = ToSFEN(.BTree, c)
            str_throw = str_throw & "," & .VectorList & MakeInputVectors(mcts_t, parent_index)
            str_throw = "p," & str_throw
            .queue_to_main_thread_p.Enqueue(str_throw)
            While .queue_from_main_thread_p.Count = 0
                Thread.Sleep(1)
                If .is_abort = True Then
                    Return
                End If
            End While
            str_receive = .queue_from_main_thread_p.Dequeue() 'string data format is like "7776FU 0.65,2226FU 0.25,5556FU 0.1" .
            .queue_from_main_thread_p.Clear()
            'the case of mate
            If str_receive = "mate" Then
                .NodeList(parent_index).WinRateSum = 1.0F
                .NodeList(parent_index).LostRateSum = 0.0F
                .NodeList(parent_index).EvalCount += 1
                Return
            End If
            s = Split(str_receive, ",")
            moves = New List(Of Move)
            For i = 0 To s.Length - 1
                'n = New Node
                n = InitNode()
                n.color = c
                s2 = Split(s(i), " ")
                m = CSA2Move(.BTree, s2(0))
                ifrom = GetFrom(m)
                ito = GetTo(m)
                If ifrom < Square_NB Then
                    'the case of discovered check
                    If IsPinnedOnKing(.BTree, ifrom, Adirec(ifrom, ito), c) <> 0 Then
                        Continue For
                    End If
                End If
                If GetCapPiece(m) = Piece.King Then
                    'the case of capture opponent king
                    Continue For
                End If
                DoMove(.BTree, m, c)
                'the case of discovered check
                If IsAttacked(.BTree, .BTree.SQ_King(c), c) <> 0 Then
                    UnDoMove(.BTree, m, c)
                    Continue For
                End If
                n.ThisIndex = .NodeList.Count
                n.ParentIndex = .NodeList(parent_index).ThisIndex
                n.move = m
                moves.Add(m)

                .NodeList(parent_index).ChildIndexes.Add(n.ThisIndex)
                n.PolicyResult = CSng(s2(1)) 'already executed softmax function.
                .NodeList.Add(n)
                current_index = n.ThisIndex
                'bt = New BoardTree
                bt = Board.Init()
                bt = DeepCopy(.BTree, False)
                result = PlayOut(mcts_t, bt, c Xor 1, current_index, ply + 1)
                EvalNode2(mcts_t, current_index, c Xor 1)
                UpdateParam(mcts_t, current_index, result)
                UnDoMove(.BTree, m, c)
                flag = True
            Next i
            If flag = True Then
                mcts_t.NodeList(parent_index).IsLeaf = False
            End If
        End With
    End Sub

    Public Function PlayOut(ByRef mcts_t As MCTSTree, ByRef temp_bt As BoardTree, ByVal start_color As Integer, ByVal node_index As Integer, ByVal temp_ply As Integer) As Integer
        Const ply_max As Integer = 384
        Dim result As Integer
        Dim ply As Integer
        Dim c As Integer
        Dim move_list As List(Of Move)
        Dim legal_move_list As List(Of Move)
        Dim mate_move As Move
        Dim iret As Integer
        Dim i As Integer
        Dim ifrom As Integer
        Dim ito As Integer
        Dim r As Random
        Dim n As Integer
        result = 2 'initialize draw.
        ply = temp_ply
        c = start_color
        move_list = New List(Of Move)
        legal_move_list = New List(Of Move)
        r = New Random
        'occasionally, raise error.
        Try
            With mcts_t
                While ply < ply_max
                    move_list.Clear()
                    If IsAttacked(temp_bt, temp_bt.SQ_King(c), c) = 0 Then
                        mate_move = 0
                        'look up mate in 1 ply.
                        If MateIn1Ply(temp_bt, c) <> 0 Then
                            If c = temp_bt.RootColor Then
                                result = 0 'turn of root wins.
                            Else
                                result = 1 'turn of opponent wins.
                            End If
                            Exit While
                        End If
                        'look up declaration win.
                        'both black and white side kings are not checked.
                        If IsAttacked(temp_bt, temp_bt.SQ_King(c Xor 1), c Xor 1) = 0 Then
                            iret = IsDeclarationWin(temp_bt)
                            If iret = 1 And c = temp_bt.RootColor And c = 0 Then
                                result = 0 'turn of root wins and turn of root is black
                                Exit While
                            ElseIf iret = 2 And c = temp_bt.RootColor And c = 1 Then
                                result = 1 'turn of opponent wins and turn of root is white
                                Exit While
                            End If
                        End If
                        GenCap(temp_bt, c, move_list)
                        GenNoCap(temp_bt, c, move_list)
                        GenDrop(temp_bt, c, move_list)
                    Else
                        GenEvasion(temp_bt, c, move_list)
                    End If
                    legal_move_list.Clear()
                    'remove illegal move.
                    For i = 0 To move_list.Count - 1
                        ifrom = GetFrom(move_list(i))
                        ito = GetTo(move_list(i))
                        If ifrom < Square_NB Then
                            'the case of discovered check
                            If IsPinnedOnKing(temp_bt, ifrom, Adirec(ifrom, ito), c) <> 0 Then
                                Continue For
                            End If
                        End If
                        If GetCapPiece(move_list(i)) = Piece.King Then
                            'the case of capture opponent king
                            Continue For
                        End If
                        legal_move_list.Add(move_list(i))
                    Next i
                    If legal_move_list.Count = 0 Then
                        If c = temp_bt.RootColor Then
                            result = 0 'turn of root wins.
                        Else
                            result = 1 'turn of opponent wins.
                        End If
                        Exit While
                    End If
                    n = r.Next(0, legal_move_list.Count - 1)
                    DoMove(temp_bt, legal_move_list(n), c)
                    c = c Xor 1
                    ply += 1
                End While
                .NodeList(node_index).PlayoutCount += 1
                .PlayOutCount += 1
            End With
        Catch ex As Exception
            result = 2
        End Try
        Return result
    End Function
    Private Sub EvalNode(ByRef mcts_t As MCTSTree, ByVal node_index As Integer, ByVal c As Integer)
        Dim f As Single
        Dim str_throw As String
        Dim str_receive As String
        Dim v As Single
        With mcts_t
            If .tt.value.TryGetValue(.BTree.CurrentHash, f) <> False Then
                .NodeList(node_index).WinRateSum = 1 - f
                .NodeList(node_index).LostRateSum = f
                .NodeList(node_index).EvalCount += 1
                Return
            End If
            str_throw = ToSFEN(.BTree, c)
            str_throw = "v," & str_throw
            .queue_to_main_thread_v.Enqueue(str_throw)
            While .queue_from_main_thread_v.Count = 0
                Thread.Sleep(1)
                If .is_abort = True Then
                    Return
                End If
            End While
            str_receive = .queue_from_main_thread_v.Dequeue() 'string data format is like "0.65" .
            .queue_from_main_thread_v.Clear()
            v = CSng(str_receive)
            .NodeList(node_index).WinRateSum = 1 - v
            .NodeList(node_index).LostRateSum = v
            .NodeList(node_index).EvalCount += 1
            .tt.value.TryAdd(.BTree.CurrentHash, v)
        End With
    End Sub

    Private Sub EvalNode2(ByRef mcts_t As MCTSTree, ByVal node_index As Integer, ByVal c As Integer)
        Dim f As Single
        Dim str_throw As String
        Dim str_receive As String
        Dim v As Single
        With mcts_t
            If .tt.value.TryGetValue(.BTree.CurrentHash, f) <> False Then
                .NodeList(node_index).WinRateSum = 1 - f
                .NodeList(node_index).LostRateSum = f
                .NodeList(node_index).EvalCount += 1
                Return
            End If
            str_throw = ToSFEN(.BTree, c)
            str_throw = str_throw & "," & mcts_t.VectorList & MakeInputVectors(mcts_t, node_index)
            str_throw = "v," & str_throw
            .queue_to_main_thread_v.Enqueue(str_throw)
            While .queue_from_main_thread_v.Count = 0
                Thread.Sleep(1)
                If .is_abort = True Then
                    Return
                End If
            End While
            str_receive = .queue_from_main_thread_v.Dequeue() 'string data format is like "0.65" .
            .queue_from_main_thread_v.Clear()
            v = CSng(str_receive)
            .NodeList(node_index).WinRateSum = 1 - v
            .NodeList(node_index).LostRateSum = v
            .NodeList(node_index).EvalCount += 1
            .tt.value.TryAdd(.BTree.CurrentHash, v)
        End With
    End Sub
    Private Sub UpdateParam(ByRef mcts_t As MCTSTree, ByVal node_index As Integer, ByVal result As Integer)
        Dim current_node As Node
        Dim delta As Single
        Dim delta2 As Single
        Dim index As Integer
        With mcts_t
            .NodeList(node_index).TrialCount += 1
            If result = 0 Then
                If .NodeList(node_index).color = .BTree.RootColor Then
                    .NodeList(node_index).WinCount += 1
                Else
                    .NodeList(node_index).LostCount += 1
                End If
            ElseIf result = 1 Then
                If .NodeList(node_index).color = .BTree.RootColor Then
                    .NodeList(node_index).LostCount += 1
                Else
                    .NodeList(node_index).WinCount += 1
                End If
            Else
                .NodeList(node_index).DrawCount += 1
            End If
            current_node = .NodeList(node_index)
            If current_node.color = .BTree.RootColor Then
                delta = .NodeList(node_index).WinRateSum
                delta2 = .NodeList(node_index).LostRateSum
            Else
                delta = .NodeList(node_index).LostRateSum
                delta2 = .NodeList(node_index).WinRateSum
            End If
            If current_node.ParentIndex = 0 Then
                Return
            End If
            While True
                index = current_node.ParentIndex
                current_node = .NodeList(index)
                .NodeList(index).TrialCount += 1
                .NodeList(index).PlayoutCount += 1
                If result = 0 Then
                    If .NodeList(index).color = .BTree.RootColor Then
                        .NodeList(index).WinCount += 1
                    Else
                        .NodeList(index).LostCount += 1
                    End If
                ElseIf result = 1 Then
                    If .NodeList(index).color = .BTree.RootColor Then
                        .NodeList(index).LostCount += 1
                    Else
                        .NodeList(index).WinCount += 1
                    End If
                Else
                    .NodeList(index).DrawCount += 1
                End If
                If current_node.color = .BTree.RootColor Then
                    .NodeList(index).WinRateSum += delta
                    .NodeList(index).LostRateSum += delta2
                Else
                    .NodeList(index).WinRateSum += delta2
                    .NodeList(index).LostRateSum += delta
                End If
                .NodeList(index).EvalCount += 1
                If current_node.ParentIndex = 0 Then
                    Exit While
                End If
            End While
        End With
    End Sub
    Private Sub DescendNode(ByRef mcts_t As MCTSTree, ByVal c As Integer, ByVal node_index As Integer, ByVal nthr As Integer, ByVal ply As Integer)
        Dim idx As Integer
        Dim flag As Boolean
        Dim elapsed As Long
        Dim t As Integer
        Dim i As Integer
        Dim u As Single
        Dim q As Single
        Dim ucb1_array As Single()
        Dim max_index As Integer
        Dim bt As BoardTree
        Dim result As Integer
        Dim current_node As Node
        Dim temp_color As Integer
        Dim index As Integer
        With mcts_t
            If .NodeList(node_index).ChildIndexes.Count = 0 Then
                Return
            End If
            idx = 0
            flag = False
            While True
                elapsed = mcts_t.sw.ElapsedMilliseconds
                If .SearchTimeLimit < (elapsed + .TimeBuffer) Then
                    Exit While
                End If
                If .is_abort = True Then
                    Exit While
                End If
                ucb1_array = New Single(.NodeList(node_index).ChildIndexes.Count) {}
                t = 0
                i = 0
                While i < .NodeList(node_index).ChildIndexes.Count
                    idx = .NodeList(node_index).ChildIndexes(i)
                    t += .NodeList(idx).PlayoutCount
                    i += 1
                End While
                i = 0
                While i < .NodeList(node_index).ChildIndexes.Count
                    idx = .NodeList(node_index).ChildIndexes(i)
                    u = .NodeList(idx).PolicyResult * CSng(Math.Sqrt(t)) / CSng(.NodeList(idx).PlayoutCount + 1)
                    q = 0.0F
                    If .NodeList(idx).EvalCount > 0 And .NodeList(idx).PlayoutCount > 0 Then
                        q = (1 - .value_lambda) * (.NodeList(idx).WinRateSum / CSng(.NodeList(idx).EvalCount)) + .value_lambda * (CSng(.NodeList(idx).WinCount) / (CSng(.NodeList(idx).PlayoutCount)))
                    End If
                    ucb1_array(i) = u + q
                    i += 1
                End While
                max_index = Array.IndexOf(ucb1_array, ucb1_array.Max())
                idx = .NodeList(node_index).ChildIndexes(max_index)
                DoMove(.BTree, .NodeList(idx).move, c)
                If .NodeList(idx).IsLeaf = True Then
                    elapsed = .sw.ElapsedMilliseconds
                    If .SearchTimeLimit < (elapsed + .TimeBuffer) Then
                        flag = True
                        GoTo end_label
                    End If

                    If .is_abort = True Then
                        Exit While
                    End If

                    If .NodeList(idx).TrialCount >= .nthr Then
                        ExpandNode(mcts_t, c Xor 1, idx, ply + 1)
                        If .is_abort = True Then
                            Exit While
                        End If
                    Else
                        'bt = New BoardTree()
                        bt = Board.Init()
                        bt = DeepCopy(.BTree, False)
                        result = PlayOut(mcts_t, bt, c Xor 1, idx, ply + 1)
                        EvalNode(mcts_t, idx, c Xor 1)
                        If .is_abort = True Then
                            Exit While
                        End If
                        UnDoMove(.BTree, .NodeList(idx).move, c)
                        AscendNode(mcts_t, c Xor 1, idx, result)
                        Return
                    End If
                Else
                    elapsed = .sw.ElapsedMilliseconds
                    If .SearchTimeLimit < elapsed + .TimeBuffer Then
                        flag = True
                        GoTo end_label
                    End If

                    If .is_abort = True Then
                        Exit While
                    End If


                    DescendNode(mcts_t, c Xor 1, idx, nthr, ply + 1)
                    Return
                End If

                UnDoMove(.BTree, .NodeList(idx).move, c)
end_label:
                If flag = True Then
                    current_node = .NodeList(idx)
                    temp_color = c Xor 1
                    While True
                        index = current_node.ParentIndex
                        current_node = .NodeList(index)
                        If current_node.ParentIndex = 0 Then
                            Exit While
                        End If
                        UnDoMove(.BTree, current_node.move, temp_color)
                        temp_color = temp_color Xor 1
                    End While
                    Exit While
                End If
            End While
        End With
    End Sub

    Private Sub DescendNode2(ByRef mcts_t As MCTSTree, ByVal c As Integer, ByVal node_index As Integer, ByVal nthr As Integer, ByVal ply As Integer)
        Dim idx As Integer
        Dim flag As Boolean
        Dim elapsed As Long
        Dim t As Integer
        Dim i As Integer
        Dim u As Single
        Dim q As Single
        Dim ucb1_array As Single()
        Dim max_index As Integer
        Dim bt As BoardTree
        Dim result As Integer
        Dim current_node As Node
        Dim temp_color As Integer
        Dim index As Integer
        With mcts_t
            If .NodeList(node_index).ChildIndexes.Count = 0 Then
                Return
            End If
            idx = 0
            flag = False
            While True
                elapsed = mcts_t.sw.ElapsedMilliseconds
                If .SearchTimeLimit < (elapsed + .TimeBuffer) Then
                    Exit While
                End If
                If .is_abort = True Then
                    Exit While
                End If
                ucb1_array = New Single(.NodeList(node_index).ChildIndexes.Count) {}
                t = 0
                i = 0
                While i < .NodeList(node_index).ChildIndexes.Count
                    idx = .NodeList(node_index).ChildIndexes(i)
                    t += .NodeList(idx).PlayoutCount
                    i += 1
                End While
                i = 0
                While i < .NodeList(node_index).ChildIndexes.Count
                    idx = .NodeList(node_index).ChildIndexes(i)
                    u = .NodeList(idx).PolicyResult * CSng(Math.Sqrt(t)) / CSng(.NodeList(idx).PlayoutCount + 1)
                    q = 0.0F
                    If .NodeList(idx).EvalCount > 0 And .NodeList(idx).PlayoutCount > 0 Then
                        q = (1 - .value_lambda) * (.NodeList(idx).WinRateSum / CSng(.NodeList(idx).EvalCount)) + .value_lambda * (CSng(.NodeList(idx).WinCount) / (CSng(.NodeList(idx).PlayoutCount)))
                    End If
                    ucb1_array(i) = u + q
                    i += 1
                End While
                max_index = Array.IndexOf(ucb1_array, ucb1_array.Max())
                idx = .NodeList(node_index).ChildIndexes(max_index)
                DoMove(.BTree, .NodeList(idx).move, c)
                If .NodeList(idx).IsLeaf = True Then
                    elapsed = .sw.ElapsedMilliseconds
                    If .SearchTimeLimit < (elapsed + .TimeBuffer) Then
                        flag = True
                        GoTo end_label
                    End If

                    If .is_abort = True Then
                        Exit While
                    End If

                    If .NodeList(idx).TrialCount >= .nthr Then
                        ExpandNode2(mcts_t, c Xor 1, idx, ply + 1)
                        If .is_abort = True Then
                            Exit While
                        End If
                    Else
                        'bt = New BoardTree()
                        bt = Board.Init()
                        bt = DeepCopy(.BTree, False)
                        result = PlayOut(mcts_t, bt, c Xor 1, idx, ply + 1)
                        EvalNode2(mcts_t, idx, c Xor 1)
                        If .is_abort = True Then
                            Exit While
                        End If
                        UnDoMove(.BTree, .NodeList(idx).move, c)
                        AscendNode(mcts_t, c Xor 1, idx, result)
                        Return
                    End If
                Else
                    elapsed = .sw.ElapsedMilliseconds
                    If .SearchTimeLimit < elapsed + .TimeBuffer Then
                        flag = True
                        GoTo end_label
                    End If

                    If .is_abort = True Then
                        Exit While
                    End If


                    DescendNode2(mcts_t, c Xor 1, idx, nthr, ply + 1)
                    Return
                End If

                UnDoMove(.BTree, .NodeList(idx).move, c)
end_label:
                If flag = True Then
                    current_node = .NodeList(idx)
                    temp_color = c Xor 1
                    While True
                        index = current_node.ParentIndex
                        current_node = .NodeList(index)
                        If current_node.ParentIndex = 0 Then
                            Exit While
                        End If
                        UnDoMove(.BTree, current_node.move, temp_color)
                        temp_color = temp_color Xor 1
                    End While
                    Exit While
                End If
            End While
        End With
    End Sub


    Private Sub AscendNode(ByRef mcts_t As MCTSTree, ByVal c As Integer, ByVal node_index As Integer, ByVal result As Integer)
        Dim current_node As Node
        Dim delta As Single
        Dim delta2 As Single
        Dim temp_color As Integer
        Dim index As Integer
        With mcts_t
            .NodeList(node_index).TrialCount += 1
            If result = 0 Then
                If .NodeList(node_index).color = .BTree.RootColor Then
                    .NodeList(node_index).WinCount += 1
                Else
                    .NodeList(node_index).LostCount += 1
                End If
            ElseIf result = 1 Then
                If .NodeList(node_index).color = .BTree.RootColor Then
                    .NodeList(node_index).LostCount += 1
                Else
                    .NodeList(node_index).WinCount += 1
                End If
            Else
                .NodeList(node_index).DrawCount += 1
            End If
            current_node = .NodeList(node_index)
            If current_node.color = .BTree.RootColor Then
                delta = .NodeList(node_index).WinRateSum
                delta2 = .NodeList(node_index).LostRateSum
            Else
                delta = .NodeList(node_index).LostRateSum
                delta2 = .NodeList(node_index).WinRateSum
            End If
            If current_node.ParentIndex = 0 Then
                Return
            End If
            temp_color = c
            While True
                index = current_node.ParentIndex
                current_node = .NodeList(index)
                .NodeList(index).TrialCount += 1
                .NodeList(index).PlayoutCount += 1
                If result = 0 Then
                    If .NodeList(index).color = .BTree.RootColor Then
                        .NodeList(index).WinCount += 1
                    Else
                        .NodeList(index).LostCount += 1
                    End If
                ElseIf result = 1 Then
                    If .NodeList(index).color = .BTree.RootColor Then
                        .NodeList(index).LostCount += 1
                    Else
                        .NodeList(index).WinCount += 1
                    End If
                Else
                    .NodeList(index).DrawCount += 1
                End If

                If current_node.color = .BTree.RootColor Then
                    .NodeList(index).WinRateSum += delta
                    .NodeList(index).LostRateSum += delta2
                Else
                    .NodeList(index).WinRateSum += delta2
                    .NodeList(index).LostRateSum += delta
                End If

                .NodeList(index).EvalCount += 1
                If current_node.ParentIndex = 0 Then
                    Exit While
                End If
                UnDoMove(.BTree, current_node.move, temp_color)
                temp_color = temp_color Xor 1
            End While
        End With
    End Sub
End Module
