Shader "Hidden/ColorPostEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _DarkColor;
			uniform float4 _LightColor;

			uniform float4 _RedColor;
			uniform float4 _YellowColor;
			uniform float4 _GreenColor;
			uniform float4 _CyanColor;
			uniform float4 _BlueColor;
			uniform float4 _PinkColor;

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

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				if (col.r > 0.9 && col.g < 0.1 && col.b < 0.1)
					return _RedColor;
				if (col.r > 0.9 && col.g > 0.9 && col.b < 0.1)
					return _YellowColor;
				if (col.r < 0.1 && col.g > 0.9 && col.b < 0.1)
					return _GreenColor;
				if (col.r < 0.1 && col.g > 0.9 && col.b > 0.9)
					return _CyanColor;
				if (col.r < 0.1 && col.g < 0.1 && col.b > 0.9)
					return _BlueColor;
				if (col.r > 0.9 && col.g < 0.1 && col.b > 0.9)
					return _PinkColor;

				float level = (col.r + col.g + col.b) / 3;
				if (level > 0.5f)
					return _LightColor;
				return _DarkColor;
			}

			ENDCG
		}
    }
}
