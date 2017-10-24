#include "CMD2Model.h";
#include <stdio.h>;
#include <glew.h>;
#include <GL/GLU.h>;
#include <iostream>;

CMD2Model::CMD2Model()
{
	numFrames = 0;
	numVertices = 0;
	numTriangles = 0;
	numST = 0;
	frameSize = 0;
	currentFrame = 0;
	nextFrame = 1;
	interpol = 0.0;
	triIndex = NULL;
	st = NULL;
	vertexList = NULL;
	modelTex = NULL;
}

CMD2Model::~CMD2Model()
{

}

void CMD2Model::SetupSkin(texture_t *thisTexture)
{
	glGenTextures(1, &thisTexture->texID);
	glBindTexture(GL_TEXTURE_2D, thisTexture->texID);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

	switch (thisTexture->textureType)
	{
	case BMP:
		gluBuild2DMipmaps(GL_TEXTURE_2D, GL_RGB, thisTexture->width, thisTexture->height,
			GL_RGB, GL_UNSIGNED_BYTE, thisTexture->data);
		break;;
	case  PCX:
			gluBuild2DMipmaps(GL_TEXTURE_2D, GL_RGBA, thisTexture->width, thisTexture->height,
				GL_RGBA, GL_UNSIGNED_BYTE, thisTexture->data);
	case TGA:
		break;
	default:
		break;
	}
}

int CMD2Model::Load(char *modelFile, char *skinFile)
{
	FILE *filePtr;
	int fileLen;
	char *buffer;

	modelHeader_t *modelHeader;

	stIndex_t *stPtr;
	frame_t *frame;
	vector3_t *vertexListPtr;
	mesh_t *bufIndexPtr;
	int i, j;

	//打开模型文件
	filePtr = fopen(modelFile, "rb");
	if (filePtr == NULL)
		return 0;

	fseek(filePtr, 0, SEEK_END);
	fileLen = ftell(filePtr);
	fseek(filePtr, 0, SEEK_SET);

	//将文件读入缓存
	buffer = new char[fileLen + 1];
	fread(buffer, sizeof(char), fileLen, filePtr);

	//
	modelHeader = (modelHeader_t *)buffer;
	vertexList = new vector3_t[modelHeader->numXYZ * modelHeader->numFrames];
	numFrames = modelHeader->numFrames;
	frameSize = modelHeader->framesize;

	for (j = 0; j < numFrames; j++)
	{
		frame = (frame_t*)&buffer[modelHeader->offsetFrames + frameSize*j];
		vertexListPtr = (vector3_t*)&vertexList[numVertices*j];
		for (i = 0; i < numVertices; i++)
		{
			vertexListPtr[i].point[0] = frame->scale[0] * frame->fp[i].v[0] + frame->tanslate[0];
			vertexListPtr[i].point[1] = frame->scale[1] * frame->fp[i].v[1] + frame->tanslate[1];
			vertexListPtr[i].point[2] = frame->scale[2] * frame->fp[i].v[2] + frame->tanslate[2];
		}
	}

	modelTex = LoadTexture(skinFile);
}