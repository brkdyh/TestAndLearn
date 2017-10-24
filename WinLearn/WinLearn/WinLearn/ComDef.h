
typedef struct
{
	float point[2];
}vector2_t;

typedef struct
{
	float point[3];
}vector3_t;

vector3_t operator+(vector3_t a,vector3_t b)
{
	vector3_t c;
	c.point[0] = a.point[0] + b.point[0];
	c.point[1] = a.point[1] + b.point[1];
	c.point[2] = a.point[2] + b.point[2];

	return c;
}

vector3_t operator-(vector3_t a, vector3_t b)
{
	vector3_t c;
	c.point[0] = a.point[0] - b.point[0];
	c.point[1] = a.point[1] - b.point[1];
	c.point[2] = a.point[2] - b.point[2];

	return c;
}

vector3_t operator*(float f, vector3_t b)
{
	vector3_t c;
	c.point[0] = f*b.point[0];
	c.point[1] = f*b.point[1];
	c.point[2] = f*b.point[2];

	return c;
}

//////////////////////////////////////////////////////////////////////////��������//////////////////////////////////////////////////////////////////////////
typedef struct 
{
	float s;
	float t;
}texCoord_t;

typedef struct
{
	short s;
	short t;
}stIndex_t;

//////////////////////////////////////////////////////////////////////////����������//////////////////////////////////////////////////////////////////////////
typedef struct
{
	unsigned short meshIndex[3];
	unsigned short stIndex[3];
}mesh_t;
//////////////////////////////////////////////////////////////////////////��������//////////////////////////////////////////////////////////////////////////
enum texTypes_t
{
	PCX,BMP,TGA
};
typedef struct
{
	texTypes_t textureType;

	int width;
	int height;
	long int scaledWidth;
	long int scaledHeight;
	unsigned int texID;
	unsigned char *data;
	unsigned char *palette;
}texture_t;

//////////////////////////////////////////////////////////////////////////֡����//////////////////////////////////////////////////////////////////////////
typedef struct
{
	unsigned char v[3];
	unsigned char normalIndex;
}framePoint_t;

typedef struct
{
	float scale[3];
	float tanslate[3];
	char name[16];
	framePoint_t fp[1];
}frame_t;

//////////////////////////////////////////////////////////////////////////MD2ģ������//////////////////////////////////////////////////////////////////////////
typedef struct
{
	int numFrames;			//֡��
	int numPoints;			//������
	int numTriangles;		//��������
	int sumST;				//������������
	int frameSize;			//ÿһ֡�����ֽ���
	int texWidth, texHeight;//����ߴ�
	int currentFrame;		//��ǰ֡����
	int nextFrame;			//��һ֡����
	float interpol;			//��ֵ������
	mesh_t *triIndex;		//�����������б�
	texCoord_t *st;			//��������
	vector3_t *pointList;	//���������б�
	texture_t *modelTex;	//��������
}md2ModelData_t;

typedef struct
{
	int ident;
	int version;
	int skinwidth;
	int skinheight;
	int framesize;
	int numSkins;
	int numXYZ;
	int numST;
	int numTris;
	int numGLcmds;
	int numFrames;
	int offestSkins;
	int offestST;
	int offsetTris;
	int offsetFrames;
	int offsetGLcmds;
	int offsetEnd;
}modelHeader_t;

