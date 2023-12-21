# 侍蹴珠 ~samurai soccer~ 開発者用ドキュメント

こちらは侍蹴珠の開発者用ドキュメントだよ。よく来たね。
見るのは自由だからゆっくりしていってね。

バグっぽいのがあれば[Issue](https://github.com/watson70percent/SAMURAI-SOCCER/issues)に書いて教えてくれると嬉しいな。
あと、プレイしてもらったらストアにレビューいただけるとうれしいなぁ。

[プライバシーポリシー](https://watson70percent.github.io/SAMURAI-SOCCER/articles/privacypolicy.html)はこっちだよ。

## このドキュメントについて

このドキュメントはDocFXで自動生成しています。

### 構成

`NameSpace`でツリー状に整理されています。
構成は以下の通りになっています。
試合中のイベントは`SamuraiSoccer.Event`中のクラスを通じて、
シーン間のデータやデータの保存は`IDataTransmitClient<T>`を用いて行います。

``` bash
SamuraiSoccer/
            ├ Editor
            ├ Event/
            |     └ Editor
            ├ Player
            ├ SoccerGame/
            |          ├ AI/
            |          |  └ Editor
            |          └ Editor
            ├ StageContents/
            |             ├ BattleDome
            |             ├ China
            |             ├ Conversation
            |             ├ EndCredits
            |             ├ Result/
            |             |      └ Editor
            |             ├ Russian
            |             ├ StageSelect
            |             ├ Tutorial
            |             ├ UK
            |             └ USA
            └ UI
```

