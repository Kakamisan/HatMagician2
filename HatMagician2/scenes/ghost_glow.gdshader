shader_type canvas_item;

uniform float ghost_spacing : hint_range(0.0, 2.0, 0.1) = 0.6;      // 最大放大范围
uniform float progress : hint_range(0.0, 1.0, 0.001) = 0.0;         // 闪烁进度 0~1

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    
    // 虚影缩放：从 1.0 到 1.0 + ghost_spacing
    float ghost_scale = 1.0 + ghost_spacing * progress;
    vec2 centered_uv = (UV - 0.5) / ghost_scale + 0.5;
    vec4 ghost = texture(TEXTURE, centered_uv);
    
    // 虚影透明度：进度0时透明，进度0.5时最亮，进度1时消失
    float ghost_alpha = ghost.a * (1.0 - abs(progress * 2.0 - 1.0));
    
    // 混合：虚影在上层
    vec3 final_color = mix(tex_color.rgb, ghost.rgb, ghost_alpha);
    float final_alpha = max(tex_color.a, ghost_alpha);
    
    COLOR = vec4(final_color, final_alpha);
}