using UnityEngine;

public class Bounds2D
{
	public Bounds2D(Collider2D source)
	{
		MakeBounds(source);
	}

	public Bounds2D(Renderer source)
	{
		MakeBounds(source);
	}
	
	private Transform tfm;
	private Renderer render;
	private Vector2 size;
	private Vector2 extents;
	private Vector2 offset;

	private void MakeBounds(Collider2D source)
	{
		if (source is BoxCollider2D)
		{
			MakeBounds((BoxCollider2D)source);
		}
		else if (source is PolygonCollider2D)
		{
			MakeBounds((PolygonCollider2D)source);
		}
		else if (source is CircleCollider2D)
		{
			MakeBounds((CircleCollider2D)source);
		}
		else if(source.renderer != null)
		{
			MakeBounds(source.renderer);
		}
	}

	private void MakeBounds(BoxCollider2D source)
	{
		MakeBounds(source.gameObject, source.size,source.center);
	}

	private void MakeBounds(PolygonCollider2D source)
	{
		if (source.renderer == null)
		{
			Vector2[] points = source.points;
			float maxX = float.NegativeInfinity;
			float maxY = float.NegativeInfinity;
			foreach (Vector2 point in points)
			{
				if (point.x > maxX)
				{
					maxX = point.x;
				}
				if (point.y > maxY)
				{
					maxY = point.y;
				}
			}
			Vector2 maxSize = new Vector2(maxX, maxY);
			Vector2 pos = source.transform.position;
			Vector2 polySize = maxSize - pos;
			MakeBounds(source.gameObject, polySize, Vector2.zero);
		}
		else
		{
			MakeBounds(source.renderer);
		}
	}

	private void MakeBounds(CircleCollider2D source)
	{
		Vector2 dia = new Vector2(source.radius * 2.0f, source.radius * 2.0f);
		MakeBounds(source.gameObject, dia,source.center);
	}

	private void MakeBounds(Renderer source)
	{
		Bounds bounds = source.bounds;
		MakeBounds(source.gameObject,bounds.size, Vector2.zero);
	}

	private void MakeBounds(GameObject gameObject, Vector2 s, Vector2 o)
	{
		size = s;
		extents = new Vector2(size.x / 2.0f, size.y / 2.0f);
		offset = o;
		render = gameObject.renderer;
		tfm = gameObject.transform;
	}

	public Vector2 GetCenter()
	{
		Vector2 center = tfm.position;
		if (render != null)
		{
			center = render.bounds.center;
		}
		center += offset;
		return center;
	}

	public Vector2 GetMax()
	{
		return GetCenter() - extents;
	}

	public Vector2 GetMin()
	{
		return GetCenter() + extents;
	}
}
