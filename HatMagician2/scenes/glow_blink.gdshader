shader_type canvas_item;

uniform vec4 glow_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float glow_intensity : hint_range(0.0, 5.0) = 0.0;
uniform float glow_thickness : hint_range(0.0, 20.0) = 4.0;
uniform float blink_speed : hint_range(0.0, 10.0) = 2.0;
uniform float min_alpha : hint_range(0.0, 1.0) = 0.2;
uniform float time : hint_range(0.0, 6.28) = 0.0;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    float alpha = tex_color.a;
    
    // 外发光：只在透明区域和边缘显示
    float glow = 0.0;
    
    // 采样周围像素检测边缘
    float radius = glow_thickness;
    for(float x = -radius; x <= radius; x += 1.0) {
        for(float y = -radius; y <= radius; y += 1.0) {
            vec2 offset = vec2(x, y) * TEXTURE_PIXEL_SIZE;
            float sample_alpha = texture(TEXTURE, UV + offset).a;
            float dist = length(vec2(x, y));
            float weight = 1.0 - smoothstep(0.0, radius, dist);
            
            // 如果当前是透明的，但周围有不透明像素，则发光
            if(alpha < 0.1 && sample_alpha > 0.1) {
                glow += weight;
            }
        }
    }
    
    glow = clamp(glow, 0.0, 1.0);
    
    // 闪烁计算
    float blink = (sin(time * blink_speed) + 1.0) * 0.5; // 0~1
    blink = mix(min_alpha, 1.0, blink);
    
    // 混合颜色
    vec3 final_color = tex_color.rgb;
    float final_alpha = tex_color.a;
    
    // 应用外发光（只在透明区域）
    if(alpha < 0.1 && glow > 0.0) {
        final_color = glow_color.rgb;
        final_alpha = glow * glow_intensity * blink;
    }
    
    COLOR = vec4(final_color, final_alpha);
}