Shader "Custom/TexShader" {
Properties {
		//_MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 0.1, 0.1)
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert
      struct Input {
          fixed3 customColor;
          float3 worldPos;
      };
      fixed3 _Color;
      void vert (inout appdata_full v, out Input o) {
          UNITY_INITIALIZE_OUTPUT(Input,o);
          o.customColor.rgb = v.color+(0.1*_CosTime.w);
      }
      sampler2D _MainTex;
      void surf (Input IN, inout SurfaceOutput o) {
          //o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          
          o.Albedo = IN.customColor
          +0.05*half3(
          (frac((IN.worldPos.y*0.1) * 40) - 0.5),
          (frac((IN.worldPos.y*0.1) * 40) - 0.5),
          (frac((IN.worldPos.y*0.1) * 40) - 0.5))
          +0.05*half3((frac((IN.worldPos.x*0.1) * 40) - 0.5),(frac((IN.worldPos.x*0.1) * 40) - 0.5),(frac((IN.worldPos.x*0.1) * 40) - 0.5))
          +0.05*half3((frac((IN.worldPos.z*0.1) * 40) - 0.5),(frac((IN.worldPos.z*0.1) * 40) - 0.5),(frac((IN.worldPos.z*0.1) * 40) - 0.5));
          
      }
      ENDCG
    } 
    //Fallback "Diffuse"
    FallBack Off
} 