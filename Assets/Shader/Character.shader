Shader "Unlit/CharacterWithOutline"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)  // 描边颜色
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1    // 描边宽度（像素单位）
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.5 // 透明判断阈值
    }
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }
        LOD 100

        // 描边Pass（先绘制）
//        Pass
//        {
//            Name "OUTLINE"
//            Blend SrcAlpha OneMinusSrcAlpha
//            ZWrite Off
//            Cull Off
//
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #include "UnityCG.cginc"
//
//            struct appdata
//            {
//                float4 vertex : POSITION;
//                float2 uv : TEXCOORD0;
//            };
//
//            struct v2f
//            {
//                float2 uv : TEXCOORD0;
//                float4 vertex : SV_POSITION;
//            };
//
//            sampler2D _MainTex;
//            float4 _MainTex_ST;
//            float4 _MainTex_TexelSize; // Unity自动提供的纹素大小
//            float4 _OutlineColor;
//            float _OutlineWidth;
//            float _AlphaThreshold;
//
//            v2f vert (appdata v)
//            {
//                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                return o;
//            }
//
//            fixed4 frag (v2f i) : SV_Target
//            {
//                // 采样当前像素的Alpha值
//                fixed4 mainCol = tex2D(_MainTex, i.uv);
//                float alpha = mainCol.a;
//
//                // 如果当前像素不透明，直接丢弃
//                if (alpha > _AlphaThreshold) discard;
//
//                // 计算实际像素偏移量
//                float2 pixelSize = _MainTex_TexelSize.xy * _OutlineWidth;
//
//                // 检测8个方向的邻居像素
//                float2 offsets[8] = {
//                    float2(1, 0),   // 左右
//                    float2(0, 1),   // 上下
//                    float2(-1, 0),
//                    float2(0, -1),
//                    float2(1, 1),  // 对角线
//                    float2(1, -1),
//                    float2(-1, -1),
//                    float2(-1, 1)
//                };
//
//                // 遍历所有偏移方向
//                for(int j = 0; j < 8; j++)
//                {
//                    float2 newUV = i.uv + offsets[j] * pixelSize;
//                    fixed4 neighborCol = tex2D(_MainTex, newUV);
//                    if (neighborCol.a > _AlphaThreshold)
//                    {
//                        return _OutlineColor; // 发现非透明邻居，绘制描边
//                    }
//                }
//
//                discard; // 无有效邻居，丢弃该像素
//                return fixed4(0,0,0,0);
//            }
//            ENDCG
//        }

        // 主纹理Pass（后绘制）
        Pass
        {
//            Tags { "LightMode" = "ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col; // 直接输出主纹理颜色
            }
            ENDCG
        }
    }
}