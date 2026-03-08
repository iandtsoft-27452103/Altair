# About Pull Request

This repository is read-only, so Pull Request is not accepted. Thank you for your understanding.

# Caution

If you use this software for commercial, you must pay fees to NVIDIA.

# Altair

Altair is a shogi engine, written by Visual Basic.

Shogi is game like chess.

This software uses follwing technologies.

(1) Convolutional Neural Network

(2) Monte Carlo Tree Search

(1) and (2) are written by Visual Basic.

## Convolutional Neural Network

Convolutional Neural Network is composed of Policy Network and Value Network. Policy Network predicts best move of current position. Value Network evaluates current position.

### Input Features of Neural Network

(1) position of pieces

(2) hand pieces

(3) turn

### Output Labels of Policy Neural Network

(1) position of move to

(2) move direction

The number of move directions are 32.

### Output Labels of Value Neural Network

win or lost

## Learning functions

Learning functions are written by PyTorch. Policy Neural Network learns multi task classification. Value Neural Network learns binary classification. This source code is packed together with Asklepios. Learned models are converted to ONNX.

### Record format for learning

Record format for learning is as below.

B,119,2726FU,3334FU,7776FU,4344FU, ...

First column is game result. Second Column is game ply. The following columns are moves. When learning Value Neural Network, you use first column. And learning Policy Neural Network, you use columns from third column to last column.

This software use the records created by Gikou-2.0.1.

## Source Code Explanation

(1) Analyze.vb : Functions of analyzing records.

(2) AttacksOperation.vb : Functions of piece attacks.

(3) BitOperation.vb : Functions of bit operations.

(4) Board.vb : Functions of shogi board.

(5) Common.vb : Common constants and variables.

(6) CSA.vb : Functions of CSA format.

(7) Feature.vb : Functions of input features.

(8) GenMoves.vb : Functions for generating moves.

(9) Hash.vb : Functions for hash value.

(10) Init.vb : Functions for initializing attack tables and so on.

(11) IO.vb : Functions for reading records.

(12) Label.vb : Function of output labels.

(13) Mate.vb : Functions for mate search.

(14) Mate1Ply.vb : Function for mate in one ply.

(15) MCTS.vb : Functions for 

(16) Move.vb : Functions for moves.

(17) Program.vb: The entry point of this software.

(18) SFEN.vb : Functions for SFEN.

(19) Test.vb : Functions for testing for generating moves, do move, undo move and so on.

(20) Test2.vb :Functions for testing for CNN.

(21) TT.vb : Functions for transposition table for alpha-beta search.

* Alpha-beta search functions are not implemented yet.

(22) TT2.vb : Functions for transposition table for MCTS.

## Operating environment

(1) OS: Windows 11 Pro

(2) Memory: 16GB or more. About 32GB is recommended.

(3) Memory usage on C# side: Less than 300MB when the MCTS task is 6.

(4) .NET Version: .NET 9.0

(5) Memory usage on the PyTorch side: Compared with Asklepios, memory usage is not heavy.

(6) Python's version: 3.13.12

(7) This software uses Microsoft.ML.OnnxRuntime.Gpu. This library is installed by NuGet Manager.

(8) It is necessary that CUDA and cuDNN corresponding to Microsoft.ML.OnnxRuntime.Gpu are installed.

## How to build

Double click "Altair.sln" and build with using Visual Studio. I identified this software is running in debug build and do not identified in release build.

## Known bugs

(1) As running Value Network, occasionally raise error. The cause of the error is not identified.

## Known problems

(1) On the play out function in MCTS, Rollout policy is not implemented.

(2) Compared to top-class software, the recognition accuracy of Policy Network is poor.

(3) This software does not support USI.

(4) If you create an executable file in a release build, you will get a deadlock when searching.

## How to use

If you navigate to the cnn folder and execute the start.bat, the specified game record will be analyzed. The command-line arguments are as follows:

(1) The number of tasks for MCTS   max = 6, min = 1

(2) The number of tasks for mate search   max = 2, min = 1

(3) Thinking seconds per one move

(4) Mate search depth

(5) Output file name

(6) Record file name for analyzing

(7) Game date

(8) Match name

(9) Black Player name

(10) White Player name

## Contents of Release file

The execution environment of this software is contained in the Release file.

## References

I developed this software referring to the softwares as below.

(1) Bonanza

(2) Apery

(3) YaneuraOu

(4) Gikou

(5) dlshogi

As far as I know, the source code for Bonanza and dlshogi is currently not publicly available.

I think I'll add search functions and analyze records functions.

I developed this software referring to the books as below. All books are written in Japanese, so I write the name of the books in Japanese.

(1) 山岡忠夫(2018),『将棋AIで学ぶディープラーニング』マイナビ出版 

(2) 山岡忠夫、加納邦彦(2021), 『強い将棋ソフトの創りかた　Pythonで実装するディープラーニング将棋AI』マイナビ出版

(3) 大槻知史(著)、三宅陽一郎(監修)(2018), 『最強囲碁AI アルファ碁解体新書　増補改訂版』翔泳社

(4) 原田達也(2017), 機械学習プロフェッショナルシリーズ『画像認識』講談社

## About the future

I will try to improve this software in a different way than other developers as much as possible.
