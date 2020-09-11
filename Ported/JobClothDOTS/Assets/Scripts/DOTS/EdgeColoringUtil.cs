using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EdgeColoringUtil
{
	// the main routine for Misra & Gries EdgeColoring Algorithm
	public static List<int> MisraGriesEdgeColoring(int m, int n, List<int2> ge, List<List<int>> gv)
	{
		//init
		var delta = 0;
		for (var i = 0; i < gv.Count; i++)
		{
			if (gv[i].Count > delta)
				delta = gv[i].Count;
		}

		List<int> c = new List<int>();
		List<int> U = new List<int>();
		List<List<int>> free = new List<List<int>>();

		for (var i = 0; i < m; i++)
		{
			c.Add(0); //color[i] == 0 means edge i not be colored yet
			U.Add(i); //U contains all edge
		}

		for (var i = 0; i < n; i++)
		{
			free.Add(new List<int>());
			for (var j = 1; j <= delta + 1; j++)
			{
				free[i].Add(j);
			}
		}

		//Misra & Gries EdgeColoring Algorithm
		int count = 0;
		while (U.Count != 0)
		{
			//Let (u,v) be any edge in U.
			var e = U[0];
			var u = ge[e][0];
			var v = ge[e][1];

			//Let F[1:k] be a maximal fan of u starting at F[1]=v.
			var f = getMaxFan(ge, gv, u, v, free, c);

			if (f.Count == 0)
			{
				Debug.LogError("error");
			}
			//Let fc.c be a color that is free on u and fc.d be a color that is free on F[k].
			var fc = new int2(free[u][0], free[f[f.Count - 1]][0]);
			//Invert the cd_u path
			invertCduPath(gv, ge, fc.x, fc.y, u, c, free);

			//Let w ∈ V(G) be such that w ∈ F, F2=[F[1]...w] is a fan and d is free on w.
			var f2 = getNewFan(f, u, v, fc.y, free);
			var w = f2[f2.Count - 1];

			//Rotate F2 and set c(u,w)=d.
			rotateF(f2, ge, u, free, c);
			var uwindex = getEdgeIndex(ge, u, w);
			c[uwindex] = fc.y;
			free[u].Remove(fc.y);
			free[w].Remove(fc.y);

			//U := U - {(u,v)}
			U.Remove(e);
			count++;
		}

		var colornum = 0;
		for (var i = 0; i < c.Count; i++)
		{
			if (c[i] > colornum)
				colornum = c[i];
		}

		// return {
		// 	color:
		// 	c, num:
		// 	colornum, maxDegree:
		// 	delta
		// }

		return c;
	}

	//Let F[1:k] be a maximal fan of u starting at F[1]=v.
	private static List<int> getMaxFan(List<int2> ge, List<List<int>> gv, int u, int v, List<List<int>> free, List<int> c)
	{
		var result = new List<int>();
		result.Add(v);
		var neighbor = getColoredNeighbor(gv[u], c);

		var endflag = false;
		while (neighbor.Count > 0 && !endflag)
		{
			var flag = false;
			for (var i = 0; i < free[v].Count; i++)
			{
				for (var j = 0; j < neighbor.Count; j++)
				{
					if (c[neighbor[j]] == free[v][i])
					{
						v = (ge[neighbor[j]][0] == u) ? ge[neighbor[j]][1] : ge[neighbor[j]][0];
						result.Add(v);
						neighbor.RemoveAt(j);
						flag = true;
						break;
					}
				}

				;
				if (flag) break;
			}

			// if can not find next fan edge, end this procedure
			if (flag == false)
				endflag = true;
		}

		return result;
	}

	// remove all uncolored edge in ori (neighbor edges of u)
	private static List<int> getColoredNeighbor(List<int> ori, List<int> c)
	{
		var neighbor = new List<int>(ori);
		// remove all uncolored edge in neighbor edges of u
		for (var i = neighbor.Count - 1; i >= 0; i--)
		{
			if (c[neighbor[i]] == 0)
				neighbor.RemoveAt(i);
		}

		return neighbor;
	}

	//Invert the cd_u path
	private static void invertCduPath(List<List<int>> gv, List<int2> ge, int fc_c, int fc_d, int u, List<int> c, List<List<int>> free)
	{
		var color = new int2(fc_c, fc_d);
		// invert left-hand path: dcdcd..dc-u-
		var lastchanged = -1;
		var currentColorIndex = 0;
		var currentu = u;
		while (true)
		{
			int i;
			for (i = 0; i < gv[currentu].Count; i++)
			{
				var e = gv[currentu][i];
				if (e == lastchanged) 
					continue;
				
				if (c[e] == color[currentColorIndex])
				{
					currentColorIndex = 1 - currentColorIndex;
					c[e] = color[currentColorIndex];
					currentu = (ge[e][0] == currentu) ? ge[e][1] : ge[e][0];
					lastchanged = e;
					break;
				}
			}
			
			if (i == gv[currentu].Count) break;
		}
		
		free[currentu].Remove(color[currentColorIndex]);
		var curColor = color[1 - currentColorIndex];
		if (!free[currentu].Contains(curColor))
			free[currentu].Add(curColor);

		// invert left-hand path: -u-dcdcd..cd
		lastchanged = -1;
		currentColorIndex = 1;
		currentu = u;
		
		while (true)
		{
			int i;
			for (i = 0; i < gv[currentu].Count; i++)
			{
				var e = gv[currentu][i];
				if (e == lastchanged) 
					continue;
				
				if (c[e] == color[currentColorIndex])
				{
					currentColorIndex = 1 - currentColorIndex;
					c[e] = color[currentColorIndex];
					currentu = (ge[e][0] == currentu) ? ge[e][1] : ge[e][0];
					lastchanged = e;
					break;
				}
			}
			
			if (i == gv[currentu].Count) break;
		}
		
		free[currentu].Remove(color[currentColorIndex]);
		// free[currentu].Add(color[1 - currentColorIndex]);
		curColor = color[1 - currentColorIndex];
		if (!free[currentu].Contains(curColor))
			free[currentu].Add(curColor);
	}

	//Let w ∈ V(G) be such that w ∈ F, F2=[F[1]...w] is a fan and d is free on w.
	private static List<int> getNewFan(List<int> f, int u, int v, int d, List<List<int>> free)
	{
		var f2 = new List<int>();

		for (var i = 0; i < f.Count; i++)
		{
			var dfound = false;
			for (var j = 0; j < free[f[i]].Count; j++)
			{
				if (free[f[i]][j] == d)
				{
					dfound = true;
					break;
				}
			}

			f2.Add(f[i]);
			if (dfound)
				break;
		}

		return f2;
	}

	//Rotate F
	private static void rotateF(List<int> f, List<int2> ge, int u, List<List<int>> free, List<int> c)
	{
		for (var i = 0; i < f.Count - 1; i++)
		{
			var v = f[i];
			var vnext = f[i + 1];
			free[v].Remove(c[getEdgeIndex(ge, u, vnext)]);
			c[getEdgeIndex(ge, u, v)] = c[getEdgeIndex(ge, u, vnext)];
		}
	}

	// get the index of an edge by its two end points
	private static int getEdgeIndex(List<int2> ge, int u, int w)
	{
		for (var i = 0; i < ge.Count; i++)
		{
			if (ge[i][0] == u && ge[i][1] == w || ge[i][0] == w && ge[i][1] == u)
				return i;
		}
		
		Debug.LogError("should not be here!");
		return -1;
	}
}
