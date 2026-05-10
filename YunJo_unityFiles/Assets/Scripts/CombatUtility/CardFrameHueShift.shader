Shader "Custom/CardFrameHueShift"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        [Header(Hue Shift)]
        _Hue ("Hue", Range(0,1)) = 0
        _Saturation ("Saturation", Range(0,2)) = 1
        _Brightness ("Brightness", Range(0,2)) = 1

        [Header(Glow)]
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowStrength ("Glow Strength", Range(0,5)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Hue;
            float _Saturation;
            float _Brightness;

            fixed4 _GlowColor;
            float _GlowStrength;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            // -------------------------
            // RGB -> HSV
            // -------------------------

            float3 RGBToHSV(float3 c)
            {
                float4 K = float4(0., -1./3., 2./3., -1.);

                float4 p =
                    lerp(
                        float4(c.bg, K.wz),
                        float4(c.gb, K.xy),
                        step(c.b, c.g)
                    );

                float4 q =
                    lerp(
                        float4(p.xyw, c.r),
                        float4(c.r, p.yzx),
                        step(p.x, c.r)
                    );

                float d = q.x - min(q.w, q.y);
                float e = 1e-10;

                return float3(
                    abs(q.z + (q.w - q.y) / (6. * d + e)),
                    d / (q.x + e),
                    q.x
                );
            }

            // -------------------------
            // HSV -> RGB
            // -------------------------

            float3 HSVToRGB(float3 hsv)
            {
                float3 rgb =
                    clamp(
                        abs(frac(hsv.x + float3(0, 2./3., 1./3.)) * 6. - 3.) - 1.,
                        0.,
                        1.
                    );

                rgb = rgb * rgb * (3. - 2. * rgb);

                return hsv.z * lerp(float3(1,1,1), rgb, hsv.y);
            }

            // -------------------------
            // FRAGMENT
            // -------------------------

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                float3 hsv = RGBToHSV(tex.rgb);

                // hue shift
                hsv.x += _Hue;
                hsv.x = frac(hsv.x);

                // saturation
                hsv.y *= _Saturation;

                // brightness
                hsv.z *= _Brightness;

                float3 finalRGB = HSVToRGB(hsv);

                // glow
                finalRGB += _GlowColor.rgb * _GlowStrength;

                fixed4 finalCol;

                finalCol.rgb = finalRGB;
                finalCol.a = tex.a;

                return finalCol * i.color;
            }

            ENDCG
        }
    }
}