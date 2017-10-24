#define WIN32_LEAN_AND_MEAN

#include <glew.h>
#include <windows.h>
#include <gl/GL.h>
#include <gl/GLU.h>
#include <GLAUX.H>
#include "Textfile.h"
#include <iostream>
#include <Magick++.h>

// textfile.cpp    
// simple reading and writing for text files
#include "Textfile.h"  
#include "Texture.h"

unsigned char * readDataFromFile(char *fn)
{
	FILE *fp;
	unsigned char *content = NULL;
	int count = 0;
	if (fn != NULL) {
		fp = fopen(fn, "rb");
		if (fp != NULL) {
			fseek(fp, 0, SEEK_END);
			count = ftell(fp);
			rewind(fp);
			if (count > 0) {
				content = (unsigned char *)malloc(sizeof(unsigned char) * (count + 1));
				count = fread(content, sizeof(unsigned char), count, fp);
				content[count] = '\0';
			}
			fclose(fp);
		}
	}
	return content;

}

//读入字符流  
char *textFileRead(const char *fn)
{
	FILE *fp;
	char *content = NULL;
	int count = 0;
	if (fn != NULL)
	{
		fp = fopen(fn, "rt");
		if (fp != NULL)
		{
			fseek(fp, 0, SEEK_END);
			count = ftell(fp);
			rewind(fp);
			if (count > 0)
			{
				content = (char *)malloc(sizeof(char) * (count + 1));
				count = fread(content, sizeof(char), count, fp);
				content[count] = '\0';
			}
			fclose(fp);
		}
	}
	return content;
}

int textFileWrite(char *fn, char *s)
{
	FILE *fp;
	int status = 0;
	if (fn != NULL) {
		fp = fopen(fn, "w");
		if (fp != NULL) {
			if (fwrite(s, sizeof(char), strlen(s), fp) == strlen(s))
				status = 1;
			fclose(fp);
		}
	}
	return(status);
}

using namespace std;

WNDCLASSEX windowClass;
HDC g_HDC;

/*Shader*/
GLuint vShader, fShader;
GLuint vaoHandle;

//顶点位置数组    
float positionData[] = {
	-0.5f,-0.5f,0.0f,1.0f,
	0.5f,-0.5f,0.0f,1.0f,
	0.5f,0.5f,0.0f,1.0f,
	-0.5f,0.5f,0.0f,1.0f,
};

//顶点uv数据
float uvData[] = {
	0.0f,1.0f,
	1.0f,1.0f,
	0.0f,0.0f,
	1.0f,0.0f,
};

//顶点颜色数组    
float colorData[] = {
	1.0f, 0.0f, 0.0f,1.0f,
	0.0f, 1.0f, 0.0f,1.0f,
	0.0f, 0.0f, 1.0f,1.0f,
	1.0f,1.0f,0.0f,1.0f
};

//////////////////////////////////////////////////////////////////////////初始化shader
void InitShader(const char *VShaderFile, const char *FShaderFile)
{


#pragma region 顶点着色器

	vShader = glCreateShader(GL_VERTEX_SHADER);

	if (0 == vShader)
	{
		//错误处理
	}

	const GLchar *vShaderCode = textFileRead(VShaderFile);
	const GLchar *vCodeArray[1] = { vShaderCode };

	//将字符数组绑定到对应的着色器对象上
	glShaderSource(vShader, 1, vCodeArray, NULL);

	//编译着色器对象
	glCompileShader(vShader);

	GLint compileResult;
	glGetShaderiv(vShader, GL_COMPILE_STATUS, &compileResult);

	if (GL_FALSE == &compileResult)
	{
		GLint logLen;

		//得到编译日志长度    
		glGetShaderiv(vShader, GL_INFO_LOG_LENGTH, &logLen);
		if (logLen > 0)
		{
			char *log = (char *)malloc(logLen);
			GLsizei written;
			//得到日志信息并输出    
			glGetShaderInfoLog(vShader, logLen, &written, log);
			//cerr << "vertex shader compile log : " << endl;
			//cerr << log << endl;
			//free(log);//释放空间    
		}
	}
#pragma endregion

#pragma region 片段着色器
	fShader = glCreateShader(GL_FRAGMENT_SHADER);

	if (0 == fShader)
	{
		//错误处理
	}

	const GLchar *fShaderCode = textFileRead(FShaderFile);
	const GLchar *fCodeArray[1] = { fShaderCode };

	glShaderSource(fShader, 1, fCodeArray, NULL);

	//编译着色器对象    
	glCompileShader(fShader);

	//检查编译是否成功    
	glGetShaderiv(fShader, GL_COMPILE_STATUS, &compileResult);
	if (GL_FALSE == compileResult)
	{
		GLint logLen;
		//得到编译日志长度    
		glGetShaderiv(fShader, GL_INFO_LOG_LENGTH, &logLen);
		if (logLen > 0)
		{
			char *log = (char *)malloc(logLen);
			GLsizei written;
			//得到日志信息并输出    
			glGetShaderInfoLog(fShader, logLen, &written, log);
			//cerr << "fragment shader compile log : " << endl;
			//cerr << log << endl;
			//free(log);//释放空间    
		}
	}

#pragma endregion

	//3、链接着色器对象

	//创建着色器程序    
	GLuint programHandle = glCreateProgram();
	if (!programHandle)
	{
		//cerr << "ERROR : create program failed" << endl;
		//exit(1);
	}
	//将着色器程序链接到所创建的程序中    
	glAttachShader(programHandle, vShader);
	glAttachShader(programHandle, fShader);
	//将这些对象链接成一个可执行程序    
	glLinkProgram(programHandle);
	//查询链接的结果    
	GLint linkStatus;
	glGetProgramiv(programHandle, GL_LINK_STATUS, &linkStatus);
	if (GL_FALSE == linkStatus)
	{
		cerr << "ERROR : link shader program failed" << endl;
		GLint logLen;
		glGetProgramiv(programHandle, GL_INFO_LOG_LENGTH,
			&logLen);
		if (logLen > 0)
		{
			char *log = (char *)malloc(logLen);
			GLsizei written;
			glGetProgramInfoLog(programHandle, logLen,
				&written, log);
			cerr << "Program log : " << endl;
			cerr << log << endl;
		}
	}
	else//链接成功，在OpenGL管线中使用渲染程序    
	{
		glUseProgram(programHandle);
	}
}

void initVBO()
{
	//绑定VAO  
	glGenVertexArrays(1, &vaoHandle);
	glBindVertexArray(vaoHandle);

	// Create and populate the buffer objects    
	GLuint vboHandles[3];
	glGenBuffers(3, vboHandles);
	GLuint positionBufferHandle = vboHandles[0];
	GLuint uvBufferHandle = vboHandles[1];
	GLuint colorBufferHandle = vboHandles[2];

	//绑定VBO以供使用    
	glBindBuffer(GL_ARRAY_BUFFER, positionBufferHandle);
	//加载数据到VBO    
	glBufferData(GL_ARRAY_BUFFER,  sizeof(positionData),
		positionData, GL_STATIC_DRAW);

	//
	glBindBuffer(GL_ARRAY_BUFFER, uvBufferHandle);
	glBufferData(GL_ARRAY_BUFFER, 8 * sizeof(float),
		uvData, GL_STATIC_DRAW);

	//绑定VBO以供使用    
	glBindBuffer(GL_ARRAY_BUFFER, colorBufferHandle);
	//加载数据到VBO    
	glBufferData(GL_ARRAY_BUFFER, 16 * sizeof(float),
		colorData, GL_STATIC_DRAW);

	glEnableVertexAttribArray(0);//顶点坐标    
	//调用glVertexAttribPointer之前需要进行绑定操作    
	glBindBuffer(GL_ARRAY_BUFFER, positionBufferHandle);
	glVertexAttribPointer(0, 4, GL_FLOAT, GL_FALSE, 0, (GLubyte *)NULL);


	glEnableVertexAttribArray(1);//顶点uv
	glBindBuffer(GL_ARRAY_BUFFER, uvBufferHandle);
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 0, (GLubyte *)NULL);

	glEnableVertexAttribArray(2);//顶点颜色
	glBindBuffer(GL_ARRAY_BUFFER, colorBufferHandle);
	glVertexAttribPointer(2, 4, GL_FLOAT, GL_FALSE, 0, (GLubyte *)NULL);
}

void SetupPixelFromat(HDC hDC)
{
	int nPixelFormat;

	static PIXELFORMATDESCRIPTOR pfd =
	{
		sizeof(PIXELFORMATDESCRIPTOR),
		1,
		PFD_DRAW_TO_WINDOW |
		PFD_SUPPORT_OPENGL |
		PFD_DOUBLEBUFFER,
		PFD_TYPE_RGBA,
		32,
		0,0,0,0,0,0,
		0,
		0,
		0,
		0,0,0,0,
		16,
		0,
		0,
		PFD_MAIN_PLANE,
		0,
		0,0,0
	};

	nPixelFormat = ChoosePixelFormat(hDC, &pfd);

	SetPixelFormat(hDC, nPixelFormat, &pfd);
}

LRESULT CALLBACK WndPoc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	static HGLRC hRC;
	static HDC hDC;
	int width, height;

	switch (message)
	{
	case  WM_CREATE:
		hDC = GetDC(hwnd);
		g_HDC = hDC;
		SetupPixelFromat(hDC);

		hRC = wglCreateContext(hDC);
		wglMakeCurrent(hDC, hRC);
		return 0;
		break;

	case  WM_CLOSE:
		wglMakeCurrent(hDC, NULL);
		wglDeleteContext(hRC);
		PostQuitMessage(0);
		return 0;
		break;

	case WM_SIZE:
		height = HIWORD(lParam);
		width = LOWORD(lParam);

		if (height == 0)
		{
			height = 1;
		}

		glViewport(0, 0, width, height);
		glMatrixMode(GL_PROJECTION);
		glLoadIdentity();

		gluPerspective(45.0f, (GLfloat)width / (GLfloat)height, 1.0f, 1000.0f);
		glMatrixMode(GL_MODELVIEW);
		glLoadIdentity();

		return 0;
		break;

	//case  WM_PAINT:
	//	Paint(hwnd);
	//	return 0;
	//	break;

	default:
		break;
	}

	return DefWindowProc(hwnd, message, wParam, lParam);
}


int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPervInstance, LPSTR lpCmdLine, int nShowCmd)
{
	HWND hwnd;
	bool done;
	MSG msg;

	windowClass.cbSize = sizeof(WNDCLASSEX);
	windowClass.hInstance = hInstance;
	windowClass.lpfnWndProc = WndPoc;
	windowClass.style = CS_VREDRAW | CS_HREDRAW;
	windowClass.cbClsExtra = 0;
	windowClass.cbWndExtra = 0;

	windowClass.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	windowClass.hIconSm = LoadIcon(NULL, IDI_WINLOGO);
	windowClass.hCursor = LoadCursor(NULL, IDC_ARROW);
	windowClass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
	windowClass.lpszMenuName = NULL;
	windowClass.lpszClassName = "MyClass";

	if (!RegisterClassEx(&windowClass))
	{
		return 0;
	}

	hwnd = CreateWindowEx(NULL,
		"MyClass",
		"A REAL Window Applacation!",
		WS_OVERLAPPEDWINDOW | WS_VISIBLE | WS_SYSMENU,
		100, 100,
		400, 400,
		NULL, NULL,
		hInstance,
		NULL);

	if (!hwnd)
	{
		return 0;
	}

	ShowWindow(hwnd, SW_SHOW);
	UpdateWindow(hwnd);

	done = false;


	//初始化glew扩展库--需要在窗口创建完成之后进行   
	GLenum err = glewInit();

	if (GLEW_OK != err)
	{
		//cout << "Error initializing GLEW: " << glewGetErrorString(err) << endl;
	}

	//绑定并加载VAO，VBO
	initVBO();

	//加载顶点和片段着色器对象并链接到一个程序对象上  
	//InitShader("VertexShader.vert", "FragmentShader.frag");

	InitShader("VertexShader.vert", "FragmentShader.frag");

	Texture tex = Texture(GL_TEXTURE_2D, "logo123.png");
	tex.Load();

	while (!done)
	{
		PeekMessage(&msg, hwnd, NULL, NULL, PM_REMOVE);

		if (msg.message == WM_QUIT)
		{
			done = true;
		}
		else
		{
			Sleep(16);

			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			glLoadIdentity();


			glClear(GL_COLOR_BUFFER_BIT);

			//使用VAO、VBO绘制    
			glBindVertexArray(vaoHandle);
			tex.Bind(GL_TEXTURE0);

			glDrawArrays(GL_TRIANGLE_FAN, 0, 4);

			glBindVertexArray(0);

			SwapBuffers(g_HDC);
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	return msg.wParam;
}
