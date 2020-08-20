Shader "Custom/Wave2D"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_WaveSize("WaveSIze",Range(0.01,10)) = 1
	_WaveWidth("WaveWidth", Range(1, 100)) = 5
    }
		SubShader
	{
		Tags {
		"RenderType" = "Opaque" }
		LOD 200

	   Pass{

		
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float _WaveSize;
			float _WaveWidth;

			struct appdata
			{
				float4 vertex: POSITION;
				float2 uv:TEXCOORD0;
			};

			struct v2g
			{
				float4 pos: SV_POSITION;
				float2 uv :TEXCOORD0;
				
			};

			struct g2f
			{
				float4 pos :SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			v2g vert(appdata v)
			{
				v2g o;
				o.pos = v.vertex;
				o.uv = v.uv;
				return o;
			}

			[maxvertexcount(120)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> outStream)
			{

				float4 fp = IN[2].pos;
				float4 lp = IN[0].pos;
				float4 bp = IN[1].pos;

				g2f o;

				float length = max(0.01,lp.x - fp.x);
				float x =0;
				float dx = abs(length/30);

				for (x ; x < length; x += dx) 
				{
					float y = fp.y+sin( (x+fp.x)*_WaveWidth)*(sin(x*_WaveWidth+ _Time * 80)+1)*_WaveSize  ;

					o.pos = UnityObjectToClipPos(float4(fp.x+x, bp.y, fp.z, fp.w));
					o.uv = float2(IN[2].uv.x*x / length + IN[0].uv.x*(1 - x / length), IN[1].uv.y);
					outStream.Append(o);


					o.pos = UnityObjectToClipPos( float4(fp.x+x, y, fp.z, fp.w));
					o.uv = float2(IN[2].uv.x*x / length + IN[0].uv.x*(1 - x / length), IN[2].uv.y);
					outStream.Append(o);

					

				}


			}


			fixed4 frag(g2f i) : SV_Target
			{
				fixed4 col=(1,1,1,1);
				col=tex2D(_MainTex,i.uv);
				col *= _Color;
				return col;
			}

			ENDCG
}
	}
		FallBack "Diffuse"
}
