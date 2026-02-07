Imports Altair.Common
Module SFEN
    Public Function ToSFEN(ByVal bt As BoardTree, ByVal c As Integer) As String
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim empty_count As Integer
        Dim num As Integer
        Dim s_piece As String
        Dim str_sfen As String
        Dim flag As Boolean
        str_sfen = ""
        flag = False
        i = 0
        empty_count = 0
        While i < Square_NB
            s_piece = Str_SFEN_Pc(bt.Board(i))
            If s_piece = "" Then
                empty_count += 1
                flag = True
            Else
                If flag = True Then
                    flag = False
                    str_sfen = str_sfen & empty_count.ToString()
                    empty_count = 0
                End If
                str_sfen = str_sfen & s_piece
            End If
            If i <> (Square_NB - 1) And FileTable(i) = File.File9 Then
                If empty_count > 0 Then
                    flag = False
                    str_sfen = str_sfen & empty_count.ToString()
                    empty_count = 0
                End If
                str_sfen = str_sfen & "/"
            End If
            i += 1
        End While
        str_sfen = str_sfen & " " & Str_Color(c) & " "
        k = 0
        If bt.Hand(Color.Black) = 0 And bt.Hand(Color.White) = 0 Then
            str_sfen = str_sfen & "-"
        Else
            For i = Color.Black To Color.White
                For j = Piece.Rook To Piece.Pawn Step -1
                    num = (bt.Hand(i) And Hand_Mask(j)) >> Hand_Rev_Bit(j)
                    If num = 0 Then
                        Continue For
                    End If
                    If num > 0 Then
                        If num = 1 Then
                            k = -Sign_Table(i) * j
                            str_sfen = str_sfen & num.ToString() & Str_SFEN_Pc(k)
                        End If
                    End If
                Next j
            Next i
        End If
        str_sfen = str_sfen & " 1"
        Return str_sfen
    End Function
    Public Function ToBoard(ByVal str_sfen As String) As BoardTree
        Dim i As Integer
        Dim j As Integer
        Dim sq As Integer
        Dim limit As Integer
        Dim empty_num As Integer
        Dim ipc As Integer
        Dim num As Integer
        Dim c As Integer
        Dim str_temp As String()
        Dim str_board As String
        Dim s_color As String
        Dim str_hand As String
        Dim s As String
        Dim flag As Boolean
        Dim bt As BoardTree
        bt = Board.Init()
        Board.Clear(bt)
        flag = False
        ipc = 0
        str_temp = str_sfen.Split(" ")
        str_board = str_temp(0)
        limit = str_board.Length
        sq = 0
        For i = 0 To limit - 1
            s = str_board.Substring(i, 1)
            If s = "+" Then
                flag = True
            ElseIf s = "/" Then
                Continue For
            Else
                If Set_Empty_Num.Contains(s) Then
                    empty_num = Int_Empty_Num(s)
                    j = 0
                    While j < empty_num
                        bt.Board(sq) = Piece.Empty
                        sq += 1
                        j += 1
                    End While
                Else
                    ipc = Int_Pc(s)
                    If ipc > 0 Then
                        If flag = True Then
                            ipc += Promote
                            flag = False
                        End If
                        bt.BB_Piece(Color.Black, ipc) = bt.BB_Piece(Color.Black, ipc) Or ABB_Mask(sq)
                        bt.BB_Occupied(Color.Black) = bt.BB_Occupied(Color.Black) Or ABB_Mask(sq)
                        If ipc = Piece.King Then
                            bt.SQ_King(Color.Black) = sq
                        End If
                    Else
                        If flag = True Then
                            ipc -= Promote
                            flag = False
                        End If
                        bt.BB_Piece(Color.White, -ipc) = bt.BB_Piece(Color.White, -ipc) Or ABB_Mask(sq)
                        bt.BB_Occupied(Color.White) = bt.BB_Occupied(Color.White) Or ABB_Mask(sq)
                        If ipc = -Piece.King Then
                            bt.SQ_King(Color.White) = sq
                        End If
                    End If
                    bt.Board(sq) = ipc
                    sq += 1
                End If
            End If
        Next i
        s_color = str_temp(1)
        bt.RootColor = Num_Color(s_color)
        str_hand = str_temp(2)
        limit = str_hand.Length
        flag = False
        num = 1
        For i = 0 To limit - 1
            s = str_hand.Substring(i, 1)
            If s = "-" Then
                Exit For
            End If
            If s = "1" And flag = False Then
                flag = True
            Else
                If flag = True Then
                    num = 10 + Int_Hand_Num(s)
                    flag = False
                Else
                    If Set_Hand_Num.Contains(s) Then
                        num = Int_Hand_Num(s)
                    Else
                        ipc = Int_Pc(s)
                        If ipc > 0 Then
                            c = Color.Black
                        Else
                            c = Color.White
                            ipc = -ipc
                        End If
                        j = 0
                        While j < num
                            bt.Hand(c) += Hand_Hash(ipc)
                            j += 1
                        End While
                        num = 1
                    End If
                End If
            End If
        Next i
        bt.CurrentHash = HashFunc(bt)
        bt.Hash(0) = bt.PrevHash
        bt.Hash(0) = bt.CurrentHash
        bt.Ply = 1
        Return bt
    End Function
End Module
