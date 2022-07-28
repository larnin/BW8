Shader "Hidden/DitherPostEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

			uniform float _DarkLevel = 0;
			uniform float _LightLevel = 1;

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

				float level = (col.r + col.g + col.b) / 3;

				float4 lightColor = float4(1, 1, 1, 1);
				float4 darkColor = float4(0, 0, 0, 1);

				if (level <= _DarkLevel)
					return darkColor;
				if (level >= _LightLevel)
					return lightColor;

				level -= _DarkLevel;
				level /= (_LightLevel - _DarkLevel);
				level *= 5;

				int2 pos = int2(floor(i.vertex.x), floor(i.vertex.y));

				if (level < 1)
					level = ditherLevel1(pos);
				else if (level < 2)
					level = ditherLevel2(pos);
				else if (level < 3)
					level = ditherLevel3(pos);
				else if (level < 4)
					level = 1 - ditherLevel2(pos);
				else level = 1 - ditherLevel1(pos);

				return lightColor * level + darkColor * (1 - level);
			}

			ENDCG
			
			CGINCLUDE

			float ditherLevel1(int2 pos)
			{
				return (pos.x + pos.y) % 4 == 0 && (pos.x - pos.y) % 4 == 0;
			}

			float ditherLevel2(int2 pos)
			{
				return (pos.x + pos.y) % 4 == 0 || (pos.x - pos.y) % 4 == 0;
			}

			float ditherLevel3(int2 pos)
			{
				return (pos.x + pos.y) % 2;
			}

			ENDCG
		}
    }
}
