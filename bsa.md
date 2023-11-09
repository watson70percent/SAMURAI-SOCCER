# bsa (Binaly Spectrum Analized)の仕様

- stride level / 1byte（時間間隔・$ 2^n $ サンプル）
- frequency division / 1byte (周波数分割数)
- level resolution / 1byte (音圧レベルの最大値)
- time count / 4byte・LE (時間サンプル数)
- data / 1byte/sample (データ)
