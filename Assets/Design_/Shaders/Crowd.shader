Shader "Custom/Crowd"
{
	Properties
	{
		_Person1("Texture", 2D) = "white" {}
		_Person2("Texture", 2D) = "white" {}
		_Person3("Texture", 2D) = "white" {}
		_Person4("Texture", 2D) = "white" {}
		_Person5("Texture", 2D) = "white" {}
		_Person6("Texture", 2D) = "white" {}
		_Person7("Texture", 2D) = "white" {}
		_Person8("Texture", 2D) = "white" {}
		_Person9("Texture", 2D) = "white" {}
		_Person10("Texture", 2D) = "white" {}
		_NumberOfPeople("NumberOfPeople",Int) = 10

	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 100




		Pass
		{
			Cull Back
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

			sampler2D _Person1, _Person2, _Person3, _Person4, _Person5, _Person6, _Person7, _Person8, _Person9, _Person10;
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


			fixed4 Person(int num, float2 uv) {
				switch (num) {
				case 1:return tex2D(_Person1, uv);
				case 2:return tex2D(_Person2, uv);
				case 3:return tex2D(_Person3, uv);
				case 4:return tex2D(_Person4, uv);
				case 5:return tex2D(_Person5, uv);
				case 6:return tex2D(_Person6, uv);
				case 7:return tex2D(_Person7, uv);
				case 8:return tex2D(_Person8, uv);
				case 9:return tex2D(_Person9, uv);
				case 0:return tex2D(_Person10, uv);

				default:return tex2D(_Person1, uv);
				}
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
					clip(-1);
					return fixed4(0, 0, 0, 0); //透過部分
				}

				
				fixed4 col = Person(num%10, uv);
				clip(col.a - 0.01);
				return col;

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
