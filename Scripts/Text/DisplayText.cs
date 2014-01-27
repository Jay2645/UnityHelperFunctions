using UnityEngine;
namespace Clickable
{
	namespace Text
	{
		[RequireComponent(typeof(TextMesh))]
		[RequireComponent(typeof(TextMeshController))]
		public class DisplayText : MonoHelper
		{

			public GameObject displayParent;

			private TextMesh mesh;
			private string currentText = "";
			private TextMeshController controller;
			public float maxWidth = Mathf.Infinity;
			public float maxHeight = Mathf.Infinity;
			private float lastWidth = 0;
			private float lastHeight = 0;

			// Use this for initialization
			protected void OnEnable()
			{
				InitalizeMesh();
			}

			void InitalizeMesh()
			{
				mesh = gameObject.GetComponent<TextMesh>();
				if (mesh == null)
				{
					mesh = gameObject.AddComponent<TextMesh>();
				}
				controller = gameObject.GetComponent<TextMeshController>();
				if (controller == null)
				{
					controller = gameObject.AddComponent<TextMeshController>();
				}
			}

			public void SetTextString(LocalizedString newString)
			{
				if (mesh == null)
				{
					InitalizeMesh();
				}
				mesh.text = newString.Text;
				currentText = newString.Text;
				SetMaxBounds(maxWidth, maxHeight);
			}

			public string GetText(bool wrapped)
			{
				if (wrapped)
				{
					return mesh.text;
				}
				else
				{
					return currentText;
				}
			}

			void Update()
			{
				if (maxHeight != lastHeight || maxWidth != lastWidth)
				{
					SetMaxBounds(maxWidth, maxHeight);
				}
			}

			public void Display(bool on)
			{
				if (mesh == null)
				{
					InitalizeMesh();
				}
				if (displayParent != null)
				{
					displayParent.renderer.enabled = on;
				}
				renderer.enabled = on;
				if (on && mesh.font == null)
				{
					mesh.font = new Font("Arial");
				}
			}

			public void SetMaxBounds(float width, float height)
			{
				if (width == Mathf.Infinity && height == Mathf.Infinity)
					return;
				maxHeight = height;
				maxWidth = width;
				if (controller == null)
				{
					InitalizeMesh();
				}
				controller.m_maxWidth = width;
				controller.UpdateText();
			}

			public Vector3 GetBottomLeftPoint()
			{
				Bounds bounds = renderer.bounds;
				Vector3 center = bounds.center;
				Vector3 extents = bounds.extents;
				return new Vector3(center.x - extents.x, center.y - extents.y, center.z);
			}

			public Vector3 GetBottomCenterPoint()
			{
				Bounds bounds = renderer.bounds;
				Vector3 center = bounds.center;
				Vector3 extents = bounds.extents;
				return new Vector3(center.x, center.y - extents.y, center.z);
			}

			public Vector3 GetBottomRightPoint()
			{
				Bounds bounds = renderer.bounds;
				Vector3 center = bounds.center;
				Vector3 extents = bounds.extents;
				return new Vector3(center.x + extents.x, center.y - extents.y, center.z);
			}

			public float GetWidth()
			{
				Bounds bounds = renderer.bounds;
				Vector3 center = bounds.center;
				Vector3 extents = bounds.extents;
				return center.x + (extents.x * 2);
			}

			public void SetAnchor(TextAnchor anchor)
			{
				mesh.anchor = anchor;
			}
		}
	}
}