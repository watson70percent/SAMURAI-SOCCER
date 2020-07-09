Shader "Custom/TOGETOGE"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_LocalTime("Animation Time", Float) = 0.0

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
			float _LocalTime;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal :NORMAL;
			};

			struct v2g
			{
				float4 pos: SV_POSITION;
				float4 vertex:TEXCOORD0;
				float3 normal :NORMAL;
			};

			struct g2f
			{
				float4 pos :SV_POSITION;
				float3 normal  :NORMAL;
			};

			v2g vert(appdata v)
			{
				v2g o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.vertex = v.vertex;
				o.normal = v.normal;
				return o;
			}



			[maxvertexcount(9)]
			void geom(triangle v2g input[3], uint pid : SV_PrimitiveID, inout TriangleStream<g2f> outStream)
			{
				float3 wp0 = input[0].vertex.xyz;
				float3 wp1 = input[1].vertex.xyz;
				float3 wp2 = input[2].vertex.xyz;

				float3 normal = normalize(cross(wp0 - wp1, wp0 - wp2));

				float3 wp3 = (wp0 + wp1 + wp2) / 3 + normal*0.3;
				g2f o;
				o.normal = cross(wp0 - wp3, wp1 - wp3);
				o.pos = UnityObjectToClipPos(float4(wp3, input[0].vertex.w));
				outStream.Append(o);
				o.pos = input[0].pos;
				o.normal = input[0].normal;
				outStream.Append(o);
				o.pos = input[1].pos;
				o.normal = input[1].normal;
				outStream.Append(o);
				outStream.RestartStrip();

				o.normal = cross(wp1 - wp3, wp2 - wp3);
				o.pos = UnityObjectToClipPos(float4(wp3, input[0].vertex.w));
				outStream.Append(o);
				o.pos = input[1].pos;
				o.normal = input[1].normal;
				outStream.Append(o);
				o.pos = input[2].pos;
				o.normal = input[2].normal;
				outStream.Append(o);
				outStream.RestartStrip();

				o.normal = cross(wp2 - wp3, wp0 - wp3);
				o.pos = UnityObjectToClipPos(float4(wp3, input[0].vertex.w));
				outStream.Append(o);
				o.pos = input[2].pos;
				o.normal = input[2].normal;
				outStream.Append(o);
				o.pos = input[0].pos;
				o.normal = input[0].normal;
				outStream.Append(o);
				outStream.RestartStrip();

			}

			fixed4 frag(g2f i) : COLOR
			{
				fixed4 col;
				col = _Color;
				col.rgb *= abs(normalize(i.normal))*1.3;
				//col.rgb *= max(0,(normalize(i.normal)))*1.3;
				return col;
			}

			ENDCG
}
	}
		FallBack "Diffuse"
}
