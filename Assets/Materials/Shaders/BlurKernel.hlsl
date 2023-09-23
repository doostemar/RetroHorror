#ifndef BLURKERNEL_INCLUDED
#define BLURKERNEL_INCLUDED

void BlurKernel_float( float2 UV, UnityTexture2D Tex, float KernelSize, float2 TexelSize, out float4 Color)
{
  Color = float4( 0, 0, 0, 0 );
    
  float total_kernel_weight = KernelSize * KernelSize;
  
  for ( float x = -KernelSize; x < KernelSize; x += 1 )
  {
    float xx = x * x;
    for ( float y = -KernelSize; y < KernelSize; y += 1 )
    {
      float yy = y * y;
      float2 uv_offset = float2( x, y ) * TexelSize;
      
      float dist_to_center = sqrt(xx + yy);
      float dist_norm = (KernelSize - dist_to_center) / KernelSize;
  
      Color += tex2D( Tex, UV + uv_offset ) * ( dist_norm / total_kernel_weight );
    }
  }
  
  Color = clamp( Color, 0, 1 );
}


#endif

//void main()
//{
//    float x, y, xx, yy, rr = r * r, dx, dy, w, w0;
//    w0 = 0.3780 / pow(r, 1.975);
//    vec2 p;
//    vec4 col = vec4(0.0, 0.0, 0.0, 0.0);
//    for (dx = 1.0 / xs, x = -r, p.x = 0.5 + (pos.x * 0.5) + (x * dx); x <= r; x++, p.x += dx)
//    {
//        xx = x * x;
//        for (dy = 1.0 / ys, y = -r, p.y = 0.5 + (pos.y * 0.5) + (y * dy); y <= r; y++, p.y += dy)
//        {
//            yy = y * y;
//            if (xx + yy <= rr)
//            {
//                w = w0 * exp((-xx - yy) / (2.0 * rr));
//                col +=
//                texture2D( txr, p) * w;
//            }
//        }
//    }
//    gl_FragColor = col;
//}