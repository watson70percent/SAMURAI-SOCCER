Shader "Custom/Magma"
{
	Properties
	{
	   _MainColor("MainColor",Color) = (255,0,0,0)
		_SubColor("SubColor",Color) = (255,255,0,0)
		_Noise("Noise",2D)="white"{}
	_NoiseSize("NoiseSize",Range(0.01,10))=2
	_WaveSize("WaveSize",Range(0.01,0.3)) = 0.1
		_WaveWidth("WaveWidth",Range(1,20)) = 10
		
				_RimLightPower("Lim Right Power", Range(1, 20.0)) = 5.0

	}
		SubShader
	{
		Tags {
		"RenderType" = "Opaque" }
		LOD 200
		//Blend SrcAlpha OneMinusSrcAlpha

		Pass{

		Cull Off

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	sampler2D _Noise;

	fixed4 _MainColor;
	fixed4 _SubColor;
	int _RimLightPower;
	float _WaveSize;
	float _NoiseSize;
	float _WaveWidth;

	struct appdata {
		float4 vertex : POSITION;
		float3 normal :NORMAL;
		float2 uv:TEXCOORD0;

	};

	struct v2f {
			float4 pos : SV_POSITION;
			float4 posWS :TEXCOORD1;
			float3 normal : TEXCOORD2;
			float2 uv:TEXCOORD3;

	};

	v2f vert(appdata v) {

		v2f o;
		v.vertex.xyz += normalize(v.normal)*_WaveSize*sin(_Time *2+(v.vertex.y + v.vertex.x + v.vertex.z)*10/_WaveWidth );

		o.pos = UnityObjectToClipPos(v.vertex);
		o.posWS = mul(unity_ObjectToWorld, v.vertex);
		o.normal = UnityObjectToWorldNormal(v.normal);
		o.uv = v.uv;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{


	fixed4 col = (1, 1, 1, 1);

		float3 L = normalize(-_WorldSpaceLightPos0.xyz);
		float3 N = i.normal;
		float3 V = normalize(i.posWS - _WorldSpaceCameraPos);
			float3 H = normalize(-L - V);

			float attent = abs(dot(V, N));
			//col += _SubColor * (1 - attent);
			col.rgb *= _MainColor.rgb;
				col.rgb *= _SubColor.rgb * (1.04 - attent) + col.rgb*attent;
			//col.r = 0;

			float rim = 1.1 - saturate(dot(N, -V));
			rim *= saturate(1.0 - saturate(dot(N, L)) + dot(V, -L)*0.2);
			fixed3 rimColor = pow(rim, _RimLightPower)* fixed4(1,1,1,1);
			col.rgb += rimColor;



			float specular = dot(H, N);
			col += fixed4(1, 1, 1, 1)*max(0, specular - 0.8) * 5;


			//col *= max(0, sin(i.posWS.x*i.posWS.y* i.posWS.z* _Time/10));

			//col *= tex2D(_Noise, i.uv*2+sin( _Time/4 )*sin( _Time/5+i.uv.x )*sin( i.uv.y+_Time/3 )   );


			//float a = 0.5 - (i.uv.x*0.1+_Time*2 - round(i.uv.x*0.1+_Time*2));
			//float a = 1 - (i.uv.x*0.1 + _Time - round(i.uv.x*0.1 + _Time))*(i.uv.x*0.1 + _Time - round(i.uv.x*0.1 + _Time))*4;
			float a = sin(i.uv.x + _Time * 6)*0.5;
			float b = sin(i.uv.y + _Time * 6)*0.5;
			i.uv.x += 3*sin(i.uv.x*_Time/40) + sin(_Time/10)+a*3;
			i.uv.y += 3 * sin(i.uv.y*_Time / 60) + sin(_Time)+b*4;
			i.uv *= _NoiseSize;
			col *= tex2D(_Noise,i.uv*0.2);

			return col;
			}
				ENDCG
		}



	}


		FallBack "Diffuse"
}
