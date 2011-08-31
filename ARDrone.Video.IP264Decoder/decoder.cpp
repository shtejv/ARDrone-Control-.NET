/*
	Written by Max Malmgren.
*/

#include "decoder.h"

namespace ARDrone { namespace Video { namespace Decoding {

			VideoDecoder::VideoDecoder(int maxWidth, int maxHeight) : maxWidth(maxWidth), maxHeight(maxHeight), decoder(new Internal::VideoDecoder(maxWidth, maxHeight))
			{	}

			VideoDecoder::~VideoDecoder()
			{
				delete decoder;
			}

			DecodeInfo VideoDecoder::Transform(array<System::Byte>^ in, array<System::Byte>^ out)
			{
				pin_ptr<uint8_t> in_pinned = &in[0];
				pin_ptr<uint8_t> out_pinned = &out[0];
				return Transform(in_pinned, in.Length, out_pinned, out.Length);
			}

			/*
				Crops the raw image rather efficiently.
			*/
			void Crop(uint8_t *in, int bytes_per_pixel, int in_width, int in_height, int out_width, int out_height, uint8_t* out)
			{
				int in_stride = in_width * bytes_per_pixel;
				int out_stride = out_width * bytes_per_pixel;

				//Below we copy the wanted width of each line to the output.
				for(int i = 0; i < out_height; i++, out += out_stride, in += in_stride)
					memcpy(out, in, out_stride);
			}

			DecodeInfo VideoDecoder::Transform(uint8_t *in, size_t in_length, uint8_t *out, size_t out_size)
			{
				uint8_t *int_out;
				int width;
				int height;
				int bytes_per_pixel;

				size_t num_bytes = decoder->Transform(in, in_length, &int_out, &bytes_per_pixel, &width, &height);

				DecodeInfo info;
				info.Width = width;
				info.Height = height;
				info.BytesPerPixel = bytes_per_pixel;
				
				Crop(int_out, info.BytesPerPixel,		//Simpler for client code if we crop here - probably more efficient also.
					maxWidth,
					maxHeight,
					info.Width,
					info.Height,
					out);

				return info;
			}

			DecodeInfo VideoDecoder::Transform(array<System::Byte>^ in, System::IntPtr out, int out_size)
			{
				pin_ptr<uint8_t> in_pinned = &in[0];
				return Transform(in_pinned, in.Length, (uint8_t*)out.ToPointer(), out_size);
			}				
		}
	}
}