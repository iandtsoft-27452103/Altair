Imports Altair.Common
Imports BitBoard = System.UInt128

Module BitOperation
    Public Function Square(ByVal bb As BitBoard) As Integer
        Dim y As BitBoard
        y = BitBoard.TrailingZeroCount(bb)
        Return Square_NB - 1 - y
    End Function
End Module
