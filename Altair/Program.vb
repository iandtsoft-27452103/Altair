Imports System
Imports Altair.Common

Module Program
    Sub Main(args As String())
        Console.WriteLine("Hello World!")
        Init.Init()
        RandAlloc()
        IniRand(5489)
        IniRandomTable()

        'Dim a = LabelTable(4)
        ' LSTMのハッシュ値 : direc << 7 | ito
        ' +2726FUは308 direc = 2, ito = 52
        ' +7776FUは303 direc = 2, ito = 47
        ' 長さは128で、先頭から

        'AnalyzeRecord(6, 2, 5, 9, 4, 0.5, "analyze_result.txt", "20220403_nhk_hai.txt", "2022/04/03", "第72回NHK杯1回戦", "木村一基九段", "黒田尭之五段")
        'AnalyzeRecord2(6, 2, 5, 9, 4, 0.5, "analyze_result.txt", "20220403_nhk_hai.txt", "2022/04/03", "第72回NHK杯1回戦", "木村一基九段", "黒田尭之五段", True)
        'Return

        Dim mode As Integer = CInt(args(0))
        Dim num_tasks As Integer = CInt(args(1))
        Dim num_mate_tasks As Integer = CInt(args(2))
        Dim thinking_time As Integer = CInt(args(3))
        Dim mate_search_depth As Integer = CInt(args(4))
        Dim policy_network_threshold As Integer = CInt(args(5))
        Dim value_lambda As Single = CSng(args(6))
        Dim analyze_file_name As String = args(7)
        Dim record_file_name As String = args(8)
        Dim str_game_date As String = args(9)
        Dim str_match_name As String = args(10)
        Dim str_black_player As String = args(11)
        Dim str_white_player As String = args(12)
        Dim use_gru As Boolean = CBool(args(13)) 'Value NetworkはLSTMを使用している。

        'AnalyzeRecord2(6, 2, 30, 9, "analyze_result.txt", "20220403_nhk_hai.txt", "2022/04/03", "第72回NHK杯1回戦", "木村一基九段", "黒田尭之五段")
        'AnalyzeRecord2(6, 2, 5, 9, 4, 0.5, "analyze_result.txt", "20220403_nhk_hai.txt", "2022/04/03", "第72回NHK杯1回戦", "木村一基九段", "黒田尭之五段", True)

        'Return

        'TestSFEN()

        'AnalyzeRecord(6, 2, 30, 9, "analyze_result.txt", "20220403_nhk_hai.txt", "2022/04/03", "第72回NHK杯1回戦", "木村一基九段", "黒田尭之五段")
        Select Case mode
            Case 0
                '畳み込みニューラルネットワークを使用して棋譜を解析する。
                AnalyzeRecord(num_tasks, num_mate_tasks, thinking_time, mate_search_depth, policy_network_threshold, value_lambda, analyze_file_name, record_file_name, str_game_date, str_match_name, str_black_player, str_white_player)
            Case 1
                '時系列ネットワークを使用して棋譜を解析する。
                AnalyzeRecord2(num_tasks, num_mate_tasks, thinking_time, mate_search_depth, policy_network_threshold, value_lambda, analyze_file_name, record_file_name, str_game_date, str_match_name, str_black_player, str_white_player, use_gru)
        End Select

    End Sub
End Module
