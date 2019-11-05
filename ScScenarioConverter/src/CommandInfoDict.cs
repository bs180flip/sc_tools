using System.Collections.Generic;

namespace Sc.Scenario
{
	/// <summary>
	/// コマンド情報辞書
	/// </summary>
	public class CommandInfoDict
	{
		/// <summary>コマンド情報リスト</summary>
		private Dictionary<string, CommandInfo> _commandInfoDict = new Dictionary<string, CommandInfo>()
		{
			{ "マップ読込", new CommandInfo(CommandType.LoadMap, "指定マップIDのマップデータを読み込む"
				, new ArgInfo(ArgType.Long, "マップID")
			)},
			{ "マップ解放", new CommandInfo(CommandType.UnloadMap, "指定マップIDのマップデータを解放"
				, new ArgInfo(ArgType.Long, "マップID")
			)},
			{ "キャラクター読込", new CommandInfo(CommandType.LoadChara, "指定ユニットIDのバストアップ画像を読み込む（現状仮ディーンのみ ID1-5設定は可）"
				, new ArgInfo(ArgType.Long, "ユニットID 1:ディーン 2:リア 3:アーヴィン 4:ミランダ 5:ザイン")
			)},
			{ "キャラクター解放", new CommandInfo(CommandType.UnloadChara, "指定ユニットIDのバストアップ画像を解放する（現状仮ディーンのみ ID1-5設定は可）"
				, new ArgInfo(ArgType.Long, "ユニットID 1:ディーン 2:リア 3:アーヴィン 4:ミランダ 5:ザイン")
			)},
			{ "キャラクター全解放", new CommandInfo(CommandType.UnloadCharaAll, "読み込み済みの全てのバストアップ画像を解放する"
			)},
			{ "オブジェクト読込", new CommandInfo(CommandType.LoadObject, "OBJECT_LAYOUT_GROUP_MSTに登録されたユニット、オブジェクト、etcの中で同IDのうちの単品の中身をメモリに読み込む。同時に定義名の付与も可。"
				, new ArgInfo(ArgType.Long, "OBJECT_LAYOUT_GROUP_ID")
				, new ArgInfo(ArgType.Int, "OBJECT_LOCAL_ID")
				, new ArgInfo(ArgType.String, "定義名", isOptional: true)
			)},
			{ "オブジェクト破棄", new CommandInfo(CommandType.UnloadObject, "load_objectで読み込んだ定義名データをメモリから破棄する。引数を省略した場合は、load_objectで読み込んだ全てのデータを破棄。"
  				, new ArgInfo(ArgType.String, "定義名")
			)},
			{ "時間待ち", new CommandInfo(CommandType.TimeWait, "指定秒数待機する"
				, new ArgInfo(ArgType.Float, "待機する秒数")
			)},
			{ "タッチ待ち", new CommandInfo(CommandType.TouchWait, "画面がタッチされるまで待機する"
			)},
			{ "フェードイン", new CommandInfo(CommandType.FadeIn, "フェードインさせる"
				, new ArgInfo(ArgType.Float, "フェードに掛ける秒数")
				, new ArgInfo(ArgType.Color, "カラー (0 ~ 255 で R|G|B の形式)")
				, new ArgInfo(ArgType.Byte, "レイヤー (1:UIの前面, 2:キャラクターの前面, 3:背景の前面)", 1, 3)
				, new ArgInfo(ArgType.Byte, "最終値 (0 ~ 100 の範囲)", 0, 100)
			)},
			{ "フェードアウト", new CommandInfo(CommandType.FadeOut, "フェードアウトさせる"
				, new ArgInfo(ArgType.Float, "フェードに掛ける秒数")
				, new ArgInfo(ArgType.Color, "カラー (0 ~ 255 で R|G|B の形式)")
				, new ArgInfo(ArgType.Byte, "レイヤー (1:UIの前面, 2:キャラクターの前面, 3:背景の前面)", 1, 3)
				, new ArgInfo(ArgType.Byte, "最終値 (0 ~ 100 の範囲)", 0, 100)
			)},
			{ "フラッシュ", new CommandInfo(CommandType.FlashScreen, "フラッシュさせる"
				, new ArgInfo(ArgType.Float, "1回のフラッシュに掛ける秒数")
				, new ArgInfo(ArgType.Color, "カラー (0 ~ 255 で R|G|B の形式)")
				, new ArgInfo(ArgType.Byte, "レイヤー (1:UIの前面, 2:キャラクターの前面, 3:背景の前面)", 1, 3)
				, new ArgInfo(ArgType.Byte, "最終値 (0 ~ 100 の範囲)", 0, 100)
				, new ArgInfo(ArgType.Int, "フラッシュの回数")
			)},
			{ "フェード待ち", new CommandInfo(CommandType.FadeWait, "フェードが終わるまで待機する。フェードイン、フェードアウト、フラッシュで使用"
			)},
			{ "メッセージ表示", new CommandInfo(CommandType.MsgShow, "メッセージを表示する"
				, new ArgInfo(ArgType.String, "メッセージテキスト")
				//, new ArgInfo(ArgType.String, "話者名")  別コマンドへ切り出しの為、外す
				, new ArgInfo(ArgType.Byte, "表示方法 (1:即時, 2:フェードイン, 3:タイピング)", 1, 3)
				, new ArgInfo(ArgType.Float, "2:フェードインは1行あたりの間隔設定（参考値0.8）。3:タイピング 1文字あたりの間隔設定（参考値0.08）。1:即時の場合は0を設定")
			)},
			{ "メッセージ消去", new CommandInfo(CommandType.MsgHide, "メッセージを消去する"
				, new ArgInfo(ArgType.Byte, "消去方法 (1:即時, 2:フェードアウト)", 1, 2)
				, new ArgInfo(ArgType.Float, "2:フェードアウト時間設定(参考値0.8）｡1:即時の場合は0を設定")
			)},
			{ "テロップ表示", new CommandInfo(CommandType.TelopShow, "テロップを表示する"
				, new ArgInfo(ArgType.String, "テロップテキスト")
				, new ArgInfo(ArgType.Byte, "表示方法 (1:即時, 2:フェードイン, 3:タイピング)", 1, 3)
				, new ArgInfo(ArgType.Float, "2:フェードインは1行あたりの間隔設定（参考値0.8）。3:タイピング 1文字あたりの間隔設定（参考値0.08）。1:即時の場合は0を設定")
				, new ArgInfo(ArgType.String, "テロップ名　テロップ消去と対になります。")
				, new ArgInfo(ArgType.Vector2, "座標 ( X|Y の形式)")
				, new ArgInfo(ArgType.Byte, "boxのアンカー(0:左上, 1:中央上, 2:右上)", 0, 2, isOptional: true)
				, new ArgInfo(ArgType.Byte, "box配下の行 寄せ(0:左寄せ, 1:中央寄せ, 2:右寄せ)", 0, 2, isOptional: true)
			)},
			{ "テロップ消去", new CommandInfo(CommandType.TelopHide, "テロップを消去する"
				, new ArgInfo(ArgType.Byte, "消去方法 (1:即時, 2:フェードアウト)", 1, 2)
				, new ArgInfo(ArgType.Float, "2:フェードアウト時間設定(参考値0.8）｡1:即時の場合は0を設定")
				, new ArgInfo(ArgType.String, "テロップ名　テロップ表示と対になります。")
			)},
			{ "全テロップ消去", new CommandInfo(CommandType.AllTelopHide, "全てのテロップを消去する"
			)},
			{ "テロップ待ち", new CommandInfo(CommandType.TelopWait, "テロップが終わるまで待機する。テロップ表示、テロップ消去で使用"
			)},
			{ "話者表示", new CommandInfo(CommandType.SpeakerShow, "話者を表示する"
				, new ArgInfo(ArgType.String, "話者の名前")
			)},
			{ "話者消去", new CommandInfo(CommandType.SpeakerHide, "話者を消去する"
			)},
			{ "メッセージウィンドウ表示", new CommandInfo(CommandType.MsgWndShow, "メッセージウィンドウを表示する"
				, new ArgInfo(ArgType.Float, "表示に掛ける秒数")
			)},
			{ "メッセージウィンドウ消去", new CommandInfo(CommandType.MsgWndHide, "メッセージウィンドウを消去する"
				, new ArgInfo(ArgType.Float, "消去に掛ける秒数")
			)},
			{ "メッセージウィンドウ待ち", new CommandInfo(CommandType.MsgWndWait, "メッセージウィンドウ待ちをする"
			)},
			{ "キャラクター表示", new CommandInfo(CommandType.CharaShow, "キャラクターを表示する（現状仮ディーンのみ ID1-5設定は可）"
				, new ArgInfo(ArgType.Long, "ユニットID 1:ディーン 2:リア 3:アーヴィン 4:ミランダ 5:ザイン")
				, new ArgInfo(ArgType.Byte, "表情ID 1:普通 2:笑顔 3:怒り 4:悲しみ 5:驚き 6:叫び 7:特殊1 8:特殊2 9:特殊3")
				, new ArgInfo(ArgType.Float, "表示に掛ける秒数")
				, new ArgInfo(ArgType.Byte, "不透明度 (0 ~ 100)", 0, 100)
			)},
			{ "キャラクター消去", new CommandInfo(CommandType.CharaHide, "キャラクターを消去する（現状仮ディーンのみ ID1-5設定は可）"
				, new ArgInfo(ArgType.Long, "ユニットID  1:ディーン 2:リア 3:アーヴィン 4:ミランダ 5:ザイン")
				, new ArgInfo(ArgType.Byte, "表情ID 1:普通 2:笑顔 3:怒り 4:悲しみ 5:驚き 6:叫び 7:特殊1 8:特殊2 9:特殊3")
				, new ArgInfo(ArgType.Float, "消去に掛ける秒数")
			)},
			{ "キャラクター待ち", new CommandInfo(CommandType.CharaWait, "キャラクターの演出が完了するまで待機する"
			)},
			{ "カメラ振動", new CommandInfo(CommandType.CameraShake, "カメラを振動させる"
				, new ArgInfo(ArgType.Float, "振動の秒数")
				, new ArgInfo(ArgType.Float, "振動の振れ幅の大きさ 参考値0.7")
				, new ArgInfo(ArgType.Float, "振動減少係数 参考値1")
			)},
			{ "カメラ振動待ち", new CommandInfo(CommandType.CameraShakeWait, "カメラの振動が完了するまで待機する"
			)},
			{ "カメラ振動停止", new CommandInfo(CommandType.CameraShakeStop, "カメラの振動を停止する"
			)},
			{ "カメラ首振り", new CommandInfo(CommandType.CameraPan, "カメラを移動（PAN）させる"
				, new ArgInfo(ArgType.Float, "移動の秒数")
				, new ArgInfo(ArgType.Byte, "移動方向 (1:上, 2:下, 3:左, 4:右)", 1, 4)
				, new ArgInfo(ArgType.Float, "回転角度　360度以上にも対応")
			)},
			{ "カメラ首振り待ち", new CommandInfo(CommandType.CameraPanWait, "カメラの移動(PAN)が完了するまで待機する"
			)},
			{ "カメラ画角変更", new CommandInfo(CommandType.ChangeFieldOfView, "カメラの画角を変更する"
				, new ArgInfo(ArgType.Float, "画角（1-179)", min: 1, max: 179)
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "カメラ表示範囲変更", new CommandInfo(CommandType.ChangeClippingPlanes, "カメラの遠近の表示範囲を変更する"
				, new ArgInfo(ArgType.Float, "Near")
				, new ArgInfo(ArgType.Float, "Far")
			)},
			{ "カメラ座標設定", new CommandInfo(CommandType.CameraPointSet, "カメラの座標を設定する"
				, new ArgInfo(ArgType.Vector3, "座標 ( X|Y|Z の形式)")
			)},
			{ "カメラ角度設定", new CommandInfo(CommandType.CameraAngleSet, "カメラの角度を設定する"
				, new ArgInfo(ArgType.Vector3, "角度 ( X|Y|Z の形式)")
				, new ArgInfo(ArgType.Float, "秒数", min: 0, isOptional: true)
			)},
			{ "SE再生", new CommandInfo(CommandType.SePlay, "SEを鳴らす"
				, new ArgInfo(ArgType.String, "キューシート名")
				, new ArgInfo(ArgType.String, "キュー名")
				, new ArgInfo(ArgType.Float, "フェードまでの時間(秒)", min: 0, isOptional: true)
			)},
			{ "SE待ち", new CommandInfo(CommandType.SeWait, "SEが完了するまで待機する（ループしないSEのみ使用可）"
			)},
			{ "SE停止", new CommandInfo(CommandType.SeStop, "SEを止める"
				, new ArgInfo(ArgType.String, "キュー名")
				, new ArgInfo(ArgType.Float, "フェードまでの時間(秒)", min: 0, isOptional: true)
			)},
			{ "SE音量", new CommandInfo(CommandType.SeVol, "SEの音量調整"
			    , new ArgInfo(ArgType.String, "キュー名")
				, new ArgInfo(ArgType.Int, "音量％", min: 0)
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "SEループ再生", new CommandInfo(CommandType.SeLoop, "SEのループ再生"
				, new ArgInfo(ArgType.String, "キューシート名")
				, new ArgInfo(ArgType.String, "キュー名")
				, new ArgInfo(ArgType.Float, "間隔", min: 0.0)
				, new ArgInfo(ArgType.Int, "回数", min: 0, isOptional: true)
				, new ArgInfo(ArgType.Float, "秒数", min: 0, isOptional: true)
			)},
			{ "SEループ停止", new CommandInfo(CommandType.SeLoopStop, "SEのループ停止"
				, new ArgInfo(ArgType.String, "キュー名")
				, new ArgInfo(ArgType.Float, "秒数", min: 0.0, isOptional: true)
			)},
			{ "BGM再生", new CommandInfo(CommandType.BgmPlay, "BGMを鳴らす"
				, new ArgInfo(ArgType.String, "キューシート名(キュー名)")
				, new ArgInfo(ArgType.Bool, "リスタートするかどうか。同じキューシート名のBGM演奏中のときリスタートする。TRUE or FALSE")
				, new ArgInfo(ArgType.Bool, "ループするかどうか。TRUE or FALSE")
				, new ArgInfo(ArgType.Float, "フェードまでの時間(秒)", min: 0, isOptional: true)
			)},
			{ "BGM待ち", new CommandInfo(CommandType.BgmWait, "BGMが完了するまで待機する（ループしないBGMのみ使用可）"
			)},
			{ "BGM停止", new CommandInfo(CommandType.BgmStop, "BGMを止める"
				, new ArgInfo(ArgType.String, "キューシート名(キュー名)")
				, new ArgInfo(ArgType.Float, "フェードまでの時間(秒)", min: 0, isOptional: true)
			)},
			{ "BGM音量", new CommandInfo(CommandType.BgmVol, "BGMの音量調整"
			    , new ArgInfo(ArgType.String, "キュー名")
				, new ArgInfo(ArgType.Int, "音量％", min: 0)
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "VOICE再生", new CommandInfo(CommandType.VoicePlay, "VOICEを鳴らす"
				, new ArgInfo(ArgType.String, "キューシート名")
				, new ArgInfo(ArgType.String, "キュー名")
				, new ArgInfo(ArgType.Bool, "同キューシートのVOICEを止めるかどうか。TRUE or FALSE")
				, new ArgInfo(ArgType.Float, "フェードまでの時間(秒)", min: 0, isOptional: true)
			)},
			{ "VOICE待ち", new CommandInfo(CommandType.VoiceWait, "VOICEが完了するまで待機する"
				, new ArgInfo(ArgType.Float, "フェードまでの時間(秒)", min: 0, isOptional: true)
			)},
			{ "VOICE停止", new CommandInfo(CommandType.VoiceStop, "VOICEを止める"
   				, new ArgInfo(ArgType.String, "キューシート名(キュー名)")
				, new ArgInfo(ArgType.Float, "フェードまでの時間(秒)", min: 0, isOptional: true)
			)},
			{ "VOICE音量", new CommandInfo(CommandType.VoiceVol, "VOICEの音量調整"
			    , new ArgInfo(ArgType.String, "キュー名")
				, new ArgInfo(ArgType.Int, "音量％", min: 0)
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "サウンド停止", new CommandInfo(CommandType.SoundStop, "BGM、SE、VOICEを止める"
			    , new ArgInfo(ArgType.Float, "フェードアウト完了までの秒数", min: 0.0, isOptional: true)
			)},
			//{ "シーン遷移", new CommandInfo(CommandType.SceneChange, "指定のシーン名のシーンに遷移させる"
			//	, new ArgInfo(ArgType.String, "シーン名")
			//)},
			{ "ホームシーン遷移", new CommandInfo(CommandType.SceneChangeHome, "ホームシーンに遷移させる"
			)},
			{ "スクリプト呼び出し", new CommandInfo(CommandType.CallScript, "同スクリプトグループ内の指定優先度のスクリプト再生を開始する"
				, new ArgInfo(ArgType.Int, "表示順（SCRIPT_MST設定）")
			)},
			//{ "ミッション呼び出し", new CommandInfo(CommandType.CallMission, "指定ミッションIDのミッションを開始する"
			//	, new ArgInfo(ArgType.Long, "ミッションID")
			//)},
			{ "コマンドジャンプ先", new CommandInfo(CommandType.JumpTarget, "このコマンドがある行数へジャンプするための目印"
				, new ArgInfo(ArgType.String, "ジャンプ先名称")
			)},
			{ "コマンドジャンプ元", new CommandInfo(CommandType.JumpTo, "このコマンドがある行数から指定したジャンプ先へ飛ばす"
				, new ArgInfo(ArgType.String, "ジャンプ先名称")
			)},
			//{ "コマンドジャンプ選択ボタン設定", new CommandInfo(CommandType.JumpSelectSet, "このボタンにコマンドジャンプ先を設定する"
			//	, new ArgInfo(ArgType.Int, "セットするボタンナンバー")
			//	, new ArgInfo(ArgType.String, "ボタン表示テキスト")
			//	, new ArgInfo(ArgType.String, "ジャンプ先名称")
			//)},
			//{ "コマンドジャンプ選択ボタン待ち", new CommandInfo(CommandType.JumpSelectWait, "コマンドジャンプ選択ボタンのいずれかが押されるのを待つ"
			//	//, new ArgInfo(ArgType.Int, "選択肢設定ボタン数")
			//	//, new ArgInfo(ArgType.String, "選択肢メッセージ")
			//)},
			{ "スクリプトエンド", new CommandInfo(CommandType.EndScenario, "スクリプトの終了。なおスクリプト呼び出しコマンドの後であっても各スクリプト最後につけること"
			)},
			{ "変数設定", new CommandInfo(CommandType.VarSet, "Long型の変数を設定する"
				, new ArgInfo(ArgType.String, "変数名", isReplaceVariable: false)
				, new ArgInfo(ArgType.Long, "数値（整数）")
			)},
			{ "変数加算", new CommandInfo(CommandType.VarAdd, "変数を加算する"
				, new ArgInfo(ArgType.String, "変数名", isReplaceVariable: false)
				, new ArgInfo(ArgType.Long, "数値（整数）")
			)},
			{ "変数減算", new CommandInfo(CommandType.VarSub, "変数を減算する"
				, new ArgInfo(ArgType.String, "変数名", isReplaceVariable: false)
				, new ArgInfo(ArgType.Long, "数値（整数）")
			)},
			{ "変数代入", new CommandInfo(CommandType.VarEqual, "変数に代入する（入れ替える）"
				, new ArgInfo(ArgType.String, "変数名", isReplaceVariable: false)
				, new ArgInfo(ArgType.Long, "数値（整数）")
			)},
			{ "ループ開始", new CommandInfo(CommandType.LoopStart, "繰り返し処理の起点。ループ処理の名前を指定する"
				, new ArgInfo(ArgType.String, "ループ名", isReplaceVariable: false)
				, new ArgInfo(ArgType.Int, "ループ回数")
			)},
			{ "ループ終了", new CommandInfo(CommandType.LoopEnd, "繰り返し処理の終点。ループ開始と対になる名前を指定する"
				, new ArgInfo(ArgType.String, "ループ名", isReplaceVariable: false)
			)},
			{ "IF判定（変数）", new CommandInfo(CommandType.IfVar, "変数と値（整数）を比較判定する"
				, new ArgInfo(ArgType.String, "変数名", isReplaceVariable: false)
				, new ArgInfo(ArgType.String, "比較演算子　=,<=,>=,>,<,")
				, new ArgInfo(ArgType.Int, "数値")
				, new ArgInfo(ArgType.Bool, "真偽　TRUE or FALSE")
				, new ArgInfo(ArgType.String, "飛び先　ジャンプ先名称")
			)},
			{ "変数（Vector3）設定", new CommandInfo(CommandType.VarVector3Set, "Vector3型の変数を設定する"
				, new ArgInfo(ArgType.String, "変数名", isReplaceVariable: false)
				, new ArgInfo(ArgType.Vector3, "Vector3 ( X|Y|Z の形式)")
			)},
			{ "変数（Vector3）加算", new CommandInfo(CommandType.VarVector3Add, "Vector3変数を加算する"
				, new ArgInfo(ArgType.String, "変数名", isReplaceVariable: false)
				, new ArgInfo(ArgType.Vector3, "Vector3 ( X|Y|Z の形式)")
			)},
			{ "変数（Vector3）減算", new CommandInfo(CommandType.VarVector3Sub, "Vector3型の変数を減算する"
				, new ArgInfo(ArgType.String, "変数名", isReplaceVariable: false)
				, new ArgInfo(ArgType.Vector3, "Vector3 ( X|Y|Z の形式)")
			)},
			{ "変数（Vector3）代入", new CommandInfo(CommandType.VarVector3Equal, "Vector3変数に代入する（入れ替える）"
				, new ArgInfo(ArgType.String, "変数名", isReplaceVariable: false)
				, new ArgInfo(ArgType.Vector3, "Vector3 ( X|Y|Z の形式)")
			)},
			{ "アクター定義", new CommandInfo(CommandType.ObjDefine, "読み込まれたオブジェクトＧマスタの個々のアクターに定義名を割り当て、アクターコマンドで制御できるようにする。"
				, new ArgInfo(ArgType.Long, "OBJECT_LAYOUT_GROUP_ID")
				, new ArgInfo(ArgType.Int, "OBJECT_LOCAL_ID")
				, new ArgInfo(ArgType.String, "定義名")
			)},
			{ "アクター定義（ユニット）", new CommandInfo(CommandType.ObjDefineUnit, "アクター定義のユニットID指定版。プレイヤー側のキャラはローカルIDだと判別できないので、UNIT_IDで識別する。"
				, new ArgInfo(ArgType.Long, "OBJECT_LAYOUT_GROUP_ID")
				, new ArgInfo(ArgType.Int, "UNIT_ID")
				, new ArgInfo(ArgType.String, "定義名")
			)},
			{ "アクター表示", new CommandInfo(CommandType.ObjOn, "定義されたアクターを表示する。秒数はフェードイン完了までの時間（透明度省略時、省略可）。透明度は完了時の透明度を示し、省略可（省略時=透明度0）。"
				, new ArgInfo(ArgType.String, "定義名")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
				, new ArgInfo(ArgType.Byte, "透明度", min: 0, max: 100, isOptional: true)
			)},
			{ "アクター非表示", new CommandInfo(CommandType.ObjOff, "定義されたアクターを非表示にする。秒数はフェードアウト完了までの時間（透明度省略時、省略可）。透明度は完了時の透明度を示し、省略可（省略時=透明度100）。"
				, new ArgInfo(ArgType.String, "定義名")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
				, new ArgInfo(ArgType.Byte, "透明度", min: 0, max: 100, isOptional: true)
			)},
			{ "アクター配置", new CommandInfo(CommandType.ObjPosition, "定義されたアクターを指定座標に配置する。"
				, new ArgInfo(ArgType.String, "定義名")
				, new ArgInfo(ArgType.Vector3, "絶対座標X|Y|Z")
			)},
			{ "アクター拡縮", new CommandInfo(CommandType.ObjScale, "定義されたアクターを拡大縮小する。完了までの時間は省略可。"
				, new ArgInfo(ArgType.String, "定義名")
  				, new ArgInfo(ArgType.Vector3, "拡縮率")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "アクター回転", new CommandInfo(CommandType.ObjRotation, "定義されたアクターを回転させる。完了までの時間は省略可。"
				, new ArgInfo(ArgType.String, "定義名")
  				, new ArgInfo(ArgType.Vector3, "軸の角度")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "アクター向き", new CommandInfo(CommandType.ObjDir, "定義されたアクターの向きを指定する。"
				, new ArgInfo(ArgType.String, "定義名")
				, new ArgInfo(ArgType.Int, "向き(DirectionId)")
			)},
			{ "アニメ切替", new CommandInfo(CommandType.ObjAnim, "定義されたアクターのアニメを指定する。"
				, new ArgInfo(ArgType.String, "定義名")
				, new ArgInfo(ArgType.Int, "アニメーションId(AnimationCondId)")
			)},
			// { "アニメ設定", new CommandInfo(CommandType.ObjAnimSet, "定義されたアクターの中でUNITが参照するANIMATION_GROUP_IDをイベント中だけ差し替える。アセット自体はload_animで読み込んでおく。"
			// 	, new ArgInfo(ArgType.String, "定義名")
			// 	, new ArgInfo(ArgType.Int, "ANIMATION_GROUP_ID")
			// )},
			{ "アクター移動（絶対）", new CommandInfo(CommandType.ObjMove, "定義されたアクターを指定座標まで移動させる。完了までの時間は省略可。"
				, new ArgInfo(ArgType.String, "定義名")
				, new ArgInfo(ArgType.Vector3, "絶対座標X|Y|Z")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "アクター移動（相対）", new CommandInfo(CommandType.ObjMoveRel, "定義されたアクターを指定座標まで相対移動させる。完了までの時間は省略可。"
				, new ArgInfo(ArgType.String, "定義名")
				, new ArgInfo(ArgType.Vector3, "相対座標X|Y|Z")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			// { "アクター移動（ナビ）", new CommandInfo(CommandType.ObjMoveNavi, "定義されたアクターを指定座標までNavimesh情報を利用して移動させる。完了までの時間は省略可。"
			// 	, new ArgInfo(ArgType.String, "定義名")
			// 	, new ArgInfo(ArgType.Vector3, "絶対座標X|Y|Z")
			// 	, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			// )},
			{ "アクター移動待ち", new CommandInfo(CommandType.ObjMoveWait, "定義されたアクターの移動が終わるまで待つ"
				, new ArgInfo(ArgType.String, "定義名")
			)},
			{ "アクター座標記録", new CommandInfo(CommandType.ObjPosMemory, "定義されたアクターの現在座標を取得する。"
				, new ArgInfo(ArgType.String, "定義名")
				, new ArgInfo(ArgType.String, "変数名")
			)},
			{ "アクター座標読込", new CommandInfo(CommandType.ObjPosMemoryLoad, "obj_pos_memoryの変数から座標を読み出し、定義されたアクターに反映させる。"
				, new ArgInfo(ArgType.String, "定義名")
				, new ArgInfo(ArgType.String, "変数名")
			)},
			{ "カメラ追従注視点", new CommandInfo(CommandType.CameraTarget, "追従カメラの注視点を設定。"
				, new ArgInfo(ArgType.Vector3, "注視点座標")
			)},
			{ "カメラ追従キャラ", new CommandInfo(CommandType.CameraChr, "追従カメラの対象（キャラ）の設定。"
				, new ArgInfo(ArgType.String, "オブジェクト定義名")
			)},
			{ "カメラ移動", new CommandInfo(CommandType.CameraPos, "カメラ自体の移動。"
				, new ArgInfo(ArgType.Vector3, "座標")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
				// Note: 暫定的な追加
				, new ArgInfo(ArgType.Bool, "角度も変更するかどうか", isOptional: true)
				, new ArgInfo(ArgType.Vector3, "角度", isOptional: true)
				, new ArgInfo(ArgType.Bool, "注視点座標も変更するかどうか", isOptional: true)
				, new ArgInfo(ArgType.Vector3, "注視点座標", isOptional: true)
				, new ArgInfo(ArgType.Bool, "注視点キャラも変更するかどうか(注視点座標変更が優先)", isOptional: true)
				, new ArgInfo(ArgType.String, "注視点キャラ", isOptional: true)
			)},
			{ "カメラ移動（注視）", new CommandInfo(CommandType.CameraPosAndLookAt, "カメラ自体の移動。"
				, new ArgInfo(ArgType.Vector3, "座標")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "カメラ移動待ち", new CommandInfo(CommandType.CameraPosWait, "カメラ自体の移動の待ち処理。"
			)},
			// { "キャラ追従カメラ", new CommandInfo(CommandType.CameraSetChr, "CAMERA_MSTの書式に則った追従カメラ（キャラ）の設定。完了までの時間は省略可。"
			// 	, new ArgInfo(ArgType.Float, "注視点からの相対z軸距離")
			// 	, new ArgInfo(ArgType.Float, "初期回転角度")
			// 	, new ArgInfo(ArgType.Float, "カメラの仰角")
			// 	, new ArgInfo(ArgType.Float, "Field Of View")
			// 	, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			// )},
			{ "注視点移動（絶対）", new CommandInfo(CommandType.CameraMoveFollowPosition, "注視点の絶対移動（キャラに追従する場合はアクターの移動で代用）。完了までの時間は省略可。"
				, new ArgInfo(ArgType.Vector3, "座標")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "注視点移動（相対）", new CommandInfo(CommandType.CameraMoveFollowPositionRel, "注視点の相対移動（キャラに追従する場合はアクターの移動で代用）。完了までの時間は省略可。"
				, new ArgInfo(ArgType.Vector3, "座標")
				, new ArgInfo(ArgType.Float, "秒", min: 0, isOptional: true)
			)},
			{ "バトル開始待ち", new CommandInfo(CommandType.BattleStartWait, "バトルを開始するまで待つ。"
			)},
		};

		/// <summary>
		/// インデクサー
		/// </summary>
		/// <param name="commandName">コマンド名</param>
		/// <returns>コマンド情報</returns>
		public CommandInfo this[string commandName]
		{
			get
			{
				CommandInfo commandInfo;
				if (_commandInfoDict.TryGetValue(commandName, out commandInfo))
				{
					return commandInfo;
				}

				return null;
			}
		}

		/// <summary>
		/// インデクサー
		/// </summary>
		/// <param name="command">コマンド</param>
		/// <returns>コマンド情報</returns>
		public CommandInfo this[CommandType command]
		{
			get
			{
				foreach (var commandInfo in _commandInfoDict.Values)
				{
					if (commandInfo.CommandType == command)
					{
						return commandInfo;
					}
				}

				return null;
			}
		}

		/// <summary>
		/// コマンドタイプからコマンド名を取得する
		/// <param name="type">コマンドタイプ</param>
		/// </summary>
		public string SearchNameFromType(CommandType type)
		{
			foreach (KeyValuePair<string, CommandInfo> kvp in _commandInfoDict)
			{
				if (kvp.Value.CommandType == type)
				{
					return kvp.Key;
				}
			}

			// 一致するものがない場合は空で返す
			return "";
		}

		/// <summary>データ個数</summary>
		public int Count { get { return _commandInfoDict.Count; } }

		/// <summary>キーコレクション</summary>
		public Dictionary<string, CommandInfo>.KeyCollection Keys { get { return _commandInfoDict.Keys; } }

		/// <summary>バリューコレクション</summary>
		public Dictionary<string, CommandInfo>.ValueCollection Values { get { return _commandInfoDict.Values; } }
	}
}
