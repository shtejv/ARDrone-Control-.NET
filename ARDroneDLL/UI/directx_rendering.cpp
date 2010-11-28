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
 * @brief Module rendering the drone video stream on a textured Direct3D polygone.
 *
 * @author Stephane Piskorski <stephane.piskorski.ext@parrot.fr>
 * @date   Sept, 8. 2010
 *
 *******************************************************************/



//-----------------------------------------------------------------------------
// Display manager for the Win32 SDK Demo application
// Based on the Microsoft DirectX SDK tutorials
//-----------------------------------------------------------------------------

#include <custom_code.h>
#include "directx_rendering.h"


//-----------------------------------------------------------------------------
// Global variables
//-----------------------------------------------------------------------------
LPDIRECT3D9             g_pD3D = NULL; // Used to create the D3DDevice
LPDIRECT3DDEVICE9       g_pd3dDevice = NULL; // Our rendering device
LPDIRECT3DVERTEXBUFFER9 g_pVB = NULL; // Buffer to hold vertices
LPDIRECT3DTEXTURE9      g_pTexture = NULL; // Our texture

static int videoWidth  = DRONE_VIDEO_MAX_WIDTH;
static int videoHeight = DRONE_VIDEO_MAX_HEIGHT;



//-----------------------------------------------------------------------------
// Name: InitD3D()
// Desc: Initializes Direct3D
//-----------------------------------------------------------------------------
HRESULT InitD3D( HWND hWnd )
{
    // Create the D3D object.
    if( NULL == ( g_pD3D = Direct3DCreate9( D3D_SDK_VERSION ) ) )
        return E_FAIL;

    // Set up the structure used to create the D3DDevice. Since we are now
    // using more complex geometry, we will create a device with a zbuffer.
    D3DPRESENT_PARAMETERS d3dpp;
    ZeroMemory( &d3dpp, sizeof( d3dpp ) );
    d3dpp.Windowed = TRUE;
    d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
    d3dpp.BackBufferFormat = D3DFMT_UNKNOWN;
    d3dpp.EnableAutoDepthStencil = TRUE;
    d3dpp.AutoDepthStencilFormat = D3DFMT_D16;

    // Create the D3DDevice
    if( FAILED( g_pD3D->CreateDevice( D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, hWnd,
                                      D3DCREATE_SOFTWARE_VERTEXPROCESSING,
                                      &d3dpp, &g_pd3dDevice ) ) )
    {
        return E_FAIL;
    }

    // Turn off culling
    g_pd3dDevice->SetRenderState( D3DRS_CULLMODE, D3DCULL_NONE );

    // Turn off D3D lighting
    g_pd3dDevice->SetRenderState( D3DRS_LIGHTING, FALSE );

    // Turn on the zbuffer
    g_pd3dDevice->SetRenderState( D3DRS_ZENABLE, TRUE );

    return S_OK;
}




//-----------------------------------------------------------------------------
// Name: InitGeometry()
// Desc: Create the textures and vertex buffers
//-----------------------------------------------------------------------------
HRESULT InitGeometry()
{
	if( FAILED( D3DXCreateTexture(g_pd3dDevice,
									TEXTURE_WIDTH,TEXTURE_HEIGHT,
									D3DX_DEFAULT,
									D3DUSAGE_DYNAMIC,
									D3DFMT_X8R8G8B8,
									D3DPOOL_DEFAULT,
									&g_pTexture)))
		{
            MessageBox( NULL, L"Could not create texture for video rendering", L"Bad news...", MB_OK );
            return E_FAIL;
        }

    // Create the vertex buffer.
    if( FAILED( g_pd3dDevice->CreateVertexBuffer( 4 * 2 * sizeof( CUSTOMVERTEX ),
                                                  0, D3DFVF_CUSTOMVERTEX,
                                                  D3DPOOL_DEFAULT, &g_pVB, NULL ) ) )
    {
        return E_FAIL;
    }

    // Fill the vertex buffer. We are setting the tu and tv texture
    // coordinates, which range from 0.0 to 1.0
    CUSTOMVERTEX* pVertices;
    if( FAILED( g_pVB->Lock( 0, 0, ( void** )&pVertices, 0 ) ) )
        return E_FAIL;
    
	// Create four points coordinates to form a quad

	pVertices[0].position = D3DXVECTOR3( -1.0f , 1.0 , 0.0f );
	pVertices[1].position = D3DXVECTOR3( 1.0f , 1.0 , 0.0f );
	pVertices[2].position = D3DXVECTOR3( -1.0 , -1.0f , 0.0f );
	pVertices[3].position = D3DXVECTOR3( 1.0f , -1.0f , 0.0f );

	for(int i=0;i<4;i++) { pVertices[i].color=0xffffffff; }

	float scaleFactorW = (float)(videoWidth) /(float)(TEXTURE_WIDTH);
	float scaleFactorH = (float)(videoHeight)/(float)(TEXTURE_HEIGHT);

	pVertices[0].tu = 0.0f;            pVertices[0].tv = 0.0f; 
	pVertices[1].tu = scaleFactorW;    pVertices[1].tv = 0.0f; 
	pVertices[2].tu = 0.0f;            pVertices[2].tv = scaleFactorH; 
	pVertices[3].tu = scaleFactorW;    pVertices[3].tv = scaleFactorH;

    g_pVB->Unlock();

    return S_OK;
}


unsigned char videoFrame[TEXTURE_WIDTH*TEXTURE_HEIGHT*4];


//-----------------------------------------------------------------------------
// Name: D3DChangeTexture()
// Desc: Loads the texture from an external RGB buffer
//-----------------------------------------------------------------------------
extern "C" void D3DChangeTexture(unsigned char* rgbtexture)
{
	int i,j;
	
	unsigned char*rgb_src=rgbtexture;
	unsigned char*xrgb_dest=videoFrame;
	
	for(i=0;i<videoHeight;i++){
		xrgb_dest = videoFrame+i*TEXTURE_WIDTH*4;
		rgb_src   = rgbtexture+i*DRONE_VIDEO_MAX_WIDTH*3;
		for (j=0;j<videoWidth;j++){
			char r = *(rgb_src++);
			char g = *(rgb_src++);
			char b = *(rgb_src++);

			*(xrgb_dest++)=b;
			*(xrgb_dest++)=g;
			*(xrgb_dest++)=r;
			*(xrgb_dest++)=255; /* unused channel */
	}}
}

extern "C" void D3DChangeTextureSize(int w,int h)
{
	/*
	Makes sure the 3D object was built.
	It might not be, since this function is called by the video pipeline
	thread which can start before the Direct3D thread.
	*/
	if (g_pVB==NULL) return;

	if (w!=videoWidth || h!=videoHeight)
	{
	videoWidth  = min(w,DRONE_VIDEO_MAX_WIDTH);
	videoHeight = min(h,DRONE_VIDEO_MAX_HEIGHT);

	/* 
	Change the texture coordinates for the 3D object which renders the video.
	The texture buffer has a fixed and large size, but only part of it is filled 
	by D3DChangeTexture.
	*/
	CUSTOMVERTEX* pVertices;
    if( !FAILED( g_pVB->Lock( 0, 0, ( void** )&pVertices, 0 ) ) )
	{
		float scaleFactorW = (float)(videoWidth) /(float)(TEXTURE_WIDTH);
		float scaleFactorH = (float)(videoHeight)/(float)(TEXTURE_HEIGHT);

		pVertices[0].tu = 0.0f;            pVertices[0].tv = 0.0f; 
		pVertices[1].tu = scaleFactorW;    pVertices[1].tv = 0.0f; 
		pVertices[2].tu = 0.0f;            pVertices[2].tv = scaleFactorH; 
		pVertices[3].tu = scaleFactorW;    pVertices[3].tv = scaleFactorH;

		g_pVB->Unlock();
	}}
}


//-----------------------------------------------------------------------------
// Name: Cleanup()
// Desc: Releases all previously initialized objects
//-----------------------------------------------------------------------------
VOID Cleanup()
{
    if( g_pTexture != NULL )
        g_pTexture->Release();

    if( g_pVB != NULL )
        g_pVB->Release();

    if( g_pd3dDevice != NULL )
        g_pd3dDevice->Release();

    if( g_pD3D != NULL )
        g_pD3D->Release();
}



//-----------------------------------------------------------------------------
// Name: SetupMatrices()
// Desc: Sets up the world, view, and projection transform matrices.
//-----------------------------------------------------------------------------
VOID SetupMatrices()
{
    // Set up world matrix
    D3DXMATRIXA16 matWorld;
    D3DXMatrixIdentity( &matWorld );
    D3DXMatrixRotationX( &matWorld, 0.0f );
    g_pd3dDevice->SetTransform( D3DTS_WORLD, &matWorld );

    // Set up our view matrix. A view matrix can be defined given an eye point,
    // a point to lookat, and a direction for which way is up. Here, we set the
    // eye five units back along the z-axis and up three units, look at the
    // origin, and define "up" to be in the y-direction.
    D3DXVECTOR3 vEyePt( 0.0f, 0.0f,-2.0f );
    D3DXVECTOR3 vLookatPt( 0.0f, 0.0f, 0.0f );
    D3DXVECTOR3 vUpVec( 0.0f, 1.0f, 0.0f );
    D3DXMATRIXA16 matView;
    D3DXMatrixLookAtLH( &matView, &vEyePt, &vLookatPt, &vUpVec );
    g_pd3dDevice->SetTransform( D3DTS_VIEW, &matView );

    // For the projection matrix, we set up a perspective transform (which
    // transforms geometry from 3D view space to 2D viewport space, with
    // a perspective divide making objects smaller in the distance). To build
    // a perpsective transform, we need the field of view (1/4 pi is common),
    // the aspect ratio, and the near and far clipping planes (which define at
    // what distances geometry should be no longer be rendered).
    D3DXMATRIXA16 matProj;
    D3DXMatrixPerspectiveFovLH( &matProj, D3DX_PI / 4, 1.0f, 1.0f, 100.0f );
    g_pd3dDevice->SetTransform( D3DTS_PROJECTION, &matProj );
}




//-----------------------------------------------------------------------------
// Name: Render()
// Desc: Draws the scene
//-----------------------------------------------------------------------------
VOID Render()
{
    // Clear the backbuffer and the zbuffer
    g_pd3dDevice->Clear( 0, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER,
                         D3DCOLOR_XRGB( 0, 0, 255 ), 1.0f, 0 );

	
		D3DLOCKED_RECT locked;
		if(g_pTexture->LockRect(0, &locked, NULL, /*D3DLOCK_DISCARD*/0)==D3D_OK)
		{	
			memcpy(locked.pBits, videoFrame, TEXTURE_WIDTH*TEXTURE_HEIGHT*3);
			g_pTexture->UnlockRect(0);
		}

    // Begin the scene
    if( SUCCEEDED( g_pd3dDevice->BeginScene() ) )
    {
        // Setup the world, view, and projection matrices
        SetupMatrices();

        // Setup our texture. Using textures introduces the texture stage states,
        // which govern how textures get blended together (in the case of multiple
        // textures) and lighting information. In this case, we are modulating
        // (blending) our texture with the diffuse color of the vertices.
        g_pd3dDevice->SetTexture( 0, g_pTexture );
        g_pd3dDevice->SetTextureStageState( 0, D3DTSS_COLOROP, D3DTOP_MODULATE );
        g_pd3dDevice->SetTextureStageState( 0, D3DTSS_COLORARG1, D3DTA_TEXTURE );
        g_pd3dDevice->SetTextureStageState( 0, D3DTSS_COLORARG2, D3DTA_DIFFUSE );
        g_pd3dDevice->SetTextureStageState( 0, D3DTSS_ALPHAOP, D3DTOP_DISABLE );

        // Render the vertex buffer contents
        g_pd3dDevice->SetStreamSource( 0, g_pVB, 0, sizeof( CUSTOMVERTEX ) );
        g_pd3dDevice->SetFVF( D3DFVF_CUSTOMVERTEX );
		// Draws two triangles (makes a quad that will support our drone video picture)
        g_pd3dDevice->DrawPrimitive( D3DPT_TRIANGLESTRIP, 0, 2 );

        // End the scene
        g_pd3dDevice->EndScene();
    }

    // Present the backbuffer contents to the display
    g_pd3dDevice->Present( NULL, NULL, NULL, NULL );
}




//-----------------------------------------------------------------------------
// Name: MsgProc()
// Desc: The window's message handler
//-----------------------------------------------------------------------------
LRESULT WINAPI MsgProc( HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam )
{
    switch( msg )
    {
        case WM_DESTROY:
            Cleanup();
            PostQuitMessage( 0 );
            return 0;
    }

    return DefWindowProc( hWnd, msg, wParam, lParam );
}




//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
DEFINE_THREAD_ROUTINE(directx_renderer_thread, data)
{
    HWND hWnd = NULL;
	
	WNDCLASSEX wc =
	{
		sizeof( WNDCLASSEX ), CS_CLASSDC, MsgProc, 0L, 0L,
		GetModuleHandle( NULL ), NULL, NULL, NULL, NULL,
		L"A.R.Drone Video", NULL
	};
	RegisterClassEx( &wc );

	if (data == NULL)
	{
		// Create the application's window
		hWnd = CreateWindow( L"A.R.Drone Video", L"A.R.Drone Video",
								  WS_OVERLAPPEDWINDOW, 100, 100, 640, 480,
								  NULL, NULL, wc.hInstance, NULL );
	}
	else
	{
		hWnd = (HWND)data;
	}

    // Initialize Direct3D
    if( SUCCEEDED( InitD3D( hWnd ) ) )
    {
        // Create the scene geometry
        if( SUCCEEDED( InitGeometry() ) )
        {
            // Show the window
            ShowWindow( hWnd, SW_SHOWDEFAULT );
            UpdateWindow( hWnd );

            // Enter the message loop
            MSG msg;
            ZeroMemory( &msg, sizeof( msg ) );
            while( msg.message != WM_QUIT )
            {
                if( PeekMessage( &msg, NULL, 0U, 0U, PM_REMOVE ) )
                {
                    TranslateMessage( &msg );
                    DispatchMessage( &msg );
                }
                else
                    Render();
            }
        }
    }
	
	UnregisterClass( L"A.R.Drone Video", wc.hInstance );

	/* Tells ARDRoneTool to shutdown */
	signal_exit();

    return 0;
}



