// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "Custom/ProgressBar2" {

	Properties{
		_Color("OriginalColor", Color) = (1,1,1,1)
		_Color2("PowerColor", Color) = (1,1,1,1)
		_MainTex("Main Tex (RGBA)", 2D) = "white" {}
		_Progress("Progress", Range(0.0,1.0)) = 0.0
	}

		SubShader{
			Tags { "Queue" = "Overlay+1" }
			Pass {
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform float4 _Color;
		uniform float4 _Color2;
		uniform float _Progress;

		struct appdata_t {
			float4 vertex : POSITION;
			half4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
			float3 objPos : TEXCOORD01;
			half4 color : COLOR;
		};

		v2f vert(appdata_t v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_UV(0);
			o.objPos = v.vertex.xyz;
			o.color = v.color;
			return o;
		}

		half4 frag(v2f i) : COLOR
		{
			half4 color = tex2D(_MainTex, i.uv);
			float total = (i.objPos.y + 1) / 2.0f;
			if (total <= _Progress) {
				color = _Color2;
				//color = lerp(_Color, _Color2, total / _Progress);
			}
			return color;
			//color.a *= i.uv.x < _Progress;
			//return color * _Color;
		}

		ENDCG

			}
		}

}