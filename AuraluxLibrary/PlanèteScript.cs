using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AuraluxLibrary;

public class PlanèteScript : MonoBehaviour
{
	public Planète planète;
	public GameObject soldat;

	float tempÉcoulé;

	public string id;
	private int radius = 50;
	private int hauteur = 40;
	private int largeur = 40;

	public int nb;
	private void Awake()
	{
		Debug.Log(1);
		planète = new Planète(id, radius);
		
		planète.OnNBSoldatsChanged += (s, d) => GénérerSoldats(d.nbNouveauxSoldats);

		Debug.Log(2);
	}
	void Start()
    {
		GetComponent<MeshFilter>().mesh = CreateTorusMesh();
		Debug.Log(3);
	}

    void Update()
    {
		tempÉcoulé += Time.deltaTime;
        if(tempÉcoulé > 2)
		{
			
			//GénérerSoldats(5);
			tempÉcoulé = 0;
		}
    }
	private void GénérerSoldats(int nbSoldatsVoulues)
	{
		var angleGrandeBase = 2 * Mathf.PI / largeur;
		
		for (int i = 0; i < nbSoldatsVoulues; ++i)
		{
			int k = Random.Range(0, largeur);
			var pos = new Vector3(Mathf.Cos(angleGrandeBase * k), 0, Mathf.Sin(angleGrandeBase * k)) * (radius + 15);
			var t = Instantiate(soldat, transform);
			t.transform.position = pos;
		}
	}

	public Mesh CreateTorusMesh() 
	{
		return new Mesh()
		{
			vertices = vertices(),
			triangles = triangles(),
			uv = uvs()
		};
	}

	private Vector3[] vertices()
	{


		float théta = Mathf.PI * 2 / largeur;
		float fi = Mathf.PI / hauteur;

		var vertices = new Vector3[(hauteur + 1) * (largeur)];
		int index = 0;


		for (int i = 0; i < largeur; ++i)
			for (int j = 0; j <= hauteur; ++j)
			{
				float x = Mathf.Cos(i * théta) * Mathf.Sin(j * fi);
				float z = Mathf.Sin(j * fi) * Mathf.Sin(i * théta);
				float y = Mathf.Cos(fi * j);
				vertices[index++] = radius * new Vector3(x, y, z);

			}


		return vertices;
	}
	private int[] triangles()
	{
		int nbTriangle = hauteur * largeur * 6;
		var triangles = new int[nbTriangle];

		int temp = 0;
		int compteur = 0;

		for (int i = 0; i < hauteur; ++i)
		{

			for (int j = 0; j < largeur - 1; ++j)
			{
				temp = j * (1 + hauteur) + 1;

				triangles[compteur++] = temp + i;
				triangles[compteur++] = triangles[compteur - 2] - 1;
				triangles[compteur++] = triangles[compteur - 2] + hauteur + 1;

				triangles[compteur++] = triangles[compteur - 2];
				triangles[compteur++] = triangles[compteur - 2] + 1;
				triangles[compteur++] = triangles[compteur - 6];
			}
		}
		for (int i = 0; i < hauteur; ++i)
		{

			triangles[compteur++] = (largeur - 1) * (hauteur + 1) + 1 + i;
			triangles[compteur++] = triangles[compteur - 2] - 1;
			triangles[compteur++] = i;

			triangles[compteur++] = triangles[compteur - 2];
			triangles[compteur++] = triangles[compteur - 2] + 1;
			triangles[compteur++] = triangles[compteur - 6];
		}
		return triangles;
	}
	private Vector2[] uvs()
	{
		Vector2[] uvs = new Vector2[(hauteur + 1) * (largeur)];
		int compteur = 0;

		for (int i = 0; i < largeur; ++i)
			for (int j = 0; j <= hauteur; ++j)
			{
				uvs[compteur++] = new Vector2(j, i);
			}
		return uvs;
	}
}
