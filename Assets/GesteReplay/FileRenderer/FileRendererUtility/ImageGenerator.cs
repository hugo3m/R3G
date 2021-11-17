using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageGenerator : MonoBehaviour
{

	public String source;
	public String dest;

	public int width;
	public int height;

	public void Generate()
	{
		saveImage(source, dest, width, height);
	}
	
	public static Texture2D GetTexture2D(string filename,int width, int height)
	{
		Geste geste = new Geste(filename);
		Texture2D t2d = new Texture2D(width, height);
		
		FrameRenderer previousFrame = geste.frames[0];
		
		for (int currentFrameIndex = 1; currentFrameIndex < geste.nbFrames; ++currentFrameIndex)
		{
			Color couleur = getColor(currentFrameIndex, geste.nbFrames);
			
			for (int currentJoinIndex = 0; currentJoinIndex < geste.nbJointures; ++currentJoinIndex)
			{
				Vector3 previousPos = previousFrame.coordonnes[currentJoinIndex]-Geste.minPos;
				Vector3 currentPos  = geste.frames[currentFrameIndex].coordonnes[currentJoinIndex]-Geste.minPos;

				bool previousJoinToRender = previousFrame.joinRendered[currentJoinIndex];
				bool currentJoinToRender = geste.frames[currentFrameIndex].joinRendered[currentJoinIndex];
				
				if(previousJoinToRender && currentJoinToRender)
					drawLine(t2d,getTextureCoord(previousPos,width,height),getTextureCoord(currentPos,width,height),couleur);
			}

			previousFrame = geste.frames[currentFrameIndex];
		}
		return t2d;
	}
	
	public static void saveImage(string filename,string dest, int width, int height)
	{
		byte[] bytes = GetTexture2D( filename, width,  height).EncodeToPNG();
		if (!Directory.Exists(Application.dataPath + "/../gestePicture"))
			Directory.CreateDirectory(Application.dataPath + "/../gestePicture");
		File.WriteAllBytes(Application.dataPath + "/../gestePicture/"+dest+".png", bytes);
	}

	private static Color getColor(int numeroFrame, int nbFrames)
	{
		float completness = ((float) numeroFrame) / ((float) nbFrames);
		Color couleur = Color.Lerp(Color.blue, Color.red, completness);

		return couleur;
	}

	private static Vector2 getTextureCoord(Vector2 gesteCoord, int width, int height)
	{
		Vector3 maxAmplitude = Geste.maxPos - Geste.minPos;
		return new Vector2((gesteCoord.x)/(maxAmplitude.x) * width, (gesteCoord.y)/maxAmplitude.z * height);
	}

	private static void drawLine(Texture2D texture, Vector2 debut, Vector2 fin, Color color)
	{
		float xPix = debut.x;
		float yPix = debut.y;
 
		float width = fin.x - debut.x;
		float height = fin.y - debut.y;
		float length = Mathf.Abs(width);
		if (Mathf.Abs(height) > length) length = Mathf.Abs(height);
		int intLength = (int)length;
		float dx = width / (float)length;
		float dy = height / (float)length;
		for (int i = 0; i <= intLength; i++)
		{
			texture.SetPixel((int)xPix, (int)yPix, color);
			xPix += dx;
			yPix += dy;
		}
	}
}
