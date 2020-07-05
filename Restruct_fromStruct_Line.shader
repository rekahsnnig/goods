Shader "Unlit/Restruct_fromStruct_Line"
{
    Properties
    {
        _MainTex1 ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "white" {}
        _Size("Size",float) = 1
        _Range("P",range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2g
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
                float3 col : TEXCOORD1;
                float id : TEXCOORD2;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : tEXCOORD2;
                float4 vertex : SV_POSITION;
                float3 col : TEXCOORD1;
            };

            struct vertexData
            {
                float3 pos;
                float3 vcolor;
                float2 uv;
                int vclength;
                int uvlength; 
            };

            StructuredBuffer<vertexData> VertexDatas1;
            StructuredBuffer<vertexData> VertexDatas2;

            sampler2D _MainTex1;
            sampler2D _MainTex2;
            float4 _MainTex_ST;
            float _Size;
            float _Range;
            
            float2x2 rot(float a){return float2x2(cos(a),sin(a),-sin(a),cos(a));}

            v2g vert (appdata v ,uint id : SV_VERTEXID)
            {
                vertexData vd1 = VertexDatas1[id];
                vertexData vd2 = VertexDatas2[id];
                v2g o;
                o.vertex = float4(vd1.pos,1.0);
                
                float P = clamp((sin(_Time.y/8. + o.vertex.y/100)),-.5,.5)+.5;
                
                o.vertex.xyz = lerp(vd1.pos,vd2.pos,P);
                o.vertex.xyz *= 1 +float3(.5,.5,.2) * sin(P * UNITY_PI);

                float a = sin(P * UNITY_PI)  * 10;
                o.vertex.xz = mul(rot(a),o.vertex.xz);

                o.uv = lerp(vd1.uv,vd2.uv,P);
                o.col = lerp(vd1.vcolor,vd2.vcolor,P);
                o.id = id;
                return o;
            }

            float4 selfRot(float3 vertex,float3 center)
            {
                float3 vp = UnityObjectToViewPos(center);
                float3 w = mul((float3x3)unity_ObjectToWorld, vertex - center);
                vp += float3(w.xy, -w.z);
                return mul(UNITY_MATRIX_P, float4(vp, 1));
            }


            [maxvertexcount(8)]
            void geom(point v2g input[1], inout LineStream<g2f> OutputStream)
            {
                g2f o;
                v2g ipt = input[0];
                ipt.vertex.xz = mul(rot(_Time.y/3.9),ipt.vertex.xz);

                float3 p = ipt.vertex.xyz;
                
                o.vertex = UnityObjectToClipPos(float4(p,1));
                o.uv = float2(0,0);
                o.uv2 = ipt.uv;
                o.col = ipt.col;
                OutputStream.Append(o);
                [unroll]
                for(int x = -1; x < 3 ; x+=2)
                {
                    [unroll]
                    for(int y = -1 ; y < 3 ; y+=2)
                    {
                        
                        vertexData vd1 = VertexDatas1[ipt.id + x + y];
                        vertexData vd2 = VertexDatas2[ipt.id + x + y];
                        g2f o2;
                        
                        float P = clamp((sin(_Time.y/8. + o.vertex.y/20)),-.5,.5)+.5;
                        o2.vertex.xyz = lerp(vd1.pos,vd2.pos,P);
                        o2.vertex.xyz *= 1 +float3(.5,.5,.2) * sin(P * UNITY_PI);
                        o2.vertex.xz = mul(rot(_Time.y/3.9),o2.vertex.xz);
                        o2.vertex.xyz = (length(p - o2.vertex.xyz) > 3.6)?p:o2.vertex.xyz;
                        o2.vertex = UnityObjectToClipPos(o2.vertex);
                        o2.uv2 = ipt.uv;
                        o2.uv = 0;
                        o2.col = ipt.col;
                        OutputStream.Append(o2);
                        OutputStream.RestartStrip();
                        OutputStream.Append(o);
                    }
                }
            }

            fixed4 frag (g2f i) : SV_Target
            {
                fixed4 col1 = tex2D(_MainTex1,i.uv2);
                fixed4 col2 = tex2D(_MainTex2,i.uv2);
                float P = _Range;
                fixed4 col = lerp(col1,col2,P) + float4(i.col,0);

                float a = max(1.0 - length(i.uv)*((_Size * 1.5) + (.1 * _Size) * sin(_Time.y)),0);
                if(a < 0.1 || i.col.r + i.col.g + i.col.b < 0.01){discard;}
                col.a = clamp(i.uv2.x,0,1);
                return col;
            }
            ENDCG
        }
    }
}
