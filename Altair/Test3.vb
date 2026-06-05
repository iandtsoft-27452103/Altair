Imports Microsoft.ML.OnnxRuntime
Imports Microsoft.ML.OnnxRuntime.Tensors

Module Test3
    'This code is created by Copilot.
    Public Sub TestONNXLSTM()
        ' GPU 実行プロバイダの設定（CUDA）
        Dim sessionOptions As New SessionOptions()
        sessionOptions.AppendExecutionProvider_CUDA(deviceId:=0) ' deviceId=0 は最初のGPU
        'sessionOptions.AppendExecutionProvider_CUDA()


        ' モデル読み込み
        Using session As New InferenceSession("model_lstm.onnx", sessionOptions)
            ' 入力データ作成（例: 1x3x224x224 の float32 テンソル）
            Dim inputData(1 * 128 - 1) As Integer
            ' ここで inputData に画像データなどをセットする

            Dim inputTensor = New DenseTensor(Of Integer)(inputData, New Integer() {1, 128})
            Dim inputName = session.InputMetadata.Keys.First()

            ' 推論実行
            Using results = session.Run(New List(Of NamedOnnxValue) From {
                NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
            })
                ' 出力取得
                For Each result In results
                    Console.WriteLine($"Output: {result.Name}")
                    Dim outputTensor = result.AsTensor(Of Single)()
                    Console.WriteLine($"First value: {outputTensor(0)}")
                Next
            End Using
        End Using
    End Sub
    Public Sub TestONNXLSTM2()
        ' GPU 実行プロバイダの設定（CUDA）
        Dim sessionOptions As New SessionOptions()
        sessionOptions.AppendExecutionProvider_CUDA(deviceId:=0) ' deviceId=0 は最初のGPU
        'sessionOptions.AppendExecutionProvider_CUDA()


        ' モデル読み込み
        Using session As New InferenceSession("model_lstm_value.onnx", sessionOptions)
            ' 入力データ作成（例: 1x3x224x224 の float32 テンソル）
            Dim inputData(1 * 128 - 1) As Integer
            ' ここで inputData に画像データなどをセットする

            Dim inputTensor = New DenseTensor(Of Integer)(inputData, New Integer() {1, 128})
            Dim inputName = session.InputMetadata.Keys.First()

            ' 推論実行
            Using results = session.Run(New List(Of NamedOnnxValue) From {
                NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
            })
                ' 出力取得
                For Each result In results
                    Console.WriteLine($"Output: {result.Name}")
                    Dim outputTensor = result.AsTensor(Of Single)()
                    Console.WriteLine($"First value: {outputTensor(0)}")
                Next
            End Using
        End Using
    End Sub
End Module
