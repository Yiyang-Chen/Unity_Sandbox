Shader "Unlit/TileAtlasGrid_TextureArray"
{
    Properties
    {
        // 将原先的图集换成 2DArray
        _AtlasArray ("Tile Atlas Texture Array", 2DArray) = "white" {}
        _GridColor ("Grid Color", Color) = (1,1,1,1)

        // 数据贴图，用于指示每个格子类型
        _TypeMap ("Type Map Texture", 2D) = "white" {}

        // 棋盘网格大小
        _GridSize("Grid Size", Float) = 10

        // 贴图数组中总共有多少个切片
        _SliceCount("Slice Count", Float) = 4

        // 网格线
        _LineThickness("Line Thickness", Float) = 0.01
        _LineColor("Line Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        // 和原先一样的透明队列设置
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _GridColor;
            
            // 声明 Texture2DArray
            UNITY_DECLARE_TEX2DARRAY(_AtlasArray);

            // 仍然使用传统的 2D sampler 读取 _TypeMap
            sampler2D _TypeMap;

            // 各种属性
            float _GridSize;
            float _SliceCount;
            float _LineThickness;
            float4 _LineColor;

            // ========================
            // 数据结构 & 顶点着色器
            // ========================
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // 平面UV [0..1]
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 worldUV : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // worldUV 用来做网格坐标计算
                o.worldUV = v.uv;
                return o;
            }

            // ================
            // 片元着色器
            // ================
            fixed4 frag (v2f i) : SV_Target
            {
                // 1) 找到当前格子的索引
                float2 gridUV = i.worldUV * _GridSize;
                float2 tileIndex = floor(gridUV);

                // 2) 采样数据贴图（_TypeMap），得到一个 [0..1] 的类型值
                //    这里加 0.5 让采样点落在格子中心
                float2 lookupUV = (tileIndex + 0.5) / _GridSize;
                fixed4 typeSample = tex2D(_TypeMap, lookupUV);

                // 3) 将 R 通道映射到贴图数组的切片索引 (0 ~ _SliceCount-1)
                float typeVal   = typeSample.r * (_SliceCount - 1);
                float sliceIndex = floor(typeVal + 0.5);

                // 4) 在每个瓦片内，我们使用 frac(gridUV) 当做 [0..1] 的局部UV
                float2 fracInTile = frac(gridUV);

                // 5) 采样贴图数组
                fixed4 terrainColor = UNITY_SAMPLE_TEX2DARRAY(
                    _AtlasArray,
                    float3(fracInTile, sliceIndex)
                );
                terrainColor *= _GridColor;

                // 6) 画网格线
                float2 distToLine = min(fracInTile, 1 - fracInTile);
                float lineFactor = saturate(step(distToLine.x, _LineThickness) +
                                            step(distToLine.y, _LineThickness));

                return lerp(terrainColor, _LineColor, lineFactor);
            }
            ENDCG
        }
    }
}
