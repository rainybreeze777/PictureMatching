Shader "Unlit/Dissolve"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _SliceGuide ("Slice Guide (RGB)", 2D) = "white" {}
        _SliceAmount ("Slice Amount", Range(0.0, 1.0)) = 0.5
		_BurnSize("Burn Size", Range(0.0, 1.0)) = 0.15
		_BurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha 
		LOD 100

		Pass
		{
            Cull Off
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
            sampler2D _SliceGuide;
			float4 _MainTex_ST;
            float _SliceAmount;
			sampler2D _BurnRamp;
			float _BurnSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				half test = tex2D(_SliceGuide, i.uv).rgb - _SliceAmount;
                clip(test);

				fixed4 col;
				if (test < _BurnSize && _SliceAmount > 0 && _SliceAmount < 1) {
					col = tex2D(_BurnRamp, float2(test *(1  /_BurnSize), 0));
				} else {
					// sample the texture
					col = tex2D(_MainTex, i.uv);
				}

				return col;
			}
			ENDCG
		}
	}
}
