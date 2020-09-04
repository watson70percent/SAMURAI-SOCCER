Shader "Custom/Crowd"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_NumberOfPeople("NumberOfPeople",Int) = 10

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
				float aspect : TEXCODE1;
			};

			sampler2D _MainTex;
			 
			int _NumberOfPeople;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				float xscale = 1 / sqrt(pow(unity_WorldToObject[0].x, 2) + pow(unity_WorldToObject[0].y, 2) + pow(unity_WorldToObject[0].z, 2));
				float zscale= 1 / sqrt(pow(unity_WorldToObject[2].x, 2) + pow(unity_WorldToObject[2].y, 2) + pow(unity_WorldToObject[2].z, 2));
				o.aspect = zscale / xscale;
				return o;
			}

			fixed4 customCol(float2 _uv,float aspect)
			{

				
				float2 uv = _uv;
				float tempx = uv.x*_NumberOfPeople;
				int num = floor(tempx);
				uv.x = tempx  - num;
				
				uv.y *= aspect*_NumberOfPeople;
				
				uv.y -= abs(sin(num * 1000 + _Time * 100)*0.3);
				if (uv.y > 1||uv.y<0)
				{
					return fixed4(0, 0, 0, 0); //透過部分
				}


				return tex2D(_MainTex, uv);

				//return fixed4(0,0,0,0); //透過部分
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = customCol(i.uv,i.aspect);
				return col;
			}
			ENDCG
		}
	}
}
