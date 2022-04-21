Shader "Custom/Default_TextureAndTransparent"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SubTex("Texture", 2D) = "white" {}

	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 100


		Pass
		{
			Cull Front   //先に内側だけ処理してる
			//ZTest Always    //まず深度バッファ無視して後ろにあるものも全て描画(これだけだと前後関係がおかしくなる)
							//そのあと深度バッファに応じてもう一度描画(これだけだと透過部分が先に処理された場合その後ろに描画されなくなる)
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST; //o.uv = TRANSFORM_TEX(v.uv, _MainTex); で使うか自分で使う. このシェーダーでは使っていない
			sampler2D _SubTex;
			float4 _SubTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);    //これを使うと勝手にoffsetとかやってくれる
				o.uv = v.uv;
				return o;
			}



			fixed4 customCol(float2 uv)
			{

				if (uv.x < 0.5&&uv.y < 0.5) {
					fixed4 col= tex2D(_MainTex, uv* _MainTex_ST.xy + _MainTex_ST.zw);
					return col;
				}
				if (uv.x > 0.5&&uv.y > 0.5) {
					fixed4 col= tex2D(_SubTex, uv* _SubTex_ST.xy + _SubTex_ST.zw);
					return col;
				}
				

				clip(-1);//これで描写を行わなくさせてる. これがないと透過部分にZバッファが書き込まれてその後ろが描写できなくなる
				//(このシェーダーではPassを分けることによって解決しているのでclipがなくても上手く動くはず)
				return fixed4(0,0,0,0); //これはダミー clipで処理は終了するからここまでくることはない
			}



			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = customCol(i.uv);
				return col;
			}
			ENDCG
		}



		//もし同一オブジェクトで内側も描写したければこのように上で内側(Cull Front)だけ処理を行って下で外側(Cull Back)を後から処理する必要がある
				Pass
		{
			Cull Back
			//ZTest Always    //まず深度バッファ無視して後ろにあるものも全て描画(これだけだと前後関係がおかしくなる)
							//そのあと深度バッファに応じてもう一度描画(これだけだと透過部分が先に処理された場合その後ろに描画されなくなる)
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST; //これをつけとけば勝手にoffsetとかやってくれる
			sampler2D _SubTex;
			float4 _SubTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);    //これを使うと勝手にoffsetとかやってくれる
				o.uv = v.uv;
				return o;
			}

			fixed4 customCol(float2 uv)
			{

				if (uv.x < 0.5&&uv.y < 0.5) {
					fixed4 col = tex2D(_MainTex, uv* _MainTex_ST.xy + _MainTex_ST.zw);
					return col;
				}
				if (uv.x > 0.5&&uv.y > 0.5) {
					fixed4 col = tex2D(_SubTex, uv* _SubTex_ST.xy + _SubTex_ST.zw);
					return col;
				}


				clip(-1);//これで描写を行わなくさせてる. これがないと透過部分にZバッファが書き込まれてその後ろが描写できなくなる
				return fixed4(0,0,0,0); //これはダミー clipで処理は終了するからここまでくることはない
			}



			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = customCol(i.uv);
				return col;
			}
			ENDCG
		}
	}
}
