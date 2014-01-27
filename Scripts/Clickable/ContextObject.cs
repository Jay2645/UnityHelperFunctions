using Clickable.Text;
using System.Collections.Generic;
using UnityEngine;
namespace Clickable
{
	/// <summary>
	/// A ContextObject is a ClickableObject which opens a Context Menu when clicked.
	/// This Context Menu is made up of more ClickableObjects, which can in turn be ContextObjects that open their own menu.
	/// </summary>
	public class ContextObject : HoverDescription
	{
		protected GameObject contextBackground = null;
		protected List<ClickableText> menuItems = new List<ClickableText>();
		/// <summary>
		/// How much to offset the context menu.
		/// </summary>
		public Vector2 contextOffset = Vector2.zero;
		protected bool menuOpen = false;

		/// <summary>
		/// Creates a ClickableText out of a ClickableObject and a string to display.
		/// The options are assumed to be the same as the Left Click option for the ClickableObject.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="menuItem"></param>
		public void AddMenuItem(LocalizedString text, ClickableObject menuItem)
		{
			AddMenuItem(text, menuItem._LeftClicked);
		}


		public void AddMenuItem(LocalizedString text, System.Action clickAction)
		{
			if (contextBackground == null)
			{
				contextBackground = Load("Prefabs/Menu Background", transform, new Vector3(contextOffset.x, contextOffset.y, -2.4f)) as GameObject;
				contextBackground.renderer.enabled = false;
			}
			GameObject go = Load("Prefabs/Menu Text", contextBackground.transform, new Vector3(0.0f, 0.0f, -0.1f), text + " Display") as GameObject;
			Transform[] tfms = go.GetComponentsInChildren<Transform>();
			GameObject textObj = tfms[1].gameObject; // This gets the first child
			ClickableText cText = textObj.AddComponent<ClickableText>();
			cText.SetLeftClickAction(clickAction);
			cText.SetTextString(text);
			AddMenuItem(cText);
		}

		public void AddMenuItem(ClickableText menuItem)
		{
			List<Transform> children = new List<Transform>(); // Unity's threading forces us to do this
			foreach (Transform child in contextBackground.transform)
			{
				children.Add(child);
			}
			contextBackground.transform.DetachChildren(); // Workaround because lossyScale is read-only
			Vector3 localScale = contextBackground.transform.localScale;
			localScale.y += 0.065f;
			contextBackground.transform.localScale = localScale;
			Vector3 position = contextBackground.transform.position;
			position.y -= 0.1f;
			contextBackground.transform.position = position;
			Transform[] childArray = children.ToArray(); // IGNORE ALL OF THIS
			for (int i = 0; i < childArray.Length; i++)
			{
				Transform child = childArray[i];
				child.localScale = Vector3.one;
				child.parent = contextBackground.transform;
				child.GetComponentInChildren<ClickableText>().SetMaxBounds(contextBackground.renderer.bounds); // STOP IGNORING HERE
				Vector3 localPos = child.localPosition;
				float change = i + (i * 1.25f);
				localPos.y -= change;
				child.localPosition = localPos;
			}
			menuItem.SetDisplayParent(contextBackground);
			menuItem.Display(false);
			menuItem.contextObject = this;
			menuItems.Add(menuItem);
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (contextBackground == null)
			{
				contextBackground = Load("Prefabs/Menu Background", transform, new Vector3(contextOffset.x, contextOffset.y, -2.4f)) as GameObject;
				contextBackground.renderer.enabled = menuOpen;
			}
			if (menuOpen && !mouseOver && InputController.isLeftClicking)
			{
				Display(false);
			}
			Vector3 localScale = contextBackground.transform.localScale;
			if (transform.root.localScale.x < 0 && localScale.x > 0 || transform.root.localScale.x > 0 && localScale.x < 0)
			{
				localScale.x = -localScale.x;
			}
			contextBackground.transform.localScale = localScale;
		}

		protected override void LeftClicked()
		{
			Display(!menuOpen);
		}


		public void Display(bool on)
		{
			menuOpen = on;
			ClickableText[] texts = menuItems.ToArray();
			foreach (ClickableText text in texts)
			{
				text.Display(on);
			}
		}
	}
}