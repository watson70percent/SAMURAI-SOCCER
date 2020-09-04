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
			Cull Off
			//ZTest Always
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

				/*fixed4 col;
				if (uv.x > 0.5) {
					col = tex2D(_MainTex, uv);
				}
				else {
					col = fixed4(1, 1, 1, 0);
				}*/

				if (uv.x < 0.5&&uv.y < 0.5) {
					return tex2D(_MainTex, uv* _MainTex_ST.xy + _MainTex_ST.zw);
				}
				if (uv.x > 0.5&&uv.y > 0.5) {
					return tex2D(_SubTex, uv* _SubTex_ST.xy + _SubTex_ST.zw);
				}

				return fixed4(0,0,0,0); //透過部分
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
