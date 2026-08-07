shader_type canvas_item;

uniform float start : hint_range(0.0, 1.0) = 0.55; // desde dónde empieza a oscurecer (0=arriba, 1=abajo)
uniform float darkness : hint_range(0.0, 1.0) = 0.82;

void fragment() {
    float t = smoothstep(start, 1.0, UV.y);
    COLOR = vec4(0.043, 0.055, 0.078, t * darkness); // #0B0E14
}