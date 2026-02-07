Imports Altair.Common
Imports BitBoard = System.UInt128
Imports Rand = System.UInt128
Imports Key = System.UInt128

Module Hash
    Public PieceRand As Rand(,,)
    Public Structure RandWorkT
        Public count As Integer
        Public cnst As ULong()
        Public vec As ULong()
    End Structure

    Public rand_work As RandWorkT

    Public Sub RandAlloc()
        PieceRand = New Rand(Color_NB, Piece_NB, Square_NB) {}
    End Sub

    Public Sub IniRand(ByVal u As ULong)
        Dim i As Integer
        rand_work.cnst = New ULong(2) {}
        rand_work.vec = New ULong(RandN) {}
        rand_work.count = RandN
        rand_work.cnst(0) = 0
        rand_work.cnst(1) = 2567483615
        For i = 1 To RandN - 1
            u = i + 1812433253 * (u Xor (u >> 30))
            u = u And Mask32
            rand_work.vec(i) = u
        Next i
    End Sub

    Public Function Rand32() As UInt32
        Dim i As Integer
        Dim u As UInt32
        Dim u0 As UInt32
        Dim u1 As UInt32
        Dim u2 As UInt32
        If rand_work.count = RandN Then
            rand_work.count = 0
            For i = 0 To RandN - RandM - 1
                u = rand_work.vec(i) And MaskU
                u = u Or (rand_work.vec(i + 1) And MaskL)
                u0 = rand_work.vec(i + RandM)
                u1 = u >> 1
                u2 = rand_work.cnst(u And 1)
                rand_work.vec(i) = u0 Xor u1 Xor u2
            Next i
            For i = RandN - RandM To RandN - 1
                u = rand_work.vec(i) And MaskU
                u = u Or (rand_work.vec(i + 1) And MaskL)
                u0 = rand_work.vec(i + RandM - RandN)
                u1 = u >> 1
                u2 = rand_work.cnst(u And 1)
                rand_work.vec(i) = u0 Xor u1 Xor u2
            Next i
            u = rand_work.vec(RandN - 1) And MaskU
            u = u Or (rand_work.vec(0) And MaskL)
            u0 = rand_work.vec(RandM - 1)
            u1 = u >> 1
            u2 = rand_work.cnst(u And 1)
            rand_work.vec(RandN - 1) = u0 Xor u1 Xor u2
        End If
        u = rand_work.vec(rand_work.count)
        rand_work.count += 1
        u = u Xor (u >> 11)
        u = u Xor ((u << 7) And 2636928640)
        u = u Xor ((u << 15) And 4022730752)
        u = u Xor (u >> 18)
        Return u
    End Function

    Public Function Rand64() As ULong
        Dim h As ULong
        Dim l As ULong
        h = Rand32()
        l = Rand32()
        Return l Or (h << 32)
    End Function

    Public Function Rand128() As Rand
        Dim h As Rand
        Dim l As Rand
        h = Rand64()
        l = Rand64()
        Return l Or (h << 64)
    End Function

    Public Sub IniRandomTable()
        Dim c As Integer
        Dim pc As Integer
        Dim sq As Integer
        For c = 0 To Color_NB - 1
            For pc = 0 To Piece_NB - 1
                For sq = 0 To Square_NB - 1
                    PieceRand(c, pc, sq) = Rand128()
                Next sq
            Next pc
        Next c
    End Sub

    Public Function HashFunc(ByVal BTree As BoardTree) As Rand
        Dim c As Integer
        Dim pc As Integer
        Dim sq As Integer
        Dim key As Key
        Dim bb As BitBoard

        key = 0
        For c = 0 To Color_NB - 1
            For pc = 0 To Piece_NB - 1
                bb = BTree.BB_Piece(c, pc)
                While bb > 0
                    sq = Square(bb)
                    bb = bb Xor ABB_Mask(sq)
                    key = key Xor PieceRand(c, pc, sq)
                End While
            Next pc
        Next c
        Return key
    End Function
End Module
