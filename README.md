目的
====

このプロジェクトでは MVVM のサンプルを作り、
MVVM がどう構成されるか具体的にコードを確認します。

構成
====

Model
-----

### Position

- Near
- Middle
- Far

### Hero

- Name
- Hp
- Attack
- Position

### Party

4 人の Hero で構成します。

一連の編成の変更はトランザクションです。
つまりロールバック可能です。

View
----

UI Toolkit で構成します。

現在の Party の Hero 枠をリストします。
枠は Hero を編成できます。未設定の場合は
空欄です。

枠を選択すると Hero 変更ダイアログを表示します。

Hero 変更ダイアログで選択した Hero を持って
Party を更新します。
