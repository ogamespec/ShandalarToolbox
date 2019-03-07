// Microprose MTG .PIC decoder reversing
// (c) ogamespec@gmail.com


uint32_t pic_Width; 			// 0x5360C8
uint32_t pic_Height; 			// 0x5360CC
uint32_t pic_CompSize; 			// 0x5360D0, Compressed size
uint8_t * pic_SomeBuffer; 		// 0x5360D4
uint32 pic_OpCount; 			// 0x5360D8, Operation count
uint8_t pic_RepeatCount; 		// 0x5360DC
uint8_t pic_RepeatByte; 		// 0x5360DD
int onesCounter; 				// 0x5360DE
uint8_t pic_MagicValueByte; 		// 0x5360DF
uint32_t bitMask; 				// 0x5360E0

uint32_t pic_MagicValueWord; 		// 0x5360E8
uint8_t bitPointer; 				// 0x5360EC
uint8_t bcdEncoded; 				// 0x5360ED   (not used in MTG files)
uint32_t savedIndex = 0;			// 0x5360EE
uint8_t savedByte = 0; 				// 0x5360F2

int picHandle; 			// 0x547CE8
int MpsFileHandle; 		// 0x547CD8

uint8_t * DibSectionPtr = unk_547CA8; 			// DIB_SECTION_INFO * 
uint8_t * off_526C9C = unk_5F7460;

uint8_t unk_5F7260[0x200]; 			// Work area (next loaded 0x200 bytes )
uint8_t unk_5F7460[0x70]; 		// Safe stub (for unaligned access)

void * MpsReadCallback; 		// 0x5F74D0
uint8_t * MpsBuffer; 		// 0x5F74D4

uint8_t LUT[0x10000]; 			// 0x5320C8

typedef struct _DIB_SECTION_INFO
{
	// 0x0

	FileMapping;

	// +4

	HDC hdc;

	// +8

	DibSection;

	// +0xc

	// ???

	// +0x10

	BITMAPINFOHEADER * bmiHead;

	// +0x14

	// +0x18

	// Current decoded image pointer

	uint8_t * CurrentPointer;

	// +0x1C

	DWORD dwMaximumSizeLow;

	// +0x20

	uint32_t  Width;

	// +0x24

	uint32_t  Height;

	// +0x28

	uint32_t  Bpp;

	// +0x2C

	uint32_t AlignedWidthAddend; 			// for DIB 4 byte alignment

} DIB_SECTION_INFO;

uint8_t unk_547CA8[0x30]; 		// DIB_SECTION_INFO


// 0x444F5F

unsigned int LoadPicInternal (
	int par04,			// PCX only
	unsigned int par08,			// Not used
	unsigned int par0C,			// Not used
	char * par10, 			// IDA arg_C    [ebp + 0x10]
	unsigned int *par14) 	// IDA arg_10   [ebp + 0x14]
{
	unsigned int v0, v1;
	unsigned int v2 = strchr(par10, '.');
	v2 = _strcmpi (".pcx", v2);

	if (v2)
	{
		v2 = PicOpen (par10, O_BINARY);
		picHandle = v2;

		if (v2 != -1)
		{
			SetupMpsReadCallback (picHandle);

			j_Mps_LoadPic(par14);

			// Align width on 4 (DIB specific)

			unsigned int v3 = (pic_Width & 3) != 0 ? 4 - (pic_Width & 3) : 0;
			gvar_547CE4 = pic_Width + v3; 			// Used only in PCX code section (not sure why this is here..)

			v2 = j_PicCreateDIBSection(pic_Width, pic_Height, 8); 		// 8 = bpp

			if (v2)
			{
				v0 = *(gvar_526C98 + 0x18);
				gvar_547CE0 = 0;

				while (gvar_547CE0 < pic_Height)
				{
					j_Mps_DecodeLine (v0, pic_Width);
					gvar_547CE0++;

					v0 += *(gvar_526C98 + 0x2c) + pic_Width; 		// +AlignedWidthAddend
				}
			}
			else
			{
				*(gvar_526C98 + 8) = 0;
			}
		}
		else
		{
			*(gvar_526C98 + 8) = 0;
		}

		return *(gvar_526C98 + 8);
	}
	else
	{
		// .PCX loading code section ... (don't care)
	}

}


int PicOpen (char * path, int mode)
{
	int result = _open (path, mode);
	gvar_547CDC = -1; 			/// Only here (not used)
	return result;
}



void SetupMpsReadCallback (int par04)
{
	MpsFileHandle = par04;
	
	// Reset buffer
	MpsBuffer = off_526C9C;

	MpsReadCallback = PicRead;
}

int PicRead ()
{
	int result = read (MpsFileHandle, unk_5F7260, 0x200);
	MpsBuffer = unk_5F7260;
	return result;
}

Mps_LoadPic (unsigned int *par14)
{
	if ( MpsBuffer >= off_526C9C )
	{
		MpsReadCallback ();
	}

	uint16_t ax = *(uint16_t)MpsBuffer;
	MpsBuffer += 2;

	// Parse header

	if ( (ax & 0xff) == 'X' )
	{
		// Compressed size

		bcdEncoded = (ax >> 8) & 1;			// BCD encoded?

		if ( MpsBuffer >= off_526C9C )
		{
			MpsReadCallback ();
		}

		ax = *(uint16_t)MpsBuffer;
		MpsBuffer += 2;
		pic_CompSize = ax;

		// Picture Width

		if ( MpsBuffer >= off_526C9C )
		{
			MpsReadCallback ();
		}

		ax = *(uint16_t)MpsBuffer;
		MpsBuffer += 2;
		pic_Width = ax;

		// Picture Height

		if ( MpsBuffer >= off_526C9C )
		{
			MpsReadCallback ();
		}

		ax = *(uint16_t)MpsBuffer;
		MpsBuffer += 2;
		pic_Height = ax;

		Mps_PrepareContext();
	}
	else
	{
		// ... Other pic formats (don't care)
	}

}

Mps_PrepareContext()			// 0x640245
{
	if ( (pic_Width | pic_Height) == 0 )
		return;

	pic_RepeatCount = 0;
	pic_RepeatByte = 0;

	pic_SomeBuffer = &unk_5364F3;

	// Get one more word, which contain LUT settings byte (always 0xb)

	if ( MpsBuffer >= off_526C9C)
	{
		MpsReadCallback ();
	}

	ax = *(uint16_t)MpsBuffer;
	MpsBuffer += 2;

	uint8_t al = (ax & 0xff);

	if (al > 11)
	{
		al = 11;
	}

	pic_MagicValueByte = al;
	pic_MagicValueWord = ax;

	bitPointer = 8;

	Mps_PrepareBuffer ();
}

/// Prepare look-up table (0x800 * 3 bytes):

// First we clear look-up indexes (set to 0xFFFFs)

// FF FF 00
// FF FF 00
// FF FF 00
// FF FF 00
// ...

// Next first 256 LUT entry values are set as following:

// FF FF 00
// FF FF 01
// FF FF 02
// FF FF 03
// ...


// LUT entries count = 1 << pic_MagicValueByte

Mps_PrepareBuffer() 			// 0x6402B5
{
	onesCounter = 9; 			// Number of significant bits in mask
	bitMask = 0x1FF;

	dword_5360E4 = 0x100; 			// WTF this meaning?

	ebx = 0;
	ecx = 0x800;
	while (ecx--)
	{
		*(uint16_t)(LUT + ebx) = 0xffff;
		ebx += 3;
	}

	al = 0;

	ebx = 0;
	ecx = 0x100;
	while (ecx--)
	{
		LUT[ebx + 2] = al;
		al++;
		ebx += 3;
	}
}

//
// Decoder (kind of RLE with crazy look-up table)
//
// Also found some info on compression here: https://github.com/vvendigo/Darklands/blob/master/file_formats/quadko/PicFileFormat.txt
//

j_Mps_DecodeLine (
	uint8_t *picPtr, 			// Pointer where to decode part of image
	int Width) 				// Unaligned width in pixels
{
	NextBytes (picPtr, Width);
}

Mps_NextBytes(uint8_t * outPtr /*edi*/, int Width /*ecx*/) 			// 0x640300
{
	if ( bcdEncoded)
	{
		pic_OpCount = (Width + 1) / 2;
	}
	else
	{
		pic_OpCount = Width;
	}

	swap (esp, pic_SomeBuffer)

	while (pic_OpCount--)
	{
		if ( pic_RepeatCount )
		{
			al = pic_RepeatByte;
			pic_RepeatCount--;
		}
		else
		{
			al = Mps_NextRun();

			if (al == 0x90)
			{
				al = Mps_NextRun();

				if (al != 0)
				{
					pic_RepeatCount = al - 1;

					al = pic_RepeatByte;
					pic_RepeatCount--;
				}
				else
				{
					al = 0x90;
					pic_RepeatByte = al;
				}
			}
			else
			{
				pic_RepeatByte = al;
			}
		}

		// al - result

		if (bcdEncoded)
		{
			// Unpack BCD as uint16

			ah = al;
			al &= 0xF;
			ah >>= 4;

			*(uint16 *)outPtr = ax;
			outPtr += 2;
		}
		else
		{
			*(uint8_t *)outPtr = al;
			outPtr++;
		}
	}

	// Quit

	swap (pic_SomeBuffer, esp)
}

// Most evil

Mps_NextRun()
{
	// esp  - Pointed at SomeBuffer (used as temporary space)

	if ( esp == &unk_5364F3 ) 		// On top of temporary buffer? (buffer is empty?)
	{
		ebx = pic_MagicValueWord >> (16 - bitPointer);
		cl = bitPointer;

		// Loop 1

		while (cl < onesCounter)
		{
			if ( esi >= off_526C9C)
			{
				MpsReadCallback ();
				esi = MpsBuffer;
			}
			eax = 0;
			ax = lodsw;
			pic_MagicValueWord = eax;

			eax <<= cl;
			ebx |= eax;
			cl += 16;
		}

		// After loop 1

		bitPointer = cl - onesCounter;

		eax = ebx & bitMask;
		ecx = eax;

		if (eax >= dword_5360E4)
		{
			ecx = dword_5360E4;
			eax = savedIndex;
			bl = savedByte;
			push ebx; 				// Push in temporary buffer
		}

		// Loop 2

		while (1)
		{
			ebx = 3 * eax;
			ax = *(uint16_t *)(LUT + ebx) + 1;

			if (ax != 0) 		// Not 0xFFFF?
			{
				ax--;
				bl = LUT[ebx + 2];
				push ebx; 			// Push in temporary buffer
			}
			else
			{
				break;
			}
		}

		// After loop 2

		al = LUT[ebx + 2];
		savedByte = al;
		push eax; 				// Push in temporary buffer

		ebx = 3 * dword_5360E4;
		LUT[ebx + 2] = al;
		eax = savedIndex;
		*(uint16_t *)(LUT + ebx) = ax;
		
		dword_5360E4++;
		if (dword_5360E4 > bitMask)
		{
			onesCounter++;
			bitMask = (bitMask << 1) | 1;
		}

		if (onesCounter > pic_MagicValueByte)
		{
			Mps_PrepareBuffer ();
			ecx = 0; 			// Cleared inside PrepareBuffer
		}

		savedIndex = ecx;
	}

	// Quit

	pop eax; 				// Pop from temporary buffer
}
