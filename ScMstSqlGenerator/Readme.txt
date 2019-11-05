## リポジトリのクローン
* [sc_tools リポジトリ](https://github.com/alimdevelop/sc_tools)

## フォルダ構成
sc_tools
　└ ScMstSqlGenerator
　　　├ output
　　　│　├ *.sql
　　　├ log
　　　│　├ erro.txt (エラー発生時に出力される）
　　　├ origin
　　　│　├ Define.txt（定義名リスト、基本的には触らない、必須）
　　　│　├ Env.txt (環境設定ファイル）
　　　├ mst
　　　│　├ Env.txt（origin/Env.txtをコピーしたもの、必須）
　　　│　├ *.xlsx（マスタファイル）
　　　├ Generate.bat

##Env.txt内容
※行数は固定の為変更しないこと
//Define作成フラグ
1　　　　　　　　　　　　　　→定義名リストを作成する場合は１、しない場合は０

//出力環境（一つだけ指定）
//dev
//check
//plan 
//stage
prod　　　　　　　　　　　　　→出力する環境をコメントアウト

//出力するファイル名
itemTest.xlsx　　　　　　　　　→sql出力するマスタをコメントアウト
//unitTest.xlsx　
//varietyTest.xlsx　
//worldTest.xlsx

## ScMstSqlGenerator使い方①Env.txtの内容のマスタを変換したい場合
* `sc_tools/ScMstSqlGenerator`フォルダに移動
* `origin/Env.txt`を`mst`フォルダへコピー（無い場合）
* `Env.txt`の内容を必要に応じて変更
* `Generate.bat`を実行
* `output`フォルダにsqlが出力される


## ScMstSqlGenerator使い方②必要なファイルだけドラッグ＆ドロップして変換したい場合
* `sc_tools/ScMstSqlGenerator`フォルダに移動
* `origin/Env.txt`を`mst`フォルダへコピー（無い場合）
* `Generate.bat`にsql出力したい*.xlsxをドラッグ＆ドロップ
* `output`フォルダにsqlが出力される