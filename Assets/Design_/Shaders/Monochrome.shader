Shader "Custom/Monochrome"
{
	Properties
	{
	   _BlockSize("BlockSize",Float) = 10
	   _Noise("Noise",Range(0,0.1)) = 0.001
	}
		SubShader
	{
		Tags {
		"Queue" = "Transparent"
		"RenderType" = "Opaque" }
		LOD 200
		//Blend SrcAlpha OneMinusSrcAlpha
		GrabPass{}

		Pass{

		//Cull Off

		CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"

		 sampler2D _GrabTexture;

	 float _BlockSize;
	 float _Noise;


	 struct appdata {
		 float4 vertex : POSITION;
	 };

	 struct v2f {
		 float4 grabPos :TEXCOOD0;
			 float4 pos : SV_POSITION;
	 };

	 v2f vert(appdata v) {

		 v2f o;
		 o.pos = UnityObjectToClipPos(v.vertex);
		 o.grabPos = ComputeGrabScreenPos(o.pos);
		 return o;
	 }


		 float random(float2 _st) {
		 //return sin(dot(_st,float2(12.9898, 78.233)))*43758.5453123;
			 return sin(sin(dot(_st, float2(12.9898, 78.233)))*43758.5453123);
	 }



	 fixed4 frag(v2f i) : SV_Target
	 {

		//float xblock = ceil(i.grabPos.x/i.grabPos.w*_BlockSize)/ _BlockSize;
		//float yblock = ceil(i.grabPos.y/i.grabPos.w*_BlockSize)/ _BlockSize;
		 float2 uv = float2(i.grabPos.x / i.grabPos.w,i.grabPos.y / i.grabPos.w);
		 uv.x += sin(random(uv + _Time))*_Noise;
		 uv.y += sin(random(uv + _Time))*_Noise;
		fixed4 col = tex2D(_GrabTexture,uv);
		col = float4(1, 1, 1, 1)*(col.r + col.g + col.b) / 3;
		col += float4(1,1,1,1)*sin(random(i.grabPos.xy)*_Time*40)*0.1;
	 //float4 col = tex2D(_GrabTexture, float4(1.5, 2, 0,0));
	 //l += (0.9, 0, 0, 0.2);
	 return col;

	 }
		 ENDCG
      }
	}
}