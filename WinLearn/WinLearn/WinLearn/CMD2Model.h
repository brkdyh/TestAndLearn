#include "ComDef.h";

class CMD2Model
{
private:
	int numFrames;
	int numVertices;
	int numTriangles;
	int numST;
	int frameSize;
	int currentFrame;
	int nextFrame;
	float interpol;
	mesh_t *triIndex;
	texCoord_t *st;
	vector3_t *vertexList;
	texture_t *modelTex;

	void SetupSkin(texture_t *thisTexture);

public:
	CMD2Model();
	~CMD2Model();

	//ͬʱ����ģ�ͺ�����
	int Load(char *modelFile, char *skinFile);

	//������ģ��
	int LoadModel(char *modelFile);

	//����������
	int LoadSkin(char *skinFile);

	//
	int SetTexture(texture_t *texture);

	int Animate(int startFrame, int endFrame, float percent);

	int RenderFrame(int ketFrame);

	//�ͷ��ڴ�
	int Unload();
};