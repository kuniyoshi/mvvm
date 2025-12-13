目的
====

このプロジェクトでは MVVM のサンプルを作り、
MVVM がどう構成されるか具体的にコードを確認します。

構成
====

レイヤ構成
----------

- Model レイヤ
  ドメインモデル（Hero, Party など）と、その操作（編成変更）を表現します。

- ViewModel レイヤ
  View に表示するための状態・コマンドを公開します。
  Party の編成変更を「トランザクション」として扱い、確定／キャンセルを制御します。

- View レイヤ
  UI Toolkit (UXML / USS) で構成します。
  ViewModel にデータバインドし、ユーザー操作をコマンドとして ViewModel に伝えます。

Model
=====

ファイルは Assets/Scripts/Mvvm/Model/ 以下に保存します。

Position
--------

- `Near`
- `Middle`
- `Far`

Hero
----

- `Name`
- `Hp`
- `Attack`
- `Position`

Position は編成の枠とは独立しています。

Party
-----

Party は 4 人の Hero で構成します。

4 人の Hero はユニークです。

トランザクション機能
====================

一連の編成の変更はトランザクションです。
つまりロールバック可能です。

View
====

ファイルは Assets/Scripts/Mvvm/View/ 以下に保存します。

UI Toolkit で構成します。

現在の Party の Hero 枠をリストします。
枠は Hero を編成できます。未設定の場合は
空欄です。

枠を選択すると Hero 変更ダイアログを表示します。

Hero 変更ダイアログで選択した Hero を持って
Party を更新します。
