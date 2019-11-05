
namespace Sc.Scenario
{
	/// <summary>
	/// コマンドタイプ
	/// </summary>
	public enum CommandType
	{
		/// <summary>無効</summary>
		None = 0,

		//読み込み・解放系コマンド	100

		/// <summary>マップ読み込み</summary>
		LoadMap = 100,

		/// <summary>マップ解放</summary>
		UnloadMap = 101,

		/// <summary>キャラクター読み込み</summary>
		LoadChara = 102,

		/// <summary>キャラクター解放</summary>
		UnloadChara = 103,

		/// <summary>キャラクター全解放</summary>
		UnloadCharaAll = 104,

		/// <summary>オブジェクト読み込み</summary>
		LoadObject = 105,

		/// <summary>オブジェクト解放</summary>
		UnloadObject = 106,

		//制御系コマンド	200

		/// <summary>時間待ち</summary>
		TimeWait = 200,

		/// <summary>タッチ待ち</summary>
		TouchWait = 201,

		/// <summary>スクリプト呼び出し</summary>
		CallScript = 202,

		/// <summary>ミッション呼び出し</summary>
		CallMission = 203,

		/// <summary>シーン遷移</summary>
		SceneChange = 204,

		/// <summary>コマンドジャンプ先</summary>
		JumpTarget = 205,

		/// <summary>コマンドジャンプ元</summary>
		JumpTo = 206,

		///// <summary>コマンドジャンプ選択ボタン設定</summary>　　検証用
		//JumpSelectSet = 207,

		///// <summary>コマンドジャンプ選択パネル設定</summary>
		//JumpSelectWait = 208,

		/// <summary>スクリプトエンド</summary>
		EndScenario = 209,

		/// <summary>変数設定(Int型）</summary>
		VarSet = 210,

		/// <summary>四則演算設定(Int型)</summary>
		VarAdd = 211,
		VarSub = 212,
		VarMul = 213,//現状不要 未実装
		VarDiv = 214,//現状不要 未実装

		/// <summary>変数代入(Int型)</summary>
		VarEqual = 215,

		/// <summary>ループ開始</summary>
		LoopStart = 220,

		/// <summary>ループ終了</summary>
		LoopEnd = 221,

		/// <summary>IF判定（変数）</summary>
		IfVar = 222,

		/// <summary>変数設定(Vector3型）</summary>
		VarVector3Set = 223,

		/// <summary>四則演算設定(Vector3型)</summary>
		VarVector3Add = 224,
		VarVector3Sub = 225,
		VarVector3Mul = 226,//現状不要 未実装
		VarVector3Div = 227,//現状不要 未実装

		/// <summary>変数代入(Vector3型)</summary>
		VarVector3Equal = 228,

		//状態変化系コマンド	300

		/// <summary>フェードイン</summary>
		FadeIn = 300,

		/// <summary>フェードアウト</summary>
		FadeOut = 301,

		/// <summary>フラッシュ</summary>
		FlashScreen = 302,

		/// <summary>フェード待ち</summary>
		FadeWait = 303,

		//メッセージ系コマンド	400

		/// <summary>メッセージ表示</summary>
		MsgShow = 400,

		/// <summary>メッセージ消去</summary>
		MsgHide = 401,

		/// <summary>メッセージウィンドウ表示</summary>
		MsgWndShow = 402,

		/// <summary>メッセージウィンドウ消去</summary>
		MsgWndHide = 403,

		/// <summary>メッセージウィンドウ表示</summary>
		SpeakerShow = 404,

		/// <summary>メッセージウィンドウ消去</summary>
		SpeakerHide = 405,

		/// <summary>テロップ表示</summary>
		TelopShow = 406,

		/// <summary>テロップ消去</summary>
		TelopHide = 407,

		/// <summary>全テロップ消去</summary>
		AllTelopHide = 408,

		/// <summary>メッセージウィンドウ待ち</summary>
		MsgWndWait = 409,

		/// <summary>テロップ待ち</summary>
		TelopWait = 410,

		//キャラクター系コマンド	500

		/// <summary>キャラクター表示</summary>
		CharaShow = 500,

		/// <summary>キャラクター消去</summary>
		CharaHide = 501,

		/// <summary>キャラクター待ち</summary>
		CharaWait = 502,

		//カメラ系処理	600

		/// <summary>カメラ振動</summary>
		CameraShake = 600,

		/// <summary>カメラ移動</summary>
		CameraPan = 601,

		/// <summary>カメラ移動待ち</summary>
		CameraPanWait = 602,

		/// <summary>カメラ画角変更</summary>
		ChangeFieldOfView = 603,

		/// <summary>カメラ表示範囲変更</summary>
		ChangeClippingPlanes = 604,

		/// <summary>カメラ振動待ち</summary>
		CameraShakeWait = 605,

		/// <summary>カメラ振動停止</summary>
		CameraShakeStop = 606,

		/// <summary>カメラ座標設定</summary>
		CameraPointSet = 607,

		/// <summary>カメラ角度設定</summary>
		CameraAngleSet = 608,

		//サウンド系処理	700

		/// <summary>効果音プレイ</summary>
		SePlay = 700,

		/// <summary>効果音待ち</summary>
		SeWait,

		/// <summary>効果音ストップ</summary>
		SeStop,

		/// <summary>効果音音量</summary>
		SeVol,

		/// <summary>効果音ループ再生</summary>
		SeLoop,

		/// <summary>効果音ループ停止</summary>
		SeLoopStop,

		/// <summary>BGMプレイ</summary>
		BgmPlay,

		/// <summary>BGM待ち</summary>
		BgmWait,

		/// <summary>BGMストップ</summary>
		BgmStop,

		/// <summary>BGM音量</summary>
		BgmVol,

		/// <summary>VOICEプレイ</summary>
		VoicePlay,

		/// <summary>VOICE待ち</summary>
		VoiceWait,

		/// <summary>VOICEストップ</summary>
		VoiceStop,

		/// <summary>VOICE音量</summary>
		VoiceVol,

		/// <summary>サウンド停止</summary>
		SoundStop,

		//シーン遷移系処理	800
		SceneChangeHome = 800,

		// キャラ＆OBJ

		/// <summary>アクター定義</summary>
		ObjDefine = 900,

		/// <summary>アクター定義（ユニット）</summary>
		ObjDefineUnit = 905,

		/// <summary>アクター移動（絶対）</summary>
		/// TODO: 移動中のアニメをバトル側に影響されない処理を追加する
		ObjMove = 909,

		/// <summary>アクター移動（相対）</summary>
		/// TODO: 移動中のアニメをバトル側に影響されない処理を追加する
		ObjMoveRel = 915,

		/// <summary>アクター移動（ナビ）</summary>
		//ObjMoveNavi = 917,

		/// <summary>アクター移動待ち</summary>
		ObjMoveWait = 919,

		/// <summary>アクター表示</summary>
		ObjOn = 929,

		/// <summary>アクター非表示</summary>
		ObjOff = 939,

		/// <summary>アクター配置</summary>
		/// TODO: バトル側の重力設定を無視して設定できるようにする
		ObjPosition = 949,

		/// <summary>アクター拡縮</summary>
		ObjScale = 959,

		/// <summary>アクター回転</summary>
		ObjRotation = 969,

		/// <summary>アクター向き</summary>
		/// TODO: カメラの位置によって勝手に向きが変わってしまうためバトルに影響されない機能を追加する
		ObjDir = 979,

		/// <summary>アニメ切替</summary>
		ObjAnim = 989,

		/// <summary>アニメ設定</summary>
		//ObjAnimSet = 999,

		/// <summary>アクター座標記録</summary>
		ObjPosMemory = 1009,

		/// <summary>アクター座標読込</summary>
		ObjPosMemoryLoad = 1019,

		/// <summary>カメラ追従注視点</summary>
		CameraTarget = 2000,

		/// <summary>カメラ追従キャラ</summary>
		CameraChr = 2010,

		/// <summary>カメラ移動</summary>
		CameraPos = 2020,

		/// <summary>カメラ移動（注視）</summary>
		CameraPosAndLookAt = 2025,

		/// <summary>カメラ移動待ち</summary>
		CameraPosWait = 2030,

		/// <summary>キャラ追従カメラ</summary>
		CameraSetChr = 2040,

		/// <summary>注視点移動（絶対）</summary>
		CameraMoveFollowPosition = 2050,

		/// <summary>注視点移動（相対）</summary>
		CameraMoveFollowPositionRel = 2060,

		/// <summary>バトル開始待ち</summary>
		BattleStartWait = 9999,
	}
}
