Shader "Custom/Toon"
{
	Properties
	{

		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BaseColor("Color", Color) = (1,1,1,1)
			[Header(OutLine)]
		_OutlineWidth("OutLine Width",Range(0.0,1.0)) = 0.02
			_OutlineColor("OutLine Color",Color) = (0,0,0,1)
		[Header(Toon Rendering)]
		[Toggle(USE_TOONRENDERING)] _UseToonRendering("Use Toon Rendering",Int) = 0
		_1stShadeColor("1st Shade Color", Color) = (1,1,1,1)
			_Step1stShade("Step 1st Shade",Range(0.0,1.0)) = 0.7
			_2ndShadeColor("2nd Shade Color", Color) = (1,1,1,1)
			_Step2ndShade("Step 2nd Shade",Range(0.0,1.0)) = 0.3
			 _OutlineSmoothThreshold("Outline Smooth Threshold", Range(-1.0, 1.0)) = 0.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

		  Pass{

			CGPROGRAM

	#pragma vertex vert
	#pragma fragment frag
#pragma shader_feature USE_TOONRENDERING
	#include "UnityCG.cginc"


			sampler2D _MainTex;
		float4 _MainTex_ST;
			fixed4 _BaseColor;
		fixed4 _1stShadeColor;
		fixed4 _2ndShadeColor;
		float _Step1stShade;
		float _Step2ndShade;


		struct appdata {

		float4 vertex :POSITION;
		float3 normal :NORMAL;
		float2 uv:TEXCOORD0;
};

	struct v2f {
	float4 vertex: SV_POSITION;
	float2 uv :TEXCOORD0;
	float3 normal:TEXCOORD1;
	float4 posWS:TEXCOORD2;
		};

	v2f vert(appdata v) {
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.normal = UnityObjectToWorldNormal(v.normal);
		o.posWS = mul(unity_ObjectToWorld, v.vertex);
		o.uv = v.uv;
		return o;
	}

	fixed4 frag(v2f i) :SV_Target
	{
		float3 N = normalize(i.normal);
		float3 L = normalize(-_WorldSpaceLightPos0);
		float3 V = normalize(i.posWS - _WorldSpaceCameraPos);


		fixed4 col = _BaseColor;

#ifdef USE_TOONRENDERING

		float d = dot(N,-L)*0.5 + 0.5;
		fixed4 firstShadeColor = _1stShadeColor;
		fixed4 secondShadeColor = _2ndShadeColor;
		col = lerp(col, firstShadeColor, step(d,_Step1stShade));
		col = lerp(col, secondShadeColor, step(d, _Step2ndShade));

#else

#endif
		float2 mainUV = i.uv * _MainTex_ST.xy + _MainTex_ST.zw;
		col = lerp(col, tex2D(_MainTex, mainUV), 0.5);
		return col;
	}

		ENDCG
}

Pass{
		Cull Front
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"
		float _OutlineWidth;
		fixed4 _OutlineColor;
		float _OutlineSmoothThreshold;

		struct appdata {
			float4 vertex:POSITION;
			float3 normal:NORMAL;
		};

		struct v2f {
			float4 vertex :SV_POSITION;
		};

		v2f vert(appdata v) {
			v2f o;
			float3 dir = normalize(v.vertex.xyz);

			float dis = length(mul(unity_ObjectToWorld,v.vertex) - _WorldSpaceCameraPos);
			v.vertex.xyz += lerp(normalize( v.normal), dir, step(dot(v.normal, dir), _OutlineSmoothThreshold))*_OutlineWidth*dis*0.1;
			//v.vertex.xyz +=normalize( v.normal) * _OutlineWidth*dis*0.1;
			o.vertex = UnityObjectToClipPos(v.vertex);
			
			return o;
		}
		fixed4 frag(v2f i) : SV_Target
		{
			return _OutlineColor;
		}

		ENDCG
}
		}
			FallBack "Diffuse"
}
