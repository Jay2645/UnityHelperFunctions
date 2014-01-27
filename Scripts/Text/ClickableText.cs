using UnityEngine;
namespace Clickable
{
	namespace Text
	{
		public class ClickableText : ClickableObject
		{
			private DisplayText text;
			private string textID = "";
			public GameObject displayParent;
			private const float BOUNDS_BORDER = 0.5f;
			protected Color startColor;

			void Awake()
			{
				text = gameObject.GetComponent<DisplayText>();
				if (text == null)
				{
					text = gameObject.AddComponent<DisplayText>();
				}
				SetDisplayParent(displayParent);
				useBounds = false;
				startColor = renderer.material.color;
			}

			protected override void LateUpdate()
			{
				if (mouseOver)
				{
					currentColor = Color.Lerp(currentColor, hoverColor, Time.deltaTime * 5);
				}
				else
				{
					currentColor = Color.Lerp(currentColor, Color.white, Time.deltaTime * 5);
				}
				renderer.material.color = currentColor;
			}

			protected override void LeftClicked()
			{
				if (displayParent != null && MessageBox.isOpen)
				{
					MessageBox.ChooseResponse(this, textID);
				}
			}

			public void SetMaxBounds(Bounds bounds)
			{
				float width = bounds.size.x - BOUNDS_BORDER;
				float height = bounds.size.y - BOUNDS_BORDER;
				text.SetMaxBounds(width, height);
			}

			public void SetMaxWidth(float width)
			{
				text.maxWidth = width;
				text.SetMaxBounds(width, text.maxHeight);
			}

			public void SetMaxHeight(float height)
			{
				text.maxHeight = height;
				text.SetMaxBounds(text.maxWidth, height);
			}

			public void SetDisplayParent(GameObject parent)
			{
				if (parent == null)
				{
					return;
				}
				displayParent = parent;
				text.displayParent = parent;
				SetMaxBounds(displayParent.renderer.bounds);
			}

			public void SetTextString(LocalizedString newString)
			{
				text.SetTextString(newString);
				textID = newString.ID;
				if (textID == "")
				{
					textID = newString.Text;
				}
				SetName(newString + " Clickable Text");
			}

			public void Display(bool on)
			{
				if (!on)
				{
					mouseOver = false;
				}
				text.Display(on);
			}

			public DisplayText GetDisplayText()
			{
				return text;
			}

			public void SetAnchor(TextAnchor anchor)
			{
				text.SetAnchor(anchor);
			}
		}
	}
}