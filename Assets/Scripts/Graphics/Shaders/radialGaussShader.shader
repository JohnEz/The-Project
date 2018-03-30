// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/RadialGaussShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
	{

		CGINCLUDE

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

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		sampler2D _MainTex;
		float _radius;
		float _texelWidth;
		float _texelHeight;
		static const float gaussianKernel[3] = { 0.38774, 0.24477, 0.06136 };

		float4 Blur(float2 uv, float2 texelOffset, sampler2D tex, float t)
		{
			float4 colour = float4(0.0f, 0.0f, 0.0f, 0.0f);
			float4 centerTap = float4(0.0f, 0.0f, 0.0f, 0.0f);
			
			colour += tex2D(tex, uv + (-2 * texelOffset)) * gaussianKernel[2];
			colour += tex2D(tex, uv + (-1 * texelOffset)) * gaussianKernel[1];
			centerTap = tex2D(tex, uv);
			colour += centerTap * gaussianKernel[0];
			colour += tex2D(tex, uv + (1 * texelOffset)) * gaussianKernel[1];
			colour += tex2D(tex, uv + (2 * texelOffset)) * gaussianKernel[2];

			//return t;
			return lerp(centerTap, colour, t);
		}

		fixed4 fragVert(v2f i) : SV_Target
		{
			float2 ndcSpace = i.uv * 2 - 1;
			float t = saturate(abs(length(ndcSpace)) * _radius);
			float2 texelOffset = float2(0.0f, 1.0f) * float2(_texelWidth, _texelHeight);

			//return t;
			return Blur(i.uv, texelOffset, _MainTex, t);
		}

		fixed4 fragHoriz(v2f i) : SV_Target
		{
			float2 ndcSpace = i.uv * 2 - 1;
			float t = saturate(abs(length(ndcSpace)) * _radius);
			float2 texelOffset = float2(1.0f, 0.0f) * float2(_texelWidth, _texelHeight);

			//return t;
			return Blur(i.uv, texelOffset, _MainTex, t);
		}

		ENDCG

		Pass //0
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragVert
			ENDCG
		}

		Pass //1
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragHoriz
			ENDCG
		}
	}
}
