Shader "Hidden/FadePostEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Depth("Texture", 2D) = "White" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float _Fade = 0;

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
			sampler2D _Depth;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 blackCol = fixed4(0, 0, 0, 1);

				fixed4 depth = tex2D(_Depth, i.uv);
				float halfValue = (depth.r + depth.g + depth.b) / 3;

				float maxScale = 0.9f;
				halfValue *= maxScale;
				halfValue += (1 - maxScale) / 2;

				float value = 0;
				if (_Fade < halfValue)
					value = (_Fade / halfValue) * 0.5f;
				else value = (_Fade - halfValue) / (1 - halfValue) * 0.5f + 0.5f;

				return blackCol * value + col * (1 - value);
			}

			ENDCG
			
		}
    }
}
