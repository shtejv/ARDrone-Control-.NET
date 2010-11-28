/********************************************************************
 *
 *  Parts of the DirectX code are from a tutorial by Microsoft
 *	which can be found in the Microsoft DirectX SDK June 2010.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *
 *  
 *  The rest of the code is COPYRIGHT PARROT 2010
 *
 ********************************************************************
 *       PARROT - A.R.Drone SDK Windows Client Example
 *-----------------------------------------------------------------*/
/**
 * @file directx_rendering.h 
 * @brief Header file for the DirectX rendering of the drone video stream.
 *
 * @author Stephane Piskorski <stephane.piskorski.ext@parrot.fr>
 * @date   Sept, 8. 2010
 *
 *******************************************************************/



//-----------------------------------------------------------------------------
// Display manager for the Win32 SDK Demo application
// Based on the Microsoft DirectX SDK tutorials
//-----------------------------------------------------------------------------


#include <Windows.h>
#include <mmsystem.h>
#include <d3dx9.h>
#pragma warning( disable : 4996 ) // disable deprecated warning 
#include <strsafe.h>
#pragma warning( default : 4996 )

#include <win32_custom.h>

struct CUSTOMVERTEX
{
    D3DXVECTOR3 position; // The position
    D3DCOLOR color;    // The color
    FLOAT tu, tv;   // The texture coordinates
};

// Our custom FVF, which describes our custom vertex structure
#define D3DFVF_CUSTOMVERTEX (D3DFVF_XYZ|D3DFVF_DIFFUSE|D3DFVF_TEX1)



//-----------------------------------------------------------------------------
// Name: InitD3D()
// Desc: Initializes Direct3D
//-----------------------------------------------------------------------------
HRESULT InitD3D( HWND hWnd );
//-----------------------------------------------------------------------------
// Name: InitGeometry()
// Desc: Create the textures and vertex buffers
//-----------------------------------------------------------------------------
HRESULT InitGeometry();

//-----------------------------------------------------------------------------
// Name: D3DChangeTexture()
// Desc: Loads the texture from an external RGB buffer
//-----------------------------------------------------------------------------
#ifdef __cplusplus
extern "C" {
#endif
	void D3DChangeTexture(unsigned char* rgbtexture);
	void D3DChangeTextureSize(int w,int h);
#ifdef __cplusplus
}
#endif

//-----------------------------------------------------------------------------
// Name: Cleanup()
// Desc: Releases all previously initialized objects
//-----------------------------------------------------------------------------
VOID Cleanup();
//-----------------------------------------------------------------------------
// Name: SetupMatrices()
// Desc: Sets up the world, view, and projection transform matrices.
//-----------------------------------------------------------------------------
VOID SetupMatrices();
//-----------------------------------------------------------------------------
// Name: Render()
// Desc: Draws the scene
//-----------------------------------------------------------------------------
VOID Render();
//-----------------------------------------------------------------------------
// Name: MsgProc()
// Desc: The window's message handler
//-----------------------------------------------------------------------------
LRESULT WINAPI MsgProc( HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam );
//-----------------------------------------------------------------------------

#ifdef __cplusplus
extern "C" {
#endif
	#include <VP_Api/vp_api_thread_helper.h>
	PROTO_THREAD_ROUTINE(directx_renderer_thread, data);
#ifdef __cplusplus
}
#endif