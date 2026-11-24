Shader "Level3/WorldSignText" {
 Properties { _MainTex("Font",2D)="white"{} _Color("Color",Color)=(1,1,1,1) }
 SubShader {
 Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
 Lighting Off Cull Back ZWrite Off ZTest LEqual Blend SrcAlpha OneMinusSrcAlpha
 Pass {
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex; fixed4 _Color;
 struct input {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
 struct output {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
 output vert(input v){output o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
 fixed4 frag(output i):SV_Target {fixed4 c=i.color;c.a*=tex2D(_MainTex,i.uv).a;return c;}
 ENDCG
 }
 }
}
