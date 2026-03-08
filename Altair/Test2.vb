Imports Microsoft.ML.OnnxRuntime
Imports Microsoft.ML.OnnxRuntime.Tensors

Module Test2
    'This code is created by Copilot.
    Public Sub TestONNX()
        ' GPU 実行プロバイダの設定（CUDA）
        Dim sessionOptions As New SessionOptions()
        sessionOptions.AppendExecutionProvider_CUDA(deviceId:=0) ' deviceId=0 は最初のGPU
        'sessionOptions.AppendExecutionProvider_CUDA()


        ' モデル読み込み
        Using session As New InferenceSession("model.onnx", sessionOptions)
            ' 入力データ作成（例: 1x3x224x224 の float32 テンソル）
            Dim inputData(1 * 105 * 9 * 9 - 1) As Single
            ' ここで inputData に画像データなどをセットする

            Dim inputTensor = New DenseTensor(Of Single)(inputData, New Integer() {1, 105, 9, 9})
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
    Public Sub TestONNX2()
        ' GPU 実行プロバイダの設定（CUDA）
        Dim sessionOptions As New SessionOptions()
        sessionOptions.AppendExecutionProvider_CUDA(deviceId:=0) ' deviceId=0 は最初のGPU
        'sessionOptions.AppendExecutionProvider_CUDA()


        ' モデル読み込み
        Using session As New InferenceSession("model_value.onnx", sessionOptions)
            ' 入力データ作成（例: 1x3x224x224 の float32 テンソル）
            Dim inputData(1 * 105 * 9 * 9 - 1) As Single
            ' ここで inputData に画像データなどをセットする

            Dim inputTensor = New DenseTensor(Of Single)(inputData, New Integer() {1, 105, 9, 9})
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
