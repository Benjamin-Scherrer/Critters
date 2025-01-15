Shader "Custom/WavesURP"
{
    Properties
    {
        [Header(Material)][Space (15)]
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", color) = (1,1,1,1)
        _Smoothness("Smoothness", Range(0,1)) = 0
        _Metallic("Metallic", Range(0,1)) = 0

        [Space (40)][Header(Waves)][Space (15)]
        _WaveA ("Wave A", Vector) = (1,1,0.25,60)
		_WaveB ("Wave B", Vector) = (1,0.6,0.25,31)
		_WaveC ("Wave C", Vector) = (1,1.3,0.25,18)
        [HideInInspector] _PI ("PI", Float) = 3.14159265359
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"            


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 4);
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _BaseColor;
            float _Smoothness, _Metallic;
			float4 _WaveA, _WaveB, _WaveC;
            float _PI;

		    float3 GerstnerWave (float4 wave, float3 p, inout float3 tangent, inout float3 binormal)
            {
		        float steepness = wave.z;
		        float wavelength = wave.w;
		        float k = 2 * PI / wavelength;
			    float c = sqrt(9.8 / k);
			    float2 d = normalize(wave.xy);
			    float f = k * (dot(d, p.xz) - c * _Time.y);
			    float a = steepness / k;

			    tangent += float3(
				    -d.x * d.x * (steepness * sin(f)),
				    d.x * (steepness * cos(f)),
				    -d.x * d.y * (steepness * sin(f))
			    );

			    binormal += float3(
				    -d.x * d.y * (steepness * sin(f)),
				    d.y * (steepness * cos(f)),
				    -d.y * d.y * (steepness * sin(f))
			    );

			    return float3(
				    d.x * (a * cos(f)),
				    a * sin(f),
				    d.y * (a * cos(f))
			    );
		    }

            v2f vert (appdata v)
            {
			    float3 gridPoint = v.vertex.xyz;
			    float3 tangent = float3(1, 0, 0);
			    float3 binormal = float3(0, 0, 1);
			    float3 p = gridPoint;
			    p += GerstnerWave(_WaveA, gridPoint, tangent, binormal);
			    p += GerstnerWave(_WaveB, gridPoint, tangent, binormal);
			    p += GerstnerWave(_WaveC, gridPoint, tangent, binormal);
			    float3 normal = normalize(cross(binormal, tangent));
			    v.vertex.xyz = p;
			    v.normal.xyz = normal;

                v2f o;
                o.positionWS = TransformObjectToWorld(v.vertex.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normal.xyz);
                o.viewDir = normalize(_WorldSpaceCameraPos - o.positionWS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = TransformWorldToHClip(o.positionWS);

                OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUV );
                OUTPUT_SH(o.normalWS.xyz, o.vertexSH );

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                InputData inputdata = (InputData)0;
                inputdata.positionWS = i.positionWS;
                inputdata.normalWS = normalize(i.normalWS);
                inputdata.viewDirectionWS = i.viewDir;
                inputdata.bakedGI = SAMPLE_GI( i.lightmapUV, i.vertexSH, inputdata.normalWS );

                float3 offsetCol = float3(i.positionWS.y/10, i.positionWS.y/10, i.positionWS.y/10);
                float4 albedo = float4(_BaseColor.xyz + offsetCol, _BaseColor.w);

                SurfaceData surfacedata;
                surfacedata.albedo = albedo;
                surfacedata.specular = 0;
                surfacedata.metallic = _Metallic;
                surfacedata.smoothness = _Smoothness;
                surfacedata.normalTS = 0;
                surfacedata.emission = 0;
                surfacedata.occlusion = 1;
                surfacedata.alpha = 0;
                surfacedata.clearCoatMask = 0;
                surfacedata.clearCoatSmoothness = 0;

                return UniversalFragmentPBR(inputdata, surfacedata);
            }
            ENDHLSL
        }
    }
}
