Imports System
Imports Altair.Common

Module Program
    Sub Main(args As String())
        Console.WriteLine("Hello World!")
        Init.Init()
        RandAlloc()
        IniRand(5489)
        IniRandomTable()

        Dim num_tasks As Integer = CInt(args(0))
        Dim num_mate_tasks As Integer = CInt(args(1))
        Dim thinking_time As Integer = CInt(args(2))
        Dim mate_search_depth As Integer = CInt(args(3))
        Dim analyze_file_name As String = args(4)
        Dim record_file_name As String = args(5)
        Dim str_game_date As String = args(6)
        Dim str_match_name As String = args(7)
        Dim str_black_player As String = args(8)
        Dim str_white_player As String = args(9)

        'TestSFEN()

        'AnalyzeRecord(6, 2, 30, 9, "analyze_result.txt", "20220403_nhk_hai.txt", "2022/04/03", "‘æ72‰ñNHK”t1‰ñí", "–Ø‘ºˆêŠî‹ã’i", "•“c‹Ä”VŒÜ’i")
        AnalyzeRecord(num_tasks, num_mate_tasks, thinking_time, mate_search_depth, analyze_file_name, record_file_name, str_game_date, str_match_name, str_black_player, str_white_player)
    End Sub
End Module
