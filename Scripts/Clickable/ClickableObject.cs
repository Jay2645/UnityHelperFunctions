using HelperFunctions.Clickable.Text;
using HelperFunctions.Clickable.Text.Menu;
using System;
using UnityEngine;
namespace HelperFunctions
{
	namespace Clickable
	{
		/// <summary>
		/// An object which is clickable. Can optionally have an outline drawn around its clickable range.
		/// </summary>
		public class ClickableObject : MonoHelper
		{
			#region Variables
			protected LineRenderer[] bounds;
			public Color hoverColor = Color.green;
			protected Color currentColor;
			protected Color nonHoverColor = Color.clear;
			protected Renderer renderObj;

			/// <summary>
			/// The Context Menu for this object.
			/// </summary>
			public ContextObject contextObject = null;

			protected Action leftClickAction = null;
			protected Action rightClickAction = null;
			protected Action middleClickAction = null;

			protected bool mouseOver = false;

			protected bool useBounds = true;

			/// <summary>
			/// The description of this object. Users will see the description on hover.
			/// </summary>
			public string description = "";
			/// <summary>
			/// Any positive effects that happen by clicking this object. Users will see these in green.
			/// </summary>
			public string positives = "";
			/// <summary>
			/// Any negative effects that happen by clicking this object. Users will see these in red.
			/// </summary>
			public string negatives = "";
			/// <summary>
			/// The description object for this ClickableObject.
			/// </summary>
			public HoverDescription hoverDescription;
			/// <summary>
			/// How much to offset the description by.
			/// </summary>
			public Vector2 descOffset = Vector2.one;
			/// <summary>
			/// How much to scale the description by.
			/// </summary>
			public Vector3 descScale = Vector3.one;
			#endregion

			protected void ListModifier(float mod, string modName)
			{
				if (mod == 0)
				{
					return;
				}
				ListModifier(mod, modName, mod > 0);
			}

			protected void ListModifier(float mod, string modName, bool positive)
			{
				if (positive)
				{
					if (positives != "")
					{
						positives = positives + "\n";
					}
					positives = positives + modName;
					if (mod > 0)
					{
						positives = positives + ": +" + mod;
					}
					else if (mod < 0)
					{
						positives = positives + ": -" + mod;
					}
				}
				else if (!positive)
				{
					if (negatives != "")
					{
						negatives = negatives + "\n";
					}
					negatives = negatives + modName;
					if (mod < 0)
					{
						negatives = negatives + ": -" + mod;
					}
					else if (mod > 0)
					{
						negatives = negatives + ": +" + mod;
					}
				}
				MakeHoverDescription();
			}

			protected override void Start()
			{
				base.Start();
				if (InputController.instance == null)
				{
					GameObject go = new GameObject("Input Controller");
					go.transform.position = Vector3.zero;
					go.AddComponent<InputController>();
				}
				if (renderer == null)
				{
					renderObj = gameObject.GetComponentInChildren<Renderer>();
				}
				else
				{
					renderObj = renderer;
				}
				if (collider2D == null && collider == null)
				{
					BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
					col.isTrigger = true;
				}
				if (useBounds && collider == null)
				{
					MakeBounds();
				}
				MakeHoverDescription();
			}

			protected void MakeBounds()
			{
				Bounds2D actualBounds = new Bounds2D(collider2D);
				Vector3 max = actualBounds.GetMax();
				Vector3 min = actualBounds.GetMin();
				Vector3 maxY = new Vector3(min.x, max.y, min.z);
				Vector3 maxX = new Vector3(max.x, min.y, min.z);
				if (bounds == null)
				{
					bounds = new LineRenderer[4];
				}
				bounds[0] = MakeOutline(min, maxY, bounds[0]);
				bounds[1] = MakeOutline(maxY, max, bounds[1]);
				bounds[2] = MakeOutline(min, maxX, bounds[2]);
				bounds[3] = MakeOutline(maxX, max, bounds[3]);
			}

			public void MakeHoverDescription()
			{
				if ((description != "" || positives != "" || negatives != "") && renderObj != null)
				{
					if (hoverDescription == null)
					{
						hoverDescription = gameObject.AddComponent<HoverDescription>();
					}
					hoverDescription.useBounds = false;
					hoverDescription.ChangeDescription(description, positives, negatives);
					hoverDescription.scale = descScale;
					if (hoverDescription.meshGO == null)
					{
						return;
					}
					Vector3 hoverCenter = renderObj.bounds.center;
					Vector3 hoverDescOffset = hoverCenter + new Vector3(descOffset.x, descOffset.y, -4.0f);
					hoverDescription.meshGO.transform.position = hoverDescOffset;
				}
			}

			protected LineRenderer MakeOutline(Vector3 cornerOne, Vector3 cornerTwo)
			{
				GameObject go = new GameObject("Bounding Box");
				go.transform.parent = transform;
				go.transform.localPosition = Vector3.zero;
				LineRenderer render = go.AddComponent<LineRenderer>();
				return MakeOutline(cornerOne, cornerTwo, render);
			}

			protected LineRenderer MakeOutline(Vector3 cornerOne, Vector3 cornerTwo, LineRenderer render)
			{
				if (render == null)
				{
					return MakeOutline(cornerOne, cornerTwo);
				}
				render.useWorldSpace = true;
				render.SetWidth(0.05f, 0.05f);
				render.SetColors(Color.clear, Color.clear);
				render.material = new Material(Shader.Find("Sprites/Default"));
				render.SetPosition(0, cornerOne);
				render.SetPosition(1, cornerTwo);
				return render;
			}

			protected virtual void LateUpdate()
			{
				if (bounds == null || !useBounds)
					return;
				if (mouseOver)
				{
					currentColor = Color.Lerp(currentColor, hoverColor, Time.deltaTime * 5);
					MakeBounds();
				}
				else
				{
					currentColor = Color.Lerp(currentColor, nonHoverColor, Time.deltaTime * 5);
					if (currentColor != nonHoverColor)
					{
						MakeBounds();
					}
				}
				foreach (LineRenderer render in bounds)
				{
					render.SetColors(currentColor, currentColor);
				}
			}

			/// <summary>
			/// A public interface for calling the MouseEnter event.
			/// Executes any ClickableObject-specific code, then the event.
			/// </summary>
			public void _MouseEnter()
			{
				if (renderObj != null && !renderObj.enabled || Globals.GameIsPaused() && !(this is MenuResponse) || MessageBox.isOpen && !(this is ClickableText))
				{
					return;
				}
				MouseEnter();
			}
			public void _MouseOver()
			{
				if (renderObj != null && !renderObj.enabled || Globals.GameIsPaused() && !(this is MenuResponse) || MessageBox.isOpen && !(this is ClickableText))
				{
					return;
				}
				mouseOver = true;
				MouseOver();
			}
			public void _MouseExit()
			{
				if (renderObj != null && !renderObj.enabled || Globals.GameIsPaused() && !(this is MenuResponse) || MessageBox.isOpen && !(this is ClickableText))
				{
					return;
				}
				mouseOver = false;
				MouseExit();
			}
			public void _LeftClicked()
			{
				if (Globals.GameIsPaused() && !(this is MenuResponse) || MessageBox.isOpen && !(this is ClickableText))
				{
					return;
				}
				if (leftClickAction != null)
				{
					leftClickAction();
					if (contextObject != null)
					{
						contextObject.Display(false);
						mouseOver = false;
					}
				}
				LeftClicked();
			}
			public void _RightClicked()
			{
				if (renderer != null && !renderer.enabled || Globals.GameIsPaused() && !(this is MenuResponse) || MessageBox.isOpen && !(this is ClickableText))
				{
					return;
				}
				if (rightClickAction != null)
				{
					rightClickAction();
				}
				RightClicked();
			}
			public void _MiddleClicked()
			{
				if (renderObj != null && !renderObj.enabled || Globals.GameIsPaused() && !(this is MenuResponse) || MessageBox.isOpen && !(this is ClickableText))
				{
					return;
				}
				if (middleClickAction != null)
				{
					middleClickAction();
				}
				MiddleClicked();
			}

			protected virtual void MouseEnter()
			{
			}
			protected virtual void MouseOver()
			{
			}
			protected virtual void MouseExit()
			{
			}
			protected virtual void LeftClicked()
			{
			}
			protected virtual void RightClicked()
			{
			}
			protected virtual void MiddleClicked()
			{
			}

			public void SetLeftClickAction(Action action)
			{
				leftClickAction = action;
			}
			public void SetRightClickAction(Action action)
			{
				rightClickAction = action;
			}
			public void SetMiddleClickAction(Action action)
			{
				middleClickAction = action;
			}
		}
	}
}