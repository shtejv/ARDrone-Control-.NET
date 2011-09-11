#ifndef VIDEO_DECODER_H
#define VIDEO_DECODER_H

#include <VLIB\Stages\vlib_stage_decode.h>
#include <VP_SDK\VP_Stages\vp_stages_yuv2rgb.h>
#include <VP_Api\vp_api_stage.h>
#include "input_data_stage.h"
#include "video_decoder_internal.h"

namespace ARDrone { namespace Video { namespace Decoding {
			public value struct DecodeInfo
			{
			public:
				property int Width;
				property int Height;
				property int BytesPerPixel;
			};
			public ref class DecodingException : System::Exception
			{
			public:
				DecodingException(System::String^ message) : Exception(message)
				{	}
			};

			/// <summary>
			/// Decodes P264 and P263 encoded images.
			/// </summary>
			public ref class VideoDecoder
			{
			public:
				/// <summary>
				/// Creates a VideoDecoder with an internal buffer capable of decoding the specified width and height.
				/// </summary>
				/// <param name="maxWidth">The maximum allowed width of the input picture.</param>
				/// <param name="maxHeight">The maximum allowed height of the input picture.</param>
				VideoDecoder(int maxWidth, int maxHeight);
				~VideoDecoder();

			   /// <summary>
				/// Decodes an input byte array containing either a P264 or P263 encoded picture.
				/// The actual codec is determined automatically.
				/// </summary>
				/// <param name="in">An array with an P264 or P263 encoded picture</param>
				/// <param name="out">The array into which the raw decoded picture should be decoded.</param>
				DecodeInfo Transform(array<System::Byte>^ in, array<System::Byte>^ out);

				/// <summary>
				/// Decodes the input byte array using either P264 or P263, which is resolved automatically.
				/// </summary>
				/// <param name="in">An array with an P264 or P263 encoded picture</param>
				/// <param name="out">A pointer to the array into which the raw decoded picture should be decoded.</param>
				/// <param name="out_size">The size of the output buffer</param>
				DecodeInfo Transform(array<System::Byte>^ in, System::IntPtr out, int out_size);
			private:
				/*
					Internal transform function. Provides a common method for the both public overloads above.
				*/
				DecodeInfo Transform(uint8_t *in, size_t in_length, uint8_t *out, size_t out_size);
				int maxWidth;
				int maxHeight;
				Internal::VideoDecoder *decoder; //I know, I know, I'd like this to be a member aswell but we can't mix native/managed types.
			};
		}
	}
}
#endif