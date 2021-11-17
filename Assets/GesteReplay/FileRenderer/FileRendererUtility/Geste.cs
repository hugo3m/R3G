using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Geste {
	
	public static Vector3 minPos = new Vector3(180,-20,-25);
	public static Vector3 maxPos = new Vector3(220,20,15);//old
	//public static Vector3 maxPos;//new
	
	public List<FrameRenderer> frames;        //liste des frames lues
	public Vector3 min;
	public Vector3 max;
	public int nbFrames;
	public int nbJointures;

	public Geste(string filename)
	{
		GetFromFile(filename);
		nbFrames = frames.Count;
		nbJointures = frames[0].coordonnes.Count;
	}
	private void GetFromFile(string filename)
	{
		frames = new List<FrameRenderer>();

		min = new Vector3(999, 999, 999);
		max = new Vector3(0, 0, 0);

		string[] lines = System.IO.File.ReadAllLines(filename);
		for(int index = 1; index<lines.Length - 1; ++index)
		{
			string line = lines[index];

			//List<float> positions = new List<float>();
			List<string> posString = Regex.Split(line, @"\s+")
				.Where(s => s != string.Empty)
				.ToList();
			
			List<Vector3> coordonnes = new List<Vector3>();

			for (int i = 0; i < posString.Count-2; i+=3)
			{
				float posx = (float) Convert.ToDouble(posString[i], CultureInfo.InvariantCulture.NumberFormat) * 20;
				float posy = (float) Convert.ToDouble(posString[i+1], CultureInfo.InvariantCulture.NumberFormat) * 20;
				float posz = (float) Convert.ToDouble(posString[i+2], CultureInfo.InvariantCulture.NumberFormat) * 20;
				Vector3 pos = new Vector3(posx, posy, posz);
				if (pos.x < min.x && pos.x!=0)
					min.x = pos.x;
				if (pos.y < min.y && pos.y != 0)
					min.y = pos.y;
				if (pos.z < min.z && pos.z != 0)
					min.z = pos.z;


				if (pos.x > max.x)
					max.x = pos.x;
				if (pos.y > max.y)
					max.y = pos.y;
				if (pos.z > max.z)
					max.z = pos.z;

				coordonnes.Add(pos);
			}

			frames.Add(new FrameRenderer(coordonnes,minPos));
		}

	}
}
