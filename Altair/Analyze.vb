Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.Json
Imports Microsoft.ML.OnnxRuntime
Imports Microsoft.ML.OnnxRuntime.Tensors
Imports Move = System.UInt32

Module Analyze
    Public sessionOptions As New SessionOptions()
    Public policy_session As InferenceSession
    Public value_session As InferenceSession
    'sessionOptions.AppendExecutionProvider_CUDA(deviceId:=0) ' deviceId=0 は最初のGPU

    '畳み込みニューラルネットワークを用いて棋譜の解析を行う。
    Public Sub AnalyzeRecord(ByVal num_tasks As Integer, ByVal num_mate_tasks As Integer, ByVal thinking_time As Integer, ByVal mate_search_depth As Integer, ByVal policy_network_threshold As Integer, ByVal value_lambda As Single, ByVal analyze_file_name As String, ByVal record_file_name As String,
                             ByVal str_game_date As String, ByVal str_match_name As String, ByVal str_black_player As String, ByVal str_white_player As String)
        Dim AppPath As String
        Dim FilePath As String
        Dim r As Record
        Dim records As List(Of Record)
        Dim sw As StreamWriter
        Dim enc As UTF8Encoding
        Dim i As Integer
        Dim denomi As Integer
        Dim denomi_black As Integer
        Dim denomi_white As Integer
        Dim move_first_accuracy As Integer() = New Integer(1) {}
        Dim move_second_accuracy As Integer() = New Integer(1) {}
        Dim move_third_accurasy As Integer() = New Integer(1) {}
        Dim m As Move
        Dim bt As BoardTree
        Dim c As Integer
        Dim str_result As String
        Dim str_mate_pv As String
        Dim mate_first_move As Move
        LoadModel()
        AppPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
        records = ReadRecords(record_file_name)
        r = records(0)
        FilePath = AppPath & "\\" & analyze_file_name
        enc = New UTF8Encoding(False)
        'ToDo: ログの出力フォーマットはなるべくAsklepiosと同じにする。
        sw = New StreamWriter(FilePath, False, enc)
        sw.WriteLine("対局日：" & str_game_date)
        sw.WriteLine()
        sw.WriteLine("棋戦名：" & str_match_name)
        sw.WriteLine()
        sw.WriteLine("先手：" & str_black_player)
        sw.WriteLine()
        sw.WriteLine("後手：" & str_white_player)
        sw.WriteLine()
        bt = Board.Init()
        c = Color.Black
        str_result = ""
        str_mate_pv = ""
        denomi = r.str_moves.Count
        denomi_black = 0
        denomi_white = 0
        For i = 0 To r.str_moves.Count - 1
            'If i <> 77 Then
            'GoTo a
            'End If
            bt.RootColor = c
            str_result = ""
            mate_first_move = 0
            Console.WriteLine("ply = " & (i + 1).ToString())
            SearchWrapper(bt, num_tasks, num_mate_tasks, thinking_time, mate_search_depth, policy_network_threshold, value_lambda, str_result, str_mate_pv, r.str_moves(i), move_first_accuracy, move_second_accuracy, move_third_accurasy, mate_first_move)
            If str_mate_pv <> "" Then
                Dim str_accurate As String = "×"
                Dim str_color As String = "+"
                If c = Color.White Then
                    str_color = "-"
                End If
                If r.str_moves(i) = Move2CSA(mate_first_move) Then
                    str_accurate = "○"
                    move_first_accuracy(c) += 1
                Else
                    Dim s As String = str_mate_pv.Substring(1, 6)
                    If r.str_moves(i) = s Then
                        str_accurate = "○"
                        move_first_accuracy(c) += 1
                    End If
                End If
                str_mate_pv = "ply=" & (i + 1).ToString() & ", 棋譜の手: " & str_color & r.str_moves(i) & ", result= " & str_accurate & ", 詰みあり： " & str_mate_pv
                sw.WriteLine(str_mate_pv)
            Else
                Dim str_color As String = "+"
                If c = Color.White Then
                    str_color = "-"
                End If
                str_result = "ply=" & (i + 1).ToString() & ", 棋譜の手: " & str_color & r.str_moves(i) & ", " & str_result
                sw.WriteLine(str_result)
            End If
            'If i = 118 Then
            'SearchWrapper(bt, num_tasks, num_mate_tasks, thinking_time, mate_search_depth, str_result, str_mate_pv, r.str_moves(i), move_first_accuracy, move_second_accuracy, move_third_accurasy, mate_first_move)
            '    If r.str_moves(i) = Move2CSA(mate_first_move) Then
            '        Dim str_accurate = "○"
            '        move_first_accuracy(c) += 1
            '    End If
            'End If
            'a:
            m = CSA2Move(bt, r.str_moves(i))
            DoMove(bt, m, c)
            c = c Xor 1
            If c = Color.Black Then
                denomi_black += 1
            Else
                denomi_white += 1
            End If
            'Exit For
        Next i

        sw.WriteLine()

        Dim temp_n As Integer
        Dim temp_s As String
        'temp_n = move_first_accuracy(0) + move_second_accuracy(0) + move_third_accurasy(0)
        temp_n = move_first_accuracy(0)
        temp_s = temp_n.ToString() & " / " & denomi_black.ToString() & " = " & (temp_n / denomi_black).ToString("P2")
        sw.WriteLine("先手一致率：" & temp_s)
        sw.WriteLine()
        'temp_n = move_first_accuracy(1) + move_second_accuracy(1) + move_third_accurasy(1)
        temp_n = move_first_accuracy(1)
        temp_s = temp_n.ToString() & " / " & denomi_white.ToString() & " = " & (temp_n / denomi_white).ToString("P2")
        sw.WriteLine("後手一致率：" & temp_s)
        sw.WriteLine()
        temp_n = move_first_accuracy(0) + move_first_accuracy(1)
        temp_s = temp_n.ToString() & " / " & denomi.ToString() & " = " & (temp_n / denomi).ToString("P2")
        sw.WriteLine("全体一致率：" & temp_s)
        sw.WriteLine()
        temp_n = move_first_accuracy(0) + move_second_accuracy(0) + move_third_accurasy(0) + move_first_accuracy(1) + move_second_accuracy(1) + move_third_accurasy(1)
        temp_s = temp_n.ToString() & " / " & denomi.ToString() & " = " & (temp_n / denomi).ToString("P2")
        sw.WriteLine("3位以内の確率：" & temp_s)
        sw.WriteLine()
        sw.WriteLine("解析エンジン名：Altair Ver.1.1.0")
        sw.Close()
    End Sub

    '時系列ネットワークを用いて棋譜の解析を行う。
    Public Sub AnalyzeRecord2(ByVal num_tasks As Integer, ByVal num_mate_tasks As Integer, ByVal thinking_time As Integer, ByVal mate_search_depth As Integer, ByVal policy_network_threshold As Integer, ByVal value_lambda As Single, ByVal analyze_file_name As String, ByVal record_file_name As String,
                             ByVal str_game_date As String, ByVal str_match_name As String, ByVal str_black_player As String, ByVal str_white_player As String, use_gru As Boolean)
        Dim AppPath As String
        Dim FilePath As String
        Dim r As Record
        Dim records As List(Of Record)
        Dim sw As StreamWriter
        Dim enc As UTF8Encoding
        Dim i As Integer
        Dim denomi As Integer
        Dim denomi_black As Integer
        Dim denomi_white As Integer
        Dim move_first_accuracy As Integer() = New Integer(1) {}
        Dim move_second_accuracy As Integer() = New Integer(1) {}
        Dim move_third_accurasy As Integer() = New Integer(1) {}
        Dim m As Move
        Dim bt As BoardTree
        Dim c As Integer
        Dim str_result As String
        Dim str_mate_pv As String
        Dim mate_first_move As Move
        Dim li_root_vectors As List(Of Integer) = New List(Of Integer)
        Dim h As Integer
        Dim temp_label As Label
        Dim sq As Integer
        If use_gru = False Then
            LoadModel2()
        Else
            LoadModel3()
        End If
        AppPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
        records = ReadRecords(record_file_name)
        r = records(0)
        FilePath = AppPath & "\\" & analyze_file_name
        enc = New UTF8Encoding(False)
        'ToDo: ログの出力フォーマットはなるべくAsklepiosと同じにする。
        sw = New StreamWriter(FilePath, False, enc)
        sw.WriteLine("対局日：" & str_game_date)
        sw.WriteLine()
        sw.WriteLine("棋戦名：" & str_match_name)
        sw.WriteLine()
        sw.WriteLine("先手：" & str_black_player)
        sw.WriteLine()
        sw.WriteLine("後手：" & str_white_player)
        sw.WriteLine()
        bt = Board.Init()
        c = Color.Black
        str_result = ""
        str_mate_pv = ""
        denomi = r.str_moves.Count
        denomi_black = 0
        denomi_white = 0
        For i = 0 To r.str_moves.Count - 1
            'If i <> 77 Then
            'GoTo a
            'End If
            bt.RootColor = c
            str_result = ""
            mate_first_move = 0
            Console.WriteLine("ply = " & (i + 1).ToString())
            SearchWrapper2(bt, num_tasks, num_mate_tasks, thinking_time, mate_search_depth, policy_network_threshold, value_lambda, str_result, str_mate_pv, r.str_moves(i), move_first_accuracy, move_second_accuracy, move_third_accurasy, mate_first_move, li_root_vectors)
            If str_mate_pv <> "" Then
                Dim str_accurate As String = "×"
                Dim str_color As String = "+"
                If c = Color.White Then
                    str_color = "-"
                End If
                If r.str_moves(i) = Move2CSA(mate_first_move) Then
                    str_accurate = "○"
                    move_first_accuracy(c) += 1
                Else
                    Dim s As String = str_mate_pv.Substring(1, 6)
                    If r.str_moves(i) = s Then
                        str_accurate = "○"
                        move_first_accuracy(c) += 1
                    End If
                End If
                str_mate_pv = "ply=" & (i + 1).ToString() & ", 棋譜の手: " & str_color & r.str_moves(i) & ", result= " & str_accurate & ", 詰みあり： " & str_mate_pv
                sw.WriteLine(str_mate_pv)
            Else
                Dim str_color As String = "+"
                If c = Color.White Then
                    str_color = "-"
                End If
                str_result = "ply=" & (i + 1).ToString() & ", 棋譜の手: " & str_color & r.str_moves(i) & ", " & str_result
                sw.WriteLine(str_result)
            End If
            'If i = 118 Then
            'SearchWrapper(bt, num_tasks, num_mate_tasks, thinking_time, mate_search_depth, str_result, str_mate_pv, r.str_moves(i), move_first_accuracy, move_second_accuracy, move_third_accurasy, mate_first_move)
            '    If r.str_moves(i) = Move2CSA(mate_first_move) Then
            '        Dim str_accurate = "○"
            '        move_first_accuracy(c) += 1
            '    End If
            'End If
            'a:
            'm = CSA2Move(bt, r.str_moves(i))
            m = CSA2Move(bt, r.str_moves(i))
            sq = GetTo(m)
            temp_label = MakeOutputLSTMLabel(m, sq)
            h = (temp_label << 7) Or sq
            If li_root_vectors.Count = 128 Then
                li_root_vectors.RemoveAt(0)
            End If
            li_root_vectors.Add(h)
            DoMove(bt, m, c)
            c = c Xor 1
            If c = Color.Black Then
                denomi_black += 1
            Else
                denomi_white += 1
            End If
            'Exit For
        Next i

        sw.WriteLine()

        Dim temp_n As Integer
        Dim temp_s As String
        'temp_n = move_first_accuracy(0) + move_second_accuracy(0) + move_third_accurasy(0)
        temp_n = move_first_accuracy(0)
        temp_s = temp_n.ToString() & " / " & denomi_black.ToString() & " = " & (temp_n / denomi_black).ToString("P2")
        sw.WriteLine("先手一致率：" & temp_s)
        sw.WriteLine()
        'temp_n = move_first_accuracy(1) + move_second_accuracy(1) + move_third_accurasy(1)
        temp_n = move_first_accuracy(1)
        temp_s = temp_n.ToString() & " / " & denomi_white.ToString() & " = " & (temp_n / denomi_white).ToString("P2")
        sw.WriteLine("後手一致率：" & temp_s)
        sw.WriteLine()
        temp_n = move_first_accuracy(0) + move_first_accuracy(1)
        temp_s = temp_n.ToString() & " / " & denomi.ToString() & " = " & (temp_n / denomi).ToString("P2")
        sw.WriteLine("全体一致率：" & temp_s)
        sw.WriteLine()
        temp_n = move_first_accuracy(0) + move_second_accuracy(0) + move_third_accurasy(0) + move_first_accuracy(1) + move_second_accuracy(1) + move_third_accurasy(1)
        temp_s = temp_n.ToString() & " / " & denomi.ToString() & " = " & (temp_n / denomi).ToString("P2")
        sw.WriteLine("3位以内の確率：" & temp_s)
        sw.WriteLine()
        sw.WriteLine("解析エンジン名：Altair Ver.1.1.0")
        sw.Close()
    End Sub

    '畳み込みニューラルネットワークのモデルをロードする。
    Public Sub LoadModel()
        sessionOptions.AppendExecutionProvider_CUDA(deviceId:=0)
        policy_session = New InferenceSession("model.onnx", sessionOptions)
        value_session = New InferenceSession("model_value.onnx", sessionOptions)

        'warming up model
        'GPUにデータを飛ばした後の初回実行は時間がかかるので、ウォーミングアップする。
        Dim inputData(1 * 105 * 9 * 9 - 1) As Single
        Dim inputTensor = New DenseTensor(Of Single)(inputData, New Integer() {1, 105, 9, 9})
        Dim inputName = policy_session.InputMetadata.Keys.First()

        ' prediction
        Using results = policy_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                Console.WriteLine($"Output: {result.Name}")
                Dim outputTensor = result.AsTensor(Of Single)()
                Console.WriteLine($"First value: {outputTensor(0)}")
            Next
        End Using

        inputName = value_session.InputMetadata.Keys.First()
        ' prediction
        Using results = value_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                Console.WriteLine($"Output: {result.Name}")
                Dim outputTensor = result.AsTensor(Of Single)()
                Console.WriteLine($"First value: {outputTensor(0)}")
            Next
        End Using
    End Sub

    'LSTMのモデルをロードする。
    Public Sub LoadModel2()
        sessionOptions.AppendExecutionProvider_CUDA(deviceId:=0)
        policy_session = New InferenceSession("model_lstm.onnx", sessionOptions)
        value_session = New InferenceSession("model_lstm_value.onnx", sessionOptions)

        'warming up model
        'GPUにデータを飛ばした後の初回実行は時間がかかるので、ウォーミングアップする。
        Dim inputData(1 * 128 - 1) As Integer
        Dim inputTensor = New DenseTensor(Of Integer)(inputData, New Integer() {1, 128})
        Dim inputName = policy_session.InputMetadata.Keys.First()

        ' prediction
        Using results = policy_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                Console.WriteLine($"Output: {result.Name}")
                Dim outputTensor = result.AsTensor(Of Single)()
                Console.WriteLine($"First value: {outputTensor(0)}")
            Next
        End Using

        inputName = value_session.InputMetadata.Keys.First()
        ' prediction
        Using results = value_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                Console.WriteLine($"Output: {result.Name}")
                Dim outputTensor = result.AsTensor(Of Single)()
                Console.WriteLine($"First value: {outputTensor(0)}")
            Next
        End Using
    End Sub

    'GRUのモデルをロードする。ただしValue NetworkはLSTMのままである。
    Public Sub LoadModel3()
        sessionOptions.AppendExecutionProvider_CUDA(deviceId:=0)
        policy_session = New InferenceSession("model_gru.onnx", sessionOptions)

        'Value NetworkはまだGRU版ができていないので、LSTMにしておく。
        value_session = New InferenceSession("model_lstm_value.onnx", sessionOptions)

        'warming up model
        'GPUにデータを飛ばした後の初回実行は時間がかかるので、ウォーミングアップする。
        Dim inputData(1 * 128 - 1) As Integer
        Dim inputTensor = New DenseTensor(Of Integer)(inputData, New Integer() {1, 128})
        Dim inputName = policy_session.InputMetadata.Keys.First()

        ' prediction
        Using results = policy_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                Console.WriteLine($"Output: {result.Name}")
                Dim outputTensor = result.AsTensor(Of Single)()
                Console.WriteLine($"First value: {outputTensor(0)}")
            Next
        End Using

        inputName = value_session.InputMetadata.Keys.First()
        ' prediction
        Using results = value_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                Console.WriteLine($"Output: {result.Name}")
                Dim outputTensor = result.AsTensor(Of Single)()
                Console.WriteLine($"First value: {outputTensor(0)}")
            Next
        End Using
    End Sub

    'Copilot先生に作ってもらったテスト用コード
    Public Sub Test0()
        Dim sessionOptions As New SessionOptions()
        sessionOptions.AppendExecutionProvider_CUDA()
        Dim session As New InferenceSession("model.onnx", sessionOptions)
        Dim batchSize As Integer = 2
        Dim inputHeight As Integer = 9
        Dim inputWidth As Integer = 9
        Dim channels As Integer = 105
        Dim inputTensor = New DenseTensor(Of Single)(New Integer() {batchSize, channels, inputHeight, inputWidth})
        Dim inputName = session.InputMetadata.Keys.First()
        Dim inputs = New List(Of NamedOnnxValue) From {NamedOnnxValue.CreateFromTensor(inputName, inputTensor)}
        Dim results = session.Run(inputs)
        Dim outputTensor = results.First().AsTensor(Of Single)()
        For i = 0 To batchSize - 1
            Console.WriteLine($"Batch {i} の結果を処理") ' outputTensor(i, ...) を取り出して処理
        Next i
    End Sub

    '畳み込みニューラルネットワークを用いてルートのPolicy Networkの出力をセットする。
    Private Sub SetRootOutput(ByRef m As MCTSTree, ByVal policy_network_threshold As Integer)
        Dim inputData(1 * 105 * 9 * 9 - 1) As Single
        m.RootOutput = New Single(m.BTree.RootMoves.Length - 1) {}
        inputData = MakeInputFeatures(m.BTree, m.BTree.RootColor)
        Dim inputTensor = New DenseTensor(Of Single)(inputData, New Integer() {1, 105, 9, 9})
        Dim inputName = policy_session.InputMetadata.Keys.First()
        Dim outputTensor As Tensor(Of Single)
        Dim i As Integer
        Dim lbl As Label
        'Dim s As String
        Dim li_v = New List(Of Single)
        Dim newShape As Integer() = {1, 32, 81}
        Dim tempRootOutput = New Single(m.BTree.RootMoves.Length - 1) {}
        Dim limit As Integer = policy_network_threshold
        Dim idxes = New List(Of Integer)
        Dim temp_index As Integer
        Dim temp_values = New List(Of Single)
        Dim temp_value As Single
        Dim temp_moves = New List(Of Move)
        Dim outputs_sum As Single

        ' prediction
        Using results = policy_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                outputTensor = result.AsTensor(Of Single)
            Next
        End Using

        outputTensor = outputTensor.Reshape(newShape)

        li_v = New List(Of Single)
        For i = 0 To m.BTree.RootMoves.Length - 1
            's = Move2CSA(m.BTree.RootMoves(i))
            lbl = MakeOutputLabel(m.BTree.RootMoves(i))
            Dim v = outputTensor(0, lbl, GetTo(m.BTree.RootMoves(i)))
            li_v.Add(v)
        Next i

        For i = 0 To m.BTree.RootMoves.Length - 1
            'm.RootOutput(i) = li_v(i)
            tempRootOutput(i) = li_v(i)
        Next i

        If m.BTree.RootMoves.Length < limit Then
            limit = m.BTree.RootMoves.Length
        End If

        For i = 0 To limit - 1
            temp_index = Array.IndexOf(tempRootOutput, tempRootOutput.Max())
            temp_value = tempRootOutput(temp_index)
            idxes.Add(temp_index)
            temp_values.Add(temp_value)
            tempRootOutput(temp_index) = Single.MinValue
        Next i

        For i = 0 To idxes.Count - 1
            temp_moves.Add(m.BTree.RootMoves(idxes(i)))
        Next i
        m.BTree.RootMoves = New Move(temp_moves.Count - 1) {}
        m.RootOutput = New Single(temp_moves.Count - 1) {}
        outputs_sum = temp_values.Sum()
        For i = 0 To idxes.Count - 1
            m.BTree.RootMoves(i) = temp_moves(i)
            m.RootOutput(i) = temp_values(i) / outputs_sum
        Next i
    End Sub

    'LSTMを用いてルートのPolicy Networkの出力をセットする。
    Private Sub SetRootOutput2(ByRef m As MCTSTree, ByVal li_vectors As List(Of Integer), ByVal policy_network_threshold As Integer)
        Dim inputData(1 * 128 - 1) As Integer
        m.RootOutput = New Single(m.BTree.RootMoves.Length - 1) {}
        Dim i As Integer
        'inputData = MakeInputFeatures(m.BTree, m.BTree.RootColor)
        For i = 0 To li_vectors.Count - 1
            inputData(i) = li_vectors(i) '※手数が短いときに0でパディングされているのを確認する。
        Next i
        Dim inputTensor = New DenseTensor(Of Integer)(inputData, New Integer() {1, 128})
        Dim inputName = policy_session.InputMetadata.Keys.First()
        Dim outputTensor As Tensor(Of Single)
        Dim lbl As Label
        'Dim s As String
        Dim li_v = New List(Of Single)
        Dim newShape As Integer() = {1, 5864}
        Dim tempRootOutput = New Single(m.BTree.RootMoves.Length - 1) {}
        Dim limit As Integer = policy_network_threshold
        Dim idxes = New List(Of Integer)
        Dim temp_index As Integer
        Dim temp_values = New List(Of Single)
        Dim temp_value As Single
        Dim temp_moves = New List(Of Move)
        Dim outputs_sum As Single
        Dim ito As Integer

        ' prediction
        Using results = policy_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                outputTensor = result.AsTensor(Of Single)
            Next
        End Using

        outputTensor = outputTensor.Reshape(newShape)

        li_v = New List(Of Single)
        For i = 0 To m.BTree.RootMoves.Length - 1
            's = Move2CSA(m.BTree.RootMoves(i))
            'lbl = MakeOutputLabel(m.BTree.RootMoves(i))
            ito = GetTo(m.BTree.RootMoves(i))
            lbl = MakeOutputLSTMLabel(m.BTree.RootMoves(i), ito)
            Dim v = outputTensor(0, lbl << 7 Or ito)
            li_v.Add(v)
        Next i

        For i = 0 To m.BTree.RootMoves.Length - 1
            'm.RootOutput(i) = li_v(i)
            tempRootOutput(i) = li_v(i)
        Next i

        If m.BTree.RootMoves.Length < limit Then
            limit = m.BTree.RootMoves.Length
        End If

        For i = 0 To limit - 1
            temp_index = Array.IndexOf(tempRootOutput, tempRootOutput.Max())
            temp_value = tempRootOutput(temp_index)
            idxes.Add(temp_index)
            temp_values.Add(temp_value)
            tempRootOutput(temp_index) = Single.MinValue
        Next i

        For i = 0 To idxes.Count - 1
            temp_moves.Add(m.BTree.RootMoves(idxes(i)))
        Next i
        m.BTree.RootMoves = New Move(temp_moves.Count - 1) {}
        m.RootOutput = New Single(temp_moves.Count - 1) {}
        outputs_sum = temp_values.Sum()
        For i = 0 To idxes.Count - 1
            m.BTree.RootMoves(i) = temp_moves(i)
            m.RootOutput(i) = temp_values(i) / outputs_sum
        Next i
    End Sub

    Public Function ExecPolicy(ByVal str_sfen As String(), ByVal policy_network_threshold As Integer) As String()
        Dim outputTensor As Tensor(Of Single)
        Dim i As Integer
        Dim j As Integer
        Dim batch_size As Integer
        Dim ifrom As Integer
        Dim ito As Integer
        Dim lbl As Label
        Dim s As String
        'Dim v_sum As Single
        Dim li_v As List(Of Single)
        Dim bt As BoardTree
        Dim moves As List(Of Move)
        Dim legal_moves As List(Of Move)
        Dim outputs As List(Of Single)
        Dim str_moves As List(Of String)
        Dim str_moves2 As List(Of String)
        Dim str_out As String()

        bt = New BoardTree
        moves = New List(Of Move)
        outputs = New List(Of Single)
        str_moves = New List(Of String)
        str_moves2 = New List(Of String)

        batch_size = str_sfen.Length
        Dim newShape As Integer() = {batch_size, 32, 81}
        str_out = New String(batch_size - 1) {}

        Dim inputData As Single() = New Single(1 * 105 * 9 * 9 - 1) {}
        Dim inputData2 As Single() = New Single(batch_size * 105 * 9 * 9 - 1) {}
        For i = 0 To batch_size - 1
            Try
                bt = ToBoard(str_sfen(i))
                inputData = MakeInputFeatures(bt, bt.RootColor)
            Catch ex As Exception
                Console.WriteLine("例外が発生しましたが、処理を続行します。")
                Console.WriteLine(str_sfen(i))
            End Try
            inputData.CopyTo(inputData2, (i * 1 * 105 * 9 * 9))
        Next i

        'Dim inputData2 As Single() = New Single(batch_size * 105 * 9 * 9) {}
        Dim inputTensor = New DenseTensor(Of Single)(inputData2, New Integer() {batch_size, 105, 9, 9})
        Dim inputName = policy_session.InputMetadata.Keys.First()

        ' prediction
        Using results = policy_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                outputTensor = result.AsTensor(Of Single)
            Next
        End Using

        For i = 0 To batch_size - 1

            bt = ToBoard(str_sfen(i))

            moves = New List(Of Move)
            legal_moves = New List(Of Move)
            If IsAttacked(bt, bt.SQ_King(bt.RootColor), bt.RootColor) = 0 Then
                GenCap(bt, bt.RootColor, moves)
                GenNoCap(bt, bt.RootColor, moves)
                GenDrop(bt, bt.RootColor, moves)
            Else
                GenEvasion(bt, bt.RootColor, moves)
            End If

            'remove illegal move.
            For j = 0 To moves.Count - 1
                ifrom = GetFrom(moves(j))
                ito = GetTo(moves(j))
                If ifrom < Square_NB Then
                    'the case of discovered check
                    If IsPinnedOnKing(bt, ifrom, Adirec(ifrom, ito), bt.RootColor) <> 0 Then
                        Continue For
                    End If
                End If
                If GetCapPiece(moves(j)) = Piece.King Then
                    'the case of capture opponent king
                    Continue For
                End If
                legal_moves.Add(moves(j))
            Next j

            outputTensor = outputTensor.Reshape(newShape)

            'v_sum = 0.0F
            li_v = New List(Of Single)
            For j = 0 To legal_moves.Count - 1
                s = Move2CSA(legal_moves(j))
                lbl = MakeOutputLabel(legal_moves(j))
                Dim v = outputTensor(i, lbl, GetTo(legal_moves(j)))
                'v_sum += v
                str_moves.Add(s)
                li_v.Add(v)
            Next j

            Dim limit As Integer = policy_network_threshold

            If legal_moves.Count < limit Then
                limit = legal_moves.Count
            End If

            Dim temp_index As Integer
            Dim temp_value As Single
            Dim idxes = New List(Of Integer)
            Dim temp_values = New List(Of Single)
            Dim temp_moves = New List(Of Move)

            For j = 0 To limit - 1
                temp_index = li_v.IndexOf(li_v.Max())
                temp_value = li_v(temp_index)
                idxes.Add(temp_index)
                temp_values.Add(temp_value)
                li_v(temp_index) = Single.MinValue
            Next j

            For j = 0 To idxes.Count - 1
                temp_moves.Add(legal_moves(idxes(j)))
                str_moves2.Add(Move2CSA(legal_moves(idxes(j))))
            Next j

            'softmax function
            str_out(i) = ""
            For j = 0 To temp_moves.Count - 1
                If j <> 0 Then
                    str_out(i) = str_out(i) & ","
                End If
                Dim v = temp_values(j)
                outputs.Add(v)
                str_out(i) = str_out(i) & str_moves2(j) & " " & v.ToString()
            Next j
            str_moves.Clear()
            outputs.Clear()
        Next i

        Return str_out
    End Function

    Public Function ExecPolicy2(ByVal str_sfen As String(), ByVal str_vectors As String(), ByVal policy_network_threshold As Integer) As String()
        Dim outputTensor As Tensor(Of Single)
        Dim i As Integer
        Dim j As Integer
        Dim batch_size As Integer
        Dim ifrom As Integer
        Dim ito As Integer
        Dim lbl As Label
        Dim s As String
        'Dim v_sum As Single
        Dim li_v As List(Of Single)
        Dim bt As BoardTree
        Dim moves As List(Of Move)
        Dim legal_moves As List(Of Move)
        Dim outputs As List(Of Single)
        Dim str_moves As List(Of String)
        Dim str_moves2 As List(Of String)
        Dim str_out As String()

        bt = New BoardTree
        moves = New List(Of Move)
        outputs = New List(Of Single)
        str_moves = New List(Of String)
        str_moves2 = New List(Of String)

        batch_size = str_sfen.Length
        Dim newShape As Integer() = {batch_size, 5864}
        str_out = New String(batch_size - 1) {}

        Dim inputData As Integer() = New Integer(1 * 128 - 1) {}
        Dim inputData2 As Integer() = New Integer(batch_size * 128 - 1) {}
        For i = 0 To batch_size - 1
            Try
                bt = ToBoard(str_sfen(i))
                'inputData = MakeInputFeatures(bt, bt.RootColor)
                Dim temp_s As String() = str_vectors(i).Split(",")
                For j = 0 To temp_s.Length - 1
                    inputData(j) = Integer.Parse(temp_s(j))
                Next j
            Catch ex As Exception
                Console.WriteLine("例外が発生しましたが、処理を続行します。")
                Console.WriteLine(str_sfen(i))
            End Try
            inputData.CopyTo(inputData2, (i * 128))
        Next i

        'Dim inputData2 As Single() = New Single(batch_size * 105 * 9 * 9) {}
        Dim inputTensor = New DenseTensor(Of Integer)(inputData2, New Integer() {batch_size, 128})
        Dim inputName = policy_session.InputMetadata.Keys.First()

        ' prediction
        Using results = policy_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                outputTensor = result.AsTensor(Of Single)
            Next
        End Using

        For i = 0 To batch_size - 1

            bt = ToBoard(str_sfen(i))

            moves = New List(Of Move)
            legal_moves = New List(Of Move)
            If IsAttacked(bt, bt.SQ_King(bt.RootColor), bt.RootColor) = 0 Then
                GenCap(bt, bt.RootColor, moves)
                GenNoCap(bt, bt.RootColor, moves)
                GenDrop(bt, bt.RootColor, moves)
            Else
                GenEvasion(bt, bt.RootColor, moves)
            End If

            'remove illegal move.
            For j = 0 To moves.Count - 1
                ifrom = GetFrom(moves(j))
                ito = GetTo(moves(j))
                If ifrom < Square_NB Then
                    'the case of discovered check
                    If IsPinnedOnKing(bt, ifrom, Adirec(ifrom, ito), bt.RootColor) <> 0 Then
                        Continue For
                    End If
                End If
                If GetCapPiece(moves(j)) = Piece.King Then
                    'the case of capture opponent king
                    Continue For
                End If
                legal_moves.Add(moves(j))
            Next j

            outputTensor = outputTensor.Reshape(newShape)

            'v_sum = 0.0F
            li_v = New List(Of Single)
            For j = 0 To legal_moves.Count - 1
                s = Move2CSA(legal_moves(j))
                Dim sq = GetTo(legal_moves(j))
                Dim temp_label = MakeOutputLSTMLabel(legal_moves(j), sq)
                Dim h = (temp_label << 7) Or sq
                Dim v = outputTensor(i, h)
                'v_sum += v
                str_moves.Add(s)
                li_v.Add(v)
            Next j

            Dim limit As Integer = policy_network_threshold

            If legal_moves.Count < limit Then
                limit = legal_moves.Count
            End If

            Dim temp_index As Integer
            Dim temp_value As Single
            Dim idxes = New List(Of Integer)
            Dim temp_values = New List(Of Single)
            Dim temp_moves = New List(Of Move)

            For j = 0 To limit - 1
                temp_index = li_v.IndexOf(li_v.Max())
                temp_value = li_v(temp_index)
                idxes.Add(temp_index)
                temp_values.Add(temp_value)
                li_v(temp_index) = Single.MinValue
            Next j

            For j = 0 To idxes.Count - 1
                temp_moves.Add(legal_moves(idxes(j)))
                str_moves2.Add(Move2CSA(legal_moves(idxes(j))))
            Next j

            'softmax function
            str_out(i) = ""
            For j = 0 To temp_moves.Count - 1
                If j <> 0 Then
                    str_out(i) = str_out(i) & ","
                End If
                Dim v = temp_values(j)
                outputs.Add(v)
                str_out(i) = str_out(i) & str_moves2(j) & " " & v.ToString()
            Next j
            str_moves.Clear()
            outputs.Clear()
        Next i

        Return str_out
    End Function

    Public Function ExecValue(ByVal str_sfen As String()) As String()
        Dim outputTensor As Tensor(Of Single)
        Dim i As Integer
        Dim batch_size As Integer
        Dim bt As BoardTree
        Dim str_out As String()

        bt = New BoardTree

        batch_size = str_sfen.Length
        str_out = New String(batch_size - 1) {}

        Dim inputData As Single() = New Single(1 * 105 * 9 * 9 - 1) {}
        Dim inputData2 As Single() = New Single(batch_size * 105 * 9 * 9 - 1) {}
        For i = 0 To batch_size - 1
            bt = ToBoard(str_sfen(i))
            inputData = MakeInputFeatures(bt, bt.RootColor)
            inputData.CopyTo(inputData2, (i * 1 * 105 * 9 * 9))
        Next i

        'Dim inputData2 As Single() = New Single(batch_size * 105 * 9 * 9) {}
        Dim inputTensor = New DenseTensor(Of Single)(inputData2, New Integer() {batch_size, 105, 9, 9})
        Dim inputName = value_session.InputMetadata.Keys.First()

        ' prediction
        Using results = value_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                outputTensor = result.AsTensor(Of Single)
            Next
        End Using

        For i = 0 To batch_size - 1
            str_out(i) = Sigmoid(outputTensor(i)).ToString()
        Next i

        Return str_out
    End Function

    Public Function ExecValue2(ByVal str_sfen As String(), ByVal str_vectors As String()) As String()
        Dim outputTensor As Tensor(Of Single)
        Dim i As Integer
        Dim batch_size As Integer
        Dim bt As BoardTree
        Dim str_out As String()

        bt = New BoardTree

        batch_size = str_sfen.Length
        str_out = New String(batch_size - 1) {}

        Dim inputData As Integer() = New Integer(1 * 128 - 1) {}
        Dim inputData2 As Integer() = New Integer(batch_size * 128 - 1) {}
        For i = 0 To batch_size - 1
            Try
                bt = ToBoard(str_sfen(i))
                'inputData = MakeInputFeatures(bt, bt.RootColor)
                Dim temp_s As String() = str_vectors(i).Split(",")
                For j = 0 To temp_s.Length - 1
                    inputData(j) = Integer.Parse(temp_s(j))
                Next j
            Catch ex As Exception
                Console.WriteLine("例外が発生しましたが、処理を続行します。")
                Console.WriteLine(str_sfen(i))
            End Try
            inputData.CopyTo(inputData2, (i * 128))
        Next i

        'Dim inputData2 As Single() = New Single(batch_size * 105 * 9 * 9) {}
        Dim inputTensor = New DenseTensor(Of Integer)(inputData2, New Integer() {batch_size, 128})
        Dim inputName = value_session.InputMetadata.Keys.First()

        ' prediction
        Using results = value_session.Run(New List(Of NamedOnnxValue) From {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        })
            ' get outputs.
            For Each result In results
                outputTensor = result.AsTensor(Of Single)
            Next
        End Using

        For i = 0 To batch_size - 1
            str_out(i) = outputTensor(i).ToString()
        Next i

        Return str_out
    End Function

    Private Function Sigmoid(ByVal x As Single)
        Return 1.0F / (1.0F + Math.Exp(-x))
    End Function

    '畳み込みニューラルネットワークによる解析を行う。
    Private Sub SearchWrapper(ByRef bt As BoardTree, ByVal num_tasks As Integer, ByVal num_mate_tasks As Integer, ByVal thinking_time As Integer, ByVal mate_search_depth As Integer, ByVal policy_network_threshold As Integer, ByVal value_lambda As Single, ByRef str_result As String, ByRef param_str_mate_pv As String, ByVal str_record_move As String,
                              ByRef move_first_accuracy As Integer(), ByRef move_second_accuracy As Integer(), ByRef move_third_accurasy As Integer(), ByRef mate_first_move As Move)
        Dim i As Integer
        Dim checkMoves = New List(Of Move)
        Dim li_checkMoves0 = New List(Of Move)
        Dim li_checkMoves1 = New List(Of Move)
        Dim str_mate_pv As String
        str_mate_pv = ""
        For i = 0 To num_tasks - 1
            'sDeepCopy(bt, False)
            Select Case i
                Case 0
                    mcts_tree0 = InitMCTSTree()
                    mcts_tree0.TaskNumber = 0
                    mcts_tree0.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree0)
                    mcts_tree0.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput(mcts_tree0, policy_network_threshold)
                    mcts_tree0.value_lambda = value_lambda
                Case 1
                    mcts_tree1 = InitMCTSTree()
                    mcts_tree1.TaskNumber = 1
                    mcts_tree1.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree1)
                    mcts_tree1.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput(mcts_tree1, policy_network_threshold)
                    mcts_tree1.value_lambda = value_lambda
                Case 2
                    mcts_tree2 = InitMCTSTree()
                    mcts_tree2.TaskNumber = 2
                    mcts_tree2.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree2)
                    mcts_tree2.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput(mcts_tree2, policy_network_threshold)
                    mcts_tree2.value_lambda = value_lambda
                Case 3
                    mcts_tree3 = InitMCTSTree()
                    mcts_tree3.TaskNumber = 3
                    mcts_tree3.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree3)
                    mcts_tree3.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput(mcts_tree3, policy_network_threshold)
                    mcts_tree3.value_lambda = value_lambda
                Case 4
                    mcts_tree4 = InitMCTSTree()
                    mcts_tree4.TaskNumber = 4
                    mcts_tree4.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree4)
                    mcts_tree4.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput(mcts_tree4, policy_network_threshold)
                    mcts_tree4.value_lambda = value_lambda
                Case 5
                    mcts_tree5 = InitMCTSTree()
                    mcts_tree5.TaskNumber = 5
                    mcts_tree5.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree5)
                    mcts_tree5.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput(mcts_tree5, policy_network_threshold)
                    mcts_tree5.value_lambda = value_lambda
            End Select
        Next i

        checkMoves = New List(Of Move)
        li_checkMoves0 = New List(Of Move)
        li_checkMoves1 = New List(Of Move)
        GenCheck(bt, bt.RootColor, checkMoves)
        If checkMoves.Count > 0 Then
            For i = 0 To checkMoves.Count - 1
                If i Mod 2 = 0 Then
                    li_checkMoves0.Add(checkMoves(i))
                Else
                    li_checkMoves1.Add(checkMoves(i))
                End If
            Next i
            For i = 0 To num_mate_tasks - 1
                Select Case i
                    Case 0
                        mst0 = InitMateSearchTree(mate_search_depth)
                        mst0.BTree = DeepCopy(bt, False)
                        mst0.RootCheckMoves = li_checkMoves0
                        mst0.max_ply = mate_search_depth
                    Case 1
                        mst1 = InitMateSearchTree(mate_search_depth)
                        mst1.BTree = DeepCopy(bt, False)
                        mst1.RootCheckMoves = li_checkMoves1
                        mst1.max_ply = mate_search_depth
                End Select
            Next
        End If

        'SetRootOutput(mcts_tree0)
        'SetRootOutput(mcts_tree1)
        'mcts_tree0.sw.Start()
        'mcts_tree1.sw.Start()
        Dim task0 As Task
        Dim task1 As Task
        Dim task2 As Task
        Dim task3 As Task
        Dim task4 As Task
        Dim task5 As Task
        For i = 0 To num_tasks - 1
            Select Case i
                Case 0
                    task0 = Task.Run(Sub()
                                         MCTS.Root(mcts_tree0)
                                     End Sub)
                Case 1
                    task1 = Task.Run(Sub()
                                         MCTS.Root(mcts_tree1)
                                     End Sub)
                Case 2
                    task2 = Task.Run(Sub()
                                         MCTS.Root(mcts_tree2)
                                     End Sub)
                Case 3
                    task3 = Task.Run(Sub()
                                         MCTS.Root(mcts_tree3)
                                     End Sub)
                Case 4
                    task4 = Task.Run(Sub()
                                         MCTS.Root(mcts_tree4)
                                     End Sub)
                Case 5
                    task5 = Task.Run(Sub()
                                         MCTS.Root(mcts_tree5)
                                     End Sub)
            End Select
        Next i

        Dim task_mate0 As Task
        Dim task_mate1 As Task

        If checkMoves.Count > 0 Then
            For i = 0 To num_mate_tasks - 1
                Select Case i
                    Case 0
                        task_mate0 = Task.Run(Sub()
                                                  MateSearchWrapper(mst0, mate_search_depth)
                                              End Sub)
                    Case 1
                        task_mate1 = Task.Run(Sub()
                                                  MateSearchWrapper(mst1, mate_search_depth)
                                              End Sub)
                End Select
            Next i
        End If

        '        Dim task0 As Task = Task.Run(Sub()
        '        Root(mcts_tree0)
        '    End Sub)

        '        Dim task1 As Task = Task.Run(Sub()
        '        Root(mcts_tree1)
        '    End Sub)
        Dim index As Integer = 0
        Dim str_policy_requests As String() = New String(num_tasks - 1) {}
        Dim str_value_requests As String() = New String(num_tasks - 1) {}
        Dim temp_s As String
        Dim temp_s2 As String()
        Dim sw As Stopwatch = New Stopwatch
        Dim base_time As Long
        Dim elapsed As Long
        Dim flag As Boolean
        Dim is_completed As Boolean
        Dim p As Integer
        Dim v As Integer
        Dim cnt As Integer
        For i = 0 To num_tasks - 1
            str_policy_requests(i) = ""
            str_value_requests(i) = ""
        Next i
        flag = False
        is_completed = False
        p = 0
        v = 0
        cnt = 0
        sw.Start()
        Dim cnt_0 = 0
        Dim cnt_1 = 0
        base_time = sw.ElapsedMilliseconds

        While True
            Select Case index
                Case 0
                    If mcts_tree0.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree0.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        'mcts_tree0.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree0.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree0.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        'Console.WriteLine(0.ToString())
                        'mcts_tree0.queue_to_main_thread_v.Clear()
                        v += 1
                    End If
                Case 1
                    If mcts_tree1.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree1.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        'mcts_tree1.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree1.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree1.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        'mcts_tree1.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
                Case 2
                    If mcts_tree2.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree2.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        'mcts_tree2.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree2.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree2.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        'mcts_tree2.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
                Case 3
                    If mcts_tree3.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree3.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        'mcts_tree2.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree3.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree3.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        'mcts_tree2.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
                Case 4
                    If mcts_tree4.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree4.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        'mcts_tree2.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree4.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree4.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        'mcts_tree2.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
                Case 5
                    If mcts_tree5.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree5.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        'mcts_tree2.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree5.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree5.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        'mcts_tree2.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
            End Select

            If v = num_tasks Or p = num_tasks Then
                flag = True
            End If

            '制限時間に達したらis_abortをTrueにする。
            elapsed = sw.ElapsedMilliseconds
            If elapsed - base_time > 300 Then
                base_time = elapsed
                flag = True
            End If
            If elapsed > thinking_time * 1000 Then
                For i = 0 To num_tasks - 1
                    Select Case i
                        Case 0
                            mcts_tree0.is_abort = True
                        Case 1
                            mcts_tree1.is_abort = True
                        Case 2
                            mcts_tree2.is_abort = True
                        Case 3
                            mcts_tree3.is_abort = True
                        Case 4
                            mcts_tree4.is_abort = True
                        Case 5
                            mcts_tree5.is_abort = True
                    End Select
                Next i
            End If

            If flag = True Then
                While True
                    If p = 0 And v = 0 Then
                        Exit While
                    End If
                    If p > 0 Then
                        Dim str_sfen As String() = New String(p - 1) {}
                        Dim idx As Integer = 0
                        For i = 0 To p - 1
                            str_sfen(i) = ""
                        Next i
                        For i = 0 To num_tasks - 1
                            If str_policy_requests(i) <> "" Then
                                str_sfen(idx) = str_policy_requests(i)
                                idx += 1
                            End If
                            If idx = p Then
                                Exit For
                            End If
                        Next i
                        Dim str_ret As String() = ExecPolicy(str_sfen, policy_network_threshold)
                        idx = 0
                        For i = 0 To num_tasks - 1
                            If str_policy_requests(i) <> "" Then
                                Select Case i
                                    Case 0
                                        mcts_tree0.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 1
                                        mcts_tree1.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 2
                                        mcts_tree2.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 3
                                        mcts_tree3.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 4
                                        mcts_tree4.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 5
                                        mcts_tree5.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                End Select
                                idx += 1
                            End If
                            If idx = p Then
                                Exit For
                            End If
                        Next i
                        p = 0
                        Exit While
                    End If
                    If v > 0 Then
                        Dim str_sfen As String() = New String(v - 1) {}
                        Dim idx As Integer = 0
                        For i = 0 To v - 1
                            str_sfen(i) = ""
                        Next i
                        For i = 0 To num_tasks - 1
                            If str_value_requests(i) <> "" Then
                                str_sfen(idx) = str_value_requests(i)
                                idx += 1
                            End If
                            If idx = v Then
                                Exit For
                            End If
                        Next i
                        Dim str_ret As String() = New String() {}
                        Try
                            str_ret = ExecValue(str_sfen) 'ここでエラーが起こることが多いので、例外処理を入れる。
                        Catch ex As Exception
                            Console.WriteLine("例外が発生しましたが、処理を続行します。")
                            Dim temp_l As Integer = str_sfen.Length
                            str_ret = New String(temp_l - 1) {}
                            For i = 0 To temp_l - 1
                                str_ret(i) = "0.0"
                            Next i
                        End Try
                        idx = 0
                        For i = 0 To num_tasks - 1
                            If str_value_requests(i) <> "" Then
                                Select Case i
                                    Case 0
                                        mcts_tree0.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 1
                                        mcts_tree1.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 2
                                        mcts_tree2.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 3
                                        mcts_tree3.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 4
                                        mcts_tree4.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 5
                                        mcts_tree5.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                End Select
                                idx += 1
                            End If
                            If idx = v Then
                                Exit For
                            End If
                        Next i
                        v = 0
                        Exit While
                    End If
                End While
                flag = False
            End If

            If checkMoves.Count > 0 Then
                str_mate_pv = ""
                For i = 0 To num_mate_tasks - 1
                    Select Case i
                        Case 0
                            If task_mate0.IsCompleted = True Then
                                If mst0.is_mate_root = True Then
                                    str_mate_pv = mst0.root_str_pv
                                    mate_first_move = mst0.first_move
                                End If
                            End If
                        Case 1
                            If task_mate1.IsCompleted = True Then
                                If task_mate1.IsCompleted = True Then
                                    If mst1.is_mate_root = True Then
                                        str_mate_pv = mst1.root_str_pv
                                        If mst0.first_move = 0 Then
                                            mate_first_move = mst1.first_move
                                        End If
                                    End If
                                End If
                            End If
                    End Select
                Next i
            End If

            cnt = 0
            For i = 0 To num_tasks - 1
                Select Case i
                    Case 0
                        If task0.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 1
                        If task1.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 2
                        If task2.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 3
                        If task3.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 4
                        If task4.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 5
                        If task5.IsCompleted = True Then
                            cnt += 1
                        End If
                End Select
            Next i

            If cnt = num_tasks Then
                Exit While
            End If

            index += 1
            If index = num_tasks Then
                index = 0
            End If
        End While

        If str_mate_pv <> "" Then
            param_str_mate_pv = str_mate_pv
            Console.WriteLine("詰みあり： " & str_mate_pv)
            'モンテカルロ木探索のタスクを終了させる。
            'はやく終了させるために、is_abortをTrueにしているが、
            '効果は今ひとつのようである。
            For i = 0 To num_tasks - 1
                Select Case i
                    Case 0
                        mcts_tree0.is_abort = True
                    Case 1
                        mcts_tree1.is_abort = True
                    Case 2
                        mcts_tree2.is_abort = True
                    Case 3
                        mcts_tree3.is_abort = True
                    Case 4
                        mcts_tree4.is_abort = True
                    Case 5
                        mcts_tree5.is_abort = True
                End Select
            Next i
            cnt = 0
            While True
                For i = 0 To num_tasks - 1
                    Select Case i
                        Case 0
                            If task0.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 1
                            If task1.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 2
                            If task2.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 3
                            If task3.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 4
                            If task4.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 5
                            If task5.IsCompleted = True Then
                                cnt += 1
                            End If
                    End Select
                Next i
                If cnt = num_tasks Then
                    Exit While
                End If
            End While
            Return
        End If

        '        While True
        '        If mcts_tree0.queue_to_main_thread.Count > 0 Then
        '        Dim temp_s As String = mcts_tree0.queue_to_main_thread.Dequeue()
        '        Dim temp_s2 As String() = temp_s.Split(",")
        '        Dim param_s As String()
        '        param_s = New String(0) {}
        '        param_s(0) = temp_s2(1)
        '        If temp_s2(0) = "p" Then
        '        Dim temp_s3 = ExecPolicy(param_s)
        '        mcts_tree0.queue_from_main_thread.Enqueue(temp_s3(0))
        '        Else
        '        Dim temp_s3 = ExecValue(param_s)
        '        mcts_tree0.queue_from_main_thread.Enqueue(temp_s3(0))
        '        End If
        '        mcts_tree0.queue_to_main_thread.Clear()
        '        Console.WriteLine(temp_s)
        '        End If
        '        If task0.IsCompleted = True Then
        '        Exit While
        '        End If
        '        End While

        Dim win_rate_array As List(Of Single) = New List(Of Single)
        Dim trial_count_array As List(Of Integer) = New List(Of Integer)
        Dim moves = TotalParam(num_tasks, win_rate_array, trial_count_array, False)
        For i = 0 To moves.Count - 1
            Dim sr As String = ""
            If Move2CSA(moves(i)) = str_record_move Then
                sr = "result= ○"
                Select Case i
                    Case 0
                        move_first_accuracy(bt.RootColor) += 1
                    Case 1
                        move_second_accuracy(bt.RootColor) += 1
                    Case 2
                        move_third_accurasy(bt.RootColor) += 1
                End Select
            Else
                sr = "result= ×"
            End If
            Dim str_color As String
            If bt.RootColor = Color.Black Then
                str_color = "+"
            Else
                str_color = "-"
            End If
            Dim s As String = "候補手" & (i + 1).ToString() & ": " & str_color & Move2CSA(moves(i)) & ", " & sr & ", 勝率：" & win_rate_array(i).ToString() & ", 訪問回数" & trial_count_array(i).ToString() & ", "
            str_result = str_result & s
            Console.WriteLine(s)
        Next i
    End Sub

    '時系列ニューラルネットワークによる解析を行う。
    Private Sub SearchWrapper2(ByRef bt As BoardTree, ByVal num_tasks As Integer, ByVal num_mate_tasks As Integer, ByVal thinking_time As Integer, ByVal mate_search_depth As Integer, ByVal policy_network_threshold As Integer, ByVal value_lambda As Single, ByRef str_result As String, ByRef param_str_mate_pv As String, ByVal str_record_move As String,
                              ByRef move_first_accuracy As Integer(), ByRef move_second_accuracy As Integer(), ByRef move_third_accurasy As Integer(), ByRef mate_first_move As Move, ByRef li_root_vectors As List(Of Integer))
        Dim i As Integer
        Dim checkMoves = New List(Of Move)
        Dim li_checkMoves0 = New List(Of Move)
        Dim li_checkMoves1 = New List(Of Move)
        Dim str_root_vectors As String = ""
        Dim str_mate_pv As String
        str_mate_pv = ""

        For i = 0 To li_root_vectors.Count - 1
            str_root_vectors &= li_root_vectors(i).ToString()
            If i <> li_root_vectors.Count - 1 Then
                str_root_vectors &= ","
            End If
        Next i

        For i = 0 To num_tasks - 1
            'sDeepCopy(bt, False)
            Select Case i
                Case 0
                    mcts_tree0 = InitMCTSTree()
                    mcts_tree0.TaskNumber = 0
                    mcts_tree0.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree0)
                    mcts_tree0.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput2(mcts_tree0, li_root_vectors, policy_network_threshold)
                    mcts_tree0.VectorList = str_root_vectors
                    mcts_tree0.value_lambda = value_lambda
                Case 1
                    mcts_tree1 = InitMCTSTree()
                    mcts_tree1.TaskNumber = 1
                    mcts_tree1.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree1)
                    mcts_tree1.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput2(mcts_tree1, li_root_vectors, policy_network_threshold)
                    mcts_tree1.VectorList = str_root_vectors
                    mcts_tree1.value_lambda = value_lambda
                Case 2
                    mcts_tree2 = InitMCTSTree()
                    mcts_tree2.TaskNumber = 2
                    mcts_tree2.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree2)
                    mcts_tree2.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput2(mcts_tree2, li_root_vectors, policy_network_threshold)
                    mcts_tree2.VectorList = str_root_vectors
                    mcts_tree2.value_lambda = value_lambda
                Case 3
                    mcts_tree3 = InitMCTSTree()
                    mcts_tree3.TaskNumber = 3
                    mcts_tree3.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree3)
                    mcts_tree3.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput2(mcts_tree3, li_root_vectors, policy_network_threshold)
                    mcts_tree3.VectorList = str_root_vectors
                    mcts_tree3.value_lambda = value_lambda
                Case 4
                    mcts_tree4 = InitMCTSTree()
                    mcts_tree4.TaskNumber = 4
                    mcts_tree4.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree4)
                    mcts_tree4.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput2(mcts_tree4, li_root_vectors, policy_network_threshold)
                    mcts_tree4.VectorList = str_root_vectors
                    mcts_tree4.value_lambda = value_lambda
                Case 5
                    mcts_tree5 = InitMCTSTree()
                    mcts_tree5.TaskNumber = 5
                    mcts_tree5.BTree = DeepCopy(bt, False)
                    GenRootMoves(mcts_tree5)
                    mcts_tree5.SearchTimeLimit = thinking_time * 1000
                    SetRootOutput2(mcts_tree5, li_root_vectors, policy_network_threshold)
                    mcts_tree5.VectorList = str_root_vectors
                    mcts_tree5.value_lambda = value_lambda
            End Select
        Next i

        checkMoves = New List(Of Move)
        li_checkMoves0 = New List(Of Move)
        li_checkMoves1 = New List(Of Move)
        GenCheck(bt, bt.RootColor, checkMoves)
        If checkMoves.Count > 0 Then
            For i = 0 To checkMoves.Count - 1
                If i Mod 2 = 0 Then
                    li_checkMoves0.Add(checkMoves(i))
                Else
                    li_checkMoves1.Add(checkMoves(i))
                End If
            Next i
            For i = 0 To num_mate_tasks - 1
                Select Case i
                    Case 0
                        mst0 = InitMateSearchTree(mate_search_depth)
                        mst0.BTree = DeepCopy(bt, False)
                        mst0.RootCheckMoves = li_checkMoves0
                        mst0.max_ply = mate_search_depth
                    Case 1
                        mst1 = InitMateSearchTree(mate_search_depth)
                        mst1.BTree = DeepCopy(bt, False)
                        mst1.RootCheckMoves = li_checkMoves1
                        mst1.max_ply = mate_search_depth
                End Select
            Next
        End If

        'SetRootOutput(mcts_tree0)
        'SetRootOutput(mcts_tree1)
        'mcts_tree0.sw.Start()
        'mcts_tree1.sw.Start()
        Dim task0 As Task
        Dim task1 As Task
        Dim task2 As Task
        Dim task3 As Task
        Dim task4 As Task
        Dim task5 As Task
        For i = 0 To num_tasks - 1
            Select Case i
                Case 0
                    task0 = Task.Run(Sub()
                                         Root2(mcts_tree0)
                                     End Sub)
                Case 1
                    task1 = Task.Run(Sub()
                                         Root2(mcts_tree1)
                                     End Sub)
                Case 2
                    task2 = Task.Run(Sub()
                                         Root2(mcts_tree2)
                                     End Sub)
                Case 3
                    task3 = Task.Run(Sub()
                                         Root2(mcts_tree3)
                                     End Sub)
                Case 4
                    task4 = Task.Run(Sub()
                                         Root2(mcts_tree4)
                                     End Sub)
                Case 5
                    task5 = Task.Run(Sub()
                                         Root2(mcts_tree5)
                                     End Sub)
            End Select
        Next i

        Dim task_mate0 As Task
        Dim task_mate1 As Task

        If checkMoves.Count > 0 Then
            For i = 0 To num_mate_tasks - 1
                Select Case i
                    Case 0
                        task_mate0 = Task.Run(Sub()
                                                  MateSearchWrapper(mst0, mate_search_depth)
                                              End Sub)
                    Case 1
                        task_mate1 = Task.Run(Sub()
                                                  MateSearchWrapper(mst1, mate_search_depth)
                                              End Sub)
                End Select
            Next i
        End If

        '        Dim task0 As Task = Task.Run(Sub()
        '        Root(mcts_tree0)
        '    End Sub)

        '        Dim task1 As Task = Task.Run(Sub()
        '        Root(mcts_tree1)
        '    End Sub)
        Dim index As Integer = 0
        Dim str_policy_requests As String() = New String(num_tasks - 1) {}
        Dim str_policy_requests2 As String() = New String(num_tasks - 1) {}
        Dim str_value_requests As String() = New String(num_tasks - 1) {}
        Dim str_value_requests2 As String() = New String(num_tasks - 1) {}
        Dim temp_s As String
        Dim temp_s2 As String()
        Dim sw As Stopwatch = New Stopwatch
        Dim base_time As Long
        Dim elapsed As Long
        Dim flag As Boolean
        Dim is_completed As Boolean
        Dim p As Integer
        Dim v As Integer
        Dim cnt As Integer
        For i = 0 To num_tasks - 1
            str_policy_requests(i) = ""
            str_policy_requests2(i) = ""
            str_value_requests(i) = ""
            str_value_requests2(i) = ""
        Next i
        flag = False
        is_completed = False
        p = 0
        v = 0
        cnt = 0
        sw.Start()
        Dim cnt_0 = 0
        Dim cnt_1 = 0
        base_time = sw.ElapsedMilliseconds

        While True
            Select Case index
                Case 0
                    If mcts_tree0.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree0.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        str_policy_requests2(index) = temp_s2(2)
                        'mcts_tree0.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree0.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree0.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        str_value_requests2(index) = temp_s2(2)
                        'Console.WriteLine(0.ToString())
                        'mcts_tree0.queue_to_main_thread_v.Clear()
                        v += 1
                    End If
                Case 1
                    If mcts_tree1.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree1.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        str_policy_requests2(index) = temp_s2(2)
                        'mcts_tree1.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree1.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree1.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        str_value_requests2(index) = temp_s2(2)
                        'mcts_tree1.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
                Case 2
                    If mcts_tree2.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree2.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        str_policy_requests2(index) = temp_s2(2)
                        'mcts_tree2.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree2.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree2.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        str_value_requests2(index) = temp_s2(2)
                        'mcts_tree2.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
                Case 3
                    If mcts_tree3.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree3.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        str_policy_requests2(index) = temp_s2(2)
                        'mcts_tree2.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree3.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree3.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        str_value_requests2(index) = temp_s2(2)
                        'mcts_tree2.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
                Case 4
                    If mcts_tree4.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree4.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        str_policy_requests2(index) = temp_s2(2)
                        'mcts_tree2.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree4.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree4.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        str_value_requests2(index) = temp_s2(2)
                        'mcts_tree2.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
                Case 5
                    If mcts_tree5.queue_to_main_thread_p.Count > 0 Then
                        temp_s = mcts_tree5.queue_to_main_thread_p.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_policy_requests(index) = temp_s2(1)
                        str_policy_requests2(index) = temp_s2(2)
                        'mcts_tree2.queue_to_main_thread_p.Clear()
                        p += 1
                    ElseIf mcts_tree5.queue_to_main_thread_v.Count > 0 Then
                        temp_s = mcts_tree5.queue_to_main_thread_v.Dequeue()
                        temp_s2 = temp_s.Split(",")
                        str_value_requests(index) = temp_s2(1)
                        str_value_requests2(index) = temp_s2(2)
                        'mcts_tree2.queue_to_main_thread_v.Clear()
                        'Console.WriteLine(1.ToString())
                        v += 1
                    End If
            End Select

            If v = num_tasks Or p = num_tasks Then
                flag = True
            End If

            '制限時間に達したらis_abortをTrueにする。
            elapsed = sw.ElapsedMilliseconds
            If elapsed - base_time > 300 Then
                base_time = elapsed
                flag = True
            End If
            If elapsed > thinking_time * 1000 Then
                For i = 0 To num_tasks - 1
                    Select Case i
                        Case 0
                            mcts_tree0.is_abort = True
                        Case 1
                            mcts_tree1.is_abort = True
                        Case 2
                            mcts_tree2.is_abort = True
                        Case 3
                            mcts_tree3.is_abort = True
                        Case 4
                            mcts_tree4.is_abort = True
                        Case 5
                            mcts_tree5.is_abort = True
                    End Select
                Next i
            End If

            If flag = True Then
                While True
                    If p = 0 And v = 0 Then
                        Exit While
                    End If
                    If p > 0 Then
                        Dim str_sfen As String() = New String(p - 1) {}
                        Dim str_vectors As String() = New String(p - 1) {}
                        Dim idx As Integer = 0
                        For i = 0 To p - 1
                            str_sfen(i) = ""
                        Next i
                        For i = 0 To num_tasks - 1
                            If str_policy_requests(i) <> "" Then
                                str_sfen(idx) = str_policy_requests(i)
                                str_vectors(idx) = str_policy_requests2(i)
                                idx += 1
                            End If
                            If idx = p Then
                                Exit For
                            End If
                        Next i
                        Dim str_ret As String() = ExecPolicy2(str_sfen, str_vectors, policy_network_threshold) 'ここを変更する。
                        idx = 0
                        For i = 0 To num_tasks - 1
                            If str_policy_requests(i) <> "" Then
                                Select Case i
                                    Case 0
                                        mcts_tree0.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 1
                                        mcts_tree1.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 2
                                        mcts_tree2.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 3
                                        mcts_tree3.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 4
                                        mcts_tree4.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                    Case 5
                                        mcts_tree5.queue_from_main_thread_p.Enqueue(str_ret(idx))
                                End Select
                                idx += 1
                            End If
                            If idx = p Then
                                Exit For
                            End If
                        Next i
                        p = 0
                        Exit While
                    End If
                    If v > 0 Then
                        Dim str_sfen As String() = New String(v - 1) {}
                        Dim str_vectors As String() = New String(v - 1) {}
                        Dim idx As Integer = 0
                        For i = 0 To v - 1
                            str_sfen(i) = ""
                            str_vectors(i) = ""
                        Next i
                        For i = 0 To num_tasks - 1
                            If str_value_requests(i) <> "" Then
                                str_sfen(idx) = str_value_requests(i)
                                str_vectors(idx) = str_value_requests2(i)
                                idx += 1
                            End If
                            If idx = v Then
                                Exit For
                            End If
                        Next i
                        Dim str_ret As String() = New String() {}
                        Try
                            'ここを変更する。
                            str_ret = ExecValue2(str_sfen, str_vectors) 'ここでエラーが起こることが多いので、例外処理を入れる。
                        Catch ex As Exception
                            Console.WriteLine("例外が発生しましたが、処理を続行します。")
                            Dim temp_l As Integer = str_sfen.Length
                            str_ret = New String(temp_l - 1) {}
                            For i = 0 To temp_l - 1
                                str_ret(i) = "0.0"
                            Next i
                        End Try
                        idx = 0
                        For i = 0 To num_tasks - 1
                            If str_value_requests(i) <> "" Then
                                Select Case i
                                    Case 0
                                        mcts_tree0.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 1
                                        mcts_tree1.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 2
                                        mcts_tree2.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 3
                                        mcts_tree3.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 4
                                        mcts_tree4.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                    Case 5
                                        mcts_tree5.queue_from_main_thread_v.Enqueue(str_ret(idx))
                                End Select
                                idx += 1
                            End If
                            If idx = v Then
                                Exit For
                            End If
                        Next i
                        v = 0
                        Exit While
                    End If
                End While
                flag = False
            End If

            If checkMoves.Count > 0 Then
                str_mate_pv = ""
                For i = 0 To num_mate_tasks - 1
                    Select Case i
                        Case 0
                            If task_mate0.IsCompleted = True Then
                                If mst0.is_mate_root = True Then
                                    str_mate_pv = mst0.root_str_pv
                                    mate_first_move = mst0.first_move
                                End If
                            End If
                        Case 1
                            If task_mate1.IsCompleted = True Then
                                If task_mate1.IsCompleted = True Then
                                    If mst1.is_mate_root = True Then
                                        str_mate_pv = mst1.root_str_pv
                                        If mst0.first_move = 0 Then
                                            mate_first_move = mst1.first_move
                                        End If
                                    End If
                                End If
                            End If
                    End Select
                Next i
            End If

            cnt = 0
            For i = 0 To num_tasks - 1
                Select Case i
                    Case 0
                        If task0.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 1
                        If task1.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 2
                        If task2.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 3
                        If task3.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 4
                        If task4.IsCompleted = True Then
                            cnt += 1
                        End If
                    Case 5
                        If task5.IsCompleted = True Then
                            cnt += 1
                        End If
                End Select
            Next i

            If cnt = num_tasks Then
                Exit While
            End If

            index += 1
            If index = num_tasks Then
                index = 0
            End If
        End While

        If str_mate_pv <> "" Then
            param_str_mate_pv = str_mate_pv
            Console.WriteLine("詰みあり： " & str_mate_pv)
            'モンテカルロ木探索のタスクを終了させる。
            'はやく終了させるために、is_abortをTrueにしているが、
            '効果は今ひとつのようである。
            For i = 0 To num_tasks - 1
                Select Case i
                    Case 0
                        mcts_tree0.is_abort = True
                    Case 1
                        mcts_tree1.is_abort = True
                    Case 2
                        mcts_tree2.is_abort = True
                    Case 3
                        mcts_tree3.is_abort = True
                    Case 4
                        mcts_tree4.is_abort = True
                    Case 5
                        mcts_tree5.is_abort = True
                End Select
            Next i
            cnt = 0
            While True
                For i = 0 To num_tasks - 1
                    Select Case i
                        Case 0
                            If task0.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 1
                            If task1.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 2
                            If task2.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 3
                            If task3.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 4
                            If task4.IsCompleted = True Then
                                cnt += 1
                            End If
                        Case 5
                            If task5.IsCompleted = True Then
                                cnt += 1
                            End If
                    End Select
                Next i
                If cnt = num_tasks Then
                    Exit While
                End If
            End While
            Return
        End If

        '        While True
        '        If mcts_tree0.queue_to_main_thread.Count > 0 Then
        '        Dim temp_s As String = mcts_tree0.queue_to_main_thread.Dequeue()
        '        Dim temp_s2 As String() = temp_s.Split(",")
        '        Dim param_s As String()
        '        param_s = New String(0) {}
        '        param_s(0) = temp_s2(1)
        '        If temp_s2(0) = "p" Then
        '        Dim temp_s3 = ExecPolicy(param_s)
        '        mcts_tree0.queue_from_main_thread.Enqueue(temp_s3(0))
        '        Else
        '        Dim temp_s3 = ExecValue(param_s)
        '        mcts_tree0.queue_from_main_thread.Enqueue(temp_s3(0))
        '        End If
        '        mcts_tree0.queue_to_main_thread.Clear()
        '        Console.WriteLine(temp_s)
        '        End If
        '        If task0.IsCompleted = True Then
        '        Exit While
        '        End If
        '        End While

        Dim win_rate_array As List(Of Single) = New List(Of Single)
        Dim trial_count_array As List(Of Integer) = New List(Of Integer)
        Dim moves = TotalParam(num_tasks, win_rate_array, trial_count_array, False)
        For i = 0 To moves.Count - 1
            Dim sr As String = ""
            If Move2CSA(moves(i)) = str_record_move Then
                sr = "result= ○"
                Select Case i
                    Case 0
                        move_first_accuracy(bt.RootColor) += 1
                    Case 1
                        move_second_accuracy(bt.RootColor) += 1
                    Case 2
                        move_third_accurasy(bt.RootColor) += 1
                End Select
            Else
                sr = "result= ×"
            End If
            Dim str_color As String
            If bt.RootColor = Color.Black Then
                str_color = "+"
            Else
                str_color = "-"
            End If
            Dim s As String = "候補手" & (i + 1).ToString() & ": " & str_color & Move2CSA(moves(i)) & ", " & sr & ", 勝率：" & win_rate_array(i).ToString() & ", 訪問回数" & trial_count_array(i).ToString() & ", "
            str_result = str_result & s
            Console.WriteLine(s)
        Next i
    End Sub


End Module
