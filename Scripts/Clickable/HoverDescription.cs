using UnityEngine;

namespace Clickable
{
	public class HoverDescription : ClickableObject
	{
		protected string formattedDesc = "";
		public TextMesh mesh;
		protected SpriteRenderer background;
		public Vector2 backgroundScale = Vector2.one;
		public Vector3 scale;
		public GameObject meshGO;

		protected override void Start()
		{
			if (hoverDescription != null)
			{
				return;
			}
			hoverDescription = this;
			base.Start();
			if (description != "" || positives != "" || negatives != "")
			{
				ChangeDescription(description, positives, negatives);
			}
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (meshGO == null)
			{
				return;
			}
			Vector3 localScale = meshGO.transform.localScale;
			if (transform.root.localScale.x < 0 && localScale.x > 0 || transform.root.localScale.x > 0 && localScale.x < 0)
			{
				localScale.x = -localScale.x;
			}
			meshGO.transform.localScale = localScale;
		}

		protected override void MouseEnter()
		{
			if ((description != "" || positives != "" || negatives != "") && meshGO != null)
			{
				meshGO.transform.localScale = scale;
			}
		}

		protected override void MouseExit()
		{
			if (meshGO != null)
			{
				meshGO.transform.localScale = new Vector3(scale.x, 0.00001f, scale.z);
			}
		}

		protected void MakeTextObject()
		{
			meshGO = Load("Prefabs/Desc Text", transform, new Vector3(0.0f, 0.0f, -4.0f)) as GameObject;
			if (meshGO == null)
			{
				return;
			}
			mesh = meshGO.GetComponentInChildren<TextMesh>();
			background = meshGO.GetComponentInChildren<SpriteRenderer>();
			background.transform.localScale = backgroundScale;
			scale = transform.localScale;
			meshGO.transform.localScale = new Vector3(scale.x, 0.00001f, scale.z);
			meshGO.layer = gameObject.layer;
			foreach (Transform child in meshGO.transform)
			{
				child.gameObject.layer = gameObject.layer;
			}
		}

		public void ChangeDescription(string descriptionStr, string positiveStr, string negativeStr)
		{
			if (mesh == null)
			{
				MakeTextObject();
			}
			description = descriptionStr;
			positives = positiveStr;
			negatives = negativeStr;
			float boxY = 0.0f;
			formattedDesc = "";
			if (description != "")
			{
				formattedDesc = description + "\n";
			}
			if (positives != "")
			{
				formattedDesc = formattedDesc + "<color=green>" + positives + "</color>\n";
			}
			if (negatives != "")
			{
				formattedDesc = formattedDesc + "<color=red>" + negatives + "</color>";
			}
			//formattedDesc = formattedDesc + "</b>";
			string newDesc = formattedDesc;
			int numLines = newDesc.Split('\n').Length;
			boxY = numLines / 10.0f;
			Vector3 backgroundScale = background.gameObject.transform.localScale;
			backgroundScale.y = boxY;
			background.gameObject.transform.localScale = backgroundScale;
			mesh.text = formattedDesc;
		}
	}
}