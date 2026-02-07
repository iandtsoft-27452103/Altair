# About Pull Request

This repository is read-only, so Pull Request is not accepted. Thank you for your understanding.

# Altair

Altair is a shogi engine framework, written by Visual Basic.

Shogi is game like chess.

## Source Code Explanation

(1) AttacksOperation.vb : Functions of piece attacks.

(2) BitOperation.vb : Functions of bit operations.

(3) Board.vb : Functions of shogi board.

(4) Common.vb : Common constants and variables.

(5) CSA.vb : Functions of CSA format.

(6) GenMoves.fs : Functions for generating moves.

(7) Hash.vb : Functions for hash value.

(8) Init.vb : Functions for initializing attack tables and so on.

(9) IO.fs : Functions for reading records.

(10) Mate.vb : Functions for mate search.

(11) Mate1Ply.vb : Function for mate in one ply.

(12) Move.vb : Functions for moves.

(13) Program.vb: The entry point of this software.

(14) SFEN.vb : Functions for SFEN.

(15) Test.vb : Functions for testing.

(16) TT.vb : Functions for transposition table.

## Operating environment

(1) OS: Windows 11 Pro

(2) .NET Version: .NET 9.0

## How to build

Double click "Altair.sln" and build with using Visual Studio. 

## References

I developed this software referring to the softwares as below.

(1) Bonanza

(2) Apery

(3) YaneuraOu

(4) Gikou

(5) dlshogi

As far as I know, the source code for Bonanza and dlshogi is currently not publicly available.

## About the future

I think I'll add search functions and analyze records functions.
